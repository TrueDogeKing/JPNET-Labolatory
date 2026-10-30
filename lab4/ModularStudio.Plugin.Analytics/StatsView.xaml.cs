using System.Text.RegularExpressions;
using System.Windows.Controls;
using ModularStudio.Contracts;

namespace ModularStudio.Plugin.Analytics;

public record BarItem(string Label, double Value, double Percent);

public partial class StatsView : UserControl
{
    public StatsView() => InitializeComponent();

    public void ShowStats(DataPayloadPublishedEvent e)
    {
        TxtSource.Text = $"Źródło: {e.SourceName} ({e.Timestamp:HH:mm:ss})";

        var words = Regex.Matches(e.RawText.ToLowerInvariant(), @"\p{L}+")
                         .Select(m => m.Value).ToList();
        TxtChars.Text = $"Znaki: {e.RawText.Length}";
        TxtWords.Text = $"Słowa: {words.Count}";
        TopWordsList.ItemsSource = words
            .GroupBy(w => w)
            .OrderByDescending(g => g.Count()).ThenBy(g => g.Key)
            .Take(5)
            .Select(g => $"{g.Key} × {g.Count()}")
            .ToList();

        var m = e.Measurements;
        if (m.Count == 0)
        {
            TxtNumStats.Text = "Brak pomiarów";
            BarsList.ItemsSource = null;
            return;
        }

        TxtNumStats.Text = $"n = {m.Count}   min = {m.Min():F2}   max = {m.Max():F2}   śr = {m.Average():F2}";

        double maxAbs = m.Max(Math.Abs);
        BarsList.ItemsSource = m
            .Select((v, i) => new BarItem($"#{i + 1}", v, maxAbs == 0 ? 0 : Math.Abs(v) / maxAbs * 100))
            .ToList();
    }
}