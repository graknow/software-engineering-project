namespace MealShareDotNet.Core.Data.Entities;

public class Ingredient
{
    public long ID { get; set; }

    public string Name { get; set; } = String.Empty;

    public int? Mass { get; set; } // Mass in 1/10 g
    public int? Volume { get; set; } // Volume in 1/10 mL
    public float? Quantity { get; set; }
}
