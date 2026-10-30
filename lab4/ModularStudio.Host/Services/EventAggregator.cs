using System.Composition;
using ModularStudio.Contracts;

namespace ModularStudio.Host.Services;

[Export(typeof(IEventAggregator))]
[Shared] 
public class EventAggregator : IEventAggregator
{
    private readonly Dictionary<Type, List<Delegate>> _subscribers = new();
    private readonly object _sync = new();

    public void Publish<TEvent>(TEvent evt) where TEvent : class
    {
        ArgumentNullException.ThrowIfNull(evt);

        List<Action<TEvent>> snapshot;
        lock (_sync)
        {
            if (!_subscribers.TryGetValue(typeof(TEvent), out var list)) return;
            snapshot = list.OfType<Action<TEvent>>().ToList();
        }

        foreach (var handler in snapshot)
            handler(evt);
    }

    public IDisposable Subscribe<TEvent>(Action<TEvent> handler) where TEvent : class
    {
        ArgumentNullException.ThrowIfNull(handler);

        lock (_sync)
        {
            if (!_subscribers.TryGetValue(typeof(TEvent), out var list))
                _subscribers[typeof(TEvent)] = list = new List<Delegate>();
            list.Add(handler);
        }

        return new SubscriptionToken(() =>
        {
            lock (_sync)
            {
                if (_subscribers.TryGetValue(typeof(TEvent), out var list))
                    list.Remove(handler);
            }
        });
    }

    private sealed class SubscriptionToken(Action unsubscribe) : IDisposable
    {
        private bool _disposed;
        public void Dispose()
        {
            if (_disposed) return;
            unsubscribe();
            _disposed = true;
        }
    }
}