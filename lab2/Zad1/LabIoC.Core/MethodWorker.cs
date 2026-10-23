namespace LabIoC.Core;

public class MethodWorker
{
    private ILogger? _logger;
    public void SetLogger(ILogger logger) => _logger = logger;
    public void Work() => _logger?.Log("Praca wykonana przez MethodWorker");
}