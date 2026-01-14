using System.Windows;
using System.Windows.Controls;
using PetCare.Models;
using PetCare.ViewModels;

namespace PetCare.Views
{
    public partial class BookAppointmentView : Page
    {
        private BookAppointmentViewModel _viewModel;

        public BookAppointmentView(UserModel user)
        {
            InitializeComponent();
            _viewModel = new BookAppointmentViewModel(user);
            this.DataContext = _viewModel;
        }

        private void ServicesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox != null)
            {
                _viewModel.SelectedServices.Clear();
                foreach (Service service in listBox.SelectedItems)
                {
                    _viewModel.SelectedServices.Add(service);
                }
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ConfirmBooking();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Cancel();
        }
    }
}
