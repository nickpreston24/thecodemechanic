using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog.Core;

namespace thecodemechanic.Pages;

public class IndexModel : PageModel
{
    private readonly Logger logger;

    public IndexModel(Logger logger)
    {
        this.logger = logger;
    }

    public void OnGet()
    {
        logger.Information("Loaded main page.");
    }
}
