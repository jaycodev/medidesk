using System.ComponentModel.DataAnnotations;

namespace sistema_citas_medicas.Models
{
    public class Medico : Usuario
    {
        [Display(Name = "ID especialidad")]
        [Required(ErrorMessage = "Seleccione una especialidad")]
        public int IdEspecialidad { get; set; }

        [Display(Name = "Especialidad")]
        public string EspecialidadNombre { get; set; }

        [Display(Name = "Estado")]
        [Required(ErrorMessage = "Seleccione un estado")]
        public bool Estado { get; set; }
    }
}