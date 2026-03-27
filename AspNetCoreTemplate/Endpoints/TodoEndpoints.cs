using AspNetCoreTemplate.Data;
using AspNetCoreTemplate.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreTemplate.Endpoints;

public static class TodoEndpoints
{
    public static IEndpointRouteBuilder MapTodoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/todos")
            .WithTags("Todos");

        group.MapGet("/", async (AppDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var items = await dbContext.TodoItems
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync(cancellationToken);

            return Results.Ok(items);
        })
        .WithName("GetTodos");

        group.MapGet("/{id:int}", async (int id, AppDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var item = await dbContext.TodoItems
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            return item is null ? Results.NotFound() : Results.Ok(item);
        })
        .WithName("GetTodoById");

        group.MapPost("/", async (CreateTodoRequest request, AppDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var validationError = Validate(request.Title);
            if (validationError is not null)
            {
                return validationError;
            }

            var item = new TodoItem
            {
                Title = request.Title.Trim(),
                Description = request.Description?.Trim(),
                IsCompleted = request.IsCompleted,
                CreatedAtUtc = DateTime.UtcNow
            };

            dbContext.TodoItems.Add(item);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.Created($"/api/todos/{item.Id}", item);
        })
        .WithName("CreateTodo");

        group.MapPut("/{id:int}", async (int id, UpdateTodoRequest request, AppDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var validationError = Validate(request.Title);
            if (validationError is not null)
            {
                return validationError;
            }

            var item = await dbContext.TodoItems.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (item is null)
            {
                return Results.NotFound();
            }

            item.Title = request.Title.Trim();
            item.Description = request.Description?.Trim();
            item.IsCompleted = request.IsCompleted;

            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.Ok(item);
        })
        .WithName("UpdateTodo");

        group.MapDelete("/{id:int}", async (int id, AppDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var item = await dbContext.TodoItems.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (item is null)
            {
                return Results.NotFound();
            }

            dbContext.TodoItems.Remove(item);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
        })
        .WithName("DeleteTodo");

        return app;
    }

    private static IResult? Validate(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["title"] = ["Title is required."]
            });
        }

        return null;
    }
}
