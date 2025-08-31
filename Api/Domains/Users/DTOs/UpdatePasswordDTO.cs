namespace Api.Domains.Users.DTOs
{
    public class UpdatePasswordDTO
    {
        public string NewPassword { get; set; }
        public string CurrentPassword { get; set; }
    }
}
