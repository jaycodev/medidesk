namespace Api.Domains.Appointments.Models
{
    public class Appointment
    {
        public int appointment_id { get; set; }

        public int doctor_id { get; set; }
        public int patient_id { get; set; }
        public int specialty_id { get; set; }
        public DateTime date { get; set; } = DateTime.Today;

        public TimeSpan time { get; set; }

        public string consultation_type { get; set; }

        public string symptoms { get; set; }

        public string status { get; set; } = "pendiente";
    }
}