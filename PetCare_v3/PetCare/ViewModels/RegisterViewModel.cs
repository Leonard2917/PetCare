using System.Windows;
using System.Linq;
using PetCare.Models;

namespace PetCare.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private string _nume;
        private string _prenume;
        private string _email;
        private string _password; 
        private string _confirmPassword;
        private string _telefon;
        private string _selectedRole;
        private ClinicsModel _selectedClinic;
        private bool _isClinicSelectionVisible;
        private bool _isClinicCreationVisible;
        

        private string _clinicaNouaNume;
        private string _clinicaNouaAdresa;
        private string _clinicaNouaTelefon;
        private string _clinicaNouaCUI;
        private string _nrParafa;
        private bool _isMedicParafaVisible;

        private string _errorMessage;
        
        public System.Collections.ObjectModel.ObservableCollection<string> Roles { get; } = new System.Collections.ObjectModel.ObservableCollection<string>
        {
            "Proprietar",
            "Medic Veterinar",
            "Administrator"
        };
        
        public System.Collections.ObjectModel.ObservableCollection<ClinicsModel> Clinics { get; } = new System.Collections.ObjectModel.ObservableCollection<ClinicsModel>();

        public RegisterViewModel()
        {
            SelectedRole = Roles[0]; // Default to Proprietar
            LoadClinics();
        }

        private void LoadClinics()
        {
            try
            {
                var clinicService = new ClinicService();
                var dbClinics = clinicService.GetClinics();
                Clinics.Clear();
                foreach (var c in dbClinics)
                {
                    Clinics.Add(c);
                }
            }
            catch
            {

            }
        }

        public string Nume
        {
            get => _nume;
            set { _nume = value; OnPropertyChanged(nameof(Nume)); }
        }

        public string SelectedRole
        {
            get => _selectedRole;
            set 
            { 
                _selectedRole = value; 
                OnPropertyChanged(nameof(SelectedRole)); 
                
                
                

                IsClinicCreationVisible = (_selectedRole == "Administrator");

                IsMedicParafaVisible = (_selectedRole == "Medic Veterinar");
            }
        }

        public ClinicsModel SelectedClinic
        {
            get => _selectedClinic;
            set { _selectedClinic = value; OnPropertyChanged(nameof(SelectedClinic)); }
        }

        public bool IsClinicSelectionVisible
        {
            get => _isClinicSelectionVisible;
            set { _isClinicSelectionVisible = value; OnPropertyChanged(nameof(IsClinicSelectionVisible)); }
        }

        public bool IsClinicCreationVisible
        {
            get => _isClinicCreationVisible;
            set { _isClinicCreationVisible = value; OnPropertyChanged(nameof(IsClinicCreationVisible)); }
        }

        public bool IsMedicParafaVisible
        {
            get => _isMedicParafaVisible;
            set { _isMedicParafaVisible = value; OnPropertyChanged(nameof(IsMedicParafaVisible)); }
        }

        public string NrParafa
        {
            get => _nrParafa;
            set { _nrParafa = value; OnPropertyChanged(nameof(NrParafa)); }
        }

        public string ClinicaNouaNume
        {
            get => _clinicaNouaNume;
            set { _clinicaNouaNume = value; OnPropertyChanged(nameof(ClinicaNouaNume)); }
        }

        public string ClinicaNouaAdresa
        {
            get => _clinicaNouaAdresa;
            set { _clinicaNouaAdresa = value; OnPropertyChanged(nameof(ClinicaNouaAdresa)); }
        }

        public string ClinicaNouaTelefon
        {
            get => _clinicaNouaTelefon;
            set { _clinicaNouaTelefon = value; OnPropertyChanged(nameof(ClinicaNouaTelefon)); }
        }

        public string ClinicaNouaCUI
        {
            get => _clinicaNouaCUI;
            set { _clinicaNouaCUI = value; OnPropertyChanged(nameof(ClinicaNouaCUI)); }
        }

        public string Prenume
        {
            get => _prenume;
            set { _prenume = value; OnPropertyChanged(nameof(Prenume)); }
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

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set { _confirmPassword = value; OnPropertyChanged(nameof(ConfirmPassword)); }
        }

        public string Telefon
        {
            get => _telefon;
            set { _telefon = value; OnPropertyChanged(nameof(Telefon)); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); }
        }

        public void PerformRegistration(string passwordFromBox, string confirmPasswordFromBox)
        {
            Password = passwordFromBox;
            ConfirmPassword = confirmPasswordFromBox;

            if (string.IsNullOrWhiteSpace(Nume) || string.IsNullOrWhiteSpace(Prenume) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Toate câmpurile (Nume, Prenume, Email, Parolă) sunt obligatorii.";
                return;
            }

            if (string.IsNullOrEmpty(SelectedRole))
            {
                ErrorMessage = "Te rog selectează un rol.";
                return;
            }
            


            

            if (SelectedRole == "Medic Veterinar" && string.IsNullOrWhiteSpace(NrParafa))
            {
                ErrorMessage = "Te rog completează numărul de parafă.";
                return;
            }


            if (SelectedRole == "Administrator")
            {
                if (string.IsNullOrWhiteSpace(ClinicaNouaNume) || string.IsNullOrWhiteSpace(ClinicaNouaAdresa) || 
                    string.IsNullOrWhiteSpace(ClinicaNouaTelefon) || string.IsNullOrWhiteSpace(ClinicaNouaCUI))
                {
                    ErrorMessage = "Toate detaliile clinicii (inclusiv CUI) sunt obligatorii.";
                    return;
                }
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Parolele nu coincid.";
                return;
            }

            var authService = new AuthenticationService();
            

            string roleToSave = "Owner"; 
            switch (SelectedRole)
            {
                case "Proprietar":
                    roleToSave = "Owner";
                    break;
                case "Medic Veterinar":
                    roleToSave = "Medic";
                    break;
                case "Administrator":
                    roleToSave = "Admin";
                    break;
            }
            

            int? clinicaId = null; 

            string errorResult = authService.RegisterUser(Nume, Prenume, Email, Password, Telefon, roleToSave, 
                                                        clinicaId, ClinicaNouaNume, ClinicaNouaAdresa, ClinicaNouaTelefon, ClinicaNouaCUI, NrParafa);

            if (errorResult == null) 
            {
                MessageBox.Show("Cont creat cu succes! Te rog să te autentifici.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

                System.Windows.Application.Current.MainWindow.Content = new System.Windows.Controls.Frame { Source = new System.Uri("Views/LoginView.xaml", System.UriKind.Relative) };
            }
            else
            {
                ErrorMessage = errorResult;
            }
        }
    }
}
