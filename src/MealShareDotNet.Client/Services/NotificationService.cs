using Avalonia.Controls.Notifications;

namespace MealShareDotNet.Client.Services;

public class NotificationService : INotificationService
{
    private readonly WindowNotificationManager _manager;

    public NotificationService(WindowNotificationManager manager)
    {
        _manager = manager;
    }

    public void ShowInfo(string title, string message) =>
        _manager.Show(new Notification(title, message, NotificationType.Information));

    public void ShowSuccess(string title, string message) =>
        _manager.Show(new Notification(title, message, NotificationType.Success));

    public void ShowWarning(string title, string message) =>
        _manager.Show(new Notification(title, message, NotificationType.Warning));

    public void ShowError(string title, string message) =>
        _manager.Show(new Notification(title, message, NotificationType.Error));
}