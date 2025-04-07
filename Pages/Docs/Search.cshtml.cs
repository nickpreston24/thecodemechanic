using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace thecodemechanic.Pages.Docs;

public class Search : PageModel
{
    public string search { get; set; } = string.Empty;

    public void OnGet(string search, string title)
    {
        Console.WriteLine($"searching for '{search}'");
        Console.WriteLine($"searching for doc named '{title}'");
        this.search = search;
    }

    public IActionResult OnPost(string search)
    {
        Console.WriteLine(nameof(OnPost));
        Console.WriteLine($"searching for '{search}'");

        return Partial("_Documents", null);
    }
}