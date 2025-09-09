namespace Web.Services.Notification
{
    public interface INotificationService
    {
        Task<bool> DeleteAsync(int id);
    }
}
