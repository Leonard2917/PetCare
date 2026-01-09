using System;
using System.Collections.Generic;
using System.Linq;

namespace PetCare.Models
{
    public class OwnerService
    {
        public int? GetProprietarID(int utilizatorID)
        {
            using (var context = new PetCareEntities())
            {
                var proprietar = context.Proprietaris.FirstOrDefault(p => p.UtilizatorID == utilizatorID);
                return proprietar?.ProprietarID;
            }
        }

        public bool CreateProprietarProfile(int utilizatorID, string adresa)
        {
            using (var context = new PetCareEntities())
            {
                try
                {
                    var newProprietar = new Proprietari
                    {
                        UtilizatorID = utilizatorID,
                        AdresaDomiciliu = adresa
                    };
                    context.Proprietaris.Add(newProprietar);
                    context.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public List<AnimalModel> GetAnimals(int proprietarID)
        {
            using (var context = new PetCareEntities())
            {
                var animals = context.Animales
                    .Where(a => a.ProprietarID == proprietarID)
                    .Select(a => new AnimalModel
                    {
                        AnimalID = a.AnimalID,
                        ProprietarID = a.ProprietarID.Value,
                        SpecieID = a.SpecieID.Value,
                        SpecieNume = a.Specii.Denumire,
                        Nume = a.Nume,
                        DataNasterii = a.DataNasterii.Value,
                        Microcip = a.Microcip
                    }).ToList();

                return animals;
            }
        }

        public bool AddAnimal(int proprietarID, int specieID, string nume, DateTime dataNasterii, string microcip)
        {
            using (var context = new PetCareEntities())
            {
                try
                {
                    var newAnimal = new Animale
                    {
                        ProprietarID = proprietarID,
                        SpecieID = specieID,
                        Nume = nume,
                        DataNasterii = dataNasterii,
                        Microcip = string.IsNullOrWhiteSpace(microcip) ? null : microcip
                    };
                    context.Animales.Add(newAnimal);
                    context.SaveChanges();
                    return true;
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

        public List<ReminderDTO> GetReminders(int proprietarID)
        {
            using (var context = new PetCareEntities())
            {
                var reminders = context.Reminderes
                    .Include("Animale")
                    .Include("Medici.Utilizatori")
                    .Where(r => r.Animale.ProprietarID == proprietarID)
                    .OrderBy(r => r.DataLimita)
                    .ToList();

                return reminders.Select(r => new ReminderDTO
                {
                    ReminderID = r.ReminderID,
                    AnimalNume = r.Animale.Nume,
                    Mesaj = r.Mesaj,
                    DataLimita = r.DataLimita,
                    IsCitit = r.IsCitit ?? false,
                    MedicNume = r.Medici.Utilizatori.Nume + " " + r.Medici.Utilizatori.Prenume
                }).ToList();
            }
        }

        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
