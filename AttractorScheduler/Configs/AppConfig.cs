using Microsoft.Extensions.Configuration;
using System.IO;

namespace AttractorScheduler.Configs;

public class AppConfig
{
    public required string ExcelFilePath { get; init; }
    public required int ClassDurationHours { get; init; }
    public required int WebinarDurationHours { get; init; }
    public required int WebinarPreparationHours { get; init; }
    public required int ClassPreparationRowOffset { get; init; }
    public required int WebinarRowOffset { get; init; }
    public required int WebinarPreparationRowOffset { get; init; }
    public required bool UseGoogleSheets { get; init; }
    public required string GoogleCredentials { get; init; }
    public required string SpreadsheetId { get; init; }
    public required string TeacherName { get; init; }
    public required string ClassDays { get; init; }
    public required string WebinarDays { get; init; }

    public static AppConfig LoadFromFile(string filePath = "appsettings.json")
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(filePath, optional: false, reloadOnChange: true)
            .Build();

        return config.Get<AppConfig>()!;
    }
}