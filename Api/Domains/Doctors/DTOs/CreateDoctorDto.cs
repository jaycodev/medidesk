namespace Api.Domains.Doctors.DTOs
{
    public class CreateDoctorDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public int SpecialtyId { get; set; }
    }
}
