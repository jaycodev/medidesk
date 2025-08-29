namespace Api.Domains.Users.DTOs
{
    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public LoggedUserDTO User { get; set; }
    }
}
