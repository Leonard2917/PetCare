using System.Windows;
using System.Windows.Controls;
using PetCare.Models;
using PetCare.ViewModels;

namespace PetCare.Views
{
    public partial class MedicDashboardView : Page
    {
        private MedicDashboardViewModel _viewModel;

        public MedicDashboardView(UserModel user)
        {
            InitializeComponent();
            _viewModel = new MedicDashboardViewModel(user);
            this.DataContext = _viewModel;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Logout();
        }

        private void SaveProfile_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveProfile();
        }

        private void SaveSchedule_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveSchedule();
        }

        private void AddClinic_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddClinic();
        }

        private void FinalizeAppointment_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var appointment = button?.DataContext as Appointment;
            if (appointment != null)
            {
                var issueView = new IssueMedicalRecordView(appointment);
                if (issueView.ShowDialog() == true)
                {
                    _viewModel.RefreshData();
                }
            }
        }
        private void ViewRecord_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is Appointment appointment)
            {
                _viewModel.ViewMedicalRecord(appointment);
            }
        }

        private void OpenChat_Click(object sender, RoutedEventArgs e)
        {
             var button = sender as Button;
            if (button?.DataContext is Appointment appointment)
            {
                _viewModel.OpenChat(appointment);
            }
        }
    }
}
