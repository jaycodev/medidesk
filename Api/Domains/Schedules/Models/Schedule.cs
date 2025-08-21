using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Domains.Schedules.Models
{
    public class Schedule
    {
        public int DoctorId { get; set; }

        public string Weekday { get; set; }

        public string DayWorkShift { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
    }
}
