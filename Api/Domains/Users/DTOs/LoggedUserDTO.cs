namespace Api.Domains.Users.DTOs
{
    public class LoggedUserDTO
    {
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; }
        public List<string>? Roles { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
