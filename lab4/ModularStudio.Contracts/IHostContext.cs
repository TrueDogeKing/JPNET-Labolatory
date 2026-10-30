namespace ModularStudio.Contracts;

public interface IHostContext
{
    string HostVersion { get; }
    void LogMessage(string pluginName, string message);
}