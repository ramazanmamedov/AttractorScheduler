using AttractorScheduler.Code;
using AttractorScheduler.Configs;
using AttractorScheduler.Contracts;
using AttractorScheduler.Services;


var currentMonthName = DateTime.Now.ToString("MMMM", new System.Globalization.CultureInfo("ru-RU"));

var resolver = new DependencyResolver();
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

    ITeacherSchedule teacherSchedule = new TeacherSchedule(scheduler, teacherRow, config, logger);
    teacherSchedule.FillSchedule(
        ParseDays(config.ClassDays), 
        ParseDays(config.WebinarDays));

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

DayOfWeek[] ParseDays(string input)
{
    var daysMapping = new[]
    {
        new { Russian = "понедельник", English = DayOfWeek.Monday },
        new { Russian = "вторник", English = DayOfWeek.Tuesday },
        new { Russian = "среда", English = DayOfWeek.Wednesday },
        new { Russian = "четверг", English = DayOfWeek.Thursday },
        new { Russian = "пятница", English = DayOfWeek.Friday },
        new { Russian = "суббота", English = DayOfWeek.Saturday },
        new { Russian = "воскресенье", English = DayOfWeek.Sunday }
    };

    try
    {
        return input.Split(',')
            .Select(day => daysMapping.First(d => d.Russian.Equals(day.Trim(), StringComparison.OrdinalIgnoreCase)).English)
            .ToArray();
    }
    catch
    {
        throw new ArgumentException($"Неправильный формат дней. Убедитесь, что дни введены на русском языке.");
    }
}