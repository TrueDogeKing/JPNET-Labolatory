using ExcelDirScan.Domain;

namespace ExcelDirScan.Services;

public sealed class StatisticsCalculator : IStatisticsCalculator
{
    private const int TopFilesCount = 10;
    /// <summary>Ile rozszerzeń pokazać osobno na wykresie; resztę sumujemy jako „(inne)”.</summary>
    private const int MaxChartSlices = 9;
    private const string NoExtension = "(brak)";
    private const string Others = "(inne)";

    public ReportStatistics Calculate(DirectoryScanResult scanResult)
    {
        var files = scanResult.Root.AllFiles().ToList();

        var topFiles = files
            .OrderByDescending(f => f.SizeBytes)
            .Take(TopFilesCount)
            .Select((f, i) => new TopFileItem(i + 1, f.Name, NormalizeExt(f.Extension), f.SizeBytes, f.FullPath))
            .ToList();

        var allExt = files
            .GroupBy(f => NormalizeExt(f.Extension))
            .Select(g => new ExtensionStat(g.Key, g.Count(), g.Sum(f => f.SizeBytes)))
            .ToList();

        var byCount = Collapse(allExt.OrderByDescending(e => e.FileCount).ThenBy(e => e.Extension).ToList());
        var bySize = Collapse(allExt.OrderByDescending(e => e.TotalSizeBytes).ThenBy(e => e.Extension).ToList());

        return new ReportStatistics(
            topFiles,
            byCount,
            bySize,
            TotalFiles: files.Count,
            TotalDirectories: CountDirs(scanResult.Root) - 1, // bez katalogu głównego
            TotalSizeBytes: files.Sum(f => f.SizeBytes));
    }

    private static string NormalizeExt(string ext) =>
        string.IsNullOrWhiteSpace(ext) ? NoExtension : ext.ToLowerInvariant();

    private static List<ExtensionStat> Collapse(List<ExtensionStat> sorted)
    {
        if (sorted.Count <= MaxChartSlices + 1)
            return sorted;

        var head = sorted.Take(MaxChartSlices).ToList();
        var tail = sorted.Skip(MaxChartSlices).ToList();
        head.Add(new ExtensionStat(Others, tail.Sum(e => e.FileCount), tail.Sum(e => e.TotalSizeBytes)));
        return head;
    }

    private static int CountDirs(DirectoryNode node) => 1 + node.SubDirectories.Sum(CountDirs);
}
