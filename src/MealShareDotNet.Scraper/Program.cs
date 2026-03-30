namespace MealShareDotNet.Scraper;

class Program
{
    async static Task Main(string[] args)
    {
        ScraperService.Initialize();
        var recipes = await ScraperService.getRecipe(ScraperService.ScraperList[3], "Canadian");
        foreach (var recipe in recipes)
        {
            Console.WriteLine(recipe.Name);
        }
    }
}
