namespace PetCare.Models
{
    /// <summary>
    /// Represents an authenticated user in the system.
    /// This is a simple POCO (Plain Old CLR Object) used to store user information
    /// after successful authentication.
    /// </summary>
    public class UserModel
    {
        public int UtilizatorID { get; set; }
        public string Nume { get; set; }
        public string Prenume { get; set; }
        public string Email { get; set; }
        public string ParolaHash { get; set; }
        public string Rol { get; set; }
        public string Telefon { get; set; }
    }
}
