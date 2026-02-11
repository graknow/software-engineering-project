namespace MealShareDotNet.Core.Data.DTOs;

public class RecipeDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public int CookTime { get; set; }
    public int Price { get; set; }
    public int ServingQuantity { get; set; }
    // steps, ingredients, ...
}

public class RecipeListingDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public int CookTime { get; set; }
    public int ServingQuantity { get; set; }
}
