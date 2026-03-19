namespace MealShareDotNet.Core.Data.Queries;

public sealed class GetTagListingsQuery
{
    public string? Name { get; set; }

    public uint? PageSize { get; set; }
    public uint? PageOffset { get; set; }
}
