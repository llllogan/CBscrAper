using System.Collections.Generic;

namespace cbscraper.Models;

public sealed record PageContent(string Title, IReadOnlyList<string> Headings);