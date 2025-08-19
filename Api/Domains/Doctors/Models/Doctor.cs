using Api.Domains.Specialties.Models;
using Api.Domains.Users.Models;

namespace Api.Domains.Doctors.Models
{
    public class Doctor : User
    {
        public Specialty Specialty { get; set; } = new Specialty();
        public bool Status { get; set; }
    }
}