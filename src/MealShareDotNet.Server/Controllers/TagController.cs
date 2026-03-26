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
[Route("v1/tags")]
[ApiController]
public class TagControllerV1 : ControllerBase
{
    private readonly ITagService _tags;

    public const int MAX_PAGE_SIZE = 1000;

    public TagControllerV1(ITagService tags)
    {
        _tags = tags;
    }

    [HttpGet("listings")]
    public async Task<ActionResult<IEnumerable<TagListingDTO>>> GetTagListings(
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

        var query = new GetTagListingsQuery()
        {
            PageSize = pager.PageSize,
            PageOffset = pager.PageOffset
        };

        return Ok(await _tags.GetTagListingsAsync(query));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IngredientDTO>> GetTag(
            long id
            )
    {
        var result = await _tags.GetTagAsync(id);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Ingredient>> InsertTag(
            [FromBody] TagDTO tag
            )
    {
        return Ok(await _tags.InsertTagAsync(tag));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTag(
            long id
            )
    {
        try
        {
            await _tags.DeleteTagAsync(id);

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
    public async Task<ActionResult<TagDTO>> UpdateTag(
            long id,
            [FromBody] TagDTO tag
            )
    {
        tag.Id = id;

        try
        {
            return Ok(await _tags.UpdateTagAsync(tag));
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
