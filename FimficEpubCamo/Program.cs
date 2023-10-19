using System.Collections.Immutable;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.RegularExpressions;


var tempDirectory = Path.Combine(Path.GetTempPath(), $"FimficEpubCamo_{Random.Shared.Next()}");

Console.WriteLine(tempDirectory);

// Get source file
Console.WriteLine("Source file:");
var sourceFile = Console.ReadLine() ?? "";

// Extract the file
Console.WriteLine("Extracting...");
ZipFile.ExtractToDirectory(sourceFile, tempDirectory);

// Get all chapter files in the directory
var allFiles = Directory.EnumerateFiles(tempDirectory, "*.*", SearchOption.AllDirectories).ToImmutableArray();
var chapterFiles = allFiles.Where(f => f.Contains("chapter-")).ToImmutableArray();
Console.WriteLine($"Found {allFiles.Length} files, {chapterFiles.Length} chapters");

// Get output file suffix
Console.WriteLine("Output file suffix:");
var outputSuffix = Console.ReadLine() ?? "";

// Asynchronously process chapters
Console.WriteLine("Processing chapters...");
await Task.WhenAll(chapterFiles.Select(EditFile));

// Zip the file back up
Console.WriteLine("Creating file...");
ZipFile.CreateFromDirectory(tempDirectory, sourceFile.Replace(".epub", $"-{outputSuffix}.epub"));

// Temp directory cleanup
Console.WriteLine("Temporary directory cleanup...");
Directory.Delete(tempDirectory, true);

Console.WriteLine("Finished all!");

Process.Start(new ProcessStartInfo
{
    Arguments = Path.GetDirectoryName(sourceFile),
    FileName = "explorer.exe"
});

return;


async Task EditFile(string fileName)
{
    Console.WriteLine($"\tModifying {fileName}");
    var text = await File.ReadAllTextAsync(fileName);

    var matches = MyRegex().Matches(text);
    foreach (Match match in matches)
    {
        text = text.Replace(match.Value, $"""
                                          "{match.Groups[1].Value.CleanUrl()}"
                                          """);
    }

    await File.WriteAllTextAsync(fileName, text);
    Console.WriteLine($"\tFinished {fileName}");
}

internal static class StringExtensions
{
    public static string CleanUrl(this string url) => url.Replace("%3A", ":").Replace("%2F", "/");
}

internal partial class Program
{
    [GeneratedRegex("""
                    \"https\:\/\/camo\.fimfiction\.net\/.+\?url\=(.+?)\"
                    """, RegexOptions.Multiline)]
    private static partial Regex MyRegex();
}