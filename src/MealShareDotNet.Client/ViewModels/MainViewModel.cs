using CommunityToolkit.Mvvm.ComponentModel;

namespace MealShareDotNet.Client.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableObject _currentPage;

    public MainViewModel(ObservableObject initPage)
    {
        _currentPage = initPage;
    }
}
