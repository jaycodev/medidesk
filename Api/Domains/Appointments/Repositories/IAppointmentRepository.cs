using Api.Domains.Appointment.DTOs;

namespace Api.Domains.Appointment.Repositories
{
    public interface IAppointmentRepository
    {
        List<AppointmentListDTO> GetAll();
        AppointmentDetailDTO? GetById(int id);
        int Create(CreateAppointmentDTO dto); 
        int Update(int id, UpdateAppointmentStatusDTO dto);

        //List<MyAppointmentDTO> GetMyAppointments(int patientId);
        //List<PendingAppointmentDTO> GetPendingAppointments(int patientId);
        //List<AppointmentHistoryDTO> GetHistory(int patientId);
    }
}
