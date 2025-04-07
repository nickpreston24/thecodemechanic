using System.Diagnostics;
using System.Text.RegularExpressions;
using CodeMechanic.Async;
using CodeMechanic.Diagnostics;
using CodeMechanic.FileSystem;
using CodeMechanic.Shargs;
using CodeMechanic.Types;
using Vogen;

namespace thecodemechanic;

public class LocalDocumentService : QueuedService
{
    private readonly ArgsMap arguments;
    private string file_mask = string.Empty;
    private string line_pattern = string.Empty;
    private string root_dir = string.Empty;
    private bool debug;
    private readonly Regex line_pattern_regex;

    public LocalDocumentService(ArgsMap arguments)
    {
        this.arguments = arguments;
        this.debug = arguments.HasFlag("--debug");

        (_, file_mask) = this.arguments.WithFlags("-m", "--mask");
        (_, line_pattern) = this.arguments.WithFlags("-p", "--pattern");
        (_, root_dir) = this.arguments.WithFlags("-d", "--dir");

        if (debug)
            arguments.Dump("args");

        this.line_pattern_regex =
            new Regex(line_pattern, RegexOptions.Compiled);

        var grepper = new Grepper()
        {
            RootPath = root_dir.IsEmpty()
                ? Directory.GetCurrentDirectory()
                : root_dir,
            FileSearchMask = file_mask
        };

        steps.Add(() =>
            SearchLocalDriveForDocs(grepper));

        // steps.Add(PromptUserForProgram);
    }

    private async Task PromptUserForProgram()
    {
    }

    public async Task<Document[]> SearchLocalDriveForDocs(Grepper grepper)
    {
        if (grepper == null || grepper.FileSearchMask.IsEmpty())
            return new Document("").AsArray();

        bool search_only_file_names = grepper.FileNamePattern.NotEmpty() &&
                                      grepper.FileSearchMask.NotEmpty() &&
                                      grepper.FileSearchLinePattern.IsEmpty();

        if (search_only_file_names)
        {
            return grepper
                .GetFileNames()
                .Select(filepath => new Document(filepath))
                .ToArray();
        }

        if (!search_only_file_names)
        {
            return grepper
                .GetMatchingFiles(line_pattern_regex)
                .Select(result => new Document(result))
                .ToArray();
        }

        return Array.Empty<Document>();
    }

    public static void OpenWithDefaultProgram(
        DefaultProgram program,
        string file_path)
    {
        using Process process = new Process();
        process.StartInfo.FileName =
            process.StartInfo.Arguments = "\"" + file_path + "\"";
        process.Start();
    }

    /// Wants:
    /// 1. Open With
    /// 2. Open in Explorer (default)
    /// 3. open file w/ nano
    /// 4. Choose file or dir to open
    /// 5. limit the max files and max dirs to search
    /// 6. test on work laptop
    /// 7. find common dev programs and their locations (vscode, vs2022, bash, etc.)
    /// 8. cache common folders and their locations
    /// 9. first refactoring pass should turn all public methods to be callable outside of this service w/ needful params added.
}

[ValueObject<string>]
[Instance("Unspecified", "Unspecified")]
[Instance("VSCode", "VSCode")]
[Instance("Explorer", "explorer")]
public partial class DefaultProgram
{
}