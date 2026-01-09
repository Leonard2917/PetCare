using System.Windows;
using System.Windows.Controls;
using PetCare.ViewModels;

namespace PetCare.Views
{
    public partial class RegisterView : Page
    {
        public RegisterView()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm)
            {
                vm.PerformRegistration(txtPass.Password, txtConfirmPass.Password);
            }
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
             NavigationService.Navigate(new System.Uri("Views/LoginView.xaml", System.UriKind.Relative));
        }
    }
}
