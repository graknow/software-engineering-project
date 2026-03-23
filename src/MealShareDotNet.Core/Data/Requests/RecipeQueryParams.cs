using System.Diagnostics.CodeAnalysis;

namespace MealShareDotNet.Core.Data.Requests;

[ExcludeFromCodeCoverage]
public class RecipeQueryParams
{
    public string? NameQuery { get; init; }
    public IEnumerable<string> IngredientQuery { get; init; } = [];
    public IEnumerable<long> TagListQuery { get; init; } = [];
}
