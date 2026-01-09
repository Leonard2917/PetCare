using System;
using System.Windows;
using PetCare.Models;

namespace PetCare.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthenticationService _authService;
        
        private string _email;
        private string _password;
        private string _errorMessage;

        public LoginViewModel()
        {
            _authService = new AuthenticationService();
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(nameof(Email)); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(nameof(Password)); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); }
        }

        public void PerformLogin(string password)
        {
            Password = password;
            ExecuteLogin();
        }

        private void ExecuteLogin()
        {
            try
            {
                var authenticatedUser = _authService.Authenticate(Email, Password);

                if (authenticatedUser != null)
                {
                    ErrorMessage = "";
                    
                    if (authenticatedUser.Rol == "Owner")
                    {
                         Application.Current.MainWindow.Content = new PetCare.Views.OwnerDashboardView(authenticatedUser);
                    }
                    else if (authenticatedUser.Rol == "Medic")
                    {
                        Application.Current.MainWindow.Content = new PetCare.Views.MedicDashboardView(authenticatedUser);
                    }
                    else
                    {
                        Application.Current.MainWindow.Content = new PetCare.Views.AdminDashboardView(authenticatedUser);
                    }
                }
                else
                {
                    ErrorMessage = "Email sau parolă incorectă.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Eroare de conexiune: {ex.Message}";
            }
        }
    }
}
