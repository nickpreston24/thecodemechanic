using Hydro;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace thecodemechanic.Pages.Shared.Components;

[HtmlTargetElement("navbar")]
public class HydroNav : HydroView
{
    public string Title { get; set; } = nameof(thecodemechanic);
    public List<NavbarLink> links { get; set; } = new();
}

public record NavbarLink(string text = "Home", string url = "/", string icon = "")
{
    public List<NavbarLink> children { get; set; } = new();
}
