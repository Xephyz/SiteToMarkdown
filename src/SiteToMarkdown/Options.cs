using CommandLine;

namespace SiteToMarkdown;

public class Options
{
    [Value(0, Required = true, HelpText = "URL to begin scraping from", MetaName = "Url")]
    public required string Url { get; set; }
}
