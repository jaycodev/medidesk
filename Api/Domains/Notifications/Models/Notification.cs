namespace Api.Domains.Notifications.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int? DoctorId { get; set; }
        public int? PatientId { get; set; }
        public int AppointmentId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}