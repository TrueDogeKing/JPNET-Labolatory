namespace LabIoC.UnitTests.Fakes;

using LabIoC.Core;

public class SpyLogger : ILogger
{
    public List<string> Messages { get; } = new();

    public void Log(string message) => Messages.Add(message);
}