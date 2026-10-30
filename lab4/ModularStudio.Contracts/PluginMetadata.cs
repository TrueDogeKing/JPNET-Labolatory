namespace ModularStudio.Contracts;

public class PluginMetadata
{
    public string Name { get; set; } = "Bez nazwy";
    public string Version { get; set; } = "1.0.0";
    public string DefaultLocation { get; set; } = "Document"; // "Document" | "Right" | "Bottom"
}