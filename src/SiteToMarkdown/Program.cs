using System.Text;

using CommandLine;

using HtmlAgilityPack;

using SiteToMarkdown;


var parseResult = Parser.Default.ParseArguments<Options>(args);
if (parseResult is not Parsed<Options> parsed)
{
    return;
}

string urlArg = parsed.Value.Url;
if (!Uri.TryCreate(urlArg, UriKind.Absolute, out var url))
{
    throw new ArgumentException($"Please provice a valid absolute URL. Provided: {urlArg}");
}

var converter = new ReverseMarkdown.Converter(new ReverseMarkdown.Config
{
    // Makes sure that we don't keep weird HTML tags that clutter the markdown file.
    UnknownTags = ReverseMarkdown.Config.UnknownTagsOption.Bypass,

    // Makes sure we keep code blocks formatted as code blocks, even though it doesn't format the code correctly.
    CleanupUnnecessarySpaces = false,
});

var mdFilename = Utils.UriToMarkdownFileName(url);

var web = new HtmlWeb();
var docs = Utils.ScrapeUrl(web, url);
var converted = docs
    .Select(static doc => doc.DocumentNode.InnerHtml)
    .Select(converter.Convert);

var markdown = string.Join("\n\n", converted);

using var writer = new StreamWriter(mdFilename, false, Encoding.UTF8);
writer.Write(markdown.ToString());
Console.WriteLine($"Wrote {markdown.Length} characters to {mdFilename}");
