using System.ComponentModel.DataAnnotations;

namespace Web.Models.User
{
    public class UserEditViewModel
    {

        [Display(Name = "Código")]
        public int UserId { get; set; }

        [Display(Name = "Nombre(s)")]
        public string FirstName { get; set; }

        [Display(Name = "Apellido(s)")]
        public string LastName { get; set; }

        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Display(Name = "Contraseña")]
        public string? Password { get; set; }

        [Display(Name = "Teléfono")]
        public string Phone { get; set; }

        [Display(Name = "Rol(es)")]
        public string SelectedRoleCombo { get; set; }

        [Display(Name = "Rol(es)")]
        public List<string> Roles { get; set; } = new List<string>();

        [Display(Name = "Foto perfil")]
        public string? ProfilePicture { get; set; }

        public string? ActiveRole { get; set; }

        public DateTime? BirthDate { get; set; } 
        public string? BloodType { get; set; }  
        public int? SpecialtyId { get; set; }    
        public bool? Status { get; set; }
        public bool CanDelete { get; set; }

    }
}
