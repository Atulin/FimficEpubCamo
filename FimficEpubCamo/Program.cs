using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.RegularExpressions;


// Get source directory
Console.WriteLine("Input source directory:");
var dir = Console.ReadLine() ?? "";

// Get all chapter files in the directory
var files = Directory.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories).ToImmutableArray();
var chapters = files.Where(f => f.Contains("chapter-")).ToImmutableArray();
var others = files.Except(chapters);
Console.WriteLine($"Found {files.Length} files, {chapters.Length} chapters");

// Get output directory
Console.WriteLine("Input out directory:");
var output = Console.ReadLine() ?? "";

// Create it if it doesn't exist
if (!Directory.Exists(output))
{
    Console.WriteLine("Directory does not exist, creating...");
    Directory.CreateDirectory(output);
    Console.WriteLine($"Created {output}");
}

// Copy files
Console.WriteLine("Copying other files");

foreach (var directory in Directory.GetDirectories(dir, "*", SearchOption.AllDirectories))
{
    Directory.CreateDirectory(directory.Replace(dir, output));
}
foreach (var file in others)
{
    File.Copy(file, file.Replace(dir, output));
}


// Asynchronously process chapters
Console.WriteLine("Processing chapters...");
await Task.WhenAll(chapters.Select(f => EditFile(dir, output, f)));



Console.WriteLine("Finished all!");

Process.Start(new ProcessStartInfo
{
    Arguments = output,
    FileName = "explorer.exe"
});

return;


async Task EditFile(string sourceDir, string targetDir, string fileName)
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
    
    await File.WriteAllTextAsync(fileName.Replace(sourceDir, targetDir), text);
    Console.WriteLine($"\tFinished {fileName}");
}

internal static class StringExtensions
{
    public static string CleanUrl(this string url) => url.Replace("%3A", ":").Replace("%2F", "/");
}

internal partial class Program
{
    [GeneratedRegex("""
                    \"https\:\/\/camo\.fimfiction\.net\/.+\?url\=(.+?\.png|jpg|jpeg)\"
                    """, RegexOptions.Multiline)]
    private static partial Regex MyRegex();
}