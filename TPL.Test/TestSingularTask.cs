using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TPL.Test
{
    public class TestSingularTask
    {
        private ReadFile _readFile;
        private string _str;

        [SetUp]
        public async Task Setup()
        {
            _readFile = new ReadFile();
            _str = await _readFile.GetData();
        }

        [Test]
        public void ReadFileTest()
        {
            Assert.IsNotNull(_readFile);
            CollectionAssert.IsNotEmpty(_str);
        }

        [Test]
        public void ReaderError()
        {
            var reader = new Reader("");
            Assert.Catch<IndexOutOfRangeException>(() => reader.Next());
        }

        [Test]
        public async Task SingularThreadTest()
        {
            var _proc = new ParallelProcessing<string, int>();
            var r = new Reader(_str); var o = new OutputWrite<string, int>();
         
            await _proc.SingularTask(r, o, CancellationToken.None);

            CollectionAssert.AllItemsAreUnique(o, "");          
            Assert.IsTrue(o.Count > 0);
            Assert.AreEqual(o["a"],  6);
            Assert.AreEqual(o["as"], 4);
            Assert.AreEqual(o["to"], 7);

        }
    }
}