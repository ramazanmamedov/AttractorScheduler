namespace AttractorScheduler.Contracts;

public interface IScheduler : IDisposable
{
    int FindRowByName(string name);
    void FillCell(int row, int column, double value);
    int GetColumnForDay(int day, int startColumn = 0);
    void Save();
}