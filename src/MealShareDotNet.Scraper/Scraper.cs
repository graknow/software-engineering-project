public class RecipeScraper
{
    public string DisplayName { get; set; } = "";
    public string ScraperName { get; set; } = "";
    public string path { get; set; } = "";

    public RecipeScraper(string displayName, string scraperName)
    {
        DisplayName = displayName;
        ScraperName = scraperName;
    }
}
