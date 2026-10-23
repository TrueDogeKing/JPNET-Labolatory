using ExcelDirScan.Domain;

namespace ExcelDirScan.Services;

public interface IDirectoryScanner
{
    DirectoryScanResult Scan(string rootPath, int maxDepth);
}

public interface IExcelReportGenerator
{
    void GenerateReport(DirectoryScanResult scanResult, ReportStatistics statistics, string outputFilePath);
}

public interface IStatisticsCalculator
{
    ReportStatistics Calculate(DirectoryScanResult scanResult);
}
