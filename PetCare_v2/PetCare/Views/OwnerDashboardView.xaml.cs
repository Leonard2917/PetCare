using System.Windows;
using System.Windows.Controls;
using PetCare.Models;
using PetCare.ViewModels;

namespace PetCare.Views
{
    public partial class OwnerDashboardView : Page
    {
        private OwnerDashboardViewModel _viewModel;

        public OwnerDashboardView(UserModel user)
        {
            InitializeComponent();
            _viewModel = new OwnerDashboardViewModel(user);
            this.DataContext = _viewModel;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Logout();
        }

        private void AddAnimalButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddAnimal();
        }

        private void SaveProfileButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveProfile();
        }

        private void ToggleAddAnimal_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ToggleAddAnimalVisibility();
        }

        private void NewAppointment_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.NewAppointment();
        }
    }
}
