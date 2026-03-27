namespace AspNetCoreTemplate.Configuration;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public string Provider { get; set; } = string.Empty;

    public string ConnectionString { get; set; } = string.Empty;

    public bool AutoApplySchema { get; set; } = true;
}
