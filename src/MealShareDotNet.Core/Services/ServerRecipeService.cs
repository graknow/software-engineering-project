using System.ComponentModel;
using System.Net.Http.Json;
using System.Web;
using MealShareDotNet.Core.Data.DTOs;
using MealShareDotNet.Core.Data.Entities;
using MealShareDotNet.Core.Data.Queries;
using MealShareDotNet.Core.Repositories;

namespace MealShareDotNet.Core.Services;

public class ServerRecipeService : IRecipeService
{
    public class ServerConnection
    {
        public string Address { get; set; } = "";
    }

    private readonly HttpClient _client;
    private readonly string _baseAddress;
    private UriBuilder _builder => new(_baseAddress);

    public string Name { get; set; } = "server";

    public ServerRecipeService(HttpClient client, ServerConnection con)
    {
        _client = client;
        _baseAddress = con.Address;
    }

    public async Task<IEnumerable<RecipeListingDTO>> GetRecipeListingsAsync(GetRecipeListingsQuery query)
    {
        var builder = _builder;
        builder.Path += "api/v1/recipes/listings";
        var reqQuery = HttpUtility.ParseQueryString(builder.Query);
        if (query.PageSize is not null)
            reqQuery["pageOffset"] = query.PageOffset.ToString();
        if (query.PageOffset is not null)
            reqQuery["pageSize"] = query.PageSize.ToString();
        reqQuery["name"] = query.Name;
        builder.Query = reqQuery.ToString();

        var response = await _client.GetAsync(builder.ToString());

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<IEnumerable<RecipeListingDTO>>();
    }

    public async Task<RecipeDTO?> GetRecipeAsync(long id)
    {
        throw new Exception();
    }

    public async Task<RecipeDTO?> GetRandomDailyRecipeAsync()
    {
        throw new Exception();
    }

    public async Task<RecipeDTO> InsertRecipeAsync(RecipeDTO recipe)
    {
        throw new Exception();
    }

    public async Task DeleteRecipeAsync(long id)
    {
        throw new Exception();
    }

    public async Task<RecipeDTO> UpdateRecipeAsync(RecipeDTO recipe)
    {
        throw new Exception();
    }

    private async Task AddMissingIngredients(RecipeDTO recipe)
    {
        throw new Exception();
    }

    private async Task AddMissingTags(RecipeDTO recipe)
    {
        throw new Exception();
    }
}
