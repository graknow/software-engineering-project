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

    public const int MAX_PAGE_SIZE = 500;

    public IngredientControllerV1(IIngredientService ingredients)
    {
        _ingredients = ingredients;
    }
}
