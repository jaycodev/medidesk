namespace Shared.DTOs.Schedules.Responses
{
    public class ScheduleByDateResponse
    {
        public string DayWorkShift { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
