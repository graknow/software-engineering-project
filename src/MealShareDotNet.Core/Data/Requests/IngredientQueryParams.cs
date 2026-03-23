using System.Diagnostics.CodeAnalysis;

namespace MealShareDotNet.Core.Data.Requests;

[ExcludeFromCodeCoverage]
public class IngredientQueryParams
{
    public string? NameQuery { get; init; }
}
