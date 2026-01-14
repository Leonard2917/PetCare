namespace PetCare.Models
{
    public class Service
    {
        public int ServiciuID { get; set; }
        public int ClinicaID { get; set; }
        public string Denumire { get; set; }
        public decimal PretStandard { get; set; }
        public int DurataEstimataMinute { get; set; }
    }
}
