using System;
using System.Collections.Generic;
using System.Linq;

namespace PetCare.Models
{
    public class StatisticsService
    {
        public List<ClinicStat> GetTopClinics(int count = 5)
        {
            using (var context = new PetCareEntities())
            {
                return context.Programaris
                    .Where(p => p.Status == "Completed")
                    .GroupBy(p => p.Clinici.Nume)
                    .Select(g => new ClinicStat
                    {
                        NumeClinica = g.Key,
                        TotalProgramari = g.Count()
                    })
                    .OrderByDescending(x => x.TotalProgramari)
                    .Take(count)
                    .ToList();
            }
        }

        public List<MedicStat> GetTopMedics(int count = 5)
        {
            using (var context = new PetCareEntities())
            {
                return context.Programaris
                    .Where(p => p.Status == "Completed")
                    .GroupBy(p => new { MedicNume = p.Medici.Utilizatori.Nume + " " + p.Medici.Utilizatori.Prenume, ClinicaNume = p.Clinici.Nume })
                    .Select(g => new MedicStat
                    {
                        NumeMedic = g.Key.MedicNume,
                        NumeClinica = g.Key.ClinicaNume,
                        TotalProgramari = g.Count()
                    })
                    .OrderByDescending(x => x.TotalProgramari)
                    .Take(count)
                    .ToList();
            }
        }

        public List<MaterialStat> GetTopConsumedMaterials(int count = 5)
        {
            using (var context = new PetCareEntities())
            {
                return context.DetaliiFisa_Materiale
                    .GroupBy(m => m.Stocuri.DenumireProdus)
                    .Select(g => new MaterialStat
                    {
                        NumeMaterial = g.Key,
                        CantitateTotala = g.Sum(x => x.CantitateConsumata)
                    })
                    .OrderByDescending(x => x.CantitateTotala)
                    .Take(count)
                    .ToList();
            }
        }

        public List<RevenueStat> GetTotalRevenuePerMedic(int clinicaId)
        {
            using (var context = new PetCareEntities())
            {
                var fiseInClinica = context.FiseMedicales
                    .Where(f => f.Programari.ClinicaID == clinicaId && f.Programari.Status == "Completed")
                    .ToList();

                var results = fiseInClinica
                    .GroupBy(f => f.Programari.Medici.Utilizatori.Nume + " " + f.Programari.Medici.Utilizatori.Prenume)
                    .Select(g => new RevenueStat
                    {
                        NumeEntitate = g.Key,
                        TotalVenit = g.Sum(f => 
                            (f.DetaliiFisa_Servicii.Sum(s => s.PretAplicat ?? 0)) +
                            (f.DetaliiFisa_Materiale.Sum(m => m.CantitateConsumata * (m.Stocuri.PretUnitar ?? 0)))
                        )
                    })
                    .OrderByDescending(x => x.TotalVenit)
                    .ToList();

                return results;
            }
        }

        public List<AnimalTypeStat> GetAnimalTypeDistribution()
        {
            using (var context = new PetCareEntities())
            {
                var total = context.Animales.Count();
                if (total == 0) return new List<AnimalTypeStat>();

                return context.Animales
                    .GroupBy(a => a.Specii.Denumire)
                    .Select(g => new AnimalTypeStat
                    {
                        TipAnimal = g.Key ?? "Altele",
                        Numar = g.Count(),
                        Procent = Math.Round((double)g.Count() * 100 / total, 2)
                    })
                    .OrderByDescending(x => x.Numar)
                    .ToList();
            }
        }
    }
}
