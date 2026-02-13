using System.Collections.Generic;
using MealShareDotNet.Core.Data.Entities;

namespace MealShareDotNet.Core.Repositories;

public interface IRecipeRepository
{
    IEnumerable<Recipe> GetRecipes();
    Recipe GetRecipeById(Guid id);
    void InsertRecipe(Recipe recipe);
    void DeleteRecipe(Guid id);
    void UpdateRecipe(Recipe recipe);
    void Save();
}
