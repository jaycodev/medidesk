using System.ComponentModel.DataAnnotations;

namespace Web.Models.Doctors
{
    public class DoctorDetailViewModel
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

        [Display(Name = "Especialidad")]
        public string SpecialtyName { get; set; } = string.Empty;

        [Display(Name = "Foto perfil")]
        public string? ProfilePicture { get; set; }

        [Display(Name = "Estado")]
        public bool Status { get; set; }
    }
}
