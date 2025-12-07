using Serilog.Core;

namespace thecodemechanic;

public class Application
{
    private readonly Logger logger;

    private LocalDocumentService docs;
    private readonly Regex101Service regex101;

    public Application(Logger logger
        , Regex101Service extractionModelGenerator,
        LocalDocumentService docs
    )
    {
        this.logger = logger;
        this.regex101 = extractionModelGenerator;
        this.docs = docs;
    }

    public async Task Run()
    {
        await docs.Run();
        await regex101.Run();
    }
}