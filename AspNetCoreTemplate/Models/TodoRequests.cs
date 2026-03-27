namespace AspNetCoreTemplate.Models;

public sealed record CreateTodoRequest(
    string Title,
    string? Description,
    bool IsCompleted);

public sealed record UpdateTodoRequest(
    string Title,
    string? Description,
    bool IsCompleted);
