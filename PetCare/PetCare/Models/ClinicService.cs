using System.Collections.Generic;
using System.Linq;

namespace PetCare.Models
{
    public class ClinicService
    {
        public int? GetAdminClinicID(int utilizatorID)
        {
            using (var context = new PetCareEntities())
            {
                var admin = context.Administratoris.FirstOrDefault(a => a.UtilizatorID == utilizatorID);
                return admin?.ClinicaID;
            }
        }

        public ClinicsModel GetClinicDetails(int clinicaID)
        {
            using (var context = new PetCareEntities())
            {
                var clinic = context.Clinicis.Find(clinicaID);
                if (clinic != null)
                {
                    return new ClinicsModel
                    {
                        ClinicaID = clinic.ClinicaID,
                        Nume = clinic.Nume,
                        Adresa = clinic.Adresa,
                        Telefon = clinic.Telefon
                    };
                }
                return null;
            }
        }

        public List<MedicInfoDTO> GetClinicMedics(int clinicaID)
        {
            using (var context = new PetCareEntities())
            {
                var medics = context.Medicis
                    .Include("Clinicis")
                    .Include("Utilizatori")
                    .Where(m => m.Clinicis.Any(c => c.ClinicaID == clinicaID))
                    .Select(m => new MedicInfoDTO
                    {
                        MedicID = m.MedicID,
                        Nume = m.Utilizatori.Nume,
                        Prenume = m.Utilizatori.Prenume,
                        Telefon = m.Utilizatori.Telefon,
                        NrParafa = m.NrParafa,
                        Bio = m.Bio
                    }).ToList();

                return medics;
            }
        }

        public List<ClinicDTO> GetAllClinicsForBooking()
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

        public List<ClinicsModel> GetClinics()
        {
            using (var context = new PetCareEntities())
            {
                return context.Clinicis.Select(c => new ClinicsModel
                {
                    ClinicaID = c.ClinicaID,
                    Nume = c.Nume,
                    Adresa = c.Adresa,
                    Telefon = c.Telefon
                }).ToList();
            }
        }

        public List<ServiceDTO> GetClinicServices(int clinicaID)
        {
            using (var context = new PetCareEntities())
            {
                return context.Serviciis.Where(s => s.ClinicaID == clinicaID).Select(s => new ServiceDTO
                {
                    ServiciuID = s.ServiciuID,
                    ClinicaID = s.ClinicaID.Value,
                    Denumire = s.Denumire,
                    PretStandard = s.PretStandard,
                    DurataEstimataMinute = s.DurataEstimataMinute.Value
                }).ToList();
            }
        }

        public bool AddService(int clinicaID, string denumire, decimal pretStandard, int durataMinute)
        {
            using (var context = new PetCareEntities())
            {
                try
                {
                    var newService = new Servicii
                    {
                        ClinicaID = clinicaID,
                        Denumire = denumire,
                        PretStandard = pretStandard,
                        DurataEstimataMinute = durataMinute
                    };
                    context.Serviciis.Add(newService);
                    context.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool UpdateService(int serviciuID, string denumire, decimal pretStandard, int durataMinute)
        {
            using (var context = new PetCareEntities())
            {
                try
                {
                    var service = context.Serviciis.Find(serviciuID);
                    if (service != null)
                    {
                        service.Denumire = denumire;
                        service.PretStandard = pretStandard;
                        service.DurataEstimataMinute = durataMinute;
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

        public bool DeleteService(int serviciuID)
        {
            using (var context = new PetCareEntities())
            {
                try
                {
                    var service = context.Serviciis.Find(serviciuID);
                    if (service != null)
                    {
                        context.Serviciis.Remove(service);
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

        public bool UpdateClinicDetails(int clinicaID, string nume, string adresa, string telefon)
        {
            using (var context = new PetCareEntities())
            {
                try
                {
                    var clinic = context.Clinicis.Find(clinicaID);
                    if (clinic != null)
                    {
                        clinic.Nume = nume;
                        clinic.Adresa = adresa;
                        clinic.Telefon = telefon;
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
        public List<SpeciesModel> GetSpecies()
        {
            using (var context = new PetCareEntities())
            {
                return context.Speciis.Select(s => new SpeciesModel
                {
                    SpecieID = s.SpecieID,
                    Denumire = s.Denumire
                }).ToList();
            }
        }
    }
}
