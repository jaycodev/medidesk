using Api.Domains.Appointments.DTOs;

namespace Api.Domains.Appointments.Repositories
{
    public interface IAppointmentRepository
    {
        List<AppointmentListDTO> GetAll();
        List<AppointmentListDTO> GetAppointmentsByStatus(int userId, string userRol, string status);
        List<AppointmentListDTO> GetHistorial(int userId, string userRol);
        AppointmentDetailDTO? GetById(int id);
        int Create(CreateAppointmentDTO dto); 
        int Update(int id, UpdateAppointmentStatusDTO dto);
    }
}
