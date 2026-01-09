using System;
using PetCare.Models;

namespace PetCare.ViewModels
{
    public class MedicalHistoryDTO
    {
        public int FisaID { get; set; }
        public DateTime Data { get; set; }
        public string Diagnostic { get; set; }
        public string MedicNume { get; set; }
        
        public FiseMedicale FullRecord { get; set; }

        public string DataDisplay => Data.ToString("dd.MM.yyyy HH:mm");
    }
}
