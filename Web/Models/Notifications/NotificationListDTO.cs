namespace Web.Models.Notifications
{
    public class NotificationListDTO
    {
        public int NotificationId { get; set; }
        public int? DoctorId { get; set; }
        public int? PatientId { get; set; }
        public int AppointmentId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
