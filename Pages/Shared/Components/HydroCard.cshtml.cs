using Hydro;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace thecodemechanic.Pages.Shared.Components;

[HtmlTargetElement("card")]
public class HydroCard : HydroView
{
    public int width { get; set; } = 96;
    public string title { get; set; } = string.Empty;
}
