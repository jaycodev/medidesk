        using Api.Domains.Notification.DTOs;

namespace Api.Domains.Notification.Repositories
{
    public interface INotification
    {
        List<NotificationDTO> GetForDoctor(int doctorId);
        List<NotificationDTO> GetForPatient(int patientId);
        int Delete(int id);
    }
}
