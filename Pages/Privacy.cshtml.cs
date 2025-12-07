using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog.Core;

namespace thecodemechanic.Pages;

public class PrivacyModel : PageModel
{
    private readonly Logger _logger;

    public PrivacyModel(Logger logger)
    {
        _logger = logger;
    }

    public void OnGet() { }
}
