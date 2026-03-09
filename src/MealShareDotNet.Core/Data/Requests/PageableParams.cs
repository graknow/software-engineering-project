using System.Diagnostics.CodeAnalysis;

namespace MealShareDotNet.Core.Data.Requests;

/// <summary>
/// Parameter object to use when supporting paging.
/// </summary>
[ExcludeFromCodeCoverage]
public class PageableParams
{
    public uint? PageNumber { get; set; }
    public uint? PageSize { get; set; }
}
