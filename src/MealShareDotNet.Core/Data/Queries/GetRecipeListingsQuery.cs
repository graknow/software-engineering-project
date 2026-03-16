using MealShareDotNet.Core.Data.Common;

namespace MealShareDotNet.Core.Data.Queries;

public sealed class GetRecipeListingsQuery
{
    public string? Name { get; set; }
    public int? CookTime { get; set; }
    public InequalityDirection? CookTimeComparison { get; set; }
}
