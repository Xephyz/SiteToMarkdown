# SiteToMarkdown

SiteToMarkdown is a command-line utility written in C# (.NET 9) that crawls a website starting from a given URL, downloads its HTML content, and converts it into Markdown format. The tool recursively follows internal links, processes each page, and outputs a single Markdown file containing the combined content. This is useful for archiving, documentation, or transforming web-based documentation into Markdown for further editing or publishing.

## Features

- **Recursive Crawling**: Starts from a base URL and follows internal links to discover and process all reachable pages within the same domain.
- **HTML to Markdown Conversion**: Uses [HtmlAgilityPack](https://github.com/zzzprojects/html-agility-pack) to parse HTML and [ReverseMarkdown](https://github.com/mysticmind/reverse-markdown) to convert HTML to Markdown.
- **Single Output File**: Aggregates all discovered pages into one Markdown file, named according to the base URL.
- **Robust Error Handling**: Skips pages that fail to load and continues processing others.
- **Customizable Markdown Conversion**: Configures ReverseMarkdown to bypass unknown HTML tags and preserve code blocks.

## Requirements

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- NuGet packages:
  - HtmlAgilityPack
  - ReverseMarkdown

## Installation

1. Clone the repository:
```sh
git clone https://github.com/xephyz/SiteToMarkdown.git
cd SiteToMarkdown
```

2. Restore dependencies:
```powershell
dotnet restore
```

3. Build the project:
```powershell
dotnet build
```

## Usage

Run the tool from the command line, providing the base URL as an argument:

```powershell
dotnet run -- "https://example.com/docs"
```

- The program will crawl all pages starting from the provided URL, convert their HTML content to Markdown, and write the result to a file named after the domain and path (e.g., `example.com.docs.md`).
- The output file will be created in the working directory.

### Example

Suppose you want to convert the documentation section of `https://openfga.dev/docs` to Markdown:

```powershell
dotnet run -- "https://openfga.dev/docs"
```


This will produce a file like `openfga.dev.docs.md` containing the combined Markdown content of all discovered documentation pages.

## How It Works

- The program initializes a queue with the base URL.
- It loads each page using HtmlAgilityPack, converts the HTML to Markdown, and appends it to a StringBuilder.
- It discovers all `<a href="...">` links, normalizes them, and adds new internal links to the queue.
- The process continues until all reachable pages are processed.
- The final Markdown is written to a file.

## Configuration

- **UnknownTags**: Set to `Bypass` to ignore unknown HTML tags.
- **CleanupUnnecessarySpaces**: Set to `false` to preserve code block formatting.

You can modify these options in `Program.cs` to suit your needs.

## Limitations

- Only internal links (same domain) are followed.
- Fragment-only links and root (`/`) links are ignored.
- The tool does not handle authentication, JavaScript-rendered content, or rate limiting.
- Output is a single Markdown file; images and other assets are not downloaded.

## Extending

- To support asset downloading (images, CSS), extend the crawler to fetch and save these files.
- For multi-file output, modify the logic to write each page to a separate Markdown file.
- Add support for command-line options to customize crawling depth, output location, or filtering.

## Contributing

Contributions are welcome! Please open issues or submit pull requests for improvements, bug fixes, or new features.

## License

This project is licensed under the MIT License.

## Credits

- [HtmlAgilityPack](https://github.com/zzzprojects/html-agility-pack)
- [ReverseMarkdown](https://github.com/mysticmind/reversemarkdown-net)

---

**SiteToMarkdown** makes it easy to convert entire websites or documentation portals into Markdown for offline use, migration, or further editing.
