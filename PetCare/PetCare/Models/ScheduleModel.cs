using System;

namespace PetCare.Models
{
    public class ScheduleModel
    {
        public int OrarID { get; set; }
        public int MedicID { get; set; }
        public int ClinicaID { get; set; }
        public int ZiSaptamana { get; set; } // 1=Luni, 7=Duminica
        public bool IsWorkingDay { get; set; }
        public TimeSpan OraInceput { get; set; }
        public TimeSpan OraSfarsit { get; set; }

        public string ZiNume
        {
            get
            {
                switch (ZiSaptamana)
                {
                    case 1: return "Luni";
                    case 2: return "Marți";
                    case 3: return "Miercuri";
                    case 4: return "Joi";
                    case 5: return "Vineri";
                    case 6: return "Sâmbătă";
                    case 7: return "Duminică";
                    default: return "Zi Necunoscută";
                }
            }
        }
        
        // For UI Binding - using Strings for simpler TextBox binding with validation in VM
        public string OraInceputStr { get; set; } 
        public string OraSfarsitStr { get; set; }
    }
}
