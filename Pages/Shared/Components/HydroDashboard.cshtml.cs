using Hydro;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace thecodemechanic.Pages.Shared.Components;

[HtmlTargetElement("dashboard")]
public class HydroDashboard : HydroView
{
    public string Name { get; set; } = string.Empty;
    public List<MenuItem> items { get; set; } = new();
}

public class MenuItem { }
