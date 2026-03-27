using AspNetCoreTemplate.Configuration;
using AspNetCoreTemplate.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MySql.EntityFrameworkCore.Extensions;

namespace AspNetCoreTemplate.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddConfiguredDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.Provider), "Database provider is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.ConnectionString), "Database connection string is required.")
            .ValidateOnStart();

        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var databaseOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            var provider = databaseOptions.Provider.Trim().ToLowerInvariant();
            var connectionString = databaseOptions.ConnectionString;

            switch (provider)
            {
                case "sqlserver":
                case "mssql":
                    options.UseSqlServer(connectionString);
                    break;
                case "postgres":
                case "postgresql":
                    options.UseNpgsql(connectionString);
                    break;
                case "mysql":
                    options.UseMySQL(connectionString);
                    break;
                default:
                    throw new InvalidOperationException(
                        $"Unsupported database provider '{databaseOptions.Provider}'. Supported values: SqlServer, PostgreSql, MySql.");
            }
        });

        return services;
    }

    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var databaseOptions = scope.ServiceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;

        if (!databaseOptions.AutoApplySchema)
        {
            return;
        }

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }
}
