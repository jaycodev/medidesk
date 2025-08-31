using Api.Domains.Users.Models;

namespace Api.Domains.Patients.Models
{
    public class Patient : User
    {
        public DateOnly BirthDate { get; set; }
        public string BloodType { get; set; }
    }
}