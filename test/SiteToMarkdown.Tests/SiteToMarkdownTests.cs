namespace SiteToMarkdown.Tests;

using FluentAssertions;

using HtmlAgilityPack;

using SiteToMarkdown;

public class SiteToMarkdownTests
{
    [Theory]
    [InlineData("https://openfga.dev/docs/concepts", "openfga.dev.docs.concepts.md")]
    [InlineData("https://openfga.dev/docs/fga", "openfga.dev.docs.fga.md")]
    [InlineData("http://localhost:8000/index.html", "localhost.index.html.md")]
    [InlineData("https://localhost:8000/", "localhost.md")]
    public void UriToMarkdownFilename(string uri, string expected)
    {
        Utils.UriToMarkdownFileName(new Uri(uri)).Should().Be(expected);
    }

    [Theory]
    [InlineData("#header_1", true)]
    [InlineData("/sub1/sub2#header_2", false)]
    [InlineData("https://openfga.dev/docs/fga#benefits", false)]
    public void IsOnlyFragment(string uri, bool expected)
    {
        Utils.IsOnlyFragment(new Uri(uri, UriKind.RelativeOrAbsolute)).Should().Be(expected);
    }

    [Fact]
    public void EnqueueRelevantLinks_AddsAllRelevantLinksToQueue_WhenNotFoundInSet()
    {
        // Arrange
        var doc = new HtmlDocument();
        doc.LoadHtml(TestSite);
        var url = new Uri(TestSiteUrl);
        HashSet<Uri> pagesDiscovered = [
            new(TestSiteUrl),
            new("https://example.com/test/page3"),
        ];
        var pagesToScrape = new Queue<Uri>();

        // Act
        Utils.EnqueueRelevantLinks(doc, url, pagesDiscovered, pagesToScrape);

        // Assert
        pagesToScrape.Should()
            .HaveCount(3).And
            .Contain([
                new Uri("https://example.com/test/page1"),
                new Uri("https://example.com/test/page2#Something"),
                new Uri("https://example.com/test/page4")
            ]).And
            .NotContain([
                new Uri("https://example.com/test/stuff"),
                new Uri("https://example.com/test/page3"),
                new("/not-test/blog", UriKind.RelativeOrAbsolute),
                new("https://example.com/not-test/blog"),
                new("https://example.com"),
                new("https://different.com"),
                new("#section1", UriKind.RelativeOrAbsolute),
            ]);

        pagesDiscovered.Should()
            .HaveCount(2).And
            .Contain([
                new(TestSiteUrl),
                new("https://example.com/test/page3"),
            ]);
    }

    private const string TestSiteUrl = "https://example.com/test/stuff";
    private const string TestSite = """
        <!DOCTYPE html>
        <head><title>Test Site</title></head>
        <body>
            <div>
                <h1>Welcome to the Test Site</h1>
                <p>This is a paragraph on the test site.</p>
                <a href="/">Home - Ignore</a>
                <a href="https://example.com/test/page1">Page 1 - Include</a>
                <a href="https://example.com/test/page2#Something">Page 2 - Include</a>
                <a href="https://example.com/test/page3">Page 3 - Include</a>
                <a href="/test/page4">Page 4 - Include</a>
                <a href="/not-test/blog">Blog - Ignore</a>
                <a href="https://example.com">Home page - Ignore</a>
                <a href="https://different.com">Different Site - Ignore</a>
                <a href="#section1">Section 1 - Ignore</a>
            </div>
        </body>
        """;
}
