namespace TPL
{
    public interface IOutputWrite<TKey, TValue>
    {
        void Add(TKey key, TValue value);

        public int Count { get; }
    }
}
