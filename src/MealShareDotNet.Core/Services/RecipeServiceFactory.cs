namespace MealShareDotNet.Core.Services;

public class RecipeServiceFactory
{
    public readonly IEnumerable<IRecipeService> RegisteredServices;

    public RecipeServiceFactory(IEnumerable<IRecipeService> registeredServices)
    {
        RegisteredServices = registeredServices;
    }
}