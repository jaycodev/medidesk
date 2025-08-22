namespace Api.Domains.Appointment.DTOs
{
    public class AppointmentListDTO
    {
        public int AppointmentId { get; set; }
        public string SpecialtyName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string ConsultationType { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
