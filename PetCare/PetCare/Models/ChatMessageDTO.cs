using System;

namespace PetCare.Models
{
    public class ChatMessageDTO
    {
        public int MesajID { get; set; }
        public string SenderName { get; set; }
        public string Text { get; set; }
        public DateTime DataOra { get; set; }
        public bool IsMe { get; set; } // To determine alignment (Left/Right)

        public string TimeDisplay => DataOra.ToString("HH:mm");
        public string Alignment => IsMe ? "Right" : "Left";
        public string BubbleColor => IsMe ? "#DCF8C6" : "#FFFFFF"; // WhatsApp-like colors
        public string TextColor => "#000000";
    }
}
