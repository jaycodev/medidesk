using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class Doctor : User
    {
        [Display(Name = "Especialidad")]
        [Required(ErrorMessage = "Seleccione una especialidad")]
        public int SpecialtyId { get; set; }

        [Display(Name = "Especialidad")]
        public string SpecialtyName { get; set; }

        [Display(Name = "Estado")]
        [Required(ErrorMessage = "Seleccione un estado")]
        public bool Status { get; set; }
    }
}