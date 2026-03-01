using System.Linq;
using Clinic_BD.Data;
using Clinic_BD.Data.Entities;

namespace Clinic_BD.Services
{
    public class AuthService
    {
        public Doctor AuthenticateAdmin(string email, string password)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Doctors.FirstOrDefault(d => 
                    d.Email == email && d.Password == password);
            }
        }
    }
}