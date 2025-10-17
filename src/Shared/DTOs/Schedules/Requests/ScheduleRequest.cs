namespace Shared.DTOs.Schedules.Requests
{
    public class ScheduleRequest
    {
        public int DoctorId { get; set; }
        public string Weekday { get; set; } = string.Empty;
        public string DayWorkShift { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
