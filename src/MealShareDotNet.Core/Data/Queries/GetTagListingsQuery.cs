namespace MealShareDotNet.Core.Data.Queries;

public sealed class GetTagListingsQuery
{
    public string? Name { get; set; }

    public int? PageSize { get; set; }
    public int? PageOffset { get; set; }
}
