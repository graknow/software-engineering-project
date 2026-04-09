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

    public partial class DailyMealPlanVM : ViewModelBase
    {
        [ObservableProperty]
        private bool _selected;

        [ObservableProperty]
        private DateOnly _date;

        [ObservableProperty]
        private ObservableCollection<MealPlanVM> _plans = [];
    }

    public partial class MealPlanVM : ViewModelBase
    {
        [ObservableProperty]
        private string _recipeName = string.Empty;

        [ObservableProperty]
        private TimeOnly? _scheduledTime;
    }

    [ObservableProperty]
    private ObservableCollection<DailyMealPlanVM> _dailyMealPlans = [];

    [ObservableProperty]
    private DateTimeOffset _selectedDateTime = DateTime.Now;

    private DateOnly _selectedDate => DateOnly.FromDateTime(SelectedDateTime.DateTime);

    [ObservableProperty]
    private DateTimeOffset? _newMealPlanDate;

    [ObservableProperty]
    private TimeSpan? _newMealPlanTime;

    [ObservableProperty]
    private MealPlanDTO _newMealPlan = new();

    [ObservableProperty]
    private ObservableCollection<RecipeListingDTO> _recipeOptions = [];

    [ObservableProperty]
    private bool _focusSelectedDate = false;

    public MealPlanViewModel(IMealPlanService mealPlans, IRecipeService recipes)
    {
        _mealPlans = mealPlans;
        _recipes = recipes;

        Task.Run(async () => RecipeOptions = new ObservableCollection<RecipeListingDTO>(
            await _recipes.GetRecipeListingsAsync(new()))
            ).Wait();
        GenerateWeekView();
    }

    private async void GenerateWeekView()
    {
        var viewStartDate = FocusSelectedDate ? _selectedDate.AddDays(-3) : _selectedDate.StartOfWeek(DayOfWeek.Sunday);

        var plans = await _mealPlans.GetWeekMealPlansAsync(viewStartDate);

        DailyMealPlans.Clear();

        for (int i = 0; i < 7; i++)
        {
            var date = viewStartDate.AddDays(i);

            var datePlans = plans
                .Where(p => DateOnly.FromDateTime(p.ScheduledTime) == date)
                .Select(p => new MealPlanVM()
                {
                    RecipeName = p.Recipe.Name,
                    ScheduledTime = TimeOnly.FromDateTime(p.ScheduledTime)
                });
            
            var isCurrentDay = date.CompareTo(_selectedDate) == 0;

            DailyMealPlans.Add(new()
            {
                Date = date,
                Plans = new ObservableCollection<MealPlanVM>(datePlans),
                Selected = isCurrentDay
            });
        }        
    }

    partial void OnFocusSelectedDateChanged(bool value)
    {
        GenerateWeekView();
    }

    partial void OnSelectedDateTimeChanged(DateTimeOffset value)
    {
        GenerateWeekView();
    }

    [RelayCommand(CanExecute = nameof(CanAddMealPlan))]
    private void AddMealPlan()
    {
        throw new Exception(_newMealPlan.Recipe.Name);
    }

    private bool CanAddMealPlan()
    {
        if (NewMealPlan is null)
        {
            return false;
        }

        if (String.IsNullOrWhiteSpace(NewMealPlan.EventName))
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
}
