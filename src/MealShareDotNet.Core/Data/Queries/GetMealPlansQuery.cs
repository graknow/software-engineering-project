using System.Diagnostics.CodeAnalysis;

namespace MealShareDotNet.Core.Data.Queries;

[ExcludeFromCodeCoverage]
public class GetMealPlansQuery
{
    public DateOnly Start { get; set; }
    public DateOnly End { get; set; }
}
