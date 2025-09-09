namespace Shared.DTOs.Doctors.Responses
{
    public class DoctorListResponse
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SpecialtyName { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; }
        public bool Status { get; set; }
    }
}
