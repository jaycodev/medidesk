using Api.Domains.Notifications.DTOs;

namespace Api.Domains.Notifications.Repositories
{
    public interface INotificationRepository
    {
        List<NotificationListDTO> GetForDoctor(int doctorId);
        List<NotificationListDTO> GetForPatient(int patientId);
        int Delete(int id);
    }
}
