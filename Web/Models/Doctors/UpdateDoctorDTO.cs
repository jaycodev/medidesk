using System.ComponentModel.DataAnnotations;

namespace Web.Models.Doctors
{
    public class UpdateDoctorDTO
    {
        [Required]
        public int SpecialtyId { get; set; }

        [Required]
        public bool Status { get; set; }
    }
}
