using System.Diagnostics.CodeAnalysis;

namespace MealShareDotNet.Core.Data.Requests;

[ExcludeFromCodeCoverage]
public class RecipeListParams
{
    public string SearchQuery { get; set; } = String.Empty;
}
