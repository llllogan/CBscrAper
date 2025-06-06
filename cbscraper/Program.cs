using System;
using System.Threading.Tasks;
using cbscraper.Infrastructure;
using cbscraper.Services;

namespace cbscraper;

internal static class Program
{
    public static async Task Main(string[] _)
    {
        bool headless = false;

        await using var browser = await BrowserFactory.CreateAsync(headless);
        var scraper            = new CodifyScraper(browser);

        var content = await scraper.ScrapeHomeAsync();

        Console.WriteLine($"Page title: {content.Title}");
        foreach (var h in content.Headings)
            Console.WriteLine($"Heading   : {h}");
    }
}