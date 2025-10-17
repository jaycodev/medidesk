namespace Shared.DTOs.Account.Responses
{
    public class LoggedUserResponse
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string>? Roles { get; set; }
        public string? Phone { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
