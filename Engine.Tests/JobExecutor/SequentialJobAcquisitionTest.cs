using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Common.Extensions;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class SequentialJobAcquisitionTest
    {
        [TearDown]
        public virtual void StopJobExecutor()
        {
            _jobExecutor.Shutdown();
        }

        [TearDown]
        public virtual void ResetClock()
        {
            ClockUtil.Reset();
        }

        [TearDown]
        public virtual void CloseProcessEngines()
        {
            var iterator = _createdProcessEngines.GetEnumerator();
            while (iterator.MoveNext())
            {
                var processEngine = iterator.Current;
                processEngine.Close();
                ProcessEngines.Unregister(processEngine);
                //JAVA TO C# CONVERTER TODO Resources.Task: .NET enumerators are read-only:
                //iterator.Remove();
            }
        }

        private static readonly string ResourceBase =
            typeof(SequentialJobAcquisitionTest).Assembly.GetName().Name.Replace(".", "/");

        private static readonly string ProcessResource = /*ResourceBase +*/ "resources/jobexecutor/IntermediateTimerEventTest.TestCatchingTimerEvent.bpmn20.xml";

        private readonly IList<IProcessEngine> _createdProcessEngines = new List<IProcessEngine>();

        private readonly ESS.FW.Bpm.Engine.Impl.JobExecutor.JobExecutor _jobExecutor = new DefaultJobExecutor();

        [Test]
        public virtual void TestExecuteJobsForSingleEngine()
        {
            // configure and build a process engine
            StandaloneProcessEngineConfiguration standaloneProcessEngineConfiguration =
                new StandaloneInMemProcessEngineConfiguration();
            //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            standaloneProcessEngineConfiguration.SetProcessEngineName(GetType().FullName + "-engine1");
            //standaloneProcessEngineConfiguration.JdbcUrl = "jdbc:h2:mem:jobexecutor-test-engine";
            standaloneProcessEngineConfiguration.SetJobExecutorActivate(false);
            standaloneProcessEngineConfiguration.SetJobExecutor(_jobExecutor);
            standaloneProcessEngineConfiguration.SetDbMetricsReporterActivate(false);
            var engine = standaloneProcessEngineConfiguration.BuildProcessEngine();

            _createdProcessEngines.Add(engine);

            engine.RepositoryService.CreateDeployment().AddClasspathResource(ProcessResource).Deploy();

            _jobExecutor.Shutdown();

            engine.RuntimeService.StartProcessInstanceByKey("intermediateTimerEventExample");

            Assert.AreEqual(1, engine.ManagementService.CreateJobQuery().Count());

            ClockUtil.CurrentTime = DateTime.Now.AddDays(6);
            _jobExecutor.Start();

            WaitForJobExecutorToProcessAllJobs(10000, 100, _jobExecutor, engine.RuntimeService, true);

            Assert.AreEqual(0, engine.ManagementService.CreateJobQuery().Count());
        }

        [Test]
        public virtual void TestExecuteJobsForTwoEnginesSameAcquisition()
        {
            // configure and build a process engine
            StandaloneProcessEngineConfiguration engineConfiguration1 = new StandaloneInMemProcessEngineConfiguration();
            //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:

            engineConfiguration1.SetProcessEngineName(GetType().FullName + "-engine1");
            //engineConfiguration1.JdbcUrl = "jdbc:h2:mem:activiti1";
            engineConfiguration1.SetJobExecutorActivate(false);
            engineConfiguration1.SetJobExecutor(_jobExecutor);
            engineConfiguration1.SetDbMetricsReporterActivate(false);

            var engine1 = engineConfiguration1.BuildProcessEngine();
            _createdProcessEngines.Add(engine1);

            // and a second one
            StandaloneProcessEngineConfiguration engineConfiguration2 = new StandaloneInMemProcessEngineConfiguration();
            //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            engineConfiguration2.SetProcessEngineName(GetType().FullName + "engine2");
            //engineConfiguration2.JdbcUrl = "jdbc:h2:mem:activiti2";
            engineConfiguration2.SetJobExecutorActivate(false);
            engineConfiguration2.SetJobExecutor(_jobExecutor);
            engineConfiguration2.SetDbMetricsReporterActivate(false);

            var engine2 = engineConfiguration2.BuildProcessEngine();
            _createdProcessEngines.Add(engine2);

            // stop the acquisition
            _jobExecutor.Shutdown();

            // deploy the processes

            engine1.RepositoryService.CreateDeployment().AddClasspathResource(ProcessResource).Deploy();

            engine2.RepositoryService.CreateDeployment().AddClasspathResource(ProcessResource).Deploy();

            // start one instance for each engine:

            var procinst1 = engine1.RuntimeService.StartProcessInstanceByKey("intermediateTimerEventExample");
            var procinst2 = engine2.RuntimeService.StartProcessInstanceByKey("intermediateTimerEventExample");

            Assert.AreEqual(1, engine1.ManagementService.CreateJobQuery().Count(c => c.ProcessInstanceId == procinst1.Id));
            Assert.AreEqual(1, engine2.ManagementService.CreateJobQuery().Count(c => c.ProcessInstanceId == procinst2.Id));
            
            ClockUtil.CurrentTime = DateTime.Now.AddDays(6);

            _jobExecutor.Start();
            // Assert task completed for the first engine
            WaitForJobExecutorToProcessAllJobs(10000, 100, _jobExecutor, engine1.RuntimeService, true);

            _jobExecutor.Start();
            // Assert task completed for the second engine
            WaitForJobExecutorToProcessAllJobs(10000, 100, _jobExecutor, engine2.RuntimeService, true);

            Assert.AreEqual(0, engine1.ManagementService.CreateJobQuery().Count(c => c.ProcessInstanceId == procinst1.Id));
            Assert.AreEqual(0, engine2.ManagementService.CreateJobQuery().Count(c => c.ProcessInstanceId == procinst2.Id));
        }


        [Test]
        public virtual void TestJobAddedGuardForTwoEnginesSameAcquisition()
        {
            // configure and build a process engine
            StandaloneProcessEngineConfiguration engineConfiguration1 = new StandaloneInMemProcessEngineConfiguration();
            //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            engineConfiguration1.SetProcessEngineName(GetType().FullName + "-engine1");
            //engineConfiguration1.JdbcUrl = "jdbc:h2:mem:activiti1";
            engineConfiguration1.SetJobExecutorActivate(false);
            engineConfiguration1.SetJobExecutor(_jobExecutor);
            engineConfiguration1.SetDbMetricsReporterActivate(false);

            var engine1 = engineConfiguration1.BuildProcessEngine();
            _createdProcessEngines.Add(engine1);

            // and a second one
            StandaloneProcessEngineConfiguration engineConfiguration2 = new StandaloneInMemProcessEngineConfiguration();
            //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:

            engineConfiguration2.SetProcessEngineName(GetType().FullName + "engine2");
            //engineConfiguration2.JdbcUrl = "jdbc:h2:mem:activiti2";
            engineConfiguration2.SetJobExecutorActivate(false);
            engineConfiguration2.SetJobExecutor(_jobExecutor);
            engineConfiguration2.SetDbMetricsReporterActivate(false);

            var engine2 = engineConfiguration2.BuildProcessEngine();
            _createdProcessEngines.Add(engine2);

            // stop the acquisition
            _jobExecutor.Shutdown();

            // deploy the processes

            engine1.RepositoryService.CreateDeployment().AddClasspathResource(ProcessResource).Deploy();

            engine2.RepositoryService.CreateDeployment().AddClasspathResource(ProcessResource).Deploy();

            // start one instance for each engine:

            var procinst1 = engine1.RuntimeService.StartProcessInstanceByKey("intermediateTimerEventExample");
            var procinst2 = engine2.RuntimeService.StartProcessInstanceByKey("intermediateTimerEventExample");
            

            ClockUtil.CurrentTime = DateTime.Now.AddDays(6);

            Assert.AreEqual(1, engine1.ManagementService.CreateJobQuery().Count(c => c.ProcessInstanceId == procinst1.Id));
            Assert.AreEqual(1, engine2.ManagementService.CreateJobQuery().Count(c => c.ProcessInstanceId == procinst2.Id));

            // Assert task completed for the first engine
            _jobExecutor.Start();
            WaitForJobExecutorToProcessAllJobs(10000, 100, _jobExecutor, engine1.RuntimeService, false);

            // Assert task completed for the second engine
            _jobExecutor.Start();
            WaitForJobExecutorToProcessAllJobs(10000, 100, _jobExecutor, engine2.RuntimeService, false);

            Thread.Sleep(2000);

            Assert.IsFalse(((SequentialJobAcquisitionRunnable)_jobExecutor.AcquireJobsRunnable).JobAdded);

            Assert.AreEqual(0, engine1.ManagementService.CreateJobQuery().Count(c => c.ProcessInstanceId == procinst1.Id));
            Assert.AreEqual(0, engine2.ManagementService.CreateJobQuery().Count(c => c.ProcessInstanceId == procinst2.Id));
        }


        ////////// helper methods ////////////////////////////


        public virtual void WaitForJobExecutorToProcessAllJobs(long maxMillisToWait, int intervalMillis,
            ESS.FW.Bpm.Engine.Impl.JobExecutor.JobExecutor jobExecutor, IRuntimeService runtimeService, bool shutdown)
        {
            try
            {
                InteruptTask task = new InteruptTask(Thread.CurrentThread);
                System.Threading.Timer timer = new Timer(task.Run, null, maxMillisToWait,0);
                
                bool areJobsAvailable = true;
                try
                {
                    while (areJobsAvailable && !task.TimeLimitExceeded)
                    {
                        Thread.Sleep(intervalMillis);
                        areJobsAvailable = AreJobsAvailable(runtimeService);
                    }
                }
                catch (ThreadInterruptedException e)
                {
                }
                finally
                {
                    timer.Change(-1, 0);
                }
                if (areJobsAvailable)
                {
                    throw new ProcessEngineException("time limit of " + maxMillisToWait + " was exceeded");
                }
            }
            finally
            {
                if (shutdown)
                    jobExecutor.Shutdown();
            }
        }

        public virtual bool AreJobsAvailable(IRuntimeService runtimeService)
        {
            var jobManager = runtimeService.GetManager<IJobManager>();
            return ! jobManager.FindJobsByExecutable().IsEmpty();            
            //return !managementService.CreateJobQuery() /*.Executable()*/.ToList().IsEmpty();
        }

        private class InteruptTask 
        {
            protected internal bool timeLimitExceeded = false;
            protected internal Thread thread;
            public InteruptTask(Thread thread)
            {
                this.thread = thread;
            }
            public virtual bool TimeLimitExceeded => timeLimitExceeded;

            public virtual void Run(object state)
            {
                timeLimitExceeded = true;
                thread.Interrupt();
            }
            
        }
    }
}