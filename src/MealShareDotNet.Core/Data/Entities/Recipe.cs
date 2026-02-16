namespace MealShareDotNet.Core.Data.Entities;

public class Recipe
{
    public int ID { get; set; }

    public string Name { get; set; } = String.Empty;
    public int CookTime { get; set; }
    public int? Price { get; set; }
    public int ServingQuantity { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
