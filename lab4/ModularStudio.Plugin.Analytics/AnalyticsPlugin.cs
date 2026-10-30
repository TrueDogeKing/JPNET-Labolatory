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

    [Import]
    [SharingBoundary("AnalysisScope")]
    public ExportFactory<BatchAuditor> AuditorFactory { get; set; } = null!;

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
        AuditResult result;

        using (Export<BatchAuditor> scope = AuditorFactory.CreateExport())
        {
            result = scope.Value.Audit(e);

            _view.Dispatcher.Invoke(() =>
            {
                _view.ShowStats(e);
                _view.ShowAudit(result);
            });

            _host.LogMessage(PluginName,
                $"Sesja {result.SessionId.ToString()[..8]}: anomalii {result.Anomalies.Count}");
        }
    }

    public void Dispose() => _subscription.Dispose();
}