using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using MealShareDotNet.Core.Data.DTOs;
using MealShareDotNet.Core.Data.Entities;

namespace MealShareDotNet.Client.ViewModels.MealPlan;

public partial class MealPlanViewModel : ViewModelBase
{
    public partial class DailyMealPlanVM : ViewModelBase
    {
        [ObservableProperty]
        private bool _currentDay;

        [ObservableProperty]
        private string _dayOfWeek = string.Empty;

        [ObservableProperty]
        private string _date = string.Empty;

        [ObservableProperty]
        private ObservableCollection<MealPlanVM> _plans = [];
    }

    public partial class MealPlanVM : ViewModelBase
    {
        [ObservableProperty]
        private string _recipeName = string.Empty;
    }

    [ObservableProperty]
    private ObservableCollection<DailyMealPlanVM> _dailyMealPlans;

    public MealPlanViewModel()
    {
        DailyMealPlans = new ObservableCollection<DailyMealPlanVM>(new DailyMealPlanVM[7]
        {
            new()
            {
                CurrentDay = false,
                DayOfWeek = "Sunday",
                Date = "04/05/2026",
                Plans = []
            },
            new()
            {
                CurrentDay = true,
                DayOfWeek = "Monday",
                Date = "04/06/2026",
                Plans = new ObservableCollection<MealPlanVM>([
                    new()
                    {
                        RecipeName = "Recipe1 Somewhat long name"
                    },
                    new()
                    {
                        RecipeName = "TestRecipe2"
                    }
                ])
            },
            new()
            {
                CurrentDay = false,
                DayOfWeek = "Tuesday",
                Date = "04/07/2026",
                Plans = []
            },
            new()
            {
                CurrentDay = false,
                DayOfWeek = "Wednesday",
                Date = "04/08/2026",
                Plans = []
            },
            new()
            {
                CurrentDay = false,
                DayOfWeek = "Thursday",
                Date = "04/09/2026",
                Plans = []
            },
            new()
            {
                CurrentDay = false,
                DayOfWeek = "Friday",
                Date = "04/10/2026",
                Plans = []
            },
            new()
            {
                CurrentDay = false,
                DayOfWeek = "Saturday",
                Date = "04/11/2026",
                Plans = []
            },
        });

        DailyMealPlans[1].CurrentDay = true;
    }
}
