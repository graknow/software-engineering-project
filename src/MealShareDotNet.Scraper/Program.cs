namespace MealShareDotNet.Scraper;

class Program
{
    async static Task Main(string[] args)
    {
        ScraperService.Initialize();
        List<string> arguments = ["Canadian"];
        var recipes = await ScraperService.ScraperList[3].getRecipe(arguments);
        foreach (var recipe in recipes)
        {
            Console.WriteLine(recipe.Name);
        }
    }
}
