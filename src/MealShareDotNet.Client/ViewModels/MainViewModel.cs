using CommunityToolkit.Mvvm.ComponentModel;
using MealShareDotNet.Core.Services;


namespace MealShareDotNet.Client.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";

    [ObservableProperty]
    private string _testRecipeName = "";

    private readonly IRecipeService _recipeService;

    public MainViewModel(IRecipeService recipeService)
    {
        _recipeService = recipeService;
        var task = _recipeService.GetRecipeAsync(1);
        task.Wait();
        _testRecipeName = task.Result?.Name ?? "";
    }
}
