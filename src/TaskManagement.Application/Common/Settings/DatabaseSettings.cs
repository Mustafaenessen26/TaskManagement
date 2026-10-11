namespace TaskManagement.Application.Common.Settings;

public class DatabaseSettings
{
    public const string SectionName = "ConnectionStrings";

    public string DefaultConnection { get; set; } = string.Empty;
}