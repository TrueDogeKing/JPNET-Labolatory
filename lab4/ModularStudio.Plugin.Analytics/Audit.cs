using System.Composition;
using ModularStudio.Contracts;

namespace ModularStudio.Plugin.Analytics;

[Export]
[Shared("AnalysisScope")]
public class AnalysisSessionContext : IDisposable
{
    private readonly IHostContext _host;

    public Guid SessionId { get; } = Guid.NewGuid();
    public DateTime StartedAt { get; } = DateTime.Now;
    public string ShortId => SessionId.ToString()[..8];

    [ImportingConstructor]
    public AnalysisSessionContext(IHostContext host) // globalny [Shared] można wstrzyknąć do części z zakresu
    {
        _host = host;
        _host.LogMessage("Audyt", $"[SCOPE START] sesja {ShortId}");
    }

    // wywoływane automatycznie, gdy zamknie się 'using' na Export<BatchAuditor>
    public void Dispose() => _host.LogMessage("Audyt", $"[SCOPE STOP] sesja {ShortId}");
}

[Export]
public class AuditLog
{
    [ImportingConstructor]
    public AuditLog(AnalysisSessionContext context) => Context = context;

    public AnalysisSessionContext Context { get; }
    public List<string> Entries { get; } = new();
}

[Export]
public class BatchAuditor
{
    public const double Threshold = 90.0;
    private static readonly string[] Keywords = { "ERROR", "CRITICAL" };

    private readonly AnalysisSessionContext _context;
    private readonly AuditLog _log;

    [ImportingConstructor]
    public BatchAuditor(AnalysisSessionContext context, AuditLog log)
    {
        _context = context;
        _log = log;
    }

    public AuditResult Audit(DataPayloadPublishedEvent e)
    {
        for (int i = 0; i < e.Measurements.Count; i++)
            if (e.Measurements[i] > Threshold)
                _log.Entries.Add($"Pomiar #{i + 1} = {e.Measurements[i]:F2} > próg {Threshold}");

        foreach (var line in e.RawText.Split('\n'))
            if (Keywords.Any(k => line.Contains(k, StringComparison.OrdinalIgnoreCase)))
                _log.Entries.Add($"Log: {line.Trim()}");

        return new AuditResult(
            _context.SessionId,
            _context.StartedAt,
            _log.Entries.ToList(),
            SharedInScope: ReferenceEquals(_context, _log.Context));
    }
}

public record AuditResult(Guid SessionId, DateTime StartedAt, IReadOnlyList<string> Anomalies, bool SharedInScope);
