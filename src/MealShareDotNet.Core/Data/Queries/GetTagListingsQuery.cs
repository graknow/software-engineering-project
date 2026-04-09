using System.Diagnostics.CodeAnalysis;

namespace MealShareDotNet.Core.Data.Queries;

[ExcludeFromCodeCoverage]
public class GetTagListingsQuery
{
    public string? Name { get; set; }

    public int? PageSize { get; set; }
    public int? PageOffset { get; set; }
}
