namespace Shared.DTOs.Appointments.Requests
{
    public class CreateAppointmentRequest
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
