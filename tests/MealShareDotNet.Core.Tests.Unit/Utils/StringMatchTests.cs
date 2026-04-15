using System.Diagnostics;
namespace MealShareDotNet.Core.Utils;
using MealShareDotNet.Core.Data.DTOs;
[TestFixture]
public class StringMatchTests
{
    [OneTimeSetUp]
    public void StartTest()
    {
        Trace.Listeners.Add(new ConsoleTraceListener());
    }

    [Test]
    public void test()
    {
        string source = "onions";
        string result = "onion";
        List<IngredientDTO> OriginalIngredients = new List<IngredientDTO>()
        {
            new IngredientDTO() {
                Id = 0,
                Name = "Onion",
            },
            new IngredientDTO() {
                Id = 1,
                Name = "green",
            },
            new IngredientDTO() {
                Id = 2,
                Name = "purple"
            },
        };

        List<IngredientDTO> NewIngredients = new List<IngredientDTO>()
        {
            new IngredientDTO() {
                Id = 0,
                Name = "onions",
            },
            new IngredientDTO() {
                Id = 1,
                Name = "Greans",
            },
            new IngredientDTO() {
                Id = 2,
                Name = "porplas"
            },
        };
        int[] sourceInt = source.Select(n => Convert.ToInt32(n)).ToArray();
        int[] resultInt = result.Select(n => Convert.ToInt32(n)).ToArray();
        Console.WriteLine("This is the test: " + StringMatch.DamerauLevenshteinDistance( sourceInt, resultInt, 4));
        List<StringMatch.Result> results = StringMatch.checkIngredients(NewIngredients, OriginalIngredients);
        foreach( StringMatch.Result robject in results)
        {
            foreach( StringMatch.Match match in robject.Matches)
            {
                Console.WriteLine("Original: " + robject.Original.Name);
                Console.WriteLine("Name: " + match.Ingredient.Name);
                Console.WriteLine("Percent: " + match.percent);
            }

        }
    }
    [OneTimeTearDown]
    public void EndTest()
    {
        Trace.Flush();
    }
}
