using CodeMechanic.Types;
using Hydro;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace thecodemechanic.Pages.Shared.Components;

[HtmlTargetElement("tooltip")]
public class HydroToolTip : HydroView
{
    public string text { get; set; } = String.Empty;

    // computed
    public bool slot_full => (Slot().Value ?? string.Empty).NotEmpty();
    public bool text_exists => text.NotEmpty();

    public bool load_as_tooltip => slot_full && text_exists;
    public bool load_slot_only => !load_as_tooltip;
}