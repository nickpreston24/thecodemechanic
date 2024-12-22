using Hydro;

namespace thecodemechanic.Pages.Shared.Components;

public class HydroAlert : HydroView
{
    public string message { get; set; } = string.Empty;
    public Exception ex { get; set; }
}