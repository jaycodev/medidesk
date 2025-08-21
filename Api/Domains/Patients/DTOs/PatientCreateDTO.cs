using Api.Domains.Users.DTOs;

namespace Api.Domains.Patients.DTOs
{
    public class PatientCreateDTO : UserDTO
    {
        public DateTime? BirthDate { get; set; }
        public string BloodType { get; set; }
    }
}
