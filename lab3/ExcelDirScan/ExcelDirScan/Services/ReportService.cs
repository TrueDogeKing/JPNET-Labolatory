using ExcelDirScan.Domain;

namespace ExcelDirScan.Services;

public sealed class ReportService(
    IDirectoryScanner scanner,
    IStatisticsCalculator statisticsCalculator,
    IExcelReportGenerator reportGenerator)
{
    public DirectoryScanResult CreateDirectoryReport(string sourceDir, int maxDepth, string targetExcelPath)
    {
        if (maxDepth < 0)
            throw new ArgumentOutOfRangeException(nameof(maxDepth), "Głębokość musi być liczbą nieujemną.");

        var result = scanner.Scan(sourceDir, maxDepth);
        var stats = statisticsCalculator.Calculate(result);
        reportGenerator.GenerateReport(result, stats, targetExcelPath);
        return result;
    }
}
