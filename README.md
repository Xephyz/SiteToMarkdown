# SiteToMarkdown

SiteToMarkdown is a command-line utility written in C# (.NET 9) that crawls a website starting from a given URL, downloads its HTML content, and converts it into Markdown format. The tool recursively follows internal links, processes each page, and outputs a single Markdown file containing the combined content. This is useful for archiving, documentation, or transforming web-based documentation into Markdown for further editing or publishing.

## Features

- **Recursive Crawling**: Starts from a base URL and follows internal links to discover and process all reachable pages within the same domain.
- **HTML to Markdown Conversion**: Uses [HtmlAgilityPack](https://github.com/zzzprojects/html-agility-pack) to parse HTML and [ReverseMarkdown](https://github.com/mysticmind/reversemarkdown-net) to convert HTML to Markdown.
- **Single Output File**: Aggregates all discovered pages into one Markdown file, named according to the base URL.
- **Robust Error Handling**: Skips pages that fail to load and continues processing others.
- **Customizable Markdown Conversion**: Configures ReverseMarkdown to bypass unknown HTML tags and preserve code blocks.
- **.NET Tool Support**: Can be installed and run as a global or local .NET CLI tool using the command `s2m`.

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

#### Arguments

- The only required argument is the **starting URL** (must be absolute).
- The output file will be named based on the domain and path, e.g., `example.com.docs.md`, and will be created in the working directory.

#### Example

Suppose you want to convert the documentation section of `https://openfga.dev/docs` to Markdown:
```powershell
s2m "https://openfga.dev/docs"
```

or

```powershell
dotnet run --project ./src/SiteToMarkdown -- "https://openfga.dev/docs"
```

This will produce a file like `openfga.dev.docs.md` containing the combined Markdown content of all discovered documentation pages.

## How It Works

1. **Initialization**: The program starts with a queue containing the base URL.
2. **Crawling**: It loads each page using HtmlAgilityPack, converts the HTML to Markdown, and appends the result to a StringBuilder.
3. **Link Discovery**: All `<a href="...">` links are discovered, normalized, and new internal links are added to the queue.
4. **Recursion**: The process continues until all reachable pages are processed.
5. **Output**: The final Markdown is written to a file named after the domain and path.

## Configuration (WIP, may be added as command-line options in the future)

- **UnknownTags**: Set to `Bypass` to ignore unknown HTML tags.
- **CleanupUnnecessarySpaces**: Set to `false` to preserve code block formatting.

You can modify these options in `Program.cs` to suit your needs.

## Limitations

- Only internal links (same domain) are followed.
- The tool does **not** handle authentication, JavaScript-rendered content, or rate limiting.
- Output is a single Markdown file; images and other assets are **not** downloaded.
- Only the HTML content is converted; no CSS or JavaScript is processed.

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
- [CommandLineParser.Commands](https://github.com/commandlineparser/commandline)

---

**SiteToMarkdown** makes it easy to convert entire websites or documentation portals into Markdown for offline use, migration, or further editing.
