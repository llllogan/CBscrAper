using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace cbscraper.Services;

/// <summary>
/// Logs in to CommBank NetBank and (for now) just verifies the landing page.
/// Extend this with account navigation / PDF download once the login flow is solid.
/// </summary>
public sealed class BankScraper : IAsyncDisposable
{
    private const string STATE_FILE      = "storageState.json";
    private const string LOGIN_URL       = "https://www.my.commbank.com.au/netbank/Logon";
    private const string DASHBOARD_URL_GLOB = "https://www.commbank.com.au/retail/netbank/home/**"; // landing pattern

    private readonly IBrowser        _browser;
    private readonly IBrowserContext _ctx;
    private readonly IPage           _page;
    private readonly bool            _headed;

    /* ---------- factory ---------- */

    public static async Task<BankScraper> CreateAsync(IBrowser browser, bool headed)
    {
        bool reuseState = false;

        // build context opts
        var opts = new BrowserNewContextOptions
        {
            AcceptDownloads = true,
            Locale          = "en-AU",
            ViewportSize    = null,           // use full-size window (helps detect headless)
        };

        if (reuseState && File.Exists(STATE_FILE))
            opts.StorageStatePath = STATE_FILE;

        var ctx  = await browser.NewContextAsync(opts);
        var page = await ctx.NewPageAsync();

        return new BankScraper(browser, ctx, page, headed);
    }

    /* ---------- private ctor ---------- */

    private BankScraper(IBrowser browser, IBrowserContext ctx, IPage page, bool headed)
    {
        _browser = browser;
        _ctx     = ctx;
        _page    = page;
        _headed  = headed;

        // global default timeout (longer when headless CI)
        _page.SetDefaultTimeout(headed ? 30_000 : 60_000);
    }

    /* ---------- public API ---------- */

    public async Task LogOnAsync()
    {
        var client = Environment.GetEnvironmentVariable("BANK_CLIENT")
                     ?? throw new InvalidOperationException("BANK_CLIENT env-var missing");
        var pass   = Environment.GetEnvironmentVariable("BANK_PASS")
                     ?? throw new InvalidOperationException("BANK_PASS env-var missing");

        await _page.GotoAsync(LOGIN_URL);

        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Client number" }).FillAsync(client);
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password"     }).FillAsync(pass);

        // var nav = _page.WaitForURLAsync("**/netbank/**");
        await _page.GetByRole(AriaRole.Button, new() { Name = "Log on" }).ClickAsync();
        // await nav;

        await _page.WaitForTimeoutAsync(20_000);

        Console.WriteLine("✅ Logged in  → " + _page.Url);

        // persist cookies / local-storage for next run
        await _ctx.StorageStateAsync(new() { Path = STATE_FILE });
    }

    /* ---------- disposal ---------- */

    public async ValueTask DisposeAsync()
    {
        await _ctx.DisposeAsync();
        await _browser.DisposeAsync();
    }
}