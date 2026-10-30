using System.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Threading;
using AvalonDock.Layout;
using AvalonDock.Themes;
using ModularStudio.Contracts;
using ModularStudio.Host.Services;

namespace ModularStudio.Host;

public partial class MainWindow : Window
{
    private readonly string _pluginFolder = Path.Combine(AppContext.BaseDirectory, "Plugins");
    private CompositionHost? _container;

    private FileSystemWatcher? _watcher;
    private readonly DispatcherTimer _reloadTimer;
    private string _lastChange = "";
    private readonly List<IPlugin> _loadedPlugins = new();
    private readonly Dictionary<string, Assembly> _assemblyCache = new(); 
    private readonly LayoutPanel? _rightParent;
    private readonly LayoutPanel? _bottomParent;

    public MainWindow()
    {
        InitializeComponent();
        DockManager.Theme = new Vs2013LightTheme();

        _rightParent = RightToolsPane.Parent as LayoutPanel;
        _bottomParent = BottomToolsPane.Parent as LayoutPanel;

        _reloadTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
        _reloadTimer.Tick += (_, _) => { _reloadTimer.Stop(); ReloadPlugins(); };

        _container = BuildPluginContainer(_pluginFolder);
        HookHostLogging();
        ComposeAndAttachPlugins();
        StartWatchingPlugins(_pluginFolder);

        Closed += (_, _) =>
        {
            _watcher?.Dispose();
            DisposePlugins();
            _container?.Dispose();
        };
    }

    private CompositionHost BuildPluginContainer(string pluginFolderPath)
    {
        Directory.CreateDirectory(pluginFolderPath);

        var assemblies = new List<Assembly> { typeof(MainWindow).Assembly };

        foreach (string dllPath in Directory.GetFiles(pluginFolderPath, "*.dll"))
        {
            try
            {
                byte[] raw = File.ReadAllBytes(dllPath);
                string hash = Convert.ToHexString(SHA256.HashData(raw));

                if (!_assemblyCache.TryGetValue(hash, out var asm))
                {
                    asm = Assembly.Load(raw);
                    _assemblyCache[hash] = asm;
                }
                assemblies.Add(asm);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Pominięto {Path.GetFileName(dllPath)}: {ex.Message}");
            }
        }

        return new ContainerConfiguration().WithAssemblies(assemblies).CreateContainer();
    }

    private void HookHostLogging()
    {
        if (_container!.GetExport<IHostContext>() is HostServices host)
            host.MessageLogged += line => Dispatcher.Invoke(() => TxtStatus.Text = line);
    }

    private void EnsurePanesAttached()
    {
        if (RightToolsPane.Parent == null) _rightParent?.Children.Add(RightToolsPane);
        if (BottomToolsPane.Parent == null) _bottomParent?.Children.Add(BottomToolsPane);
    }

    private void ComposeAndAttachPlugins()
    {
        EnsurePanesAttached();
        DocumentsPane.Children.Clear();
        RightToolsPane.Children.Clear();
        BottomToolsPane.Children.Clear();

        var errors = new List<string>();
        var plugins = _container!.GetExports<Lazy<IPlugin, PluginMetadata>>().ToList();

        foreach (var lazy in plugins)
        {
            PluginMetadata meta = lazy.Metadata;
            IPlugin plugin;
            try
            {
                plugin = lazy.Value;
                plugin.Initialize();
                _loadedPlugins.Add(plugin);
            }
            catch (Exception ex)
            {
                errors.Add($"{meta.Name}: {ex.GetBaseException().Message}");
                continue;
            }

            string title = $"{meta.Name} (v{meta.Version})";
            switch (meta.DefaultLocation)
            {
                case "Right":
                    RightToolsPane.Children.Add(new LayoutAnchorable { Title = title, Content = plugin.View, CanClose = false });
                    break;
                case "Bottom":
                    BottomToolsPane.Children.Add(new LayoutAnchorable { Title = title, Content = plugin.View, CanClose = false });
                    break;
                default:
                    DocumentsPane.Children.Add(new LayoutDocument { Title = title, Content = plugin.View });
                    break;
            }
        }

        TxtStatus.Text = $"Wykryto: {plugins.Count}, załadowano: {_loadedPlugins.Count}, błędy: {errors.Count}";
        if (errors.Count > 0)
            MessageBox.Show(string.Join("\n\n", errors), "Błędy wtyczek");
    }

    private void DisposePlugins()
    {
        foreach (var p in _loadedPlugins)
            (p as IDisposable)?.Dispose(); 
        _loadedPlugins.Clear();
    }

    private void StartWatchingPlugins(string pluginFolder)
    {
        _watcher = new FileSystemWatcher(pluginFolder, "*.dll")
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
        };

        _watcher.Created += OnPluginFolderChanged;
        _watcher.Deleted += OnPluginFolderChanged;
        _watcher.Changed += OnPluginFolderChanged; 
        _watcher.Renamed += OnPluginFolderChanged;
        _watcher.EnableRaisingEvents = true;
    }

    private void OnPluginFolderChanged(object sender, FileSystemEventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            _lastChange = $"{e.ChangeType}: {e.Name}";
            _reloadTimer.Stop();
            _reloadTimer.Start(); 
        });
    }

    private void ReloadPlugins()
    {
        DisposePlugins();
        _container?.Dispose();
        _container = BuildPluginContainer(_pluginFolder);
        HookHostLogging();
        ComposeAndAttachPlugins();
        TxtStatus.Text += $"  |  odświeżono {DateTime.Now:HH:mm:ss} ({_lastChange})";
    }
}