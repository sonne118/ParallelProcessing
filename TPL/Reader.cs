using System;
using System.Threading.Tasks;

namespace TPL
{
    public class Reader : IReader
    {
        string _readText; int nextIndex = 0;
        public Reader(string readText)
        {
            _readText = readText;
        }
        public async Task<char> awaitNext()
        {
            await Task.Delay(0);
            if (nextIndex >= _readText.Length)
                throw new IndexOutOfRangeException($"The collection can hold only {_readText.Length} elements.");

            return _readText[nextIndex++];
        }

        public char Next()
        {
            if (nextIndex >= _readText.Length)
                throw new IndexOutOfRangeException($"The collection can hold only {_readText.Length} elements.");

            return _readText[nextIndex++];
        }
    }
}
