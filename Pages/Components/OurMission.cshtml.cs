using Drip.UI;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace thecodemechanic.Pages.Components;

[HtmlTargetElement("our-mission")]
public class OurMission : IslandTagHelper
{
    public void OnGet()
    {
    }
}