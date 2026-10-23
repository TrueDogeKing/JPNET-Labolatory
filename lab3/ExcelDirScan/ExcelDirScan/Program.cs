using ExcelDirScan.Infrastructure;
using ExcelDirScan.Services;
using Microsoft.Extensions.DependencyInjection;
using Terminal.Gui;

var services = new ServiceCollection();

services.AddSingleton(new ReportOptions(
    AuthorName: "Filip Pudlak",      
    Company: "Politechnika Gdańska"));

services.AddTransient<IDirectoryScanner, FileSystemDirectoryScanner>();
services.AddTransient<IStatisticsCalculator, StatisticsCalculator>();
services.AddTransient<IExcelReportGenerator, EpplusReportGenerator>();
services.AddTransient<ReportService>();

using ServiceProvider serviceProvider = services.BuildServiceProvider();
var reportService = serviceProvider.GetRequiredService<ReportService>();

if (args.Length >= 2)
{
    string dir = args[0];
    string outPath = args.Length >= 3 ? args[2] : Path.Combine(Environment.CurrentDirectory, "scanDir.xlsx");

    if (!int.TryParse(args[1], out int depth) || depth < 0)
    {
        Console.Error.WriteLine("Głębokość musi być liczbą nieujemną.");
        return 1;
    }

    var (ok, log) = RunReport(reportService, dir, depth, outPath);
    Console.WriteLine(log);
    return ok ? 0 : 2;
}

Application.Init();
try
{
    var top = Application.Top;
    var win = new Window("Skaner katalogów → raport Excel   (Ctrl+Q – wyjście)")
    {
        X = 0, Y = 1,
        Width = Dim.Fill(), Height = Dim.Fill()
    };

    var menu = new MenuBar(new[]
    {
        new MenuBarItem("_Plik", new[]
        {
            new MenuItem("_Wyjście", "", () => Application.RequestStop(), null, null, Key.CtrlMask | Key.Q)
        })
    });
    top.Add(menu, win);

    string defaultOut = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "scanDir.xlsx");
    string defaultDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    var lblOut = new Label("Ścieżka zapisu (.xlsx):") { X = 2, Y = 1 };
    var txtOut = new TextField(defaultOut) { X = 30, Y = 1, Width = Dim.Fill() - 2 };

    var lblDir = new Label("Ścieżka do przeszukania:") { X = 2, Y = 3 };
    var txtDir = new TextField(defaultDir) { X = 30, Y = 3, Width = Dim.Fill() - 2 };

    var lblDepth = new Label("Głębokość przeszukiwania:") { X = 2, Y = 5 };
    var txtDepth = new TextField("3") { X = 30, Y = 5, Width = 6 };

    var btnRun = new Button("_Generuj raport", is_default: true) { X = 30, Y = 7 };

    var lblLog = new Label("Log (pominięte katalogi / błędy):") { X = 2, Y = 9 };
    var txtLog = new TextView
    {
        X = 2, Y = 10,
        Width = Dim.Fill() - 2,
        Height = Dim.Fill() - 1,
        ReadOnly = true,
        WordWrap = true
    };

    btnRun.Clicked += () =>
    {
        string outPath = txtOut.Text.ToString()?.Trim() ?? "";
        string dirPath = txtDir.Text.ToString()?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(dirPath))
        {
            MessageBox.ErrorQuery("Błąd walidacji", "Podaj ścieżkę katalogu do przeszukania!", "OK");
            return;
        }
        if (!outPath.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.ErrorQuery("Błąd walidacji", "Plik wynikowy musi mieć rozszerzenie .xlsx!", "OK");
            return;
        }
        if (!int.TryParse(txtDepth.Text.ToString(), out int depth) || depth < 0)
        {
            MessageBox.ErrorQuery("Błąd walidacji", "Głębokość musi być liczbą nieujemną!", "OK");
            return;
        }

        txtLog.Text = "Skanowanie w toku...";
        Application.Refresh();

        var (ok, log) = RunReport(reportService, dirPath, depth, outPath);
        txtLog.Text = log;
        if (ok)
            MessageBox.Query("Gotowe", $"Raport zapisano w:\n{outPath}", "OK");
    };

    win.Add(lblOut, txtOut, lblDir, txtDir, lblDepth, txtDepth, btnRun, lblLog, txtLog);
    Application.Run();
}
finally
{
    Application.Shutdown();
}
return 0;

static (bool Ok, string Log) RunReport(ReportService service, string dir, int depth, string outPath)
{
    var nl = Environment.NewLine;
    try
    {
        var result = service.CreateDirectoryReport(dir, depth, outPath);

        string errors = result.Errors.Count == 0
            ? "Brak pominiętych katalogów."
            : $"Pominięto {result.Errors.Count} element(ów):{nl}" + string.Join(nl, result.Errors);

        return (true, $"{errors}{nl}{nl}Pomyślnie utworzono plik!{nl}Znajduje się on w: {Path.GetFullPath(outPath)}");
    }
    catch (DirectoryNotFoundException ex)
    {
        return (false, $"Nie znaleziono katalogu: {ex.Message}");
    }
    catch (UnauthorizedAccessException ex)
    {
        return (false, $"Brak uprawnień (katalog źródłowy lub miejsce zapisu): {ex.Message}");
    }
    catch (IOException ex)
    {
        return (false, $"Błąd We/Wy – czy plik .xlsx nie jest otwarty w Excelu? {ex.Message}");
    }
    catch (ArgumentException ex)
    {
        return (false, $"Niepoprawny parametr: {ex.Message}");
    }
    catch (Exception ex)
    {
        return (false, $"Błąd krytyczny: {ex.GetType().Name}: {ex.Message}");
    }
}
