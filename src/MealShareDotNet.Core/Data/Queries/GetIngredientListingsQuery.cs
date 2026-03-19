namespace MealShareDotNet.Core.Data.Queries;

public sealed class GetIngredientListingsQuery
{
    public string? Name { get; set; }

    public uint? PageSize { get; set; }
    public uint? PageOffset { get; set; }
}
