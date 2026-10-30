using System.Composition;
using System.Windows.Controls;
using ModularStudio.Contracts;

[Export(typeof(IPlugin))]
[ExportMetadata("Name", "Test")]
[ExportMetadata("Version", "0.1")]
[ExportMetadata("DefaultLocation", "Right")]
public class TestPlugin : IPlugin
{
    private readonly IHostContext _host;

    [ImportingConstructor]
    public TestPlugin(IHostContext host) => _host = host;

    public object View { get; } = new TextBlock { Text = "Działa!", Margin = new(10) };
    public void Initialize() => _host.LogMessage("Test", "Zainicjalizowano");
}