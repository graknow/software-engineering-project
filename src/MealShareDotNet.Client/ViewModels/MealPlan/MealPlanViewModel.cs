using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace MealShareDotNet.Client.ViewModels.MealPlan;

public partial class MealPlanViewModel : ViewModelBase
{
    private readonly IMealPlanService _mealPlans;
    private readonly IRecipeService _recipes;

    [ObservableProperty]
    private ObservableCollection<DailyMealPlanVM> _dailyMealPlans = [];

    [ObservableProperty]
    private DateTimeOffset _filterDate = DateTimeOffset.Now;
    private DateOnly _filterDateOnly => DateOnly.FromDateTime(FilterDate.Date);

    [ObservableProperty]
    private bool _focusFilterDate = false;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddMealPlanCommand))]
    private DateTimeOffset? _newMealPlanDate;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddMealPlanCommand))]
    private TimeSpan? _newMealPlanTime;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddMealPlanCommand))]
    private RecipeDropdownVM? _newMealPlanRecipe;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddMealPlanCommand))]
    private string _newMealPlanEventName = string.Empty;

    [ObservableProperty]
    private ObservableCollection<RecipeDropdownVM> _recipeOptions = [];


    public MealPlanViewModel(IMealPlanService mealPlans, IRecipeService recipes)
    {
        _mealPlans = mealPlans;
        _recipes = recipes;

        Task.Run(async () => RecipeOptions = new ObservableCollection<RecipeDropdownVM>(
            (await _recipes.GetRecipeListingsAsync(new())).Select(r => new RecipeDropdownVM()
            {
                Id = r.Id,
                Name = r.Name,
            }))
            ).Wait();
        GenerateWeekView();
    }

    private async void GenerateWeekView()
    {
        var viewStartDate = FocusFilterDate ? _filterDateOnly.AddDays(-3) : _filterDateOnly.StartOfWeek(DayOfWeek.Sunday);

        var plans = await _mealPlans.GetWeekMealPlansAsync(viewStartDate);

        DailyMealPlans.Clear();

        for (int i = 0; i < 7; i++)
        {
            var date = viewStartDate.AddDays(i);

            var datePlans = plans
                .Where(p => DateOnly.FromDateTime(p.ScheduledTime) == date)
                .Select(p => new MealEventVM()
                {
                    RecipeName = p.Recipe.Name,
                    ScheduledTime = TimeOnly.FromDateTime(p.ScheduledTime)
                });
            
            var isCurrentDay = date.CompareTo(_filterDateOnly) == 0;

            DailyMealPlans.Add(new()
            {
                Date = date,
                Plans = new ObservableCollection<MealEventVM>(datePlans),
                Selected = isCurrentDay
            });
        }        
    }

    partial void OnFocusFilterDateChanged(bool value)
    {
        GenerateWeekView();
    }

    partial void OnFilterDateChanged(DateTimeOffset value)
    {
        GenerateWeekView();
    }

    [RelayCommand(CanExecute = nameof(CanAddMealPlan))]
    private async Task AddMealPlan()
    {
        var date = DateOnly.FromDateTime(NewMealPlanDate?.Date ?? throw new Exception());
        var time = TimeOnly.FromTimeSpan(NewMealPlanTime ?? throw new Exception());

        var dto = new MealPlanDTO()
        {
            EventName = NewMealPlanEventName,
            ScheduledTime = new DateTime(date, time),
            Recipe = new RecipeDTO()
            {
                Id = NewMealPlanRecipe!.Id,
                Name = NewMealPlanRecipe!.Name
            }
        };

        await _mealPlans.InsertMealPlanAsync(dto);

        GenerateWeekView();
    }

    private bool CanAddMealPlan()
    {
        if (NewMealPlanRecipe is null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(NewMealPlanEventName))
        {
            return false;
        }

        if (NewMealPlanDate is null)
        {
            return false;
        }

        if (NewMealPlanTime is null)
        {
            return false;
        }

        return true;
    }
    public partial class DailyMealPlanVM : ViewModelBase
    {
        [ObservableProperty]
        private bool _selected;

        [ObservableProperty]
        private DateOnly _date;

        [ObservableProperty]
        private ObservableCollection<MealEventVM> _plans = [];
    }

    public partial class MealEventVM : ViewModelBase
    {
        [ObservableProperty]
        private string _recipeName = string.Empty;

        [ObservableProperty]
        private TimeOnly? _scheduledTime;
    }

    public partial class RecipeDropdownVM : ViewModelBase
    {
        [ObservableProperty]
        private long? _id;

        [ObservableProperty]
        private string _name = string.Empty;
    }
}
