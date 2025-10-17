using Shared.DTOs.Appointments.Requests;
using Shared.DTOs.Appointments.Responses;
using Web.Models.Appointments;

namespace Web.Mappers
{
    public static class AppointmentMapper
    {
        // ViewModel -> Create Request
        public static CreateAppointmentRequest ToCreateRequest(this AppointmentCreateViewModel model)
            => new()
            {
                DoctorId = model.DoctorId,
                PatientId = model.PatientId,
                SpecialtyId = model.SpecialtyId,
                Date = model.Date ?? DateOnly.MinValue,
                Time = model.Time,
                ConsultationType = model.ConsultationType,
                Symptoms = model.Symptoms
            };

        // Response -> List ViewModel
        public static AppointmentListViewModel ToListViewModel(this AppointmentListResponse response)
            => new()
            {
                AppointmentId = response.AppointmentId,
                SpecialtyName = response.SpecialtyName,
                DoctorName = response.DoctorName,
                PatientName = response.PatientName,
                ConsultationType = response.ConsultationType,
                Date = response.Date,
                Time = response.Time,
                Status = response.Status
            };

        // Response -> Detail / View ViewModel
        public static AppointmentViewModel ToViewModel(this AppointmentResponse response)
            => new()
            {
                AppointmentId = response.AppointmentId,
                SpecialtyName = response.SpecialtyName,
                DoctorName = response.DoctorName,
                PatientName = response.PatientName,
                ConsultationType = response.ConsultationType,
                Date = response.Date,
                Time = response.Time,
                Status = response.Status,
                Symptoms = response.Symptoms
            };
    }
}
