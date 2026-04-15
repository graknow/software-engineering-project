using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Primitives;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MealShareDotNet.Client.Extensions;
using MealShareDotNet.Core.Data.DTOs;
using MealShareDotNet.Core.Data.Entities;
using MealShareDotNet.Core.Services;
using MealShareDotNet.Core.Data.Queries;

namespace MealShareDotNet.Client.ViewModels.RecipeListing;

public partial class RecipeListingViewModel : ViewModelBase
{
    private readonly IRecipeService _recipeService;

    [ObservableProperty]
    private ObservableCollection<RecipeListingDTO> recipes = [];

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isEmpty;

    // Convenience property so XAML doesn't need a converter.
    public bool IsNotLoading => !IsLoading;

    // Visible when loading finished and list is empty
    public bool ShowEmpty => IsNotLoading && IsEmpty;

    public RecipeListingViewModel(IRecipeService recipeService)
    {
        _recipeService = recipeService;

        // Keep IsEmpty in sync when collection changes
        Recipes.CollectionChanged += OnRecipesCollectionChanged;
        UpdateEmptyState();

        InitializeAsync();
    }

    partial void OnIsLoadingChanged(bool value)
    {
        // notify dependent computed properties
        OnPropertyChanged(nameof(IsNotLoading));
        OnPropertyChanged(nameof(ShowEmpty));
    }

    private void OnRecipesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateEmptyState();
    }

    private void UpdateEmptyState()
    {
        IsEmpty = Recipes is null || Recipes.Count == 0;
        OnPropertyChanged(nameof(ShowEmpty));
    }

    private async void InitializeAsync()
    {
        // keep as-is but avoid swallowing exceptions silently in LoadRecipesAsync
        await LoadRecipesAsync();
    }

    public async Task LoadRecipesAsync(GetRecipeListingsQuery? query = null)
    {
        try
        {
            IsLoading = true;
            query ??= new GetRecipeListingsQuery();

            IEnumerable<RecipeListingDTO> results;
            try
            {
                results = await _recipeService.GetRecipeListingsAsync(query);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading recipes: {ex}");
                Recipes.Clear();
                UpdateEmptyState();
                throw;
            }

            // Materialize immediately and log count for diagnostics
            var list = results?.ToList() ?? new List<RecipeListingDTO>();
            Debug.WriteLine($"RecipeListingViewModel: loaded {list.Count} items");

            Recipes.Clear();
            foreach (var recipe in list)
            {
                Recipes.Add(recipe);
            }

            UpdateEmptyState();
        }
        finally
        {
            IsLoading = false;
        }
    }
}
