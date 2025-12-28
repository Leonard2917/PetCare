using System;
using System.Linq;

namespace PetCare.Models
{
    public class AuthenticationService
    {
        public UserModel Authenticate(string email, string password)
        {
            using (var context = new PetCareEntities())
            {
                string hashedPassword = Utilities.PasswordHelper.HashPassword(password);

                var userEntity = context.Utilizatoris
                                        .FirstOrDefault(u => u.Email == email && u.ParolaHash == hashedPassword);

                if (userEntity != null)
                {
                    // Map Entity to Model
                    return new UserModel
                    {
                        UtilizatorID = userEntity.UtilizatorID,
                        Nume = userEntity.Nume,
                        Prenume = userEntity.Prenume,
                        Email = userEntity.Email,
                        ParolaHash = userEntity.ParolaHash,
                        Rol = userEntity.Rol,
                        Telefon = userEntity.Telefon
                    };
                }

                return null;
            }
        }

        public string RegisterUser(string nume, string prenume, string email, string password, string telefon, string rol, int? clinicaId, string clinicaNume, string clinicaAdresa, string clinicaTelefon)
        {
            using (var context = new PetCareEntities())
            {
                // Check if email already exists
                if (context.Utilizatoris.Any(u => u.Email == email))
                {
                    return "Acest email este deja utilizat.";
                }

                // If Admin, check if Clinic Name already exists to prevent duplicates
                if (rol == "Admin")
                {
                    if (context.Clinicis.Any(c => c.Nume == clinicaNume))
                    {
                        return "O clinică cu acest nume există deja în sistem.";
                    }
                }

                var hashedPassword = Utilities.PasswordHelper.HashPassword(password);

                var newUser = new Utilizatori
                {
                    Nume = nume,
                    Prenume = prenume,
                    Email = email,
                    ParolaHash = hashedPassword,
                    Telefon = telefon,
                    Rol = rol
                };

                context.Utilizatoris.Add(newUser);
                context.SaveChanges();

                // Logic for Admin: Create new Clinic first, then Link
                if (rol == "Admin")
                {
                    var newClinic = new Clinici
                    {
                        Nume = clinicaNume,
                        Adresa = clinicaAdresa,
                        Telefon = clinicaTelefon
                    };
                    context.Clinicis.Add(newClinic);
                    context.SaveChanges();

                    var newAdmin = new Administratori
                    {
                        UtilizatorID = newUser.UtilizatorID,
                        ClinicaID = newClinic.ClinicaID
                    };
                    context.Administratoris.Add(newAdmin);
                    context.SaveChanges();
                }
                // If Role is Medic, add to Medici table (existing clinic)
                else if (rol == "Medic" && clinicaId.HasValue)
                {
                    var newMedic = new Medici
                    {
                        UtilizatorID = newUser.UtilizatorID
                    };
                    context.Medicis.Add(newMedic);
                    context.SaveChanges();

                    // Add association to clinic via MediciClinici
                    var medic = context.Medicis.Include("Clinicis").FirstOrDefault(m => m.MedicID == newMedic.MedicID);
                    var clinic = context.Clinicis.Find(clinicaId.Value);
                    if (medic != null && clinic != null)
                    {
                        medic.Clinicis.Add(clinic);
                        context.SaveChanges();
                    }
                }
                // If Role is Owner, add to Proprietari table
                else if (rol == "Owner")
                {
                    // Proprietar entry will be created later when they complete profile
                }

                return "SUCCESS";
            }
        }
    }
}
