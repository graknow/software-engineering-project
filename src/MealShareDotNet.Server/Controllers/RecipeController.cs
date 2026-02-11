using Microsoft.AspNetCore.Mvc;
using MealShareDotNet.Core.Data.Requests;

namespace MealShareDotNet.Server.Controllers;

[ApiController]
[Route("api/recipe")]
public class RecipeController : ControllerBase
{
    public const int MAX_PAGE_SIZE = 100;

    [HttpGet]
    public async Task<ActionResult<string>> GetRecipeListings(
            [FromQuery] PageableParams pager)
    {
        if (pager.PageSize is null != pager.PageNumber is null)
        {
            return BadRequest($"Either PageSize or PageNumber is not defined while the other is defined.");
        }

        if (pager.PageSize is null)
        {
            pager.PageSize = MAX_PAGE_SIZE;
        }
        else if (pager.PageSize > MAX_PAGE_SIZE)
        {
            return Forbid("PageSize exceeds the server's set maximum page size");
        }


        return $"PageSize: {pager.PageSize}, Page#: {pager.PageNumber}";
    }
}
