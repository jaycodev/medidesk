namespace Api.Domains.Appointments.DTOs
{
    public class CreateAppointmentDTO
    {
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public int SpecialtyId { get; set; }
        public DateOnly Date { get; set; }
        public TimeSpan Time { get; set; }
        public string ConsultationType { get; set; } = string.Empty;
        public string? Symptoms { get; set; }
    }
}
