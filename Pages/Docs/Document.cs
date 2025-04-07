using CodeMechanic.FileSystem;
using CodeMechanic.Types;

namespace thecodemechanic;

public sealed class Document
{
    public string file_path { get; set; } = string.Empty;

    public Grepper.GrepResult grep_result { get; set; }

    public Document(string file_path)
    {
        this.file_path = file_path;
    }

    public Document(Grepper.GrepResult result)
    {
        this.grep_result = result;
    }


    public string extension => file_path.NotEmpty()
        ? Path.GetExtension(file_path)
        : string.Empty;
}