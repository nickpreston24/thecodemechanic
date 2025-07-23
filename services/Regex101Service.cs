using CodeMechanic.Async;
using CodeMechanic.Diagnostics;
using CodeMechanic.Shargs;
using CodeMechanic.Types;
using JsonFlatFileDataStore;
using Sharprompt;
using Spectre.Console;
using Vogen;

namespace thecodemechanic;

public class Regex101Service : QueuedService
{
    /// <summary>
    /// todo: deep dive into making a custom Regex101HttpClient.
    /// For now, just straight up downloading inefficiently.
    /// </summary>
    // https://medium.com/@iamprovidence/http-client-in-c-best-practices-for-experts-840b36d8f8c4
    // private IHttpClientFactory factory;
    public Regex101Service(ArgsMap arguments
        // , [FromServices] IHttpClientFactory factory
    )
    {
        // this.factory = factory;
        if (arguments.HasCommand("regex"))
        {
            steps.Add(SyncPatterns);
        }
    }

    private async Task SyncPatterns()
    {
        bool sync = Prompt.Confirm("Run regex101 sync?");
        if (!sync)
        {
            AnsiConsole.Markup("[yellow]exiting Regex101 sync[/]\n");
            return;
        }

        var store = new DataStore("regex101.patterns.json");


        var collection = store.GetCollection<Regex101Pattern>("data");
        // var results = collection.AsQueryable().ToList();
        var results = collection
            .AsQueryable().ToList();
        results.Dump(nameof(results));

        AnsiConsole.Markup($"[green]total patterns {results.Count}![/]\n\n");

        // var client = factory.CreateClient(); // 👈 resolve a client
        // var response = await client.GetFromJsonAsync<QuotesResponse>("https://dummyjson.com/quotes");
        //
        // AnsiConsole.Markup($"[yellow]response\n{response}[/]\n");
        // return response;

        return;
        var range = Enumerable.Range(1, Regex101Constants.MaxPages.Value.ToInt());

        var Q = new SerialQueue();

        // // using HttpClient client = new HttpClient();
        //
        // var tasks = range
        //     .Select(next_page => Q
        //         .Enqueue(() =>
        //         {
        //             //
        //             var dl = new Regex101Download();
        //             // client.BaseAddress = new Uri(Regex101Download.fetch_url);
        //         }));
    }

    record Regex101Download
    {
        public string fetch_url = string.Empty;

        public Regex101Download(int page_num = 1)
        {
            PageNum = page_num;
            this.fetch_url = Regex101Constants.HistoryUrl.Value + PageNum;
        }

        public int PageNum { get; init; }

        public void Deconstruct(out int pagenum)
        {
            pagenum = this.PageNum;
        }
    }
}

public class Regex101Pattern
{
    public int version { get; set; }
    public string permalinkFragment { get; set; } = string.Empty;
    public string regex { get; set; } = string.Empty;
    public string delimiter { get; set; } = string.Empty;
    public string flags { get; set; } = string.Empty;

    public string flavor { get; set; } = string.Empty;
    public string dateAdded { get; set; } = string.Empty;

    public string title { get; set; } = string.Empty;
    public string regexDeleteCode { get; set; } = string.Empty;

    public string versionDeleteCode { get; set; } = string.Empty;

    public string libraryTitle { get; set; } = string.Empty;
    // public string tags { get; set; } = string.Empty;
    // public string isPrivate { get; set; } = string.Empty;
    // public string isFavorite { get; set; } = string.Empty;
    // public string isOwner { get; set; } = string.Empty;
    // public string isLibraryEntry { get; set; } = string.Empty;
    // public string isLocked { get; set; } = string.Empty;
}

// sample
public class QuotesResponse
{
    public int id { get; set; }
    public string quote { get; set; }
    public string author { get; set; }
}

[ValueObject<string>]
[Instance("MaxPages", 7)]
[Instance("HistoryUrl", "https://regex101.com/api/user/history/mine/")]
public partial class Regex101Constants
{
}