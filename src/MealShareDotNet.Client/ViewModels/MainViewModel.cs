using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using MealShareDotNet.Core.Services;
using MealShareDotNet.Core.Repositories;
using MealShareDotNet.Client.ViewModels.MealPlan;
using MealShareDotNet.Client.ViewModels.RecipeListing;

namespace MealShareDotNet.Client.ViewModels;

public record MenuItem(string Title, Func<ObservableObject> CreatePage);

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<MenuItem> _menuItems;

    [ObservableProperty]
    private MenuItem? _selectedMenuItem;

    [ObservableProperty]
    private ObservableObject? _currentPage;

    private ServiceProvider _provider;

    public MainViewModel(string connectionString, MenuItem? initialPage = null)
    {
        var collection = new ServiceCollection();
        collection.AddTransient<IRecipeRepository>(s => new SqliteRecipeRepository(connectionString));
        collection.AddTransient<IRecipeService, RepositoryRecipeService>();
        collection.AddTransient<IMealPlanRepository>(s => new SqliteMealPlanRepository(connectionString));
        collection.AddTransient<IMealPlanService, RepositoryMealPlanService>();

        // Provide pages for automated dependency injection
        collection.AddTransient<HomeViewModel>();
        collection.AddTransient<MealPlanViewModel>();
        collection.AddTransient<RecipeListingViewModel>();

        _provider = collection.BuildServiceProvider();

        _menuItems = new ObservableCollection<MenuItem>([
            new MenuItem("Home", () => _provider.GetRequiredService<HomeViewModel>()),
            new MenuItem("Meal Plan", () => _provider.GetRequiredService<MealPlanViewModel>()),
            new MenuItem("Recipe Listing", () => _provider.GetRequiredService<RecipeListingViewModel>()),
        ]);

        var page = initialPage ?? MenuItems.First();
        _selectedMenuItem = page;
        _currentPage = page.CreatePage();
    }

    partial void OnSelectedMenuItemChanged(MenuItem? value)
    {
        CurrentPage = value?.CreatePage();
    }
}
