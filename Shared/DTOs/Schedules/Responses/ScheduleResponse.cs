namespace Shared.DTOs.Schedules.Responses
{
    public class ScheduleResponse
    {
        public int DoctorId { get; set; }
        public string Weekday { get; set; } = string.Empty;
        public string DayWorkShift { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsActive { get; set; }
    }
}
