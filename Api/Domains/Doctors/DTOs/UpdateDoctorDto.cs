using System.ComponentModel.DataAnnotations;

namespace Api.Domains.Doctors.DTOs
{
    public class UpdateDoctorDto
    {
        public int SpecialtyId { get; set; }
        public bool Status { get; set; }
    }
}
