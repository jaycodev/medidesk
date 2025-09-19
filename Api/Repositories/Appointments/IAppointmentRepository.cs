using Api.Queries;
using Shared.DTOs.Appointments.Requests;
using Shared.DTOs.Appointments.Responses;

namespace Api.Repositories.Appointments
{
    public interface IAppointmentRepository
    {
        List<AppointmentListResponse> GetAll(ListQuery listQuery, AppointmentQuery query);
        List<AppointmentListResponse> GetAppointmentsByStatus(AppointmentQuery query);
        List<AppointmentListResponse> GetHistorial(int userId, string userRol);
        AppointmentResponse? GetById(int id);
        List<AppointmentTimeResponse> GetByDoctorAndDate(int doctorId, DateTime date);
        int Reserve(CreateAppointmentRequest request);
        int Update(int id, UpdateAppointmentRequest request);
    }
}
