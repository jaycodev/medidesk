using System.ComponentModel.DataAnnotations;

namespace sistema_citas_medicas.Models
{
    public class Specialty
    {
        [Display(Name = "Código")]
        public int SpecialtyId { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "Ingrese un nombre")]
        public string Name { get; set; }

        [Display(Name = "Descripción")]
        public string Description { get; set; }
    }
}