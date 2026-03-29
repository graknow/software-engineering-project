using System.Diagnostics.CodeAnalysis;

namespace MealShareDotNet.Core.Data.Queries;

[ExcludeFromCodeCoverage]
public class GetMealPlansQuery
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}
