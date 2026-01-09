using System.Windows;
using PetCare.ViewModels;
using PetCare.Models;

namespace PetCare.Views
{
    public partial class ViewMedicalRecordView : Window
    {
        public ViewMedicalRecordView(FiseMedicale record)
        {
            InitializeComponent();
            DataContext = new ViewMedicalRecordViewModel(record);
        }
        
         public ViewMedicalRecordView()
        {
            InitializeComponent();
        }
    }
}
