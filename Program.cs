using CodeMechanic.Shargs;
using Hydro.Configuration;
using Serilog;
using Serilog.Core;
using Sharpify.Core;

namespace thecodemechanic;

internal class Program
{
    static async Task Main(string[] args)
    {
        var arguments = new ArgsMap(args);
        bool debug = arguments.HasFlag("--debug");

        var logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File(
                "./thecodemechanic/thecodemechanic.log",
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true
            )
            .CreateLogger();

        bool run_as_web = arguments.HasCommand("web");
        bool run_as_cli = !run_as_web;

        if (run_as_cli)
        {
            await RunAsCli(arguments, logger);
        }

        if (run_as_web)
        {
            RunAsWeb(args);
        }
    }

    static async Task RunAsCli(ArgsMap arguments, Logger logger)
    {
        var services = CreateServices(arguments, logger);
        Application app = services.GetRequiredService<Application>();
        await app.Run();
    }

    private static void RunAsWeb(params string[] args)
    {
        var arguments = new ArgsMap(args);


        var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddHydro();

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
    }

    private static ServiceProvider CreateServices(
        ArgsMap arguments
        ,
        Logger logger
    )
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton(arguments)
            .AddSingleton<Logger>(logger)
            .AddSingleton<Application>()
            .AddScoped<RegexExtractionGenerator>()
            .BuildServiceProvider();

        return serviceProvider;
    }
}

public class Application
{
    private readonly Logger logger;
    private readonly RegexExtractionGenerator model_generator;

    public Application(Logger logger
        , RegexExtractionGenerator model_generator
    )
    {
        this.logger = logger;
        this.model_generator = model_generator;
    }

    public async Task Run()
    {
        await model_generator.Run();
    }
}