using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace ModularStudio.Plugin.DataGenerator;

public partial class GeneratorView : UserControl
{
    private readonly Action<string, IReadOnlyList<double>> _onPublish;

    public GeneratorView(Action<string, IReadOnlyList<double>> onPublish)
    {
        InitializeComponent(); // nie zapomnij! (pułapka z punktu 8.3)
        _onPublish = onPublish;
    }

    private void Generate_Click(object sender, RoutedEventArgs e)
    {
        int n = int.TryParse(TxtCount.Text, out var c) && c > 0 ? c : 20;
        var rnd = new Random();
        var values = Enumerable.Range(0, n)
            .Select(_ => Math.Round(20 + rnd.NextDouble() * 80, 2)); // symulacja czujnika 20–100
        TxtNumbers.Text = string.Join(" ", values.Select(v => v.ToString(CultureInfo.InvariantCulture)));
    }

    private void Publish_Click(object sender, RoutedEventArgs e)
    {
        var values = TxtNumbers.Text
            .Split(new[] { ' ', ';', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => double.TryParse(s.Replace(',', '.'), NumberStyles.Float,
                                         CultureInfo.InvariantCulture, out var v) ? (double?)v : null)
            .Where(v => v.HasValue)
            .Select(v => v!.Value)
            .ToList();

        _onPublish(TxtInput.Text, values);
    }
}