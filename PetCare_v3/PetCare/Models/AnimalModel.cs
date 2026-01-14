using System;

namespace PetCare.Models
{
    public class AnimalModel
    {
        public int AnimalID { get; set; }
        public int ProprietarID { get; set; }
        public int SpecieID { get; set; }
        public string SpecieNume { get; set; }
        public string Nume { get; set; }
        public DateTime? DataNasterii { get; set; }
        public string Microcip { get; set; }
        
        public string Varsta
        {
            get
            {
                if (!DataNasterii.HasValue) return "Necunoscută";
                var age = DateTime.Today.Year - DataNasterii.Value.Year;
                if (DataNasterii.Value.Date > DateTime.Today.AddYears(-age)) age--;
                return age + " ani";
            }
        }
    }
}
