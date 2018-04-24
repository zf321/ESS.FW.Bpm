using System;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor.ThreadTask
{
    [TestFixture]
    public class ThreadPoolExecutorTest
    {
        private static readonly JobExecutorLogger Log = ProcessEngineLogger.JobExecutorLogger;
        protected ThreadPoolExecutor threadPoolExecutor;
        [SetUp]
        public void SetUP()
        {
            threadPoolExecutor = new ThreadPoolExecutor();
        }
        [TearDown]
        public void TearDown()
        {
            threadPoolExecutor.Stop();
        }
        [Test]
        public void LogTest()
        {
            for (int i = 0; i < 10; i++)
            {
                threadPoolExecutor.AddTask(() => { Log.LogDebug("任务" , "内容。。。"); });
            }
            threadPoolExecutor.Start();
            while (threadPoolExecutor.TaskList.Count > 0)
            {
                System.Threading.Thread.Sleep(500);
            }
            threadPoolExecutor.Stop();
            threadPoolExecutor.AddTask(() => { Log.LogDebug("任务", "时间:"+DateTime.Now); },DateTime.Now.AddSeconds(5));
            threadPoolExecutor.Start();
            while (threadPoolExecutor.TaskList.Count > 0)
            {
                System.Threading.Thread.Sleep(500);
            }
            
        }
        [Test]
        public void CancelTest()
        {
            threadPoolExecutor.AddTask(() => { Log.LogDebug("任务", "时间:" + DateTime.Now); }, DateTime.Now.AddSeconds(5));
            threadPoolExecutor.Start();
            threadPoolExecutor.cancelToken.Cancel();
            while (threadPoolExecutor.TaskList.Count > 0)
            {
                System.Threading.Thread.Sleep(500);
            }
            Log.LogDebug("剩余任务",threadPoolExecutor.TaskList.Count.ToString());
        }
    }
}
