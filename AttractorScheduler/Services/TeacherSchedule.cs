using AttractorScheduler.Configs;
using AttractorScheduler.Contracts;

namespace AttractorScheduler.Services;

public class TeacherSchedule : ITeacherSchedule
{
    private readonly IScheduler _scheduler;
    private readonly AppConfig _config;
    private readonly int _classPreparationRow;
    private readonly int _webinarRow;
    private readonly int _webinarPreparationRow;
    private readonly ILogger _logger;

    public TeacherSchedule(IScheduler scheduler, int teacherRow, AppConfig config, ILogger logger)
    {
        _scheduler = scheduler;
        _config = config;
        _logger = logger;
        _classPreparationRow = teacherRow + _config.ClassPreparationRowOffset;
        _webinarRow = teacherRow + _config.WebinarRowOffset;
        _webinarPreparationRow = teacherRow + _config.WebinarPreparationRowOffset;
    }

    public void FillSchedule(DayOfWeek[] classDays, DayOfWeek[] webinarDays)
    {
        var year = DateTime.Now.Year;
        var month = DateTime.Now.Month;
        var today = DateTime.Now.Day;
        var startColumn = 1;

        for (var day = 1; day <= today; day++)
        {
            var currentDate = new DateTime(year, month, day);

            if (classDays.Contains(currentDate.DayOfWeek) || webinarDays.Contains(currentDate.DayOfWeek))
            {
                var column = _scheduler.GetColumnForDay(day, startColumn);
                if (column == -1) break;

                startColumn = column + 1;

                if (classDays.Contains(currentDate.DayOfWeek)) 
                    AddClassDay(column);

                if (webinarDays.Contains(currentDate.DayOfWeek)) 
                    AddWebinarDay(column);
            }
        }
    }

    private void AddClassDay(int column)
    {
        if (column != -1)
            _scheduler.FillCell(_classPreparationRow, column, _config.ClassDurationHours);
    }

    private void AddWebinarDay(int column)
    {
        if (column != -1)
        {
            _scheduler.FillCell(_webinarRow, column, _config.WebinarDurationHours);
            _scheduler.FillCell(_webinarPreparationRow, column, _config.WebinarPreparationHours);
        }
    }
}