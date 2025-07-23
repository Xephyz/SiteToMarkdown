using System.Text;
using HtmlAgilityPack;

var baseUrl = args.Length > 0 ? args[0] : throw new ArgumentException("Please provide a URL.");
if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri))
{
    throw new ArgumentException($"Please provice a valid URL. Provided: {baseUrl}");
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
var mdFilename = UriToMarkdownFileName(uri);

var pagesDiscovered = new HashSet<Uri>();
var pagesToScrape = new Queue<Uri>();
pagesToScrape.Enqueue(uri);

while (pagesToScrape.Count > 0)
{
    var currentUrl = pagesToScrape.Dequeue();
    if (pagesDiscovered.Contains(currentUrl))
    {
        continue;
    }

    pagesDiscovered.Add(currentUrl);

    HtmlDocument? doc;
    try
    {
        Console.WriteLine($"Loading {currentUrl}");
        doc = web.Load(currentUrl);
    }
    catch
    {
        Console.WriteLine($"Failed to load {currentUrl}");
        continue;
    }

    if (doc.DocumentNode is null)
    {
        continue;
    }

    var md = converter.Convert(doc.DocumentNode.InnerHtml);
    markdownBuilder.AppendLine(md);

    var links = doc.DocumentNode.SelectNodes("//a[@href]");
    if (links is null)
    {
        continue;
    }

    foreach (var link in links)
    {
        var href = link.GetAttributeValue("href", string.Empty);
        if (string.IsNullOrEmpty(href)
            || !Uri.TryCreate(href, UriKind.RelativeOrAbsolute, out var result)
            || !uri.IsBaseOf(result)
            || result.OriginalString == "/" // This feels like a hack...
            || IsOnlyFragment(result))
        {
            continue;
        }

        if (!result.IsAbsoluteUri)
        {
            result = new Uri(uri, result);
        }

        if (pagesDiscovered.Contains(result))
        {
            continue;
        }

        pagesToScrape.Enqueue(result);
    }
}

using var writer = new StreamWriter(mdFilename, false, Encoding.UTF8);
writer.Write(markdownBuilder.ToString());
Console.WriteLine($"Wrote {markdownBuilder.Length} characters to {mdFilename}");

static string UriToMarkdownFileName(Uri uri)
{
    var segments = uri.Segments
        .Where(s => !string.IsNullOrWhiteSpace(s) && s != "/")
        .Select(s => s.Trim('/'))
        .ToList();

    if (segments.Count == 0)
    {
        return $"{uri.Host}.md";
    }

    var segmentsString = string.Join('.', segments);
    return $"{uri.Host}.{segmentsString}.md";
}

// TODO: Is this the best way to check this?
static bool IsOnlyFragment(Uri uri)
{
    return uri is not null
        && !uri.IsAbsoluteUri
        && uri.OriginalString.StartsWith('#');
}