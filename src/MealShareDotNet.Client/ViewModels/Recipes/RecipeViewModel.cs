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

namespace MealShareDotNet.Client.ViewModels.Recipes;

public partial class RecipeViewModel : ViewModelBase
{
    private readonly IRecipeService _recipeService;
    
    [ObservableProperty]
    private VM _recipe = new();

    public RecipeViewModel(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    public async Task LoadRecipeAsync(long id)
    {
        // TODO: Proper error handling
        var recipe = await _recipeService.GetRecipeAsync(id) ?? throw new System.Exception("no");

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
        }).ToList();

        Recipe.Tags = recipe.Tags.Select(t => new TagVM()
        {
            Id = t.Id ?? throw new Exception("ERJEROIERJOIERJ"),
            Name = t.Name,
            Description = t.Description
        }).ToList();
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
        public long Id { get; set; }

        [ObservableProperty]
        private string _name = "";

        [ObservableProperty]
        private string? _description = "";
    }
}