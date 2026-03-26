using System.Diagnostics;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.RepresentationModel;
using MealShareDotNet.Core.Data.DTOs;

//Set up new process for python script
async Task<List<RecipeDTO>> getRecipe()
{
    List<RecipeDTO> recipes = new List<RecipeDTO>();
    using var process = new Process();
    process.StartInfo.UseShellExecute = false;
    process.StartInfo.RedirectStandardOutput = true;
    process.StartInfo.FileName = "./TheMealDB/handler.py";
    process.StartInfo.Arguments = "random";

    //Start the process
    process.Start();

    //capture the output to the string output
    string output = process.StandardOutput.ReadToEnd();

    //await the process to complete, this is actually synchronous, but this makes sure that the thread does not get locked

    var exitProcess = process.WaitForExitAsync();
    await exitProcess;

    Console.WriteLine(output);
    var yaml = new YamlStream();
    using var sr = new StringReader(output);
    yaml.Load(sr);
    var root = (YamlMappingNode)yaml.Documents[0].RootNode;
    var deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .WithTagMapping("!Ingredient", typeof(IngredientDTO))
        .Build();
    foreach (var e in yaml.Documents.Where(d => d is not null))
    {
        recipes.Add(deserializer.Deserialize<RecipeDTO>(e.ToString()!));
    }
    //print the result


    return recipes;
}


List<RecipeDTO> recipes = await getRecipe();

foreach (var e in recipes)
{
    Console.WriteLine(e.Name);
}

