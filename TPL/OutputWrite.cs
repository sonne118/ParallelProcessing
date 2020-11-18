using System.Collections;
using System.Collections.Generic;

namespace TPL
{
    sealed public class OutputWrite<TKey, TValue> : IOutputWrite<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>> where TKey : notnull
    {
        private readonly SortedDictionary<TKey, TValue> _sorted;
        public OutputWrite()
        {
            _sorted = new SortedDictionary<TKey, TValue>();
        }
        public TValue this[TKey i] => _sorted[i];
        public void Add(TKey key, TValue value)
        {
            _sorted.Add(key, value);
        }
        public int Count { get { return _sorted.Count; } }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (KeyValuePair<TKey, TValue> item in _sorted)
            {
                yield return item;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this).GetEnumerator();
        }
    }
}
