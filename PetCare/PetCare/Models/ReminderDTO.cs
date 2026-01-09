using System;

namespace PetCare.Models
{
    public class ReminderDTO
    {
        public int ReminderID { get; set; }
        public string AnimalNume { get; set; }
        public string Mesaj { get; set; }
        public DateTime DataLimita { get; set; }
        public bool IsCitit { get; set; }
        public string MedicNume { get; set; }
        
        public string DataLimitaDisplay => DataLimita.ToString("dd.MM.yyyy");
        public string StatusColor => DataLimita < DateTime.Today ? "#FFCDD2" : "#E8F5E9"; // Red if overdue, Green if future
    }
}
