using Shared.DTOs.Appointments.Requests;
using Shared.DTOs.Appointments.Responses;

namespace Api.Repositories.Appointments
{
    public interface IAppointmentRepository
    {
        List<AppointmentListResponse> GetAll();
        List<AppointmentListResponse> GetAppointmentsByStatus(int userId, string userRol, string? status);
        List<AppointmentListResponse> GetHistorial(int userId, string userRol);
        AppointmentResponse? GetById(int id);
        List<AppointmentTimeResponse> GetByDoctorAndDate(int doctorId, DateTime date);
        int Reserve(CreateAppointmentRequest request);
        int Update(int id, UpdateAppointmentRequest request);
    }
}
