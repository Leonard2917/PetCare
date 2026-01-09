using System;
using System.Collections.Generic;

namespace PetCare.Models
{
    public class AppointmentDTO : ViewModels.BaseViewModel 
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

        public bool CanFinalize => Status == "Confirmed";
        public bool CanViewRecord => Status == "Completed"; 

        // Chat UI Properties
        private bool _hasUnreadMessages;
        public bool HasUnreadMessages
        {
            get => _hasUnreadMessages;
            set
            {
                if (_hasUnreadMessages != value)
                {
                    _hasUnreadMessages = value;
                    OnPropertyChanged(nameof(HasUnreadMessages));
                    OnPropertyChanged(nameof(ChatButtonStyle));
                }
            }
        }
        
        public string ChatButtonStyle => HasUnreadMessages ? "#FF5722" : "#00897B";
    }
}
