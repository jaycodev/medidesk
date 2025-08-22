using Api.Domains.Appointment.DTOs;

namespace Api.Domains.Appointment.Repositories
{
    public interface IAppointment
    {
        List<AppointmentListDTO> GetList();
        AppointmentDetailDTO? GetById(int id);
        int Create(CreateAppointmentDTO dto); 
        int Update(int id, UpdateAppointmentStatusDTO dto);
    }
}
