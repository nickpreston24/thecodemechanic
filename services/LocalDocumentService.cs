using System.Diagnostics;
using System.Text.RegularExpressions;
using CodeMechanic.Async;
using CodeMechanic.Diagnostics;
using CodeMechanic.FileSystem;
using CodeMechanic.Shargs;
using CodeMechanic.Types;
using Sharprompt;
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
        (_, line_pattern) = this.arguments.WithFlags("-l", "--line-pattern");
        (_, root_dir) = this.arguments.WithFlags("-d", "--dir");

        if (file_mask.IsEmpty())
            return;

        if (debug)
            arguments.Dump("args");

        // Todo: update mask regex so we don't need this fix:
        file_mask = !file_mask.StartsWith("*") ? $"*{file_mask}" : file_mask;

        Console.WriteLine(file_mask);

        this.line_pattern_regex = line_pattern.NotEmpty()
            ? new Regex(line_pattern, RegexOptions.Compiled)
            : null;

        steps.Add(PromptUserForProgram);
    }

    private async Task PromptUserForProgram()
    {
        string root = root_dir.IsEmpty()
            ? Directory.GetCurrentDirectory()
            : root_dir;

        Console.WriteLine("root :>> " + root);

        var grepper = new Grepper()
        {
            RootPath = root,
            FileSearchMask = file_mask,
            Recursive = true
        };

        var docs_found =
            (await SearchLocalDriveForDocs(grepper)).Select(x => x.file_path);

        // docs_found.Dump(nameof(docs_found), ignoreNulls: true);

        var chosen_doc = Prompt.Select("Open which doc?", docs_found);

        var chosen_program =
            Prompt.Select("With which program?",
                ["NotePad", "Explorer", "nano"]);

        var program = DefaultProgram.From(chosen_program);

        OpenWithDefaultProgram(program, chosen_doc);
    }

    public async Task<Document[]> SearchLocalDriveForDocs(Grepper grepper)
    {
        if (grepper == null || grepper.FileSearchMask.IsEmpty())
            return new Document("").AsArray();

        bool search_only_file_names = grepper.FileNamePattern.NotEmpty() &&
                                      grepper.FileSearchMask.NotEmpty() &&
                                      grepper.FileSearchLinePattern.IsEmpty();

        Console.WriteLine($"only names? {search_only_file_names}");
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
        process.StartInfo.FileName = program.Value;
        process.StartInfo.Arguments = "\"" + file_path + "\"";
        process.Start();
    }

    /// Wants:
    /// 1. Open With [x]
    /// 2. Open in Explorer (default) [x]
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
[Instance("nano", "nano")]
public partial class DefaultProgram
{
}