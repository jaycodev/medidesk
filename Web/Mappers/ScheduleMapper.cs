using Shared.DTOs.Schedules.Requests;
using Shared.DTOs.Schedules.Responses;
using Web.Models.Schedules;

namespace Web.Mappers
{
    public static class ScheduleMapper
    {
        // Response -> ViewModel
        public static ScheduleViewModel ToViewModel(this ScheduleResponse response)
        {
            return new ScheduleViewModel
            {
                DoctorId = response.DoctorId,
                Weekday = response.Weekday,
                DayWorkShift = response.DayWorkShift,
                StartTime = response.StartTime,
                EndTime = response.EndTime,
                IsActive = response.IsActive
            };
        }

        public static List<ScheduleViewModel> ToViewModelList(this List<ScheduleResponse> responses)
        {
            return responses.Select(r => r.ToViewModel()).ToList();
        }

        // ViewModel -> Request
        public static ScheduleRequest ToRequest(this ScheduleViewModel viewModel)
        {
            return new ScheduleRequest
            {
                DoctorId = viewModel.DoctorId,
                Weekday = viewModel.Weekday,
                DayWorkShift = viewModel.DayWorkShift,
                StartTime = viewModel.StartTime,
                EndTime = viewModel.EndTime,
                IsActive = viewModel.IsActive
            };
        }

        public static List<ScheduleRequest> ToRequestList(this List<ScheduleViewModel> viewModels)
        {
            return viewModels.Select(v => v.ToRequest()).ToList();
        }
    }
}
