﻿using HtmlAgilityPack;

namespace SiteToMarkdown;

public static class Utils
{
    public static string UriToMarkdownFileName(Uri uri)
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
    public static bool IsOnlyFragment(Uri uri)
    {
        return uri is not null
            && !uri.IsAbsoluteUri
            && uri.OriginalString.StartsWith('#');
    }

    public static bool TryLoadUrl(HtmlWeb web, Uri url, out HtmlDocument? doc)
    {
        doc = null;
        if (!url.IsAbsoluteUri)
        {
            return false;
        }

        try
        {
            doc = web.Load(url);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static void EnqueueRelevantLinks(
        HtmlDocument doc,
        Uri baseUrl,
        HashSet<Uri> pagesDiscovered,
        Queue<Uri> pagesToScrape)
    {
        ArgumentNullException.ThrowIfNull(doc);
        ArgumentNullException.ThrowIfNull(doc.DocumentNode);

        if (doc.DocumentNode.SelectNodes("//a[@href]") is not { Count: > 0 } links)
        {
            return;
        }

        foreach (var link in links)
        {
            var href = link.GetAttributeValue("href", string.Empty);
            if (string.IsNullOrEmpty(href)
                || !Uri.TryCreate(href, UriKind.RelativeOrAbsolute, out var result)
                || !baseUrl.IsBaseOf(result)
                || result.OriginalString == "/" // This feels like a hack...
                || IsOnlyFragment(result))
            {
                continue;
            }

            if (!result.IsAbsoluteUri)
            {
                // Turn `result` into an absolute Uri
                result = new Uri(baseUrl, result);
            }

            if (pagesDiscovered.Contains(result))
            {
                continue;
            }

            pagesToScrape.Enqueue(result);
        }
    }

    public static HtmlDocument FilterHtmlTags(
        HtmlDocument document,
        IEnumerable<string> idFilters,
        IEnumerable<string> classFilters)
    {
        ArgumentNullException.ThrowIfNull(document);

        foreach (var idFilter in idFilters)
        {
            document.GetElementbyId(idFilter)?.Remove();
        }

        foreach (var classFilter in classFilters)
        {
            var nodes = document.DocumentNode.SelectNodes($"//*[contains(@class, '{classFilter}')]");
            if (nodes is not null)
            {
                foreach (var node in nodes)
                {
                    node.Remove();
                }
            }
        }

        return document;
    }

    public static IEnumerable<HtmlDocument> ScrapeUrl(HtmlWeb web, Uri url) =>
        ScrapeSiteAndUrl(web, url)
            .Select(static tuple => tuple.Item2);

    public static IEnumerable<(Uri, HtmlDocument)> ScrapeSiteAndUrl(HtmlWeb web, Uri url)
    {
        ArgumentNullException.ThrowIfNull(web);
        ArgumentNullException.ThrowIfNull(url);
        if (!url.IsAbsoluteUri)
        {
            yield break;
        }

        var pagesDiscovered = new HashSet<Uri>();
        var pagesToScrape = new Queue<Uri>();
        pagesToScrape.Enqueue(url);

        while (pagesToScrape.Count > 0)
        {
            var currentUrl = pagesToScrape.Dequeue();
            if (pagesDiscovered.Contains(currentUrl))
            {
                continue;
            }
            pagesDiscovered.Add(currentUrl);

            Console.WriteLine($"Loading {currentUrl}");
            if (!TryLoadUrl(web, currentUrl, out var doc)
                || doc is null
                || doc.DocumentNode is null)
            {
                Console.WriteLine($"Failed to load {currentUrl}");
                continue;
            }

            EnqueueRelevantLinks(doc, url, pagesDiscovered, pagesToScrape);
            yield return (currentUrl, doc);
        }
    }

    public static List<HtmlDocument> ConvertLinks(IEnumerable<(Uri Url, HtmlDocument Doc)> values)
    {
        List<(Uri Url, HtmlDocument Doc, string markdownAnchor)> valuesList = values
            .Select(x =>
            {
                var h1 = x.Doc.DocumentNode.SelectSingleNode("//h1");
                if (h1 is null)
                {
                    return (x.Url, x.Doc, string.Empty);
                }

                var title = h1!.InnerText;
                var titleAnchor = title
                    .Trim()
                    .Replace(' ', '-')
                    .ToLowerInvariant();
                var markdownAnchor = $"[{title}](#{titleAnchor})";

                return (x.Url, x.Doc, markdownAnchor);
            })
            .ToList();

        var anchorMap = valuesList
            .Where(x => !string.IsNullOrEmpty(x.markdownAnchor))
            .ToDictionary(
                x => x.Url,
                x => x.markdownAnchor);

        foreach(var (url, doc, anchor) in valuesList)
        {
            anchorMap[new Uri(url.AbsolutePath, UriKind.RelativeOrAbsolute)] = anchor;
        }

        var docs = valuesList
            .Select(static x => x.Doc)
            .ToList();
        foreach (var doc in docs)
        {
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
                    || !anchorMap.TryGetValue(result, out var anchor))
                {
                    continue;
                }

                link.ParentNode.InsertBefore(
                    HtmlNode.CreateNode($"<p>{anchor}</p>"),
                    link);
                link.Remove();
            }
        }

        return docs;
    }
}
