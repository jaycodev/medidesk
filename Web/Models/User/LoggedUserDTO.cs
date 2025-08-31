namespace Web.Models.User
{
    public class LoggedUserDTO
    {
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; }
        public List<string>? Roles { get; set; }
        public string? ActiveRole { get; set; }
        public string? ProfilePicture { get; set; }
        public string FullName => $"{FirstName} {LastName}".Trim();
        public string Phone { get; set; }
    }
}
