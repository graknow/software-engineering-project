using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using MealShareDotNet.Client.Services;
using MealShareDotNet.Client.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace MealShareDotNet.Client.Views;

public partial class MainWindow : Window
{
    private readonly WindowNotificationManager _notificationManager;

    public MainWindow(string connectionString)
    {
        InitializeComponent();

        _notificationManager = new()
        {
            Position = NotificationPosition.BottomRight,
            MaxItems = 3  
        };

        var collection = new ServiceCollection();
        collection.AddSingleton(_notificationManager);

        DataContext = new MainViewModel(connectionString, collection);
    }
}
