using System.Composition;
using ModularStudio.Contracts;

namespace ModularStudio.Plugin.Analytics;

[Export(typeof(IPlugin))]
[ExportMetadata("Name", "Statystyki")]
[ExportMetadata("Version", "1.0")]
[ExportMetadata("DefaultLocation", "Right")]
public class AnalyticsPlugin : IPlugin, IDisposable
{
    private const string PluginName = "Statystyki";
    private readonly IHostContext _host;
    private readonly IDisposable _subscription;
    private readonly StatsView _view = new();

    [ImportingConstructor]
    public AnalyticsPlugin(IEventAggregator eventBus, IHostContext host)
    {
        _host = host;
        _subscription = eventBus.Subscribe<DataPayloadPublishedEvent>(OnDataReceived);
    }

    public object View => _view;

    public void Initialize() => _host.LogMessage(PluginName, "Nasłuchuję na dane");

    private void OnDataReceived(DataPayloadPublishedEvent e)
    {
        // zdarzenie może przyjść z dowolnego wątku, a UI zmieniamy tylko z wątku UI
        _view.Dispatcher.Invoke(() => _view.ShowStats(e));
        _host.LogMessage(PluginName, $"Przeliczono statystyki ({e.Measurements.Count} pomiarów)");
    }

    public void Dispose() => _subscription.Dispose(); // wypisanie z szyny (ważne w części 5)
}