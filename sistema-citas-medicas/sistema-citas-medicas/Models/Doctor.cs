using System.ComponentModel.DataAnnotations;

namespace sistema_citas_medicas.Models
{
    public class Doctor : User
    {
        [Display(Name = "Código")]
        [Required(ErrorMessage = "Seleccione una especialidad")]
        public int SpecialtyId { get; set; }

        [Display(Name = "Especialidad")]
        public string SpecialtyName { get; set; }

        [Display(Name = "Estado")]
        [Required(ErrorMessage = "Seleccione un estado")]
        public bool Status { get; set; }
    }
}