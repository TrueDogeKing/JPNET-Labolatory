namespace ZtmGdansk;

public class DeparturesResponse
{
    public DateTime LastUpdate { get; set; }
    public List<Departure> Departures { get; set; } = new();
}

public class Departure
{
    public string RouteShortName { get; set; } = "";
    public string Headsign { get; set; } = "";
    public DateTime EstimatedTime { get; set; }
    public DateTime TheoreticalTime { get; set; }
    public int? DelayInSeconds { get; set; }
    public string Status { get; set; } = "";
}

public static class DepartureFilter
{
    // Metoda do przetestowania w części 9
    public static List<Departure> ByLine(IEnumerable<Departure> departures, string? line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return departures.ToList();

        return departures
            .Where(d => string.Equals(d.RouteShortName, line.Trim(), StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public static string FormatDelay(int? seconds)
    {
        if (seconds is null) return "rozklad";
        int s = Math.Abs(seconds.Value);
        string sign = seconds.Value < 0 ? "-" : "+";
        return $"{sign}{s / 60}:{s % 60:D2}";
    }
}