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
[Route("v1/ingredients")]
[ApiController]
public class IngredientControllerV1 : ControllerBase
{
    private readonly IIngredientService _ingredients;

    public const int MAX_PAGE_SIZE = 1000;

    public IngredientControllerV1(IIngredientService ingredients)
    {
        _ingredients = ingredients;
    }

    [HttpGet("listings")]
    public async Task<ActionResult<IEnumerable<IngredientListingDTO>>> GetIngredientListings(
            [FromQuery] PageableParams pager
            )
    {
        if (pager.PageSize is null != pager.PageNumber is null)
        {
            return BadRequest($"PageSize and PageNumber must both be defined or null");
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

        var query = new GetIngredientListingsQuery()
        {
            PageSize = pager.PageSize,
            PageOffset = pager.PageOffset
        };

        return Ok(await _ingredients.GetIngredientListingsAsync(query));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IngredientDTO>> GetIngredient(
            long id
            )
    {
        var result = await _ingredients.GetIngredientAsync(id);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Ingredient>> InsertIngredient(
            [FromBody] IngredientDTO ingredient
            )
    {
        return Ok(await _ingredients.InsertIngredientAsync(ingredient));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteIngredient(
            long id
            )
    {
        try
        {
            await _ingredients.DeleteIngredientAsync(id);

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
    public async Task<ActionResult<IngredientDTO>> UpdateIngredient(
            long id,
            [FromBody] IngredientDTO ingredient
            )
    {
        ingredient.Id = id;

        try
        {
            return Ok(await _ingredients.UpdateIngredientAsync(ingredient));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentNullException)
        {
            return BadRequest();
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
