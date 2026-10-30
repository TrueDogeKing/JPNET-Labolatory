using System.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.Windows;
using AvalonDock.Layout;
using AvalonDock.Themes;
using ModularStudio.Contracts;
using ModularStudio.Host.Services;

namespace ModularStudio.Host;

public partial class MainWindow : Window
{
    private readonly string _pluginFolder = Path.Combine(AppContext.BaseDirectory, "Plugins");
    private CompositionHost? _container;

    public MainWindow()
    {
        InitializeComponent();
        DockManager.Theme = new Vs2013LightTheme();

        _container = BuildPluginContainer(_pluginFolder);
        HookHostLogging();
        ComposeAndAttachPlugins();

        Closed += (_, _) => _container?.Dispose();
    }

    private CompositionHost BuildPluginContainer(string pluginFolderPath)
    {
        Directory.CreateDirectory(pluginFolderPath);

        var assemblies = new List<Assembly>
        {
            typeof(MainWindow).Assembly 
        };

        foreach (string dllPath in Directory.GetFiles(pluginFolderPath, "*.dll"))
        {
            try
            {
                assemblies.Add(Assembly.Load(File.ReadAllBytes(dllPath)));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Pominięto {Path.GetFileName(dllPath)}: {ex.Message}");
            }
        }

        return new ContainerConfiguration()
            .WithAssemblies(assemblies)
            .CreateContainer();
    }

    private void HookHostLogging()
    {
        if (_container!.GetExport<IHostContext>() is HostServices host)
        {
            host.MessageLogged += line =>
                Dispatcher.Invoke(() => TxtStatus.Text = line); 
        }
    }

    private void ComposeAndAttachPlugins()
    {
        DocumentsPane.Children.Clear();
        RightToolsPane.Children.Clear();
        BottomToolsPane.Children.Clear();

        var plugins = _container!.GetExports<Lazy<IPlugin, PluginMetadata>>();
        int count = 0;

        foreach (var lazy in plugins)
        {
            PluginMetadata meta = lazy.Metadata;
            IPlugin plugin;
            try
            {
                plugin = lazy.Value;
                plugin.Initialize();
            }
            catch (Exception ex)
            {
                TxtStatus.Text = $"Błąd wtyczki '{meta.Name}': {ex.Message}";
                continue;
            }

            string title = $"{meta.Name} (v{meta.Version})";

            switch (meta.DefaultLocation)
            {
                case "Right":
                    RightToolsPane.Children.Add(new LayoutAnchorable
                    { Title = title, Content = plugin.View, CanClose = false });
                    break;
                case "Bottom":
                    BottomToolsPane.Children.Add(new LayoutAnchorable
                    { Title = title, Content = plugin.View, CanClose = false });
                    break;
                default: // "Document"
                    DocumentsPane.Children.Add(new LayoutDocument
                    { Title = title, Content = plugin.View });
                    break;
            }
            count++;
        }

        TxtStatus.Text = $"Załadowano wtyczek: {count} (folder: {_pluginFolder})";
    }
}