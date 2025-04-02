using System.Data.SqlClient;
using CodeMechanic.MySql;
using Dapper;
using Htmx;
using justdoit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace thecodemechanic.Pages.Samples.JustDoIt;

public class Index : PageModel
{
    [BindProperty(SupportsGet = true)] public int Id { get; set; }

    public static List<Todo> Database = new()
    {
        new(1, "Buy Milk"),
        new(2, "Moopsy! :3"),
    };

    public void OnGet()
    {
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
        string sql = @"insert into todos(content) values (@content)";

        using var connection = SqlConnections.Create();
        // using var command = new SqlCommand(connection)
        var anonymousCustomer = new
            { content = "ZZZ Top" };

        var rowsAffected =
            await connection.ExecuteAsync(sql, anonymousCustomer);
        Console.WriteLine(rowsAffected);

        if (Database.FirstOrDefault(x => x.Id == Id) is { } t)
        {
            t.Content = todo.Content;
            t.Id = todo.Id;

            return Request.IsHtmx()
                ? Partial("_Row", t)
                : Redirect("Index");
        }

        return BadRequest();
    }

    public List<Todo> Todos => Database;
}