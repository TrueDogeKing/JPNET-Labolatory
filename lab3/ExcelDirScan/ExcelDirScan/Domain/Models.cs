using System.ComponentModel;

namespace ExcelDirScan.Domain;

/// <summary>Plik znaleziony podczas skanowania.</summary>
public sealed record FileNode(
    string Name,
    string FullPath,
    string Extension,
    long SizeBytes,
    FileAttributes Attributes,
    int Depth);

/// <summary>Katalog (węzeł drzewa) wraz z zawartością.</summary>
public sealed class DirectoryNode
{
    public required string Name { get; init; }
    public required string FullPath { get; init; }
    public FileAttributes Attributes { get; init; }
    public int Depth { get; init; }

    /// <summary>true, gdy katalog został wypisany, ale nie wchodzimy do środka (osiągnięto maxDepth).</summary>
    public bool NotExpanded { get; set; }

    /// <summary>true, gdy zawartości nie dało się odczytać (brak uprawnień / błąd We/Wy).</summary>
    public bool AccessDenied { get; set; }

    public List<DirectoryNode> SubDirectories { get; } = new();
    public List<FileNode> Files { get; } = new();

    /// <summary>Suma rozmiarów plików znalezionych w tym katalogu i jego podkatalogach (w ramach maxDepth).</summary>
    public long TotalSizeBytes => Files.Sum(f => f.SizeBytes) + SubDirectories.Sum(d => d.TotalSizeBytes);

    public IEnumerable<FileNode> AllFiles() =>
        Files.Concat(SubDirectories.SelectMany(d => d.AllFiles()));
}

/// <summary>Wynik skanowania: drzewo katalogów + log błędów.</summary>
public sealed class DirectoryScanResult
{
    public required DirectoryNode Root { get; init; }
    public int MaxDepth { get; init; }
    public List<string> Errors { get; } = new();
}

/// <summary>Wiersz tabeli „Top 10” (nagłówki kolumn z [DisplayName] – czyta je LoadFromCollection).</summary>
public sealed record TopFileItem(
    [property: DisplayName("Lp.")] int Rank,
    [property: DisplayName("Nazwa pliku")] string Name,
    [property: DisplayName("Rozszerzenie")] string Extension,
    [property: DisplayName("Rozmiar [B]")] long SizeBytes,
    [property: DisplayName("Pełna ścieżka")] string FullPath);

/// <summary>Statystyka dla jednego rozszerzenia.</summary>
public sealed record ExtensionStat(
    [property: DisplayName("Rozszerzenie")] string Extension,
    [property: DisplayName("Liczba plików")] int FileCount,
    [property: DisplayName("Łączny rozmiar [B]")] long TotalSizeBytes);

/// <summary>Statystyki wyliczone w warstwie aplikacji (bez EPPlus).</summary>
public sealed record ReportStatistics(
    IReadOnlyList<TopFileItem> TopFiles,
    IReadOnlyList<ExtensionStat> ExtensionsByCount,
    IReadOnlyList<ExtensionStat> ExtensionsBySize,
    int TotalFiles,
    int TotalDirectories,
    long TotalSizeBytes);
