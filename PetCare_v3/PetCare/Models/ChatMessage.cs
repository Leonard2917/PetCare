using System;

namespace PetCare.Models
{
    public class ChatMessage
    {
        public int MesajID { get; set; }
        public string SenderName { get; set; }
        public string Text { get; set; }
        public DateTime DataOra { get; set; }
        public bool IsMe { get; set; } 

        public string TimeDisplay => DataOra.ToString("HH:mm");
        public string Alignment => IsMe ? "Right" : "Left";
        public string BubbleColor => IsMe ? "#DCF8C6" : "#FFFFFF"; 
        public string TextColor => "#000000";
    }
}
