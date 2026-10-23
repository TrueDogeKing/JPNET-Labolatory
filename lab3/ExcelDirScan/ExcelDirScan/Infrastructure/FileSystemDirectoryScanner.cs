using ExcelDirScan.Services;
using ExcelDirScan.Domain;

namespace ExcelDirScan.Infrastructure;

/// <summary>
/// Odporna rekurencja po drzewie katalogów z limitem głębokości.
/// Błąd w jednym podkatalogu jest logowany i NIE przerywa skanowania reszty drzewa.
/// </summary>
public sealed class FileSystemDirectoryScanner : IDirectoryScanner
{
    public DirectoryScanResult Scan(string rootPath, int maxDepth)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
            throw new ArgumentException("Nie podano ścieżki katalogu do przeszukania.", nameof(rootPath));

        var rootInfo = new DirectoryInfo(rootPath);
        if (!rootInfo.Exists)
            throw new DirectoryNotFoundException($"Katalog '{rootInfo.FullName}' nie istnieje.");

        var root = new DirectoryNode
        {
            Name = rootInfo.FullName,
            FullPath = rootInfo.FullName,
            Attributes = rootInfo.Attributes,
            Depth = 0
        };

        var result = new DirectoryScanResult { Root = root, MaxDepth = maxDepth };
        ScanDirectorySafe(rootInfo, root, currentDepth: 0, maxDepth, result.Errors);
        return result;
    }

    private static void ScanDirectorySafe(
        DirectoryInfo currentDir, DirectoryNode node, int currentDepth, int maxDepth, IList<string> errorLog)
    {
        // --- 1. Pliki bieżącego katalogu ---
        FileInfo[] files;
        try
        {
            files = currentDir.GetFiles();
        }
        catch (UnauthorizedAccessException ex)
        {
            errorLog.Add($"[Brak uprawnień] Pliki w '{currentDir.FullName}': {ex.Message}");
            node.AccessDenied = true;
            return; // pomijamy ten katalog, skanowanie trwa dalej w pozostałych
        }
        catch (DirectoryNotFoundException ex) // musi być PRZED IOException (dziedziczy po nim)
        {
            errorLog.Add($"[Nie znaleziono] '{currentDir.FullName}': {ex.Message}");
            node.AccessDenied = true;
            return;
        }
        catch (IOException ex)
        {
            errorLog.Add($"[Błąd We/Wy] '{currentDir.FullName}': {ex.Message}");
            node.AccessDenied = true;
            return;
        }

        foreach (var file in files)
        {
            try
            {
                node.Files.Add(new FileNode(
                    file.Name, file.FullName, file.Extension, file.Length, file.Attributes, currentDepth + 1));
            }
            catch (IOException ex) // np. plik usunięty w trakcie skanowania
            {
                errorLog.Add($"[Błąd We/Wy] Plik '{file.FullName}': {ex.Message}");
            }
        }

        // --- 2. Podkatalogi ---
        DirectoryInfo[] subDirs;
        try
        {
            subDirs = currentDir.GetDirectories();
        }
        catch (UnauthorizedAccessException ex)
        {
            errorLog.Add($"[Brak uprawnień] Podkatalogi '{currentDir.FullName}': {ex.Message}");
            return;
        }
        catch (DirectoryNotFoundException ex)
        {
            errorLog.Add($"[Nie znaleziono] Podkatalogi '{currentDir.FullName}': {ex.Message}");
            return;
        }
        catch (IOException ex)
        {
            errorLog.Add($"[Błąd We/Wy] Podkatalogi '{currentDir.FullName}': {ex.Message}");
            return;
        }

        foreach (var subDir in subDirs)
        {
            var child = new DirectoryNode
            {
                Name = subDir.Name,
                FullPath = subDir.FullName,
                Attributes = subDir.Attributes,
                Depth = currentDepth + 1
            };
            node.SubDirectories.Add(child);

            // Podkatalog jest wypisany zawsze, ale wchodzimy do środka tylko poniżej limitu.
            if (currentDepth < maxDepth)
                ScanDirectorySafe(subDir, child, currentDepth + 1, maxDepth, errorLog);
            else
                child.NotExpanded = true;
        }
    }
}
