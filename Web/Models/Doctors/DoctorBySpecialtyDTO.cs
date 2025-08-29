namespace Web.Models.Doctors
{
    public class DoctorBySpecialtyDTO
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int SpecialtyId { get; set; }
    }
}
