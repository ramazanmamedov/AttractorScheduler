using AttractorScheduler.Contracts;

namespace AttractorScheduler.Services;

public class ConsoleLogger : ILogger
{
    public void LogSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[INFO] {DateTime.Now}: {message}");
        Console.ResetColor();
    }

    public void LogError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[ERROR] {DateTime.Now}: {message}");
        Console.ResetColor();
    }
}