namespace Web.Models.Account
{
    public class UserSession
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
        public string? ActiveRole { get; set; }
        public string? ProfilePicture { get; set; }
        public string FullName => $"{FirstName} {LastName}".Trim();
        public string? Phone { get; set; }
    }
}
