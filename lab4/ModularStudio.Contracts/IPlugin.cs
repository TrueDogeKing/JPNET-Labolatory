namespace ModularStudio.Contracts;

public interface IPlugin
{
    object View { get; }
    void Initialize();
}