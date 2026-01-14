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
            

            Loaded += (s, e) => ScrollToBottom();
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as ChatViewModel).SendMessage();
            ScrollToBottom();
        }

        private void ScrollToBottom()
        {

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
    

}
