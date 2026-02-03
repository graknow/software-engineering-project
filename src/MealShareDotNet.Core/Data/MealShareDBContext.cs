using System.Data.Entity;
using MealShareDotNet.Core.Data.Entities;

namespace MealShareDotNet.Core.Data;

public class MealShareDBContext : DbContext
{
    public MealShareDBContext() : base("")
    {

    }

    public DbSet<Recipe> Recipes { get; set; } = default!;
    public DbSet<Ingredient> Ingredients { get; set; } = default!;
}
