using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using MealShareDotNet.Core.Data.DTOs;
using MealShareDotNet.Core.Data.Entities;
using MealShareDotNet.Core.Services;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls.Notifications;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using MealShareDotNet.Client.Services;

namespace MealShareDotNet.Client.ViewModels.Recipes;

public partial class RecipeAddViewModel : ViewModelBase
{
    private readonly IRecipeService _recipeService;
    private readonly INotificationService _notifications;
    
    [ObservableProperty]
    private VM _recipe = new();

    private RecipeDTO _dto = new();

    public RecipeAddViewModel(IRecipeService recipeService, INotificationService notificationService)
    {
        _recipeService = recipeService;
        _notifications = notificationService;
    }

    public async Task LoadRecipeAsync(long id)
    {
        // TODO: Proper error handling
        _dto = await _recipeService.GetRecipeAsync(id) ?? throw new System.Exception("no");

        Recipe.Id = _dto.Id;
        Recipe.Name = _dto.Name;
        Recipe.CookTime = _dto.CookTime;
        Recipe.Price = _dto.Price;
        Recipe.ServingQuantity = _dto.ServingQuantity;
        Recipe.Instructions = _dto.Instructions;
        // TODO: maybe store a formatted string instead of the datetime object
        Recipe.UpdatedDate = _dto.UpdatedDate;

        Recipe.Ingredients = new ObservableCollection<IngredientVM>(_dto.Ingredients.Select(i => 
        {
            var vm = new IngredientVM();
            vm.Id = i.Id;
            vm.Name = i.Name;
            if (i.Quantity is not null)
            {
                vm.Value = i.Quantity.ToString()!;
                vm.Unit = "";
                vm.UnitOptions = [];
            }
            else if (i.Mass is not null)
            {
                vm.Value = i.Mass.ToString()!;
                vm.Unit = "massPlaceholder";
                vm.UnitOptions = []; // TODO: replace with known list of mass units
            }
            else if (i.Volume is not null)
            {
                vm.Value = i.Volume.ToString()!;
                vm.Unit = "volumePlaceholder";
                vm.UnitOptions = []; // TODO: replace with know list of volume units
            }
            
            return vm;
        }).ToList());

        Recipe.Tags = new ObservableCollection<TagVM>(_dto.Tags.Select(t => new TagVM()
        {
            Id = t.Id ?? throw new Exception("ERJEROIERJOIERJ"),
            Name = t.Name,
            Description = t.Description
        }).ToList());
    }

    [RelayCommand(CanExecute = nameof(IsRecipeValid))]
    public async Task InsertOrUpdateRecipe()
    {
        _dto.Name = Recipe.Name;
        _dto.ServingQuantity = Recipe.ServingQuantity;
        _dto.CookTime = Recipe.CookTime;
        _dto.Price = Recipe.Price;
        _dto.Instructions = Recipe.Instructions;

        _dto.Ingredients = Recipe.Ingredients.Select(vm =>
        {
            var i = new IngredientDTO();

            i.Id = vm.Id;
            i.Name = vm.Name;

            // TODO: Ingredient conversion logic and additions

            return i;
        }).ToList();

        _dto.Tags = Recipe.Tags.Select(vm => new TagDTO()
        {
            Id = vm.Id,
            Name = vm.Name,
            Description = vm.Description,
        }).ToList();

        RecipeDTO result;

        // TODO: Move this logic to service
        if (_dto.Id is null)
        {
            result = await _recipeService.InsertRecipeAsync(_dto);
        }
        else
        {
            result = await _recipeService.UpdateRecipeAsync(_dto);
        }

        var args = new PageChangeEventArgs()
        {
            NextViewType = typeof(RecipeViewModel),
            NextPageConfig = (view) =>
            {
                var viewVM = view as RecipeViewModel;
                _ = viewVM.LoadRecipeAsync(_dto.Id ?? -1);
                return viewVM;   
            }
        };

        EmitPageChange(args);
    }

    public bool IsRecipeValid()
    {
        return true;
    }

    [RelayCommand]
    public void AddTag()
    {
        Recipe.Tags.Add(new());
        _notifications.ShowInfo("test", "message");
    }

    [RelayCommand]
    public void AddIngredient()
    {
        Recipe.Ingredients.Add(new());
    }

    [RelayCommand]
    public void RemoveTag(TagVM vm)
    {
        Recipe.Tags.Remove(vm);
    }

    [RelayCommand]
    public void RemoveIngredient(IngredientVM vm)
    {
        Recipe.Ingredients.Remove(vm);
    }

    public partial class VM : ViewModelBase
    {
        public long? Id { get; set; }

        [ObservableProperty]
        private string _name = "";

        [ObservableProperty]
        private int? _cookTime;

        [ObservableProperty]
        private int? _price;

        [ObservableProperty]
        private int? _servingQuantity;

        [ObservableProperty]
        private string _instructions = "";

        [ObservableProperty]
        private DateTime _updatedDate;

        [ObservableProperty]
        private ObservableCollection<IngredientVM> _ingredients = [];

        [ObservableProperty]
        private ObservableCollection<TagVM> _tags = [];
    }

    public partial class IngredientVM : ViewModelBase
    {
        public long? Id { get; set; }

        [ObservableProperty]
        private string _name = "";
        
        [ObservableProperty]
        private string _value = "";

        [ObservableProperty]
        private string? _unit;

        [ObservableProperty]
        private IEnumerable<string> _unitOptions = [];
    }

    public partial class TagVM : ViewModelBase
    {
        public long? Id { get; set; }

        [ObservableProperty]
        private string _name = "";

        [ObservableProperty]
        private string? _description = "";
    }
}