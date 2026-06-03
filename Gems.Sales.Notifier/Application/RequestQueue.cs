using System.Collections.Concurrent;

namespace Gems.Sales.Notifier.Application
{
    internal sealed class RequestQueue<TElement> : IRequestQueue<TElement>
    {
        private readonly ConcurrentQueue<TElement> _queue;

        public int Count => _queue.Count;

        public RequestQueue() => _queue = new();
        public void Enqueue (TElement value) => _queue.Enqueue(value);
        public TElement? Dequeue() => _queue.TryDequeue(out var result) ? result : default;
    }
}
