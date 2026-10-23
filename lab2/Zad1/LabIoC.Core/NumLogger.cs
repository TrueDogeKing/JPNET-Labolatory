namespace LabIoC.Core;

public class NumLogger : ILogger
{
    private int _counter;
    public NumLogger(int initialValue = 0) => _counter = initialValue;
    public void Log(string message) => Console.WriteLine($"[NumLogger ({_counter++})]: {message}");
}

public class PropertyWorker
{
    public ILogger? Logger { get; set; }

    public void Work()
    {
        Logger?.Log("Praca wykonana przez PropertyWorker");
    }
}