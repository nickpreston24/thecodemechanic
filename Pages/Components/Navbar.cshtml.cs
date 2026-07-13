using Drip.UI;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace thecodemechanic.Pages.Components;

[HtmlTargetElement("cm-navbar")]
public class Navbar : IslandTagHelper
{
    public void OnGet()
    {
        Console.WriteLine("navbar has loaded");
    }
}