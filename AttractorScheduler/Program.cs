using AttractorScheduler.Code;
using AttractorScheduler.Configs;
using AttractorScheduler.Contracts;
using AttractorScheduler.Services;



var resolver = new DependencyResolver();
var currentMonthName = DateTime.Now.ToString("MMMM", new System.Globalization.CultureInfo("ru-RU"));
SetUp(resolver, currentMonthName);

try
{
    var logger = resolver.Resolve<ILogger>();
    var scheduler = resolver.Resolve<IScheduler>();
    var config = resolver.Resolve<AppConfig>();
    
    var teacherRow = scheduler.FindRowByName(config.TeacherName);

    if (teacherRow == -1)
    {
        logger.LogError($"Преподаватель '{config.TeacherName}' не найден.");
        return;
    }

    logger.LogSuccess($"INFO: Начинаю заполнять расписание для {config.TeacherName.ToUpper()} на {currentMonthName.ToUpper()}");

    ITeacherSchedule teacherSchedule = new TeacherSchedule(scheduler, teacherRow, config);
    teacherSchedule.FillSchedule();

    scheduler.Save();
    logger.LogSuccess("Расписание успешно обновлено.");
}
catch (Exception ex)
{
    var logger = resolver.Resolve<ILogger>();
    logger.LogError($"Произошла ошибка: {ex.Message}");
}

return;

static void SetUp(DependencyResolver resolver, string currentMonthName)
{
    resolver.Register<ILogger>(() => new ConsoleLogger());
    resolver.Register(() => AppConfig.LoadFromFile());
    var logger = resolver.Resolve<ILogger>();

    resolver.Register<IScheduler>(() =>
    {
        var config = resolver.Resolve<AppConfig>();

        if (config.UseGoogleSheets)
            return new GoogleSheetsScheduler(config.GoogleCredentials, config.SpreadsheetId, currentMonthName, logger);

        return new ExcelScheduler(config.ExcelFilePath, currentMonthName);
    });
}
