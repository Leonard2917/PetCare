using System;

namespace PetCare.Models
{
    public class StockModel
    {
        public int StocID { get; set; }
        public int ClinicaID { get; set; }
        public string DenumireProdus { get; set; }
        public string UnitateMasura { get; set; }
        public decimal CantitateDisponibila { get; set; }
        public decimal PretUnitar { get; set; }

        public string DisplayPrice => $"{PretUnitar:F2} RON";
    }
}
