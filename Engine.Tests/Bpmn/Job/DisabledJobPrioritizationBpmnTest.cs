using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Job
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class DisabledJobPrioritizationBpmnTest : PluggableProcessEngineTestCase
    {
        public DisabledJobPrioritizationBpmnTest()
        {
            SetUpAfterEvent += SetUp;
            TearDownAfterEvent += this.TearDown;
        }

        //[SetUp]
        protected internal virtual void SetUp()
        {
            processEngineConfiguration.ProducePrioritizedJobs = false;
        }

        //[TearDown]
        protected internal virtual void TearDown()
        {
            processEngineConfiguration.ProducePrioritizedJobs = true;
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/job/jobPrioProcess.bpmn20.xml" })]
        public virtual void TestJobPriority()
        {
            // when
            var test = runtimeService.CreateProcessInstanceByKey("jobPrioProcess");
            test.StartBeforeActivity("task1");
            test.StartBeforeActivity("task2");
            test.ExecuteWithVariablesInReturn();
            //test.Execute();

            // Todo: 在数据清理功能稳定前，用例跑不通需手动清理TB_GOS_BPM_RU_JOB表数据
            // then
            IList<IJob> jobs = managementService.CreateJobQuery().ToList();
            Assert.AreEqual(2, jobs.Count);

            foreach (IJob job in jobs)
            {
                Assert.NotNull(job);
                Assert.AreEqual(0, job.Priority);
            }
        }
    }

}