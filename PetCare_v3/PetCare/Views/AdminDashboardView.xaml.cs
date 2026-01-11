using PetCare.Models;
using PetCare.ViewModels;
using System.Windows;

namespace PetCare.Views
{
    using System.Windows.Controls;

    public partial class AdminDashboardView : Page
    {
        public AdminDashboardView(UserModel authenticatedUser)
        {
            InitializeComponent();
            DataContext = new AdminDashboardViewModel(authenticatedUser);
        }
        
        // Helper
        private AdminDashboardViewModel VM => DataContext as AdminDashboardViewModel;

        // --- Event Handlers ---

        private void SaveClinic_Click(object sender, RoutedEventArgs e) => VM?.SaveClinic();

        private void AddService_Click(object sender, RoutedEventArgs e) => VM?.AddService();
        private void UpdateService_Click(object sender, RoutedEventArgs e) => VM?.UpdateService();
        private void DeleteService_Click(object sender, RoutedEventArgs e)
        {
             if (sender is System.Windows.Controls.Button btn && btn.DataContext is ServiceDTO service)
             {
                 VM?.DeleteService(service);
             }
        }

        private void AddStock_Click(object sender, RoutedEventArgs e) => VM?.AddStock();
        private void UpdateStock_Click(object sender, RoutedEventArgs e) => VM?.UpdateStock();
         private void DeleteStock_Click(object sender, RoutedEventArgs e)
        {
             if (sender is System.Windows.Controls.Button btn && btn.DataContext is StockModel stoc)
             {
                 VM?.DeleteStock(stoc);
             }
        }

        private void ApproveRequest_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is MedicRequestDTO request)
            {
                VM?.ApproveRequest(request);
            }
        }

        private void RejectRequest_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is MedicRequestDTO request)
            {
                VM?.RejectRequest(request);
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to Login wrapping it in a Frame to ensure NavigationService works
             Application.Current.MainWindow.Content = new System.Windows.Controls.Frame { Source = new System.Uri("Views/LoginView.xaml", System.UriKind.Relative) };
        }
    }
}
