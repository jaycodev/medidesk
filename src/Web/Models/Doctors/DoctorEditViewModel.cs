using System.ComponentModel.DataAnnotations;

namespace Web.Models.Doctors
{
    public class DoctorEditViewModel
    {
        [Display(Name = "Código")]
        public int UserId { get; set; }

        [Display(Name = "Nombre(s)")]
        public string? FirstName { get; set; }

        [Display(Name = "Apellido(s)")]
        public string? LastName { get; set; }

        [Display(Name = "Correo electrónico")]
        public string? Email { get; set; }

        [Display(Name = "Teléfono")]
        public string? Phone { get; set; }

        [Display(Name = "Especialidad")]
        public int SpecialtyId { get; set; }

        [Display(Name = "Estado")]
        public bool Status { get; set; }
    }
}
