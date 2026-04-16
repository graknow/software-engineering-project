using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using Avalonia.Metadata;
using CommunityToolkit.Mvvm.ComponentModel;
using MealShareDotNet.Core.Data.DTOs;
using MealShareDotNet.Core.Data.Entities;
using MealShareDotNet.Core.Services;

namespace MealShareDotNet.Client.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    [ObservableProperty]
    private RecipeDTO? _dailyRecipe;

    public HomeViewModel(IEnumerable<IRecipeService> service)
    {
        Task.Run(async () => _dailyRecipe = await service.First(r => r.Name == "local").GetRandomDailyRecipeAsync());
    }
}
