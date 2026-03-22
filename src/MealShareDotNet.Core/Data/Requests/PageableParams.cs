using System.Diagnostics.CodeAnalysis;

namespace MealShareDotNet.Core.Data.Requests;

/// <summary>
/// Parameter object to use when supporting paging.
/// </summary>
[ExcludeFromCodeCoverage]
public class PageableParams
{
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }

    public int? PageOffset {
        get
        {
            if (PageSize is null || PageNumber is null)
                return null;

            return PageSize * (PageNumber - 1);
        }
    }
}
