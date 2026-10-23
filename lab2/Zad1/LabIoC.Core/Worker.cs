namespace LabIoC.Core;

public class Worker
{
    private readonly ILogger _logger;

    public Worker(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public int Execute(int tasksCount)
    {
        if (tasksCount < 0)
            throw new ArgumentException("Liczba zadań nie może być ujemna.", nameof(tasksCount));

        _logger.Log($"Rozpoczęto przetwarzanie {tasksCount} zadań.");
        _logger.Log("Zakończono pomyślnie.");
        return tasksCount * 10; // Zwraca punkty za wykonane zadania
    }
}