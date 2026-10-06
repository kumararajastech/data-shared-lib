namespace DataSharedLib.Connection;

public class DatabaseConnectionOptions
{
    public const string SectionName = "DatabaseConnection";

    public string ConnectionString { get; set; } = string.Empty;
    public int CommandTimeoutSeconds { get; set; } = 30;
    public int MaxRetryCount { get; set; } = 3;
}
