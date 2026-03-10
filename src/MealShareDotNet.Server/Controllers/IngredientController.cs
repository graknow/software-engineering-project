using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MealShareDotNet.Core.Repositories;
using MealShareDotNet.Core.Data.DTOs;
using MealShareDotNet.Core.Data.Entities;
using MealShareDotNet.Core.Data.Requests;
using MealShareDotNet.Server.Auth;

namespace MealShareDotNet.Server.Controllers;

[ApiController]
[Route("v1/ingredients")]
public class IngredientControllerV1 : ControllerBase
{
    private readonly IRecipeRepository _recipes;

    public const int MAX_PAGE_SIZE = 100;

    public IngredientControllerV1(IRecipeRepository recipes)
    {
        _recipes = recipes;
    }

    [HttpGet("listings")]
    public async Task<ActionResult<IEnumerable<TagListingDTO>>> GetTagListings(
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

        return Ok(await _recipes.GetTagListings(pager));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RecipeDTO>> GetTag(
            long id
            )
    {
        var result = await _recipes.GetTag(id);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Tag>> InsertTag(
            [FromBody] TagDTO tag
            )
    {
        return Ok(_recipes.InsertTag(tag));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTag(
            long id
            )
    {
        try
        {
            _recipes.DeleteTag(id);
            return NoContent();
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTag(
            long id,
            [FromBody] TagDTO tag
            )
    {
        return Ok(_recipes.UpdateTag(tag));
    }
}
