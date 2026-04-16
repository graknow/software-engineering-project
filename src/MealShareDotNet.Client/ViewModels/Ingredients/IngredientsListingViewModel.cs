using MealShareDotNet.Core.Services;

namespace MealShareDotNet.Client.ViewModels.Ingredients;

public partial class IngredientsListingViewModel : ViewModelBase
{
    private readonly IIngredientService _ingredientService;

    public IngredientsListingViewModel(IIngredientService ingredientService)
    {
        _ingredientService = ingredientService;   
    }
}