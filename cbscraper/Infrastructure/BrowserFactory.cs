using System.Threading.Tasks;
using Microsoft.Playwright;

namespace cbscraper.Infrastructure;

internal static class BrowserFactory
{
    public static async Task<IBrowser> CreateAsync(bool headless = true)
    {
        var playwright = await Playwright.CreateAsync();

        return await playwright.Chromium.LaunchAsync(new()
        {
            Headless = headless,
            SlowMo   = headless ? 0 : 150,     // watch the clicks in headed mode
            Args     = new[] { "--disable-blink-features=AutomationControlled" }
        });
    }
}