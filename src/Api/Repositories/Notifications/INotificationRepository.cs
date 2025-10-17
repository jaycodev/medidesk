using Shared.DTOs.Notifications;

namespace Api.Repositories.Notifications
{
    public interface INotificationRepository
    {
        List<NotificationResponse> GetForDoctor(int doctorId);
        List<NotificationResponse> GetForPatient(int patientId);
        int Delete(int id);
    }
}
