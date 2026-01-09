using System.Data.Entity;

namespace PetCare
{
    public partial class PetCareEntities
    {
        public PetCareEntities(string connectionString) : base(connectionString)
        {
        }
    }
}
