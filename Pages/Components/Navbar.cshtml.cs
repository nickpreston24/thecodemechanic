using Microsoft.AspNetCore.Razor.TagHelpers;

namespace thecodemechanic.Pages.Components;

[HtmlTargetElement("cm-navbar")]
public class Navbar : TagHelper
{
    public void OnGet()
    {
        Console.WriteLine("navbar has loaded");
    }
}