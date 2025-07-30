using CommandLine;

namespace SiteToMarkdown;

public class Options
{
    [Value(0, Required = true, HelpText = "URL to begin scraping from", MetaName = "Url")]
    public required string Url { get; set; }

    [Option("filter-id", HelpText = "Ignores all HTML tags with the given ids", Separator = ',')]
    public IEnumerable<string> IdFilters { get; set; } = [];

    [Option("filter-class", HelpText = "Ignores all HTML tags with the given classes", Separator = ',')]
    public IEnumerable<string> ClassFilters { get; set; } = [];
}
