using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TPL
{
    public sealed class ParallelProcessing<TKey, TValue>
    {
        private readonly ConcurrentDictionary<string, int> _cdic;
        private List<KeyValuePair<string, int>> _list;
        private HashSet<Task> _asyncWaiters;
        

        public ParallelProcessing()
        {
            _cdic = new ConcurrentDictionary<string, int>();
        }
        public async Task SingularTask(IReader reader, IOutputWrite<string, int> output, CancellationToken cancellationToken)
        {
            char ch = '\0', prev = '\0'; string word = ""; IReader r = reader;
            bool error = true; bool asyncThread = _asyncWaiters != null ? true : false;

            while (error)
            {
                try
                {
                    ch = asyncThread ? await r.awaitNext() : r.Next();
                    error = true;
                }
                catch (IndexOutOfRangeException)
                {
                    error = false;
                }

                if (ch == '\t')
                    continue;

                if (Char.IsLetter(ch)) word += Char.ToLowerInvariant(ch);

                if ((ch == ' ' && prev != '\0'))
                {
                    if (!_cdic.ContainsKey(word))
                    {
                        _cdic.AddOrUpdate(word, 1, (key, oldValue) => 1);
                    }
                    else
                    {
                        _cdic.AddOrUpdate(word, 0, (key, oldValue) => oldValue + 1);
                    }

                    word = "";
                }
                prev = ch;
            }

            if (!asyncThread)
            {
                await OutputWriting(output, CancellationToken.None);
            }
        }


        private async Task OutputWriting(IOutputWrite<string, int> output, CancellationToken cancellationToken)
        {
            _list = _cdic.OrderByDescending(i => i.Value).ThenBy(k => k.Key).ToList();

            for (int i = 0; i < _list.Count; i++)
            {
                var item = _list.ElementAt(i);
                {
                    output.Add(item.Key, item.Value);
                }
            }

            await Task.Yield();
        }

        public async Task MultipleTasks(IReader[] readers, IOutputWrite<string, int> output, CancellationToken cancellationToken)
        {
            IReader reader = null;
            _asyncWaiters = new HashSet<Task>();

            for (int i = 0; i < readers.Length; i++)
            {
                reader = readers[i];

                _asyncWaiters.Add(Task.Factory.StartNew(async delegate (Object obj)
                {

                    CustomData data = obj as CustomData;
                    if (data == null)
                    {
                        return;
                    }

                    await SingularTask(data.r, output, CancellationToken.None);

                }, new CustomData { r = reader }).Unwrap());
            }

            try
            {
                await Task.WhenAll(_asyncWaiters);
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    Debug.WriteLine("{0}:\n   {1}", e.GetType().Name, e.Message);
                }
            }

            try
            {
                await OutputWriting(output, CancellationToken.None);
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    Debug.WriteLine("{0}:\n   {1}", e.GetType().Name, e.Message);
                }
            }

        }
    }
    class CustomData
    {
        public IReader r;
    }
}