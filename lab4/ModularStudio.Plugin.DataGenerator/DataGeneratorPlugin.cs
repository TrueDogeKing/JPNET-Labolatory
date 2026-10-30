using System.Composition;
using ModularStudio.Contracts;

namespace ModularStudio.Plugin.DataGenerator;

[Export(typeof(IPlugin))]
[ExportMetadata("Name", "Generator Danych")]
[ExportMetadata("Version", "1.0")]
[ExportMetadata("DefaultLocation", "Document")]
public class DataGeneratorPlugin : IPlugin
{
    private const string PluginName = "Generator Danych";
    private readonly IEventAggregator _eventBus;
    private readonly IHostContext _host;
    private readonly GeneratorView _view;

    [ImportingConstructor]
    public DataGeneratorPlugin(IEventAggregator eventBus, IHostContext host)
    {
        _eventBus = eventBus;
        _host = host;
        _view = new GeneratorView(PublishPayload);
    }

    public object View => _view;

    public void Initialize() =>
        _host.LogMessage(PluginName, $"Gotowy (host {_host.HostVersion})");

    private void PublishPayload(string text, IReadOnlyList<double> values)
    {
        _host.LogMessage(PluginName, $"Wysłano do analizy: {text.Length} znaków, {values.Count} pomiarów");

        // wydawca nie wie, kto (i czy ktokolwiek) słucha
        _eventBus.Publish(new DataPayloadPublishedEvent(PluginName, text, values, DateTime.Now));
    }
}