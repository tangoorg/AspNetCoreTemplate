using AspNetCoreTemplate.Endpoints;
using AspNetCoreTemplate.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddConfiguredDatabase(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

await app.InitializeDatabaseAsync();

app.MapGet("/", () => Results.Ok(new
{
    Message = "ASP.NET Core EF Core CRUD API is running.",
    OpenApi = "/openapi/v1.json",
    Swagger = "/swagger"
}));

app.MapTodoEndpoints();

app.Run();
