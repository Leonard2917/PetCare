using System;
using System.Collections.Generic;

namespace PetCare.Models
{
    public class AppointmentDTO
    {
        public int ProgramareID { get; set; }
        public int ClinicaID { get; set; }
        public string ClinicaNume { get; set; }
        public int MedicID { get; set; }
        public string MedicNume { get; set; }
        public int AnimalID { get; set; }
        public string AnimalNume { get; set; }
        public DateTime DataOra { get; set; }
        public int DurataMinute { get; set; }
        public string Status { get; set; }
        public List<string> ServiciiList { get; set; }

        public string DataOraDisplay => DataOra.ToString("dd.MM.yyyy HH:mm");
        public string ServiciiDisplay => ServiciiList != null ? string.Join(", ", ServiciiList) : "";
    }
}
