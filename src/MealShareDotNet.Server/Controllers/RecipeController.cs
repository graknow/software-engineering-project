using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MealShareDotNet.Core.Repositories;
using MealShareDotNet.Core.Data.Requests;
using MealShareDotNet.Core.Data.DTOs;
using MealShareDotNet.Server.Auth;

namespace MealShareDotNet.Server.Controllers;

[ApiController]
[Route("v1/recipes")]
public class RecipeControllerV1 : ControllerBase
{
    private readonly IRecipeRepository _recipes;

    public const int MAX_PAGE_SIZE = 100;

    public RecipeControllerV1(IRecipeRepository recipes)
    {
        _recipes = recipes;
    }

    [HttpGet("listings")]
    public async Task<ActionResult<IEnumerable<RecipeListingDTO>>> GetRecipeListings(
            [FromQuery] PageableParams pager
            )
    {
        if (pager.PageSize is null != pager.PageNumber is null)
        {
            return BadRequest($"Either PageSize or PageNumber is not defined while the other is defined.");
        }
        else if (pager.PageSize > MAX_PAGE_SIZE)
        {
            return Forbid("PageSize exceeds the server's set maximum page size");
        }

        if (pager.PageSize is null)
        {
            pager.PageSize = MAX_PAGE_SIZE;
            pager.PageNumber = 1;
        }

        return Ok(await _recipes.GetRecipeListings(pager));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RecipeDTO>> GetRecipe(
            long id
            )
    {
        return Ok(await _recipes.GetRecipeById(id));
    }

}
