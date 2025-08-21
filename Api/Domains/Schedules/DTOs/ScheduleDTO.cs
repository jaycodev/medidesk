namespace Api.Domains.Schedules.DTOs
{
    public class ScheduleDTO
    {
        public int DoctorId { get; set; }

        public string Weekday { get; set; }

        public string DayWorkShift { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
