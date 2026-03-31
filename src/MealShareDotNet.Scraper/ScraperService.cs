using System.Diagnostics;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.RepresentationModel;
using MealShareDotNet.Core.Data.DTOs;

namespace MealShareDotNet.Scraper;

//Set up new process for python script
public static class ScraperService
{
    public static List<RecipeScraper> ScraperList = [];
    public static async Task<List<RecipeDTO>> getRecipe(RecipeScraper scraper, string Arguments)
    {
        string fileArguments = "-n " + scraper.ScraperName + " -a " + Arguments;
        //Console.WriteLine(fileArguments);
        using var process = new Process();
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.FileName = "./TheMealDB/handler.py";
        process.StartInfo.Arguments = fileArguments;

        //Start the process
        process.Start();

        //capture the output to the string output
        string output = process.StandardOutput.ReadToEnd();

        //await the process to complete, this is actually synchronous, but this makes sure that the thread does not get locked

        var exitProcess = process.WaitForExitAsync();
        await exitProcess;

        //Console.WriteLine(output);
        var yaml = new YamlStream();
        using var sr = new StringReader(output);
        //yaml.Load(sr);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithTagMapping("!Ingredient", typeof(IngredientDTO))
            .Build();

        try
        {
            return deserializer.Deserialize<IEnumerable<RecipeDTO>>(sr).ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            return null;
        }
        //foreach (var e in yaml.Documents.Where(d => d is not null))
        //{
        //recipes.Add(deserializer.Deserialize<RecipeDTO>(e.ToString()!));
        //}
        //print the result


    }
    public static async void Initialize()
    {
        string path = "./TheMealDB/scrapers.txt";

        if (!File.Exists(path))
        {
            using var process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = "./TheMealDB/handler.py";
            process.StartInfo.Arguments = "-d";

            //Start the process
            process.Start();

            //await the process to complete, this is actually synchronous, but this makes sure that the thread does not get locked

            var exitProcess = process.WaitForExitAsync();
            await exitProcess;
            if (!File.Exists(path))
            {
                //Create Object with an error
            }
        }

        else
        {
            string names = File.ReadAllText(path);
            using (var reader = new StringReader(names))
            {
                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    ScraperList.Add(new RecipeScraper(line, line));

                }
            }
        }
    }
}




