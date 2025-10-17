using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Appointments.Requests;
using Shared.DTOs.Appointments.Responses;

namespace Web.Services.Appointment
{
    public interface IAppointmentService
    {
        Task<List<AppointmentListResponse>> GetListAsync(string filter, int? userId = null, string? userRol = null);
        Task<AppointmentResponse?> GetByIdAsync(int id);
        Task<List<AppointmentTimeResponse>> GetByDateAsync(int doctorId, DateTime date);
        Task<HttpResponseMessage> ReserveAsync(CreateAppointmentRequest request);
        Task<HttpResponseMessage> UpdateStatusAsync(int id, string status);

        FileResult GeneratePdf(List<AppointmentListResponse> appointments, string title, string userRole);
        FileResult GenerateExcel(List<AppointmentListResponse> appointments, string title, string userRole);
    }
}
