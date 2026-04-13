using Avalonia.Controls;
using Avalonia.Controls.Notifications;

namespace MealShareDotNet.Client.Views;

public partial class MainWindow : Window
{
    private readonly WindowNotificationManager _notificationManager;

    public MainWindow()
    {
        InitializeComponent();

        _notificationManager = new()
        {
            Position = NotificationPosition.BottomRight,
            MaxItems = 3  
        };
    }
}
