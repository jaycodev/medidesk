using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class Notification
    {
        [Display(Name = "Notificacion")]
        public int NotificationId { get; set; }

        [Display(Name = "Doctor")]
        public int? DoctorId { get; set; }

        [Display(Name = "Paciente")]
        public int? PatientId { get; set; }

        [Display(Name = "Id Cita")]
        public int AppointmentId { get; set; }

        [Display(Name = "Mensaje")]
        public string Message { get; set; }

        public bool IsRead { get; set; }

        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreatedAt { get; set; }

        public string Role { get; set; }
    }
}