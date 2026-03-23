using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MealShareDotNet.Core.Services;
using MealShareDotNet.Core.Data.DTOs;
using MealShareDotNet.Core.Data.Entities;
using MealShareDotNet.Core.Data.Queries;
using MealShareDotNet.Core.Data.Requests;
using MealShareDotNet.Server.Auth;

namespace MealShareDotNet.Server.Controllers;

[AllowAnonymous]
[Route("v1/recipes")]
[ApiController]
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

        var query = new GetRecipeListingsQuery()
        {
            PageSize = pager.PageSize,
            PageOffset = offset
        };

        return Ok(await _recipes.GetRecipeListingsAsync(query));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RecipeDTO>> GetRecipe(
            long id
            )
    {
        var result = await _recipes.GetRecipeAsync(id);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Recipe>> InsertRecipe(
            [FromBody] RecipeDTO recipe
            )
    {
        return Ok(await _recipes.InsertRecipeAsync(recipe));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecipe(
            long id
            )
    {
        try
        {
            await _recipes.DeleteRecipeAsync(id);

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRecipe(
            long id,
            [FromBody] RecipeDTO recipe
            )
    {
        recipe.Id = id;

        try
        {
            var result = await _recipes.UpdateRecipeAsync(recipe);

            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentNullException)
        {
            return NotFound();
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

    }
}
