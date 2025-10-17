namespace Shared.DTOs.Patients.Responses
{
    public class PatientResponse
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? ProfilePicture { get; set; }
        public DateOnly BirthDate { get; set; }
        public string? BloodType { get; set; }
    }
}
