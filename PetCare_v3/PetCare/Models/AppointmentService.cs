using System;
using System.Collections.Generic;
using System.Linq;

namespace PetCare.Models
{
    public class AppointmentService
    {
        public List<TimeSlot> GetAvailableTimeSlots(int medicID, int clinicaID, DateTime date, int durationMinutes)
        {
            using (var context = new PetCareEntities())
            {
                var dayOfWeek = ((int)date.DayOfWeek == 0) ? 7 : (int)date.DayOfWeek;

                var schedule = context.OrarMedicis
                    .FirstOrDefault(o => o.MedicID == medicID && o.ClinicaID == clinicaID && o.ZiSaptamana == dayOfWeek);

                if (schedule == null)
                    return new List<TimeSlot>();

                var existingAppointments = context.Programaris
                    .Where(p => p.MedicID == medicID &&
                                p.ClinicaID == clinicaID &&
                                p.DataOra.Year == date.Year &&
                                p.DataOra.Month == date.Month &&
                                p.DataOra.Day == date.Day &&
                                p.Status != "Cancelled")
                    .ToList();

                var occupiedRanges = new List<(TimeSpan Start, TimeSpan End)>();
                foreach (var appt in existingAppointments)
                {
                    var apptStart = appt.DataOra.TimeOfDay;

                    var serviciiIDs = context.Database.SqlQuery<int>(
                        "SELECT ServiciuID FROM ProgramariServicii WHERE ProgramareID = @p0", appt.ProgramareID).ToList();

                    var totalDuration = context.Serviciis
                        .Where(s => serviciiIDs.Contains(s.ServiciuID))
                        .Sum(s => (int?)s.DurataEstimataMinute) ?? 30;

                    var apptEnd = apptStart.Add(TimeSpan.FromMinutes(totalDuration));
                    occupiedRanges.Add((apptStart, apptEnd));
                }

                var availableSlots = new List<TimeSlot>();
                var slotInterval = TimeSpan.FromMinutes(15);
                var requiredDuration = TimeSpan.FromMinutes(durationMinutes);

                for (var currentTime = schedule.OraInceput; currentTime < schedule.OraSfarsit; currentTime = currentTime.Add(slotInterval))
                {
                    var slotEnd = currentTime.Add(requiredDuration);

                    if (slotEnd > schedule.OraSfarsit)
                        break;

                    bool isAvailable = true;
                    foreach (var occupied in occupiedRanges)
                    {
                        if (currentTime < occupied.End && slotEnd > occupied.Start)
                        {
                            isAvailable = false;
                            break;
                        }
                    }

                    if (isAvailable)
                    {
                        availableSlots.Add(new TimeSlot
                        {
                            StartTime = currentTime,
                            EndTime = slotEnd
                        });
                    }
                }

                return availableSlots;
            }
        }

        public bool CreateAppointment(int animalID, int clinicaID, int medicID, DateTime dataOra, List<int> serviciiIDs)
        {
            using (var context = new PetCareEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var newAppointment = new Programari
                        {
                            AnimalID = animalID,
                            ClinicaID = clinicaID,
                            MedicID = medicID,
                            DataOra = dataOra,
                            Status = "Confirmed",
                            Motiv = ""
                        };

                        context.Programaris.Add(newAppointment);
                        context.SaveChanges();

                        foreach (var serviciiID in serviciiIDs)
                        {
                            context.Database.ExecuteSqlCommand(
                                "INSERT INTO ProgramariServicii (ProgramareID, ServiciuID) VALUES (@p0, @p1)",
                                newAppointment.ProgramareID, serviciiID);
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        public List<Appointment> GetOwnerAppointments(int proprietarID)
        {
            using (var context = new PetCareEntities())
            {
                var appointments = context.Programaris
                    .Include("Animale")
                    .Include("Clinici")
                    .Include("Medici.Utilizatori")
                    .Where(p => p.Animale.ProprietarID == proprietarID)
                    .OrderByDescending(p => p.DataOra)
                    .ToList();

                var result = new List<Appointment>();

                foreach (var appt in appointments)
                {
                    var serviciiIDs = context.Database.SqlQuery<int>(
                        "SELECT ServiciuID FROM ProgramariServicii WHERE ProgramareID = @p0", appt.ProgramareID).ToList();

                    var serviciiNames = context.Serviciis
                        .Where(s => serviciiIDs.Contains(s.ServiciuID))
                        .Select(s => s.Denumire)
                        .ToList();

                    var totalDuration = context.Serviciis
                        .Where(s => serviciiIDs.Contains(s.ServiciuID))
                        .Sum(s => (int?)s.DurataEstimataMinute) ?? 0;

                    result.Add(new Appointment
                    {
                        ProgramareID = appt.ProgramareID,
                        ClinicaID = appt.ClinicaID.Value,
                        ClinicaNume = appt.Clinici.Nume,
                        MedicID = appt.MedicID.Value,
                        MedicNume = $"Dr. {appt.Medici.Utilizatori.Nume} {appt.Medici.Utilizatori.Prenume}",
                        AnimalID = appt.AnimalID.Value,
                        AnimalNume = appt.Animale.Nume,
                        DataOra = appt.DataOra,
                        DurataMinute = totalDuration,
                        Status = appt.Status,
                        ServiciiList = serviciiNames
                    });
                }

                return result;
            }
        }

        public List<Appointment> GetMedicAppointments(int medicID)
        {
            using (var context = new PetCareEntities())
            {
                var appointments = context.Programaris
                    .Include("Animale")
                    .Include("Animale.Proprietari.Utilizatori")
                    .Include("Clinici")
                    .Where(p => p.MedicID == medicID)
                    .OrderBy(p => p.DataOra)
                    .ToList();

                var result = new List<Appointment>();

                foreach (var appt in appointments)
                {
                    var serviciiIDs = context.Database.SqlQuery<int>(
                        "SELECT ServiciuID FROM ProgramariServicii WHERE ProgramareID = @p0", appt.ProgramareID).ToList();

                    var serviciiNames = context.Serviciis
                        .Where(s => serviciiIDs.Contains(s.ServiciuID))
                        .Select(s => s.Denumire)
                        .ToList();

                    var totalDuration = context.Serviciis
                        .Where(s => serviciiIDs.Contains(s.ServiciuID))
                        .Sum(s => (int?)s.DurataEstimataMinute) ?? 0;

                    var ownerName = appt.Animale?.Proprietari?.Utilizatori != null
                        ? $"{appt.Animale.Proprietari.Utilizatori.Nume} {appt.Animale.Proprietari.Utilizatori.Prenume}"
                        : "N/A";

                    result.Add(new Appointment
                    {
                        ProgramareID = appt.ProgramareID,
                        ClinicaID = appt.ClinicaID.Value,
                        ClinicaNume = appt.Clinici.Nume,
                        MedicID = appt.MedicID.Value,
                        MedicNume = ownerName,
                        AnimalID = appt.AnimalID.Value,
                        AnimalNume = appt.Animale.Nume,
                        DataOra = appt.DataOra,
                        DurataMinute = totalDuration,
                        Status = appt.Status,
                        ServiciiList = serviciiNames
                    });
                }

                return result;
            }
        }
    }
}
