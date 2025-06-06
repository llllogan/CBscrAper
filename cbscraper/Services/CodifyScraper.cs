using System.Collections.Generic;
using System.Threading.Tasks;
using cbscraper.Models;
using Microsoft.Playwright;

namespace cbscraper.Services;

public sealed class CodifyScraper
{
    private readonly IBrowser _browser;

    public CodifyScraper(IBrowser browser) => _browser = browser;

    /// <summary>
    /// Loads https://www.codify.com/ and returns the page title + all H1/H2 headings.
    /// </summary>
    public async Task<PageContent> ScrapeHomeAsync()
    {
        var context = await _browser.NewContextAsync(new() { AcceptDownloads = false });
        var page    = await context.NewPageAsync();

        await page.GotoAsync(
            "https://www.codify.com/",
            new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });

        string title = await page.TitleAsync();
        IReadOnlyList<string> headings = await page
            .Locator("h1, h2")
            .AllInnerTextsAsync();

        await context.CloseAsync();
        return new PageContent(title, headings);
    }
}