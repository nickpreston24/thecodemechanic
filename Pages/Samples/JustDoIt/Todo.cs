namespace justdoit;

public class Todo
{
    public Todo()
    {
    }

    public Todo(int id, string content)
    {
        Id = id;
        Content = content;
    }

    public int Id { get; set; }
    public string Content { get; set; }
}