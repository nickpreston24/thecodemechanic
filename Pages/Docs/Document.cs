using CodeMechanic.FileSystem;
using CodeMechanic.Types;

namespace thecodemechanic;

public sealed class Document
{
    private readonly Grepper.GrepResult grep_result;

    public Document(string file_path)
    {
        this.file_path = file_path;
    }

    public Document(Grepper.GrepResult result)
    {
        this.grep_result = result;
    }

    public string file_path { get; set; } = string.Empty;

    public string extension => file_path.NotEmpty()
        ? Path.GetExtension(file_path)
        : string.Empty;
}