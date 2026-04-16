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
using MealShareDotNet.Client.Converters;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace MealShareDotNet.Client.ViewModels.Recipes;

public partial class RecipeViewModel : ViewModelBase
{
    private readonly IEnumerable<IRecipeService> _recipeService;
    
    [ObservableProperty]
    private VM _recipe = new();

    public RecipeViewModel(IEnumerable<IRecipeService> recipeService)
    {
        _recipeService = recipeService;
        _ = LoadRecipeAsync(1);
    }

    public async Task LoadRecipeAsync(long id, string source = "local")
    {
        // TODO: Proper error handling
        var recipe = await _recipeService.First(r => r.Name == source).GetRecipeAsync(id) ?? throw new System.Exception("no");

        Recipe.Id = recipe.Id ?? throw new Exception("");
        Recipe.Name = recipe.Name;
        Recipe.CookTime = recipe.CookTime;
        Recipe.Price = recipe.Price;
        Recipe.ServingQuantity = recipe.ServingQuantity;
        Recipe.Instructions = recipe.Instructions;
        // TODO: maybe store a formatted string instead of the datetime object
        Recipe.UpdatedDate = recipe.UpdatedDate;

        Recipe.Ingredients = recipe.Ingredients.Select(i => 
        {
            var vm = new IngredientVM();
            vm.Id = i.Id ?? throw new Exception("bleh");
            vm.Name = i.Name;
            if (i.Quantity is not null)
            {
                vm.Value = i.Quantity.ToString()!;
                vm.Unit = "ct";
                vm.UnitOptions = [];
            }
            else if (i.Mass is not null)
            {
                vm.Measure = i.Mass ?? -1;
                vm.Unit = StandardUnitConverter.Instance.BestUnitMatch(i.Mass ?? throw new Exception(), StandardUnitConverter.UnitType.MASS);
                vm.Value = StandardUnitConverter.Instance.GetUnitMeasurement(i.Mass ?? throw new Exception(), vm.Unit);
                vm.UnitOptions = new ObservableCollection<string>(StandardUnitConverter.Instance.MassConversions.Keys);
            }
            else if (i.Volume is not null)
            {
                vm.Measure = i.Volume ?? -1;
                vm.Unit = StandardUnitConverter.Instance.BestUnitMatch(i.Volume ?? throw new Exception(), StandardUnitConverter.UnitType.VOLUME);
                vm.Value = StandardUnitConverter.Instance.GetUnitMeasurement(i.Volume ?? throw new Exception(), vm.Unit);
                vm.UnitOptions = new ObservableCollection<string>(StandardUnitConverter.Instance.VolumeConversions.Keys);
            }
            
            return vm;
        }).ToList();

        Recipe.Tags = recipe.Tags.Select(t => new TagVM()
        {
            Id = t.Id ?? throw new Exception("ERJEROIERJOIERJ"),
            Name = t.Name,
            Description = t.Description
        }).ToList();
    }

    [RelayCommand]
    public void LoadUpdateView()
    {
        var args = new PageChangeEventArgs()
        {
            NextViewType = typeof(RecipeAddViewModel),
            NextPageConfig = (viewModel) =>
            {
                var addViewModel = viewModel as RecipeAddViewModel;
                _ = addViewModel.LoadRecipeAsync(_recipe.Id);
                return addViewModel;
            }
        };

        EmitPageChange(args);
    }

    public partial class VM : ViewModelBase
    {
        public long Id { get; set; }

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
        private ICollection<IngredientVM> _ingredients = [];

        [ObservableProperty]
        private ICollection<TagVM> _tags = [];
    }

    public partial class IngredientVM : ViewModelBase
    {
        public long Id { get; set; }

        public long Measure { get; set; }

        [ObservableProperty]
        private string _name = "";
        
        [ObservableProperty]
        private string _value = "";

        [ObservableProperty]
        private string? _unit;

        [ObservableProperty]
        private ObservableCollection<string> _unitOptions = [];

        // i hate you
        // also this is named to the opposite but nothing matters
        public bool IsCount => UnitOptions.Any();

        partial void OnUnitChanged(string? value)
        {
            if (IsCount)
            {

            Value = StandardUnitConverter.Instance.GetUnitMeasurement(Measure, value);
            }
        }
    }

    public partial class TagVM : ViewModelBase
    {
        public long Id { get; set; }

        [ObservableProperty]
        private string _name = "";

        [ObservableProperty]
        private string? _description = "";
    }
}