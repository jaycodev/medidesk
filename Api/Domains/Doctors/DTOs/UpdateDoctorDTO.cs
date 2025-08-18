using System.ComponentModel.DataAnnotations;

namespace Api.Domains.Doctors.DTOs
{
    public class UpdateDoctorDTO
    {
        [Required]
        public int SpecialtyId { get; set; }

        [Required]
        public bool Status { get; set; }
    }
}
