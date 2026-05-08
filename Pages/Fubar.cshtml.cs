using CodeMechanic.Razorhat;
using CodeMechanic.Shargs;
using Microsoft.AspNetCore.Mvc;
using Serilog.Core;

namespace thecodemechanic.Pages;

public class Fubar : RazorhatIsland
{
    public Fubar(ArgsMap a, Logger l) : base(a, l)
    {
    }

    public async Task<IActionResult> OnGet()
    {
        var message = $"WELCOME TO THE ISLAND!";
        logger.Information(message);
        return Content($"<div class=\"bg-black\"><p class=\"badge\">{message}</b></div>");
    }
}
