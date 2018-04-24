using System;
using System.Linq;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class AcquireJobsCmdTest : PluggableProcessEngineTestCase
    {
        [Test][Deployment("resources/standalone/jobexecutor/oneJobProcess.bpmn20.xml")]
        public virtual void TestJobsNotVisisbleToAcquisitionIfInstanceSuspended()
        {
            var pd = repositoryService.CreateProcessDefinitionQuery().First(c=>c.Key=="oneTaskProcess");
            var pi = runtimeService.StartProcessInstanceByKey(pd.Key);

            // now there is one job:
            var job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);

            MakeSureJobDue(job);
            
            // the acquirejobs command sees the job:
            var acquiredJobs = ExecuteAcquireJobsCommand();
            Assert.AreEqual(1, acquiredJobs.Size());

            // suspend the process instance:
            runtimeService.SuspendProcessInstanceById(pi.Id);

            // now, the acquirejobs command does not see the job:
            acquiredJobs = ExecuteAcquireJobsCommand();
            Assert.AreEqual(0, acquiredJobs.Size());
        }

        [Test]
        [Deployment("resources/standalone/jobexecutor/oneJobProcess.bpmn20.xml")]
        public virtual void TestJobsNotVisisbleToAcquisitionIfDefinitionSuspended()
        {
            var pd = repositoryService.CreateProcessDefinitionQuery().FirstOrDefault(c=>c.Key == "oneTaskProcess");
            runtimeService.StartProcessInstanceByKey(pd.Key);
            // now there is one job:
            var job = managementService.CreateJobQuery().FirstOrDefault();
            Assert.NotNull(job);

            MakeSureJobDue(job);

            // Todo: shutdown-job-clear
            // the acquirejobs command sees the job:
            var acquiredJobs = ExecuteAcquireJobsCommand();
            Assert.AreEqual(1, acquiredJobs.Size());

            // suspend the process instance:
            repositoryService.SuspendProcessDefinitionById(pd.Id);

            // now, the acquirejobs command does not see the job:
            acquiredJobs = ExecuteAcquireJobsCommand();
            Assert.AreEqual(0, acquiredJobs.Size());
        }
        
        protected internal virtual void MakeSureJobDue(IJob job)
        {
            processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this, job));
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly AcquireJobsCmdTest _outerInstance;

            private readonly IJob _job;

            public CommandAnonymousInnerClass(AcquireJobsCmdTest outerInstance, IJob job)
            {
                _outerInstance = outerInstance;
                _job = job;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                var currentTime = ClockUtil.CurrentTime;
                commandContext.JobManager.FindJobById(_job.Id).Duedate = new DateTime(currentTime.Ticks - 10000);
                return null;
            }
        }

        private AcquiredJobs ExecuteAcquireJobsCommand()
        {
            return
                processEngineConfiguration.CommandExecutorTxRequired.Execute(
                    new AcquireJobsCmd(processEngineConfiguration.JobExecutor));
        }
    }
}