using System.Windows;
using PetCare.ViewModels;
using PetCare.Models;

namespace PetCare.Views
{
    public partial class IssueMedicalRecordView : Window
    {
        public IssueMedicalRecordView()
        {
            InitializeComponent();
        }

        public IssueMedicalRecordView(AppointmentDTO appointment)
        {
            InitializeComponent();
            DataContext = new IssueMedicalRecordViewModel(appointment);
        }

        private void AddMaterial_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as IssueMedicalRecordViewModel).AddMaterial();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string error = (DataContext as IssueMedicalRecordViewModel).Save();
            if (error == null)
            {
                MessageBox.Show("Fișa medicală a fost salvată cu succes!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Eroare la salvarea fișei medicale: " + error, "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
