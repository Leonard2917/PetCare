using System;
using System.Collections.ObjectModel;
using System.Windows;
using PetCare.Models;

namespace PetCare.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        private readonly int _programareID;
        private readonly int _currentUserId;
        private readonly ChatService _chatService;

        public ObservableCollection<ChatMessageDTO> Messages { get; set; } = new ObservableCollection<ChatMessageDTO>();

        private string _newMessageText;
        public string NewMessageText
        {
            get => _newMessageText;
            set { _newMessageText = value; OnPropertyChanged(nameof(NewMessageText)); }
        }

        private string _title;
        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(nameof(Title)); }
        }

        // Commands (relied on RelayCommand like implementation in BaseViewModel if available, else simple methods called from code-behind)
        
        public ChatViewModel(int programareID, int currentUserId, string chatPartnerName)
        {
            _programareID = programareID;
            _currentUserId = currentUserId;
            _chatService = new ChatService();
            Title = $"Chat cu {chatPartnerName}";
            
            // Mark as read when opening chat
            _chatService.MarkAsRead(_programareID, _currentUserId);
            
            LoadMessages();
        }

        public void LoadMessages()
        {
            var msgs = _chatService.GetMessages(_programareID, _currentUserId);
            Messages.Clear();
            foreach (var m in msgs)
            {
                Messages.Add(m);
            }
        }

        public void SendMessage()
        {
            if (string.IsNullOrWhiteSpace(NewMessageText)) return;

            bool success = _chatService.SendMessage(_programareID, _currentUserId, NewMessageText);
            if (success)
            {
                NewMessageText = ""; // Clear input
                LoadMessages(); // Refresh
            }
            else
            {
                MessageBox.Show("Eroare la trimiterea mesajului.");
            }
        }
    }
}
