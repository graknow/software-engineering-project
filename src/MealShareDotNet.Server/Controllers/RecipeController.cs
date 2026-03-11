using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MealShareDotNet.Core.Services;
using MealShareDotNet.Core.Data.DTOs;
using MealShareDotNet.Core.Data.Entities;
using MealShareDotNet.Core.Data.Requests;
using MealShareDotNet.Server.Auth;

namespace MealShareDotNet.Server.Controllers;

[ApiController]
[AllowAnonymous]
[Route("v1/recipes")]
public class RecipeControllerV1 : ControllerBase
{
    private readonly IRecipeService _recipes;

    public const int MAX_PAGE_SIZE = 100;

    public RecipeControllerV1(IRecipeService recipes)
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
            return BadRequest($"PageSize and PageNumber must both be defined or null.");
        }
        else if (pager.PageSize > MAX_PAGE_SIZE)
        {
            return Forbid("PageSize exceeds the server's set maximum page size.");
        }

        if (pager.PageSize is null)
        {
            pager.PageSize = MAX_PAGE_SIZE;
            pager.PageNumber = 1;
        }

        var offset = pager.PageSize * (pager.PageNumber - 1);

        return Ok(await _recipes.GetRecipeListingsAsync(pager.PageSize, offset));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RecipeDTO>> GetRecipe(
            long id
            )
    {
        return Ok(await _recipes.GetRecipeAsync(id));
    }

    [HttpPost]
    public async Task<ActionResult<Recipe>> InsertRecipe(
            [FromBody] RecipeDTO recipe
            )
    {
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecipe(
            long id
            )
    {
        var success = await _recipes.DeleteRecipeAsync(id);

        if (!success)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRecipe(
            long id,
            [FromBody] RecipeDTO recipe
            )
    {
        return Ok();
    }
}
