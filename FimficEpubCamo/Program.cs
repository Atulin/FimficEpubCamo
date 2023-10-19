using System.Collections.Immutable;
using System.Text.RegularExpressions;

Console.WriteLine("Input source directory:");
var dir = Console.ReadLine() ?? "";


var files = Directory.EnumerateFiles(dir).Where(f => f.Contains("chapter-")).ToImmutableArray();
Console.WriteLine($"Found {files.Length} files");


Console.WriteLine("Input out directory:");
var output = Console.ReadLine() ?? "";


if (!Directory.Exists(output))
{
    Console.WriteLine("Directory does not exist, creating...");
    Directory.CreateDirectory(output);
    Console.WriteLine($"Created {output}");
}


await Task.WhenAll(files.Select(f => EditFile(dir, output, f)));

Console.WriteLine("Finished all");
return;


async Task EditFile(string sourceDir, string targetDir, string fileName)
{
    Console.WriteLine($"Modifying {fileName}");
    var text = await File.ReadAllTextAsync(fileName);

    var changed = MyRegex().Replace(text, $"""
                                           "{CleanUrl("$1")}"
                                           """);
    
    await File.WriteAllTextAsync(fileName.Replace(sourceDir, targetDir), changed);
    Console.WriteLine($"Finished {fileName}");
}

string CleanUrl(string url) => url.Replace("%3A", ":").Replace("%2F", "/");

internal partial class Program
{
    [GeneratedRegex("""
                    \"https\:\/\/camo\.fimfiction\.net\/.+\?url\=(.+?\.png|jpg|jpeg)\"
                    """, RegexOptions.Multiline)]
    private static partial Regex MyRegex();
}