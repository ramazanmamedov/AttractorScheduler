namespace AttractorScheduler.Contracts;

public interface ILogger
{
    void LogSuccess(string message);
    void LogError(string message);
}