using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TPL.Test
{
    public class TestMultipleTasks
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
            Assert.CatchAsync<IndexOutOfRangeException>(async delegate () { await reader.awaitNext(); });
        }

        [Test]
        public async Task MultipleTaskTest()
        {
            var _proc = new ParallelProcessing<string, int>();
            var o = new OutputWrite<string, int>();

            //run 100 Task parallel
            Reader[] range = Enumerable.Range(1, 100).Select(i => new Reader(_str)).ToArray();

            await _proc.MultipleTasks(range, o, CancellationToken.None);

            CollectionAssert.AllItemsAreUnique(o, "");
            Assert.IsTrue(o.Count > 0);
            Assert.AreEqual(o["a"], 600);
            Assert.AreEqual(o["as"], 400);
            Assert.AreEqual(o["to"], 700);
        }
    }
}