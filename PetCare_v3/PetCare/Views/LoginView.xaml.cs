using System;
using System.Windows;
using System.Windows.Controls;
using PetCare.ViewModels;

namespace PetCare.Views
{
    public partial class LoginView : Page
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.PerformLogin(txtPass.Password);
            }
        }

        private void RegisterLink_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new System.Uri("Views/RegisterView.xaml", System.UriKind.Relative));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}