namespace MealShareDotNet.Core.Data.DTOs;

public class RecipeListingDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public int CookTime { get; set; }
    public int ServingQuantity { get; set; }
}
