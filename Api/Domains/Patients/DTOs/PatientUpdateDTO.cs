namespace Api.Domains.Patients.DTOs
{
    public class PatientUpdateDTO
    {
        public int UserId { get; set; }
        public DateTime? BirthDate { get; set; }
        public string BloodType { get; set; }
    }
}
