namespace Api.Domains.Doctors.DTOs
{
    public class DoctorListDTO
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SpecialtyName { get; set; } = string.Empty;
        public string ProfilePicture { get; set; }
        public bool Status { get; set; }
    }
}
