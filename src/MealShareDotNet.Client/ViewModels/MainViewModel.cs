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
using MealShareDotNet.Client.ViewModels.Recipes;
using Avalonia.Controls.Notifications;
using MealShareDotNet.Client.Services;
using Avalonia.Controls;
using MealShareDotNet.Client.ViewModels.RecipeListing;

namespace MealShareDotNet.Client.ViewModels;

public record MenuItem(string Title, Func<ViewModelBase> CreatePage);

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<MenuItem> _menuItems;

    [ObservableProperty]
    private MenuItem? _selectedMenuItem;

    [ObservableProperty]
    private ViewModelBase _currentPage;

    private readonly ServiceProvider _provider;

    public MainViewModel(string connectionString, IServiceCollection windowServices, MenuItem? initialPage = null)
    {
        var collection = windowServices;
        collection.AddTransient<IRecipeRepository>(s => new SqliteRecipeRepository(connectionString));
        collection.AddTransient<IRecipeService, RepositoryRecipeService>();
        collection.AddTransient<IMealPlanRepository>(s => new SqliteMealPlanRepository(connectionString));
        collection.AddTransient<IMealPlanService, RepositoryMealPlanService>();
        collection.AddSingleton<INotificationService, NotificationService>();

        // Provide pages for automated dependency injection
        collection.AddTransient<HomeViewModel>();
        collection.AddTransient<MealPlanViewModel>();
        collection.AddTransient<RecipeViewModel>();
        collection.AddTransient<RecipeAddViewModel>();
        collection.AddTransient<RecipeListingViewModel>();

        _provider = collection.BuildServiceProvider();

        _menuItems = new ObservableCollection<MenuItem>([
            new MenuItem("Home", () => _provider.GetRequiredService<HomeViewModel>()),
            new MenuItem("Recipes", () => _provider.GetRequiredService<RecipeListingViewModel>()),
            new MenuItem("Meal Plan", () => _provider.GetRequiredService<MealPlanViewModel>()),
        ]);

        var page = initialPage ?? MenuItems.First();
        _selectedMenuItem = page;
        _currentPage = page.CreatePage();
        _currentPage.PageChangeEventHandler += OnPageChange;
    }

    public void OnPageChange(object? sender, PageChangeEventArgs args)
    {
        CurrentPage.PageChangeEventHandler -= OnPageChange;
        CurrentPage = args.NextPage;
        CurrentPage.PageChangeEventHandler += OnPageChange;
    }

    partial void OnSelectedMenuItemChanged(MenuItem? value)
    {
        CurrentPage = value!.CreatePage();
        CurrentPage.PageChangeEventHandler += OnPageChange;
    }
}
