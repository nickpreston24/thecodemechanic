using CodeMechanic.FileSystem;
using CodeMechanic.Shargs;
using Hydro.Configuration;
using Serilog;
using Serilog.Core;

namespace thecodemechanic;

internal class Program
{
    static async Task Main(string[] args)
    {
        var arguments = new ArgsMap(args);

        var logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File(
                ".logs/thecodemechanic.log",
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true
            )
            .CreateLogger();

        bool run_as_web = arguments.HasCommand("web");
        bool run_as_cli = !run_as_web;

        if (run_as_cli) await RunAsCli(arguments, logger);
        if (run_as_web) RunAsWeb(logger, args);
    }

    private static async ValueTask CreateToolsDir()
    {
        var user_profile =
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        string dotnet_tools_dir = Path
            .Combine(user_profile, ".dotnet/tools", ".cm")
            .Replace("\\", "/");

        Console.WriteLine($"tools dir :>> {dotnet_tools_dir}");

        // await $"ls {dotnet_tools_dir}".Bash();

        var fi = new SaveFile("foo")
            .To(dotnet_tools_dir)
            .As("test.txt", debug: false);

        // await $"ls {fi.Directory}".Bash();
    }

    static async Task RunAsCli(ArgsMap arguments, Logger logger)
    {
        var services = CreateServices(arguments, logger);
        Application app = services.GetRequiredService<Application>();
        await app.Run();
    }

    private static void RunAsWeb(Logger logger, params string[] args)
    {
        logger.Information("Setting up as a web app.");

        var arguments = new ArgsMap(args);

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddHydro();

        builder.Services.AddSingleton<Logger>(logger);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();
        app.UseHydro(builder.Environment);

        app.Run();
        logger.Information("Running as a web app.");
    }

    private static ServiceProvider CreateServices(ArgsMap arguments,
        Logger logger)
    {
        var serviceProvider = new ServiceCollection()
            .UseHttpClients()
            .AddSingleton(arguments)
            .AddSingleton<Logger>(logger)
            .AddSingleton<Application>()
            .AddSingleton<LocalDocumentService>()
            .AddScoped<Regex101Service>()
            .BuildServiceProvider();

        return serviceProvider;
    }
}