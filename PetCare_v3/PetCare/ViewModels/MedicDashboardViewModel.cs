using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using PetCare.Models;

namespace PetCare.ViewModels
{
    public class MedicDashboardViewModel : BaseViewModel
    {
        private readonly UserModel _loggedInUser;
        private int? _medicID;

        // Profile Properties
        private string _nrParafa;
        private string _bio;
        private bool _isProfileComplete;

        // Schedule Properties
        public ObservableCollection<ScheduleModel> WeeklySchedule { get; set; } = new ObservableCollection<ScheduleModel>();

        // Collections
        public ObservableCollection<SelectableSpeciesModel> Specializations { get; set; } = new ObservableCollection<SelectableSpeciesModel>();
        
        // Multi-Clinic Properties
        public ObservableCollection<ClinicDTO> MyClinics { get; set; } = new ObservableCollection<ClinicDTO>();
        
        // Appointments
        public ObservableCollection<AppointmentDTO> MyAppointments { get; set; } = new ObservableCollection<AppointmentDTO>();

        // Join Requests
        public ObservableCollection<MedicRequestDTO> MedicRequests { get; set; } = new ObservableCollection<MedicRequestDTO>();

        private bool _hasRequests;
        public bool HasRequests
        {
            get => _hasRequests;
            set { _hasRequests = value; OnPropertyChanged(nameof(HasRequests)); }
        }

        public StatisticsViewModel Statistics { get; set; }

        private ClinicDTO _selectedClinic;
        public ClinicDTO SelectedClinic
        {
            get => _selectedClinic;
            set
            {
                _selectedClinic = value;
                OnPropertyChanged(nameof(SelectedClinic));
                if (_selectedClinic != null && _medicID.HasValue)
                {
                    LoadSchedule(new MedicService());
                }
            }
        }

        public MedicDashboardViewModel(UserModel user)
        {
            _loggedInUser = user;
            Statistics = new StatisticsViewModel(user);
            LoadData();
        }

        public string WelcomeMessage => $"Salut, Dr. {_loggedInUser.Nume}!";

        public string NrParafa
        {
            get => _nrParafa;
            set { _nrParafa = value; OnPropertyChanged(nameof(NrParafa)); }
        }

        public string Bio
        {
            get => _bio;
            set { _bio = value; OnPropertyChanged(nameof(Bio)); }
        }
        
        // This could determine which tab is focused initially
        public int SelectedTabIndex { get; set; } = 0; 

        public void RefreshData()
        {
            LoadData();
        }

        private void LoadData()
        {
            var medicService = new MedicService();
            var appointmentService = new AppointmentService();
            
            _medicID = medicService.GetMedicID(_loggedInUser.UtilizatorID);

            if (_medicID.HasValue)
            {
                // LOAD PROFILE
                var profile = medicService.GetMedicProfile(_medicID.Value);
                NrParafa = profile?.NrParafa;
                Bio = profile?.Bio;

                if (string.IsNullOrEmpty(NrParafa))
                {
                    MessageBox.Show("Te rugăm să îți completezi profilul (Nr. Parafă este obligatoriu).", "Profil Incomplet", MessageBoxButton.OK, MessageBoxImage.Information);
                    SelectedTabIndex = 0; // Force Profile Tab
                }

                // LOAD SPECIALTIES
                LoadSpecialties(medicService);

                // LOAD CLINICS
                LoadClinics(medicService);

                // LOAD SCHEDULE (for first clinic if available)
                if (SelectedClinic != null)
                {
                    LoadSchedule(medicService);
                }
                
                // LOAD APPOINTMENTS
                LoadAppointments(appointmentService);

                // LOAD REQUESTS
                LoadMedicRequests(medicService);
            }
        }

        private void LoadSpecialties(MedicService medicService)
        {
            var specs = medicService.GetMedicSpecialties(_medicID.Value);
            Specializations.Clear();
            foreach (var s in specs)
            {
                Specializations.Add(s);
            }
        }

        private void LoadClinics(MedicService medicService)
        {
            var clinics = medicService.GetMedicClinicsList(_medicID.Value);
            MyClinics.Clear();
            foreach (var clinic in clinics)
            {
                MyClinics.Add(clinic);
            }
            
            // Auto-select first clinic
            if (MyClinics.Count > 0)
            {
                SelectedClinic = MyClinics[0];
            }
        }

        private void LoadSchedule(MedicService medicService)
        {
            if (SelectedClinic == null) return;
            
            var schedule = medicService.GetMedicSchedule(_medicID.Value, SelectedClinic.ClinicaID);
            WeeklySchedule.Clear();
            foreach (var day in schedule)
            {
                WeeklySchedule.Add(day);
            }
        }

        private void LoadAppointments(AppointmentService appointmentService)
        {
            var appointments = appointmentService.GetMedicAppointments(_medicID.Value);
            MyAppointments.Clear();
            foreach (var appt in appointments)
            {
                appt.HasUnreadMessages = new ChatService().GetUnreadCount(appt.ProgramareID, _loggedInUser.UtilizatorID) > 0;
                MyAppointments.Add(appt);
            }
        }

        private void LoadMedicRequests(MedicService medicService)
        {
            var requests = medicService.GetMedicRequests(_medicID.Value);
            MedicRequests.Clear();
            foreach (var req in requests)
            {
                MedicRequests.Add(req);
            }
            HasRequests = MedicRequests.Count > 0;
        }

        public void SaveProfile()
        {
            if (string.IsNullOrWhiteSpace(NrParafa))
            {
                MessageBox.Show("Numărul de parafă este obligatoriu.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var selectedIds = Specializations.Where(s => s.IsSelected).Select(s => s.SpecieID).ToList();
            
            var medicService = new MedicService();
            bool profileSuccess = medicService.UpdateMedicProfile(_medicID.Value, NrParafa, Bio);
            bool specialtiesSuccess = medicService.UpdateMedicSpecialties(_medicID.Value, selectedIds);

            if (profileSuccess && specialtiesSuccess)
            {
                MessageBox.Show("Profil actualizat cu succes!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Eroare la salvarea profilului.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ViewMedicalRecord(AppointmentDTO appointment)
        {
            if (appointment == null) return;

            var medicalService = new MedicalRecordService();
            var record = medicalService.GetMedicalRecordByAppointment(appointment.ProgramareID);

            if (record != null)
            {
                var viewWindow = new Views.ViewMedicalRecordView(record);
                viewWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Nu s-a găsit fișa medicală pentru această programare.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        public void OpenChat(AppointmentDTO appointment)
        {
            if (appointment == null) return;
            
            // Partner name for medic is the Animal Owner (or Animal Name)
            // appointment.MedicNume in MedicDashboard assumes Owner Name based on my previous read of DTO mapping in Service
            // Let's verify DTO mapping in AppointmentService.GetMedicAppointments
            // Verified: MedicNume property in DTO is mapped to Owner Name in GetMedicAppointments. 
            // "MedicNume = ownerName,"
            
            var chatView = new Views.ChatView(appointment.ProgramareID, _loggedInUser.UtilizatorID, appointment.MedicNume);
            
            // Mark as read locally immediately when opening
            appointment.HasUnreadMessages = false;
            
            chatView.ShowDialog();
        }

        public void SaveSchedule()
        {
            if (SelectedClinic == null)
            {
                MessageBox.Show("Selectează o clinică pentru a salva orarul.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Convert string inputs to TimeSpan
            foreach (var item in WeeklySchedule.Where(s => s.IsWorkingDay))
            {
                if (TimeSpan.TryParse(item.OraInceputStr, out TimeSpan start) &&
                    TimeSpan.TryParse(item.OraSfarsitStr, out TimeSpan end))
                {
                    item.OraInceput = start;
                    item.OraSfarsit = end;
                }
                else
                {
                    MessageBox.Show($"Format invalid pentru ore în ziua de {item.ZiNume}. Te rugăm să folosești formatul HH:mm.", "Eroare Format", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            var medicService = new MedicService();
            bool success = medicService.UpdateMedicSchedule(_medicID.Value, SelectedClinic.ClinicaID, WeeklySchedule.ToList());

            if (success)
            {
                 MessageBox.Show("Orar actualizat cu succes!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Eroare la salvarea orarului. Verifică formatul orelor (HH:mm).", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AddClinic()
        {
            var clinicService = new ClinicService();
            var allClinics = clinicService.GetAllClinicsForBooking();
            
            // Simple selection dialog - you could make this fancier
            var availableClinics = allClinics.Where(c => !MyClinics.Any(mc => mc.ClinicaID == c.ClinicaID)).ToList();
            
            if (availableClinics.Count == 0)
            {
                MessageBox.Show("Ești deja asociat cu toate clinicile din sistem.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // For now, show a simple input box asking for Clinic ID
            // In a real app, you'd use a proper selection dialog
            var clinicList = string.Join("\n", availableClinics.Select(c => $"{c.ClinicaID}: {c.Nume} - {c.Adresa}"));
            var result = Microsoft.VisualBasic.Interaction.InputBox(
                $"Introdu ID-ul clinicii la care vrei să te asociezi:\n\n{clinicList}",
                "Adaugă Clinică",
                "");

            if (!string.IsNullOrEmpty(result) && int.TryParse(result, out int clinicID))
            {
                var medicService = new MedicService();
                if (medicService.CreateJoinRequest(_medicID.Value, clinicID))
                {
                    MessageBox.Show("Cererea a fost trimisă cu succes administratorului clinicii!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadMedicRequests(medicService);
                }
                else
                {
                    MessageBox.Show("Nu s-a putut trimite cererea. Verifică dacă ai deja o cerere activă pentru această clinică.", "Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        public void Logout()
        {
             System.Windows.Application.Current.MainWindow.Content = new System.Windows.Controls.Frame { Source = new System.Uri("Views/LoginView.xaml", System.UriKind.Relative) };
        }
    }
}
