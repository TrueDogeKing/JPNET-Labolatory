using System.Drawing;
using ExcelDirScan.Services;
using ExcelDirScan.Domain;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;

namespace ExcelDirScan.Infrastructure;

public sealed record ReportOptions(string AuthorName, string Company);

public sealed class EpplusReportGenerator : IExcelReportGenerator
{
    private const int MaxOutlineLevel = 7; // limit formatu Excel
    private const string NumberFormat = "#,##0";

    private readonly ReportOptions _options;

    public EpplusReportGenerator(ReportOptions options)
    {
        _options = options;
        // EPPlus 8+: licencja MUSI być ustawiona przed pierwszym new ExcelPackage(...)
        ExcelPackage.License.SetNonCommercialPersonal(options.AuthorName);
    }

    public void GenerateReport(DirectoryScanResult scanResult, ReportStatistics statistics, string outputFilePath)
    {
        var file = new FileInfo(outputFilePath);
        file.Directory?.Create();
        if (file.Exists)
            file.Delete(); // IOException, jeśli plik jest otwarty w Excelu – obsłużone w Program.cs

        using var ep = new ExcelPackage(file);

        // --- Metadane dokumentu (docProps/core.xml) ---
        var props = ep.Workbook.Properties;
        props.Title = "Raport struktury katalogu";
        props.Subject = scanResult.Root.FullPath;
        props.Author = _options.AuthorName;
        props.Company = _options.Company;
        props.Comments = $"Wygenerowano automatycznie w .NET, głębokość skanowania: {scanResult.MaxDepth}";
        props.SetCustomPropertyValue("WersjaGeneratora", "1.2");
        props.SetCustomPropertyValue("DataSkanowania", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

        BuildStructureSheet(ep.Workbook.Worksheets.Add("Struktura katalogu"), scanResult);
        BuildStatisticsSheet(ep.Workbook.Worksheets.Add("Statystyki"), statistics);

        ep.Save();
    }

    // =====================================================================
    //  ARKUSZ 1: „Struktura katalogu”  (Część 2)
    // =====================================================================
    private static void BuildStructureSheet(ExcelWorksheet ws, DirectoryScanResult scan)
    {
        string[] headers = ["Nazwa", "Typ", "Rozszerzenie", "Rozmiar [B]", "Atrybuty", "Pełna ścieżka"];
        for (int c = 0; c < headers.Length; c++)
            ws.Cells[1, c + 1].Value = headers[c];

        using (var h = ws.Cells[1, 1, 1, headers.Length])
        {
            h.Style.Font.Bold = true;
            h.Style.Font.Color.SetColor(Color.White);
            h.Style.Fill.PatternType = ExcelFillStyle.Solid;
            h.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(31, 78, 121));
            h.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            h.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        // Konspekt: wiersz katalogu NAD jego zawartością
        ws.OutLineSummaryBelow = false;
        ws.OutLineSummaryRight = false;

        int row = 2;
        WriteDirectory(ws, scan.Root, ref row);
        int lastRow = row - 1;

        ws.Cells[2, 4, lastRow, 4].Style.Numberformat.Format = NumberFormat;
        ws.Cells[2, 4, lastRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

        ws.View.FreezePanes(2, 1); // nagłówek zawsze widoczny
        ws.Cells[1, 1, lastRow, headers.Length].AutoFilter = true;
        ws.Cells.AutoFitColumns(8, 90);

        // Nagłówek / stopka wydruku
        ws.HeaderFooter.OddHeader.CenteredText = "&14&\"Calibri,Bold\" Struktura katalogu";
        ws.HeaderFooter.OddFooter.RightAlignedText =
            $"Strona {ExcelHeaderFooter.PageNumber} z {ExcelHeaderFooter.NumberOfPages}";
        ws.HeaderFooter.OddFooter.CenteredText = ExcelHeaderFooter.SheetName;
        ws.HeaderFooter.OddFooter.LeftAlignedText = ExcelHeaderFooter.FilePath + ExcelHeaderFooter.FileName;
        ws.PrinterSettings.RepeatRows = ws.Cells["1:1"];
    }

    /// <summary>Zapis drzewa w porządku pre-order: katalog, jego pliki, potem jego podkatalogi.</summary>
    private static void WriteDirectory(ExcelWorksheet ws, DirectoryNode dir, ref int row)
    {
        string status = dir.AccessDenied ? " [brak dostępu]" : dir.NotExpanded ? " [nie rozwinięto]" : "";

        ws.Cells[row, 1].Value = (dir.Depth == 0 ? dir.FullPath : dir.Name) + status;
        ws.Cells[row, 2].Value = "Katalog";
        ws.Cells[row, 3].Value = "";
        if (!dir.NotExpanded && !dir.AccessDenied)
            ws.Cells[row, 4].Value = dir.TotalSizeBytes;
        ws.Cells[row, 5].Value = dir.Attributes.ToString();
        ws.Cells[row, 6].Value = dir.FullPath;

        using (var r = ws.Cells[row, 1, row, 6])
        {
            r.Style.Font.Bold = true;
            r.Style.Fill.PatternType = ExcelFillStyle.Solid;
            r.Style.Fill.BackgroundColor.SetColor(dir.Depth == 0 ? Color.FromArgb(189, 215, 238) : Color.FromArgb(221, 235, 247));
            if (dir.AccessDenied)
                r.Style.Font.Color.SetColor(Color.DarkRed);
        }
        SetIndentAndOutline(ws, row, dir.Depth);
        row++;

        foreach (var f in dir.Files.OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase))
        {
            ws.Cells[row, 1].Value = f.Name;
            ws.Cells[row, 2].Value = "Plik";
            ws.Cells[row, 3].Value = f.Extension;
            ws.Cells[row, 4].Value = f.SizeBytes;
            ws.Cells[row, 5].Value = f.Attributes.ToString();
            ws.Cells[row, 6].Value = f.FullPath;

            if (f.Attributes.HasFlag(FileAttributes.Hidden) || f.Attributes.HasFlag(FileAttributes.System))
                ws.Cells[row, 1, row, 6].Style.Font.Color.SetColor(Color.Gray);

            SetIndentAndOutline(ws, row, f.Depth);
            row++;
        }

        foreach (var sub in dir.SubDirectories.OrderBy(d => d.Name, StringComparer.OrdinalIgnoreCase))
            WriteDirectory(ws, sub, ref row);
    }

    private static void SetIndentAndOutline(ExcelWorksheet ws, int row, int depth)
    {
        ws.Cells[row, 1].Style.Indent = Math.Min(depth * 2, 30);
        ws.Row(row).OutlineLevel = Math.Min(depth, MaxOutlineLevel);
    }

    // =====================================================================
    //  ARKUSZ 2: „Statystyki”  (Część 3)
    // =====================================================================
    private static void BuildStatisticsSheet(ExcelWorksheet ws, ReportStatistics stats)
    {
        ws.Cells["A1"].Value = "Statystyki skanowania";
        ws.Cells["A1:E1"].Merge = true;
        ws.Cells["A1"].Style.Font.Size = 16;
        ws.Cells["A1"].Style.Font.Bold = true;

        // --- Top 10 (A3...) ---
        WriteSectionTitle(ws, "A3", "10 największych plików");
        int topEnd;
        if (stats.TopFiles.Count > 0)
        {
            var range = ws.Cells["A4"].LoadFromCollection(stats.TopFiles, true, TableStyles.Medium2);
            topEnd = range.End.Row;
            ws.Cells[5, 4, topEnd, 4].Style.Numberformat.Format = NumberFormat;
        }
        else
        {
            ws.Cells["A4"].Value = "Nie znaleziono żadnych plików.";
            topEnd = 4;
        }

        // --- Podsumowanie pod Top 10 ---
        int s = topEnd + 2;
        ws.Cells[s, 1].Value = "Liczba plików:";            ws.Cells[s, 2].Value = stats.TotalFiles;
        ws.Cells[s + 1, 1].Value = "Liczba podkatalogów:";  ws.Cells[s + 1, 2].Value = stats.TotalDirectories;
        ws.Cells[s + 2, 1].Value = "Łączny rozmiar [B]:";   ws.Cells[s + 2, 2].Value = stats.TotalSizeBytes;
        ws.Cells[s, 1, s + 2, 1].Style.Font.Bold = true;
        ws.Cells[s, 2, s + 2, 2].Style.Numberformat.Format = NumberFormat;
        int summaryEnd = s + 2;

        // --- Zestawienia rozszerzeń: G:I (wg liczby) i K:M (wg rozmiaru) ---
        int extCountEnd = WriteExtensionTable(ws, stats.ExtensionsByCount, col: 7, "Rozszerzenia wg liczby plików");
        int extSizeEnd = WriteExtensionTable(ws, stats.ExtensionsBySize, col: 11, "Rozszerzenia wg rozmiaru");

        ws.Cells.AutoFitColumns(8, 70);

        // --- Wykresy kołowe ---
        if (stats.TotalFiles == 0)
            return;

        int chartRow0 = Math.Max(summaryEnd, Math.Max(extCountEnd, extSizeEnd)) + 1; // SetPosition: wiersz od 0!

        // Wykres 1: ilościowy — wartości H5:H?, kategorie G5:G?
        AddPieChart(ws, "PieChartExtensionsCount", "Udział ilościowy rozszerzeń plików",
            valAddr: new ExcelAddress(5, 8, extCountEnd, 8),
            catAddr: new ExcelAddress(5, 7, extCountEnd, 7),
            row0: chartRow0);

        // Wykres 2: rozmiarowy — wartości M5:M?, kategorie K5:K?
        AddPieChart(ws, "PieChartExtensionsSize", "Udział rozszerzeń w zajmowanej powierzchni",
            valAddr: new ExcelAddress(5, 13, extSizeEnd, 13),
            catAddr: new ExcelAddress(5, 11, extSizeEnd, 11),
            row0: chartRow0 + 22);
    }

    private static int WriteExtensionTable(ExcelWorksheet ws, IReadOnlyList<ExtensionStat> data, int col, string title)
    {
        WriteSectionTitle(ws, ws.Cells[3, col].Address, title);
        if (data.Count == 0)
            return 4;

        var range = ws.Cells[4, col].LoadFromCollection(data, true, TableStyles.Medium9);
        ws.Cells[5, col + 2, range.End.Row, col + 2].Style.Numberformat.Format = NumberFormat;
        return range.End.Row;
    }

    private static void WriteSectionTitle(ExcelWorksheet ws, string address, string text)
    {
        ws.Cells[address].Value = text;
        ws.Cells[address].Style.Font.Bold = true;
        ws.Cells[address].Style.Font.Size = 12;
        ws.Cells[address].Style.Font.Color.SetColor(Color.FromArgb(31, 78, 121));
    }

    private static void AddPieChart(
        ExcelWorksheet ws, string name, string title, ExcelAddress valAddr, ExcelAddress catAddr, int row0)
    {
        var chart = (ExcelPieChart)ws.Drawings.AddChart(name, eChartType.Pie3D);
        chart.Title.Text = title;
        chart.SetPosition(row0, 5, 0, 5); // wiersz/kolumna indeksowane od 0
        chart.SetSize(640, 400);

        var serie = (ExcelPieChartSerie)chart.Series.Add(valAddr.Address, catAddr.Address);
        serie.Header = title;

        chart.DataLabel.ShowCategory = true;
        chart.DataLabel.ShowPercent = true;
        chart.DataLabel.ShowLeaderLines = true;

        chart.Legend.Position = eLegendPosition.Right;
        chart.Legend.Border.LineStyle = eLineStyle.Solid;
        chart.Legend.Border.Fill.Style = eFillStyle.SolidFill;
        chart.Legend.Border.Fill.Color = Color.DarkBlue;
    }
}
