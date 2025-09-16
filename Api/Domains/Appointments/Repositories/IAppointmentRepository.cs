using Api.Domains.Appointments.DTOs;
using Api.Queries;

namespace Api.Domains.Appointments.Repositories
{
    public interface IAppointmentRepository
    {
        List<AppointmentListDTO> GetAll(ListQuery listQuery, AppointmentQuery query);
        List<AppointmentListDTO> GetAppointmentsByStatus(AppointmentQuery query);
        List<AppointmentListDTO> GetHistorial(int userId, string userRol);
        AppointmentDetailDTO? GetById(int id);
        List<AppointmentTimeDTO> GetByDoctorAndDate(int doctorId, DateTime date);
        List<ScheduleDTO> GetScheduleByDoctorAndDay(int doctorId, DateTime date);
        int Reserve(CreateAppointmentDTO dto);
        int Update(int id, UpdateAppointmentStatusDTO dto);
    }
}
