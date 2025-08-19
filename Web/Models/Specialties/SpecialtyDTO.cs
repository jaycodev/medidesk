using System.ComponentModel.DataAnnotations;

namespace Web.Models.Specialties
{
    public class SpecialtyDTO
    {
        [Display(Name = "Código")]
        public int SpecialtyId { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "Ingrese un nombre")]
        public string Name { get; set; }

        [Display(Name = "Descripción")]
        public string? Description { get; set; }
    }
}