namespace Shared.DTOs.Appointments.Responses
{
    public class AppointmentListResponse
    {
        public int AppointmentId { get; set; }
        public string SpecialtyName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string ConsultationType { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
