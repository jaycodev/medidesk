using Web.Models.User;

namespace Web.Models.Patients
{
    public class PatientCreateDTO : UserDTO
    {
        public DateTime? BirthDate { get; set; }
        public string BloodType { get; set; }
    }
}
