namespace Shared.DTOs.Users.Responses
{
    public class UserResponse
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public string? ProfilePicture { get; set; }

        public string? SpecialtyName { get; set; }
        public bool? Status { get; set; }

        public DateOnly? BirthDate { get; set; }
        public string? BloodType { get; set; }
    }
}
