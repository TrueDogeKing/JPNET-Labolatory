using System.Net.Http.Json;
using System.Text;
using ZtmGdansk;

Console.OutputEncoding = Encoding.UTF8;   // polskie znaki w nazwach kierunków

if (args.Length < 1 || !int.TryParse(args[0], out int stopId))
{
    Console.WriteLine("Uzycie:   ztm-gdansk <stopId> [linia]");
    Console.WriteLine("Przyklad: ztm-gdansk 2019");
    Console.WriteLine("          ztm-gdansk 2019 6");
    return 1;
}
string? line = args.Length > 1 ? args[1] : null;

using var http = new HttpClient();
DeparturesResponse? data;
try
{
    data = await http.GetFromJsonAsync<DeparturesResponse>(
        $"https://ckan2.multimediagdansk.pl/departures?stopId={stopId}");
}
catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
{
    Console.Error.WriteLine($"Nie udalo sie pobrac danych: {ex.Message}");
    return 2;
}

if (data is null)
{
    Console.Error.WriteLine("Pusta odpowiedz z API.");
    return 2;
}

var list = DepartureFilter.ByLine(data.Departures, line);
DateTime now = DateTime.Now;

Console.WriteLine($"Przystanek {stopId}" + (line is null ? "" : $", linia {line}")
                  + $"  (aktualizacja {data.LastUpdate.ToLocalTime():HH:mm:ss})");
Console.WriteLine(new string('-', 52));
Console.WriteLine($"{"Linia",-6}{"Kierunek",-24}{"Odjazd",-8}{"Za",-8}{"Opozn.",-8}");

if (list.Count == 0)
    Console.WriteLine("Brak odjazdow.");

foreach (var d in list)
{
    DateTime est = d.EstimatedTime.ToLocalTime();
    int minutes = (int)Math.Max(0, (est - now).TotalMinutes);
    Console.WriteLine($"{d.RouteShortName,-6}{d.Headsign,-24}{est:HH:mm}   {minutes + " min",-8}{DepartureFilter.FormatDelay(d.DelayInSeconds),-8}");
}
return 0;