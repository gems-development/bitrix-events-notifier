namespace Gems.Sales.Notifier.Application
{
    public interface IRequestQueue<TElement>
    {
        public int Count { get; }

        public void Enqueue(TElement value);
        public TElement? Dequeue();
    }
}
