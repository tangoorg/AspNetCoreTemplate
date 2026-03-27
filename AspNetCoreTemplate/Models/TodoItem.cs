namespace AspNetCoreTemplate.Models;

public sealed class TodoItem
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
