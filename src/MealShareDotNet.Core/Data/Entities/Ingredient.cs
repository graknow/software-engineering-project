namespace MealShareDotNet.Core.Data.Entities;

public class Ingredient
{
    public Guid ID { get; set; }

    public string Name { get; set; } = String.Empty;
    public Guid ImperialUnit { get; set; }
    public Guid MetricUnit { get; set; }
}
