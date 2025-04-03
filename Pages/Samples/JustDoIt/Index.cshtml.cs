using CodeMechanic.Diagnostics;
using CodeMechanic.MySql;
using Dapper;
using Htmx;
using justdoit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using railway;

namespace thecodemechanic.Pages.Samples.JustDoIt;

public class Index : PageModel
{
    [BindProperty(SupportsGet = true)] public int Id { get; set; }

    public static List<Todo> Database = new();

    public async Task OnGet()
    {
        Console.WriteLine(nameof(OnGet));
        using var connection = SqlConnections.Create();
        var all_todos =
            await connection.QueryAsync<Todo>(
                @"select content, id from todos order by created_at desc limit 10;");

        // var logs = await Procs.ViewLatestLogs.QueryAsync();
        // logs.Take(5).Dump("logs");

        Database.AddRange(all_todos);
    }

    public IActionResult OnGetRow()
    {
        return Partial("_Row", Database.FirstOrDefault(p => p.Id == Id));
    }

    public IActionResult OnGetEdit()
    {
        return Partial("_Edit", Database.FirstOrDefault(p => p.Id == Id));
    }

    public async Task<IActionResult> OnPostUpdate([FromForm] Todo todo)
    {
        int rows = await Upsert(todo);

        // if nothing is updated, throw:
        if (rows == 0 || Database.FirstOrDefault(x => x.Id == Id) is not { } t)
            return BadRequest();

        // update the table row:
        t.Content = todo.Content;
        t.Id = todo.Id;

        return Request.IsHtmx() ? Partial("_Row", t) : Redirect("Index");
    }

    private async Task<int> Upsert(Todo todo)
    {
        // string sql =
        //     @"insert into todos(content) values (@content) ON DUPLICATE KEY UPDATE content = VALUES(content)";
        //
        // using var connection = SqlConnections.Create();

        // var updated_todo = new { content = todo.Content };
        //
        // var rowsAffected = await connection.ExecuteAsync(sql, updated_todo);

        // Console.WriteLine(rowsAffected);

        int rows = await Procs.upserttodo.UpsertAsync(todo);

        return rows;
    }

    public List<Todo> Todos => Database;
}