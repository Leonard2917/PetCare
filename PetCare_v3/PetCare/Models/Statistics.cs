using System;

namespace PetCare.Models
{
    public class ClinicStat
    {
        public string NumeClinica { get; set; }
        public int TotalProgramari { get; set; }
    }

    public class MedicStat
    {
        public string NumeMedic { get; set; }
        public string NumeClinica { get; set; }
        public int TotalProgramari { get; set; }
    }

    public class MaterialStat
    {
        public string NumeMaterial { get; set; }
        public decimal CantitateTotala { get; set; }
    }

    public class RevenueStat
    {
        public string NumeEntitate { get; set; }
        public decimal TotalVenit { get; set; }
    }

    public class AnimalTypeStat
    {
        public string TipAnimal { get; set; }
        public int Numar { get; set; }
        public double Procent { get; set; }
    }
}
