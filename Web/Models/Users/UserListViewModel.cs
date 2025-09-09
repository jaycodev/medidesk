using System.ComponentModel.DataAnnotations;

namespace Web.Models.Users
{
    public class UserListViewModel
    {
        [Display(Name = "Código")]
        public int UserId { get; set; }

        [Display(Name = "Nombre(s)")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Apellido(s)")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Teléfono")]
        public string? Phone { get; set; }

        [Display(Name = "Rol(es)")]
        public List<string> Roles { get; set; } = new List<string>();

        [Display(Name = "Foto perfil")]
        public string? ProfilePicture { get; set; }

        public bool CanDelete { get; set; }
    }
}
