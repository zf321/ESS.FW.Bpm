using System;
using System.Collections.Generic;
using System.Threading;

namespace Engine.Tests.Api.Cfg
{


    using ResourceProcessEngineTestCase = ResourceProcessEngineTestCase;


    public class IdGeneratorDataSourceTest : ResourceProcessEngineTestCase
    {

        public IdGeneratorDataSourceTest() : base("resources/api/cfg/IdGeneratorDataSourceTest.Camunda.cfg.xml")
        {
        }

        [Deployment]
        public virtual void testIdGeneratorDataSource()
        {
            IList<Thread> threads = new List<Thread>();
            for (int i = 0; i < 20; i++)
            {
                Thread thread = new Thread(() =>
            {
                for (int j = 0; j < 5; j++)
                {
                    runtimeService.StartProcessInstanceByKey("idGeneratorDataSource");
                }
            });
                thread.Start();
                threads.Add(thread);
            }

            foreach (Thread thread in threads)
            {
                try
                {
                    thread.Join();
                }
                catch (ThreadInterruptedException e)
                {
                    Console.WriteLine(e.ToString());
                    Console.Write(e.StackTrace);
                }
            }
        }
    }

}