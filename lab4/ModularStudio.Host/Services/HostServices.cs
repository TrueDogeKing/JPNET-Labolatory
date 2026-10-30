using System.Composition;
using ModularStudio.Contracts;

namespace ModularStudio.Host.Services;

[Export(typeof(IHostContext))]
[Shared] // singleton: każda wtyczka dostaje tę samą instancję
public class HostServices : IHostContext
{
    public string HostVersion => "v1.0.0 (.NET 8)";

    public event Action<string>? MessageLogged;

    public void LogMessage(string pluginName, string message)
    {
        string line = $"[{DateTime.Now:HH:mm:ss}] [{pluginName}]: {message}";
        System.Diagnostics.Debug.WriteLine(line);
        MessageLogged?.Invoke(line);
    }
}