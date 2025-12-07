using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog.Core;

namespace thecodemechanic.Pages;

public class IndexModel : PageModel
{
    private readonly Logger _logger;

    public IndexModel(Logger logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        _logger.Information("Loaded main page.");
    }
}