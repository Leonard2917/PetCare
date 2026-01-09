using System;
using System.Collections.Generic;
using System.Linq;

namespace PetCare.Models
{
    public class MedicService
    {
        public int? GetMedicID(int utilizatorID)
        {
            using (var context = new PetCareEntities())
            {
                var medic = context.Medicis.FirstOrDefault(m => m.UtilizatorID == utilizatorID);
                return medic?.MedicID;
            }
        }

        public MedicInfoDTO GetMedicProfile(int medicID)
        {
            using (var context = new PetCareEntities())
            {
                var medic = context.Medicis.Include("Utilizatori").FirstOrDefault(m => m.MedicID == medicID);
                if (medic != null)
                {
                    return new MedicInfoDTO
                    {
                        MedicID = medic.MedicID,
                        Nume = medic.Utilizatori.Nume,
                        Prenume = medic.Utilizatori.Prenume,
                        Telefon = medic.Utilizatori.Telefon,
                        NrParafa = medic.NrParafa,
                        Bio = medic.Bio
                    };
                }
                return null;
            }
        }

        public bool UpdateMedicProfile(int medicID, string nrParafa, string bio)
        {
            using (var context = new PetCareEntities())
            {
                try
                {
                    var medic = context.Medicis.Find(medicID);
                    if (medic != null)
                    {
                        medic.NrParafa = nrParafa;
                        medic.Bio = bio;
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
                catch
                {
                    return false;
                }
            }
        }

        public List<SelectableSpeciesModel> GetMedicSpecialties(int medicID)
        {
            using (var context = new PetCareEntities())
            {
                var medic = context.Medicis.Include("Speciis").FirstOrDefault(m => m.MedicID == medicID);
                var allSpecies = context.Speciis.ToList();
                var result = new List<SelectableSpeciesModel>();

                foreach (var species in allSpecies)
                {
                    result.Add(new SelectableSpeciesModel
                    {
                        SpecieID = species.SpecieID,
                        Denumire = species.Denumire,
                        IsSelected = medic?.Speciis.Any(s => s.SpecieID == species.SpecieID) ?? false
                    });
                }

                return result;
            }
        }

        public bool UpdateMedicSpecialties(int medicID, List<int> selectedSpeciesIDs)
        {
            using (var context = new PetCareEntities())
            {
                try
                {
                    var medic = context.Medicis.Include("Speciis").FirstOrDefault(m => m.MedicID == medicID);
                    if (medic == null) return false;

                    medic.Speciis.Clear();

                    foreach (var specieID in selectedSpeciesIDs)
                    {
                        var species = context.Speciis.Find(specieID);
                        if (species != null)
                        {
                            medic.Speciis.Add(species);
                        }
                    }

                    context.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public List<ClinicDTO> GetMedicClinicsList(int medicID)
        {
            using (var context = new PetCareEntities())
            {
                var medic = context.Medicis.Include("Clinicis").FirstOrDefault(m => m.MedicID == medicID);
                if (medic != null)
                {
                    return medic.Clinicis.Select(c => new ClinicDTO
                    {
                        ClinicaID = c.ClinicaID,
                        Nume = c.Nume,
                        Adresa = c.Adresa
                    }).ToList();
                }
                return new List<ClinicDTO>();
            }
        }

        public List<ClinicDTO> GetAllClinics()
        {
            using (var context = new PetCareEntities())
            {
                return context.Clinicis.Select(c => new ClinicDTO
                {
                    ClinicaID = c.ClinicaID,
                    Nume = c.Nume,
                    Adresa = c.Adresa
                }).ToList();
            }
        }

        public bool AddMedicToClinic(int medicID, int clinicID)
        {
            using (var context = new PetCareEntities())
            {
                try
                {
                    var medic = context.Medicis.Include("Clinicis").FirstOrDefault(m => m.MedicID == medicID);
                    var clinic = context.Clinicis.Find(clinicID);

                    if (medic != null && clinic != null)
                    {
                        if (!medic.Clinicis.Any(c => c.ClinicaID == clinicID))
                        {
                            medic.Clinicis.Add(clinic);
                            context.SaveChanges();
                        }
                        return true;
                    }
                    return false;
                }
                catch { return false; }
            }
        }

        public List<ScheduleModel> GetMedicSchedule(int medicID, int clinicaID)
        {
            using (var context = new PetCareEntities())
            {
                var dbSchedule = context.OrarMedicis.Where(o => o.MedicID == medicID && o.ClinicaID == clinicaID).ToList();

                var result = new List<ScheduleModel>();
                string[] dayNames = { "Luni", "Marți", "Miercuri", "Joi", "Vineri", "Sâmbătă", "Duminică" };

                for (int i = 1; i <= 7; i++)
                {
                    var existingDay = dbSchedule.FirstOrDefault(d => d.ZiSaptamana == i);
                    if (existingDay != null)
                    {
                        result.Add(new ScheduleModel
                        {
                            OrarID = existingDay.OrarID,
                            MedicID = medicID,
                            ClinicaID = clinicaID,
                            ZiSaptamana = i,
                            IsWorkingDay = true,
                            OraInceput = existingDay.OraInceput,
                            OraSfarsit = existingDay.OraSfarsit
                        });
                    }
                    else
                    {
                        result.Add(new ScheduleModel
                        {
                            OrarID = 0,
                            MedicID = medicID,
                            ClinicaID = clinicaID,
                            ZiSaptamana = i,
                            IsWorkingDay = false,
                            OraInceput = new TimeSpan(9, 0, 0),
                            OraSfarsit = new TimeSpan(17, 0, 0)
                        });
                    }
                }

                return result;
            }
        }

        public bool UpdateMedicSchedule(int medicID, int clinicaID, List<ScheduleModel> scheduleItems)
        {
            using (var context = new PetCareEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var existing = context.OrarMedicis.Where(o => o.MedicID == medicID && o.ClinicaID == clinicaID);
                        context.OrarMedicis.RemoveRange(existing);

                        foreach (var item in scheduleItems.Where(s => s.IsWorkingDay))
                        {
                            var newEntry = new OrarMedici
                            {
                                MedicID = medicID,
                                ClinicaID = clinicaID,
                                ZiSaptamana = item.ZiSaptamana,
                                OraInceput = item.OraInceput,
                                OraSfarsit = item.OraSfarsit
                            };
                            context.OrarMedicis.Add(newEntry);
                        }

                        context.SaveChanges();
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

        public List<int> GetMedicWorkingDays(int medicID, int clinicaID)
        {
            using (var context = new PetCareEntities())
            {
                return context.OrarMedicis
                    .Where(o => o.MedicID == medicID && o.ClinicaID == clinicaID)
                    .Select(o => o.ZiSaptamana.Value)
                    .Distinct()
                    .ToList();
            }
        }

        public List<MedicInfoDTO> GetAvailableMedicsForAnimal(int clinicaID, int specieID)
        {
            using (var context = new PetCareEntities())
            {
                var medics = context.Medicis
                    .Include("Clinicis")
                    .Include("Speciis")
                    .Include("Utilizatori")
                    .Where(m => m.Clinicis.Any(c => c.ClinicaID == clinicaID) &&
                                m.Speciis.Any(s => s.SpecieID == specieID))
                    .Select(m => new MedicInfoDTO
                    {
                        MedicID = m.MedicID,
                        Nume = m.Utilizatori.Nume,
                        Prenume = m.Utilizatori.Prenume,
                        Telefon = m.Utilizatori.Telefon,
                        NrParafa = m.NrParafa
                    }).ToList();

                return medics;
            }
        }
    }
}
