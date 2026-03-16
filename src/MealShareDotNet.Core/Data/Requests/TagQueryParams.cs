using System.Diagnostics.CodeAnalysis;

namespace MealShareDotNet.Core.Data.Requests;

[ExcludeFromCodeCoverage]
public class TagQueryParams
{
    public string? NameQuery { get; init; }
    public string? DescriptionQuery { get; init; }
}
