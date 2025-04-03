using Hydro;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace thecodemechanic.Pages.Shared.Components;

[HtmlTargetElement("modal")]
public class HydroModal : HydroView
{
    public string btnText { get; set; } = "Open";
    public string Title { get; set; } = "Learn";
    public bool has_button => true;
}
