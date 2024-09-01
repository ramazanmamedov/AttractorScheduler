namespace AttractorScheduler.Contracts;

public interface ITeacherSchedule
{
    void FillSchedule(DayOfWeek[] classDays, DayOfWeek[] webinarDays);
}