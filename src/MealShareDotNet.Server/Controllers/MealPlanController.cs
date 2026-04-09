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
[Route("v1/meal-plans")]
[ApiController]
public class MealPlanControllerV1 : ControllerBase
{
    private readonly IMealPlanService _meals;

    public MealPlanControllerV1(IMealPlanService meals)
    {
        _meals = meals;
    }
}
