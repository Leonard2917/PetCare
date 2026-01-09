using System;

namespace PetCare.Models
{
    public class TimeSlot
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        
        public string DisplayText => $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
    }
}
