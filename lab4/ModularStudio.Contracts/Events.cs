namespace ModularStudio.Contracts;

public record DataPayloadPublishedEvent(
    string SourceName,
    string RawText,
    IReadOnlyList<double> Measurements,
    DateTime Timestamp);

public record SystemAlertEvent(string Severity, string Message, DateTime Timestamp);