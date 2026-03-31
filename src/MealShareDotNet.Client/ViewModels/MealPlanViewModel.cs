using CommunityToolkit.Mvvm.ComponentModel;

namespace MealShareDotNet.Client.ViewModels;

public partial class MealPlanViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";
}
