using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using PetCare.Models;

namespace PetCare.ViewModels
{
    public class OwnerDashboardViewModel : BaseViewModel
    {
        private readonly UserModel _loggedInUser;
        private int? _proprietarID;
        
        private bool _isProfileIncomplete;
        private string _adresaDomiciliu;
        
        private bool _isAddAnimalVisible;
        
        // Add Animal Inputs
        private string _newAnimalName;
        private SpeciesModel _selectedSpecies;
        private string _newAnimalMicrocip;
        private DateTime _newAnimalBirthDate = DateTime.Today;

        public ObservableCollection<AnimalModel> Animals { get; set; } = new ObservableCollection<AnimalModel>();
        public ObservableCollection<SpeciesModel> Species { get; set; } = new ObservableCollection<SpeciesModel>();
        public ObservableCollection<AppointmentDTO> Appointments { get; set; } = new ObservableCollection<AppointmentDTO>();

        public OwnerDashboardViewModel(UserModel user)
        {
            _loggedInUser = user;
            LoadProfile();
        }

        // --- Properties ---

        public string WelcomeMessage => $"Salut, {_loggedInUser.Prenume}!";

        public bool IsProfileIncomplete
        {
            get => _isProfileIncomplete;
            set { _isProfileIncomplete = value; OnPropertyChanged(nameof(IsProfileIncomplete)); OnPropertyChanged(nameof(IsProfileComplete)); }
        }

        public bool IsProfileComplete => !IsProfileIncomplete;

        public bool IsAddAnimalVisible
        {
            get => _isAddAnimalVisible;
            set { _isAddAnimalVisible = value; OnPropertyChanged(nameof(IsAddAnimalVisible)); }
        }

        public string AdresaDomiciliu
        {
            get => _adresaDomiciliu;
            set { _adresaDomiciliu = value; OnPropertyChanged(nameof(AdresaDomiciliu)); }
        }

        public string NewAnimalName
        {
            get => _newAnimalName;
            set { _newAnimalName = value; OnPropertyChanged(nameof(NewAnimalName)); }
        }


        public SpeciesModel SelectedSpecies
        {
            get => _selectedSpecies;
            set { _selectedSpecies = value; OnPropertyChanged(nameof(SelectedSpecies)); }
        }

        public string NewAnimalMicrocip
        {
            get => _newAnimalMicrocip;
            set { _newAnimalMicrocip = value; OnPropertyChanged(nameof(NewAnimalMicrocip)); }
        }

        public DateTime NewAnimalBirthDate
        {
            get => _newAnimalBirthDate;
            set { _newAnimalBirthDate = value; OnPropertyChanged(nameof(NewAnimalBirthDate)); }
        }

        // --- Logic ---

        private void LoadProfile()
        {
            var ownerService = new OwnerService();
            _proprietarID = ownerService.GetProprietarID(_loggedInUser.UtilizatorID);

            if (_proprietarID == null)
            {
                IsProfileIncomplete = true;
            }
            else
            {
                IsProfileIncomplete = false;
                LoadData();
            }
        }

        private void LoadData()
        {
            var ownerService = new OwnerService();
            
            // Load Species
            var speciesList = ownerService.GetSpecies();
            Species.Clear();
            foreach (var s in speciesList) Species.Add(s);
            if (Species.Any()) SelectedSpecies = Species.First();

            // Load Animals
            LoadAnimals();
            
            // Load Appointments
            LoadAppointments();
        }

        private void LoadAnimals()
        {
            if (_proprietarID == null) return;
            var ownerService = new OwnerService();
            var myAnimals = ownerService.GetAnimals(_proprietarID.Value);
            
            Animals.Clear();
            foreach (var a in myAnimals) Animals.Add(a);
        }

        private void LoadAppointments()
        {
            if (_proprietarID == null) return;
            var appointmentService = new AppointmentService();
            var appointments = appointmentService.GetOwnerAppointments(_proprietarID.Value);
            
            Appointments.Clear();
            foreach (var appt in appointments) Appointments.Add(appt);
        }

        // --- Actions ---

        public void SaveProfile()
        {
            if (string.IsNullOrWhiteSpace(AdresaDomiciliu))
            {
                MessageBox.Show("Te rugăm să introduci adresa.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var ownerService = new OwnerService();
            bool success = ownerService.CreateProprietarProfile(_loggedInUser.UtilizatorID, AdresaDomiciliu);

            if (success)
            {
                // Refresh to get the new ID
                _proprietarID = ownerService.GetProprietarID(_loggedInUser.UtilizatorID);
                IsProfileIncomplete = false;
                LoadData();
                MessageBox.Show("Profil salvat! Acum poți adăuga animale.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Eroare la salvarea profilului.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AddAnimal()
        {
            if (string.IsNullOrWhiteSpace(NewAnimalName) || SelectedSpecies == null)
            {
                MessageBox.Show("Numele și Specia sunt obligatorii.", "Validare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (NewAnimalBirthDate > DateTime.Today)
            {
                MessageBox.Show("Data nașterii nu poate fi în viitor.", "Validare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var ownerService = new OwnerService();
            bool success = ownerService.AddAnimal(_proprietarID.Value, SelectedSpecies.SpecieID, NewAnimalName, NewAnimalBirthDate, NewAnimalMicrocip);

            if (success)
            {
                MessageBox.Show("Animal adăugat cu succes!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                // Reset inputs
                NewAnimalName = "";
                NewAnimalMicrocip = "";
                NewAnimalBirthDate = DateTime.Today;
                
                // Close form
                IsAddAnimalVisible = false;

                LoadAnimals();
            }
            else
            {
                 MessageBox.Show("Eroare la adăugarea animalului. Verifică datele.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        public void ToggleAddAnimalVisibility()
        {
            IsAddAnimalVisible = !IsAddAnimalVisible;
        }

        public void NewAppointment()
        {
            System.Windows.Application.Current.MainWindow.Content = new Views.BookAppointmentView(_loggedInUser);
        }

        public void Logout()
        {
             System.Windows.Application.Current.MainWindow.Content = new System.Windows.Controls.Frame { Source = new System.Uri("Views/LoginView.xaml", System.UriKind.Relative) };
        }
    }
}
