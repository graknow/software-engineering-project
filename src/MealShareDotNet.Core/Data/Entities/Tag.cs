namespace MealShareDotNet.Core.Data.Entities;

public class Tag
{
    public long ID { get; set; }

    public string Name { get; set; } = String.Empty;
    public string? Description { get; set; }
}
