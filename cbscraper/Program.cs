using System;
using System.Threading.Tasks;
using cbscraper.Infrastructure;
using cbscraper.Services;

namespace cbscraper;

internal static class Program
{
    public static async Task Main(string[] _)
    {

        bool headed = true;

        await using var browser = await BrowserFactory.CreateAsync(headless: !headed);
        await using var scraper  = await BankScraper.CreateAsync(browser, headed);

        await scraper.LogOnAsync();
    }
}