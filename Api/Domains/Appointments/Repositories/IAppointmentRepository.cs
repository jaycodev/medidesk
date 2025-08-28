using Api.Domains.Appointments.DTOs;

namespace Api.Domains.Appointments.Repositories
{
    public interface IAppointmentRepository
    {
        List<AppointmentListDTO> GetAll();
        AppointmentDetailDTO? GetById(int id);
        int Create(CreateAppointmentDTO dto); 
        int Update(int id, UpdateAppointmentStatusDTO dto);

        List<AppointmentListDTO> GetMyAppointments(int userId, string userRol);
        List<AppointmentListDTO> GetPendingAppointments(int patientId);
        List<AppointmentListDTO> GetHistory(int patientId);
    }
}
