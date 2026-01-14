using System.Collections.ObjectModel;
using System.Linq;
using PetCare.Models;

namespace PetCare.ViewModels
{
    public class StatisticsViewModel : BaseViewModel
    {
        private readonly StatisticsService _statisticsService;
        private readonly UserModel _currentUser;

        public ObservableCollection<ClinicStat> TopClinics { get; set; } = new ObservableCollection<ClinicStat>();
        public ObservableCollection<MedicStat> TopMedics { get; set; } = new ObservableCollection<MedicStat>();
        public ObservableCollection<MaterialStat> TopMaterials { get; set; } = new ObservableCollection<MaterialStat>();
        public ObservableCollection<RevenueStat> RevenueStats { get; set; } = new ObservableCollection<RevenueStat>();
        public ObservableCollection<AnimalTypeStat> AnimalStats { get; set; } = new ObservableCollection<AnimalTypeStat>();

        public bool IsAdmin => _currentUser?.Rol == "Admin";

        public StatisticsViewModel(UserModel user)
        {
            _currentUser = user;
            _statisticsService = new StatisticsService();
            LoadStats();
        }

        public void LoadStats()
        {

            var clinics = _statisticsService.GetTopClinics();
            TopClinics.Clear();
            foreach (var c in clinics) TopClinics.Add(c);

            var medics = _statisticsService.GetTopMedics();
            TopMedics.Clear();
            foreach (var m in medics) TopMedics.Add(m);

            var animals = _statisticsService.GetAnimalTypeDistribution();
            AnimalStats.Clear();
            foreach (var a in animals) AnimalStats.Add(a);


            if (IsAdmin)
            {
                var materials = _statisticsService.GetTopConsumedMaterials();
                TopMaterials.Clear();
                foreach (var mat in materials) TopMaterials.Add(mat);

                int? clinicaId = GetAdminClinicId(_currentUser.UtilizatorID);
                if (clinicaId.HasValue)
                {
                    var revenue = _statisticsService.GetTotalRevenuePerMedic(clinicaId.Value);
                    RevenueStats.Clear();
                    foreach (var r in revenue) RevenueStats.Add(r);
                }
            }
        }

        private int? GetAdminClinicId(int userId)
        {
            using (var context = new PetCareEntities())
            {
                return context.Administratoris
                    .Where(a => a.UtilizatorID == userId)
                    .Select(a => a.ClinicaID)
                    .FirstOrDefault();
            }
        }
    }
}
