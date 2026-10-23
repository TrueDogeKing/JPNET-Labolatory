namespace LabIoC.Core;

public interface ILogger
{
    void Log(string message);
}

public class Logger : ILogger
{
    public void Log(string message) => Console.WriteLine($"[Logger]: {message}");
}