using NUnit.Framework;

namespace Engine.Tests.ConCurrency
{
    [TestFixture]
    public class UuidGeneratorTest
    {

        private const int THREAD_COUNT = 10;
        private const int LOOP_COUNT = 10000;

        //[Test]
        //public virtual void testMultithreaded()
        //{
        //    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //    //ORIGINAL LINE: final java.util.List<Thread> threads = new java.util.ArrayList<Thread>();
        //    IList<Thread> threads = new List<Thread>();

        //    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //    //ORIGINAL LINE: final com.Fasterxml.uuid.impl.TimeBasedGenerator timeBasedGenerator = com.Fasterxml.uuid.Generators.TimeBasedGenerator(com.Fasterxml.uuid.EthernetAddress.FromInterface());
        //    TimeBasedGenerator timeBasedGenerator = Generators.TimeBasedGenerator(EthernetAddress.FromInterface());
        //    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //    //ORIGINAL LINE: final java.util.concurrent.ConcurrentSkipListSet<String> generatedIds = new java.util.concurrent.ConcurrentSkipListSet<String>();
        //    ConcurrentSkipListSet<string> generatedIds = new ConcurrentSkipListSet<string>();
        //    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //    //ORIGINAL LINE: final java.util.concurrent.ConcurrentSkipListSet<String> duplicatedIds = new java.util.concurrent.ConcurrentSkipListSet<String>();
        //    ConcurrentSkipListSet<string> duplicatedIds = new ConcurrentSkipListSet<string>();

        //    for (int i = 0; i < THREAD_COUNT; i++)
        //    {
        //        Thread thread = new Thread(() =>
        //    {
        //        for (int j = 0; j < LOOP_COUNT; j++)
        //        {

        //            string id = timeBasedGenerator.generate().ToString();
        //            bool wasAdded = generatedIds.Add(id);
        //            if (!wasAdded)
        //            {
        //                duplicatedIds.Add(id);
        //            }
        //        }
        //    });
        //        threads.Add(thread);
        //        thread.Start();
        //    }

        //    foreach (Thread thread in threads)
        //    {
        //        thread.Join();
        //    }

        //    Assert.AreEqual(THREAD_COUNT * LOOP_COUNT, generatedIds.Count);
        //    Assert.True(duplicatedIds.Empty);
        //}

    }

}