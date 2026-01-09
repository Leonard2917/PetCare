using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using PetCare.ViewModels;

namespace PetCare.Views
{
    public partial class ChatView : Window
    {
        public ChatView(int programareID, int currentUserId, string partnerName)
        {
            InitializeComponent();
            DataContext = new ChatViewModel(programareID, currentUserId, partnerName);
            
            // Scroll to bottom on load
            Loaded += (s, e) => ScrollToBottom();
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as ChatViewModel).SendMessage();
            ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            // Simple helper to find the ListBox and scroll
             var listBox = this.Content as Grid;
             if(listBox != null)
             {
                 foreach(var child in listBox.Children)
                 {
                     if(child is ListBox lb && lb.Items.Count > 0)
                     {
                         lb.ScrollIntoView(lb.Items[lb.Items.Count - 1]);
                         break;
                     }
                 }
             }
        }
    }
    
    // Quick BooleanToVisibilityConverter inverse implementation if needed, 
    // but usually we rely on standard one. 
    // Since Resource lookup might be tricky if not in App.xaml, I'll rely on global resource or add it safely.
}
