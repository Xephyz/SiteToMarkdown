using System.Text;

using CommandLine;

using HtmlAgilityPack;

using SiteToMarkdown;


var parseResult = Parser.Default.ParseArguments<Options>(args);
if (parseResult is not Parsed<Options> parsed)
{
    return;
}

var url = parsed.Value.Url;
if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
{
    throw new ArgumentException($"Please provice a valid absolute URL. Provided: {url}");
}

var web = new HtmlWeb();
var converter = new ReverseMarkdown.Converter(new ReverseMarkdown.Config
{
    // Makes sure that we don't keep weird HTML tags that clutter the markdown file.
    UnknownTags = ReverseMarkdown.Config.UnknownTagsOption.Bypass,

    // Makes sure we keep code blocks formatted as code blocks, even though it doesn't format the code correctly.
    CleanupUnnecessarySpaces = false,
});

var markdownBuilder = new StringBuilder();
var mdFilename = Utils.UriToMarkdownFileName(uri);

var docs = Utils.ScrapeUrl(web, uri);
var converted = docs
    .Select(static doc => doc.DocumentNode.InnerHtml)
    .Select(converter.Convert);

markdownBuilder.Append(string.Join("\n\n", converted));

using var writer = new StreamWriter(mdFilename, false, Encoding.UTF8);
writer.Write(markdownBuilder.ToString());
Console.WriteLine($"Wrote {markdownBuilder.Length} characters to {mdFilename}");
