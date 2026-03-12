using System.Diagnostics.CodeAnalysis;

namespace MealShareDotNet.Core.Data.Requests;

[ExcludeFromCodeCoverage]
public class RecipeListParams
{
    public string NameQuery { get; set; } = String.Empty;
    public string IngredientQuery { get; set; } = String.Empty;
    public IEnumerable<long> TagListQuery { get; set; } = [];
}
