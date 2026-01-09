using System;
using System.Collections.Generic;
using System.Linq;

namespace PetCare.Models
{
    public class MedicalRecordService
    {
        public string SaveMedicalRecord(int programareID, string diagnostic, string tratament, List<int> serviciiIDs, List<MaterialUsageDTO> materiale, 
                                      bool addReminder, DateTime? reminderDate, string reminderMessage)
        {
            using (var context = new PetCareEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        // 1. Create the FisaMedicale
                        var fisa = new FiseMedicale
                        {
                            ProgramareID = programareID,
                            Diagnostic = diagnostic,
                            Tratament = tratament ?? " ", // Ensure not null
                            DataCrearii = DateTime.Now
                        };
                        context.FiseMedicales.Add(fisa);
                        context.SaveChanges();

                        // 2. Add Services
                        foreach (var sId in serviciiIDs)
                        {
                            var service = context.Serviciis.Find(sId);
                            if (service != null)
                            {
                                context.DetaliiFisa_Servicii.Add(new DetaliiFisa_Servicii
                                {
                                    FisaID = fisa.FisaID,
                                    ServiciuID = sId,
                                    PretAplicat = service.PretStandard
                                });
                            }
                        }

                        // 3. Add Materials and deduct stock
                        foreach (var mat in materiale)
                        {
                            context.DetaliiFisa_Materiale.Add(new DetaliiFisa_Materiale
                            {
                                FisaID = fisa.FisaID,
                                StocID = mat.StocID,
                                CantitateConsumata = mat.Cantitate
                            });

                            var stock = context.Stocuris.Find(mat.StocID);
                            if (stock != null)
                            {
                                stock.CantitateDisponibila = (stock.CantitateDisponibila ?? 0) - mat.Cantitate;
                            }
                        }

                        // 4. Update Appointment Status & Add Reminder
                        var appointment = context.Programaris.Find(programareID);
                        if (appointment != null)
                        {
                            appointment.Status = "Completed";

                            if (addReminder && reminderDate.HasValue && appointment.MedicID.HasValue && appointment.AnimalID.HasValue)
                            {
                                context.Reminderes.Add(new Remindere
                                {
                                    MedicID = appointment.MedicID.Value,
                                    AnimalID = appointment.AnimalID.Value,
                                    DataLimita = reminderDate.Value,
                                    Mesaj = reminderMessage ?? "Reminder Automat",
                                    IsCitit = false,
                                    DataCreare = DateTime.Now
                                });
                            }
                        }

                        context.SaveChanges();
                        transaction.Commit();
                        return null; // Success
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        // Return the inner exception message if available for detailed debugging
                        var msg = ex.Message;
                        if (ex.InnerException != null) msg += " Inner: " + ex.InnerException.Message;
                        return msg;
                    }
                }
            }
        }

        public decimal GetAppointmentServicesCost(int programareID)
        {
            using (var context = new PetCareEntities())
            {
                var serviciiIDs = context.Database.SqlQuery<int>(
                    "SELECT ServiciuID FROM ProgramariServicii WHERE ProgramareID = @p0", programareID).ToList();

                if (serviciiIDs.Any())
                {
                    return context.Serviciis
                        .Where(s => serviciiIDs.Contains(s.ServiciuID))
                        .Sum(s => s.PretStandard);
                }
                return 0;
            }
        }

        public List<int> GetAppointmentServiceIDs(int programareID)
        {
            using (var context = new PetCareEntities())
            {
                 return context.Database.SqlQuery<int>(
                    "SELECT ServiciuID FROM ProgramariServicii WHERE ProgramareID = @p0", programareID).ToList();
            }
        }

        public List<FiseMedicale> GetAnimalHistory(int animalID)
        {
            using (var context = new PetCareEntities())
            {
                return context.FiseMedicales
                    .Include("Programari.Medici.Utilizatori")
                    .Include("DetaliiFisa_Servicii.Servicii")
                    .Include("DetaliiFisa_Materiale.Stocuri")
                    .Where(f => f.Programari.AnimalID == animalID)
                    .OrderByDescending(f => f.DataCrearii)
                    .ToList();
            }
        }

        public FiseMedicale GetMedicalRecordByAppointment(int programareID)
        {
            using (var context = new PetCareEntities())
            {
                return context.FiseMedicales
                    .Include("Programari.Medici.Utilizatori")
                    .Include("DetaliiFisa_Servicii.Servicii")
                    .Include("DetaliiFisa_Materiale.Stocuri")
                    .FirstOrDefault(f => f.ProgramareID == programareID);
            }
        }
    }

    public class MaterialUsageDTO
    {
        public int StocID { get; set; }
        public string NumeMaterial { get; set; }
        public decimal Cantitate { get; set; }
        
        // Extended for View View
        public decimal Cost { get; set; }
    }
}
