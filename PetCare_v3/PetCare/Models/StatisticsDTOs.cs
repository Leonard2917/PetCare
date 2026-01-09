using System;

namespace PetCare.Models
{
    public class ClinicStatDTO
    {
        public string NumeClinica { get; set; }
        public int TotalProgramari { get; set; }
    }

    public class MedicStatDTO
    {
        public string NumeMedic { get; set; }
        public string NumeClinica { get; set; }
        public int TotalProgramari { get; set; }
    }

    public class MaterialStatDTO
    {
        public string NumeMaterial { get; set; }
        public decimal CantitateTotala { get; set; }
    }

    public class RevenueStatDTO
    {
        public string NumeEntitate { get; set; }
        public decimal TotalVenit { get; set; }
    }

    public class AnimalTypeStatDTO
    {
        public string TipAnimal { get; set; }
        public int Numar { get; set; }
        public double Procent { get; set; }
    }
}
