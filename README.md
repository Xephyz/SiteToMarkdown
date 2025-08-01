[![NuGet](https://img.shields.io/nuget/v/SiteToMarkdown?color=blue)](https://www.nuget.org/packages/SiteToMarkdown)

# SiteToMarkdown

SiteToMarkdown is a command-line utility written in C# (.NET 9) that crawls a website starting from a given URL, downloads its HTML content, and converts it into Markdown format. The tool recursively follows internal links, processes each page, and outputs a single Markdown file containing the combined content. This is useful for archiving, documentation, or transforming web-based documentation into Markdown for further editing or publishing.

## Features

- **Recursive Crawling**: Starts from a base URL and follows internal links to discover and process all reachable pages within the same domain.
- **HTML to Markdown Conversion**: Uses [HtmlAgilityPack](https://github.com/zzzprojects/html-agility-pack) to parse HTML and [ReverseMarkdown](https://github.com/mysticmind/reversemarkdown-net) to convert HTML to Markdown.
- **Single Output File**: Aggregates all discovered pages into one Markdown file, named according to the base URL.
- **Robust Error Handling**: Skips pages that fail to load and continues processing others.
- **Customizable Markdown Conversion**: Configures ReverseMarkdown to bypass unknown HTML tags and preserve code blocks.
- **.NET Tool Support**: Can be installed and run as a global or local .NET CLI tool using the command `s2m`.
- **Advanced Filtering**: Supports filtering out HTML elements by their `id` or `class` attributes via command-line options.
- **Experimental Link Conversion**: Optionally converts internal HTML links to Markdown anchor links for improved navigation in the output file.

## Requirements

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- NuGet packages (automatically restored):
    - [HtmlAgilityPack](https://www.nuget.org/packages/HtmlAgilityPack)
    - [ReverseMarkdown](https://www.nuget.org/packages/ReverseMarkdown)
    - [CommandLineParser.Commands](https://www.nuget.org/packages/CommandLineParser.Commands)

## Installation

### Clone and Build

1. **Clone the repository:**
    ```sh
    git clone https://github.com/xephyz/SiteToMarkdown.git
    cd SiteToMarkdown
    ```

2. **Restore dependencies:**
    ```powershell
    dotnet restore
    ```

3. **Build the project:**
    ```powershell
    dotnet build
    ```

### Install as a .NET Tool (Optional)

You can package and install SiteToMarkdown as a .NET CLI tool for easier usage:

1. **Pack the tool:**
    ```powershell
    dotnet pack -c Release -o ./nupkg
    ```

2. **Install the tool globally:**
    ```powershell
    dotnet tool install --global --add-source ./nupkg s2m
    ```

---

## Usage

### As a .NET Tool

After installing as a tool, simply run:
```powershell
s2m "https://example.com/docs"
```

### From Source

You can also run the tool directly from the source:
```powershell
dotnet run --project ./src/SiteToMarkdown -- "https://example.com/docs"
```

---

### Command-Line Arguments and Options

| Argument / Option         | Description                                                                                      | Example                                      |
|-------------------------- |--------------------------------------------------------------------------------------------------|----------------------------------------------|
| `<Url>`                   | **(Required)** The absolute URL to begin scraping from.                                          | `https://openfga.dev/docs`                   |
| `--filter-id`             | Comma-separated list of HTML element IDs to ignore (remove from output).                         | `--filter-id=sidebar,footer`                 |
| `--filter-class`          | Comma-separated list of HTML classes to ignore (remove from output).                             | `--filter-class=ad-banner,nav`               |
| `-c`, `--convert-links`   | **EXPERIMENTAL**: Convert internal HTML links to Markdown anchor links in the output.            | `-c` or `--convert-links`                    |


#### Example: Filtering and Link Conversion

```powershell
s2m "https://openfga.dev/docs" --filter-id=sidebar,footer --filter-class=ad-banner,nav -c
```

This command will:
- Crawl all internal pages starting from the given URL.
- Remove any HTML elements with IDs `sidebar` or `footer`.
- Remove any HTML elements with classes `ad-banner` or `nav`.
- Convert internal HTML links to Markdown anchor links.
- Output the combined Markdown to a file named after the domain and path.

---

## How It Works

1. **Initialization**: The program starts with a queue containing the base URL.
2. **Crawling**: It loads each page using HtmlAgilityPack, converts the HTML to Markdown, and appends the result to a StringBuilder.
3. **Link Discovery**: All `<a href="...">` links are discovered, normalized, and new internal links are added to the queue.
4. **Filtering**: If specified, elements matching the provided IDs or classes are removed from the HTML before conversion.
5. **Link Conversion**: If enabled, internal links are converted to Markdown anchor links for easier navigation in the output.
6. **Recursion**: The process continues until all reachable pages are processed.
7. **Output**: The final Markdown is written to a file named after the domain and path.

## Configuration (WIP, may be added as command-line options in the future)

- **UnknownTags**: Set to `Bypass` to ignore unknown HTML tags.
- **CleanupUnnecessarySpaces**: Set to `false` to preserve code block formatting.

You can modify these options in `Program.cs` to suit your needs.

## Limitations

- Only internal links (same domain) are followed.
- Fragment-only links (e.g., `#section`) and root (`/`) links are ignored.
- The tool does **not** handle authentication, JavaScript-rendered content, or rate limiting.
- Output is a single Markdown file; images and other assets are **not** downloaded.
- Only the HTML content is converted; no CSS or JavaScript is processed.
- The `--convert-links` feature is experimental and may not handle all edge cases.

## Extending

- **Asset Downloading**: To support downloading images, CSS, or other assets, extend the crawler to fetch and save these files.
- **Multi-file Output**: Modify the logic to write each page to a separate Markdown file if desired.
- **Additional Command-line Options**: Add support for more options to customize crawling depth, output location, filtering, etc.
- **Authentication**: Implement support for authenticated sites if needed.

## Contributing

Contributions are welcome! Please open issues or submit pull requests for improvements, bug fixes, or new features.

## License

This project is licensed under the MIT License. See the [LICENSE](../../LICENSE) file for details.

## Credits

- [HtmlAgilityPack](https://github.com/zzzprojects/html-agility-pack)
- [ReverseMarkdown](https://github.com/mysticmind/reversemarkdown-net)
- [CommandLineParser.Commands](https://github.com/commandlineparser/commandline)

---

**SiteToMarkdown** makes it easy to convert entire websites or documentation portals into Markdown for offline use, migration, or further editing.
