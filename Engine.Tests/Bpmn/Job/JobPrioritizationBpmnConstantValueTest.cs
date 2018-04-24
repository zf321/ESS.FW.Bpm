using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Job
{

    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class JobPrioritizationBpmnConstantValueTest : PluggableProcessEngineTestCase
    {
        protected internal const long EXPECTED_DEFAULT_PRIORITY = 0;
        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/job/oneTaskProcess.bpmn20.xml" })]
        public virtual void TestDefaultPrioritizationAsyncBefore()
        {
            // when
            runtimeService.CreateProcessInstanceByKey("oneTaskProcess").StartBeforeActivity("task1").Execute();

            // then
            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);
            Assert.AreEqual(EXPECTED_DEFAULT_PRIORITY, job.Priority);
        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/job/oneTaskProcess.bpmn20.xml" })]
        public virtual void TestDefaultPrioritizationAsyncAfter()
        {

            // given
            runtimeService.CreateProcessInstanceByKey("oneTaskProcess").StartBeforeActivity("task1").ExecuteWithVariablesInReturn();

            // Todo: java源码ExecuteJob后JobEntity.PostExecute删除Ru_Job表数据仍然能查询出数据
            // when
            managementService.ExecuteJob(managementService.CreateJobQuery().First().Id);

            // then
            IJob job = managementService.CreateJobQuery().FirstOrDefault();
            Assert.NotNull(job);
            Assert.AreEqual(EXPECTED_DEFAULT_PRIORITY, job.Priority);

        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/job/oneTimerProcess.bpmn20.xml" })]
        public virtual void TestDefaultPrioritizationTimer()
        {
            // when
            runtimeService.CreateProcessInstanceByKey("oneTimerProcess").StartBeforeActivity("timer1").Execute();

            // then
            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);
            Assert.AreEqual(EXPECTED_DEFAULT_PRIORITY, job.Priority);
        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/job/jobPrioProcess.bpmn20.xml" })]
        public virtual void TestProcessDefinitionPrioritizationAsyncBefore()
        {
            // when
            runtimeService.CreateProcessInstanceByKey("jobPrioProcess").StartBeforeActivity("task1").Execute();

            // then
            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);
            Assert.AreEqual(10, job.Priority);
        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/job/jobPrioProcess.bpmn20.xml" })]
        public virtual void TestProcessDefinitionPrioritizationAsyncAfter()
        {
            // given
            runtimeService.CreateProcessInstanceByKey("jobPrioProcess").StartBeforeActivity("task1").ExecuteWithVariablesInReturn();

            // when
            managementService.ExecuteJob(managementService.CreateJobQuery().First().Id);

            // then
            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);
            Assert.AreEqual(10, job.Priority);
        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/job/intermediateTimerJobPrioProcess.bpmn20.xml" })]
        public virtual void TestProcessDefinitionPrioritizationTimer()
        {
            // when
            runtimeService.CreateProcessInstanceByKey("intermediateTimerJobPrioProcess").StartBeforeActivity("timer1").Execute();

            // then
            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);
            Assert.AreEqual(8, job.Priority);
        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/job/jobPrioProcess.bpmn20.xml" })]
        public virtual void TestActivityPrioritizationAsyncBefore()
        {
            // when
            runtimeService.CreateProcessInstanceByKey("jobPrioProcess").StartBeforeActivity("task2").Execute();

            // then
            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);
            Assert.AreEqual(5, job.Priority);
        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/job/jobPrioProcess.bpmn20.xml" })]
        public virtual void TestActivityPrioritizationAsyncAfter()
        {
            // given
            runtimeService.CreateProcessInstanceByKey("jobPrioProcess").StartBeforeActivity("task2").ExecuteWithVariablesInReturn();

            // Todo: java源码ExecuteJob后JobEntity.PostExecute删除Ru_Job表数据仍然能查询出数据
            // when
            managementService.ExecuteJob(managementService.CreateJobQuery().First().Id);

            // then
            IJob job = managementService.CreateJobQuery().FirstOrDefault();
            Assert.NotNull(job);
            Assert.AreEqual(5, job.Priority);
        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/job/intermediateTimerJobPrioProcess.bpmn20.xml" })]
        public virtual void TestActivityPrioritizationTimer()
        {
            // when
            runtimeService.CreateProcessInstanceByKey("intermediateTimerJobPrioProcess").StartBeforeActivity("timer2").Execute();

            // then
            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);
            Assert.AreEqual(4, job.Priority);
        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/job/subProcessJobPrioProcess.bpmn20.xml" })]
        public virtual void TestSubProcessPriorityIsNotDefaultForContainedActivities()
        {
            // when starting an activity contained in the sub process where the
            // sub process has job priority 20
            runtimeService.CreateProcessInstanceByKey("subProcessJobPrioProcess").StartBeforeActivity("task1").Execute();

            // then the job for that activity has priority 10 which is the process definition's
            // priority; the sub process priority is not inherited
            IJob job = managementService.CreateJobQuery().First();
            Assert.AreEqual(10, job.Priority);
        }
        [Test]
        public virtual void TestFailOnMalformedInput()
        {
            try
            {
                repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/job/invalidPrioProcess.bpmn20.xml").Deploy();
               // Assert.Fail("deploying a process with malformed priority should not succeed");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresentIgnoreCase("value 'thisIsNotANumber' for attribute 'jobPriority' " + "is not a valid number", e.Message);
            }
        }
        [Test]
        public virtual void TestParsePriorityOnNonAsyncActivity()
        {

            // deploying a process definition where the activity
            // has a priority but defines no jobs succeeds
            IDeployment deployment = repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/job/JobPrioritizationBpmnTest.TestParsePriorityOnNonAsyncActivity.bpmn20.xml").Deploy();

            // cleanup
            repositoryService.DeleteDeployment(deployment.Id);
        }
        [Test]
        public virtual void TestTimerStartEventPriorityOnProcessDefinition()
        {
            // given a timer start job
            IDeployment deployment = repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/job/JobPrioritizationBpmnConstantValueTest.TestTimerStartEventPriorityOnProcessDefinition.bpmn20.xml").Deploy();

            IJob job = managementService.CreateJobQuery().First();

            // then the timer start job has the priority defined in the process definition
            Assert.AreEqual(8, job.Priority);

            // cleanup
            repositoryService.DeleteDeployment(deployment.Id, true);
        }
        [Test]
        public virtual void TestTimerStartEventPriorityOnActivity()
        {
            // given a timer start job
            IDeployment deployment = repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/job/JobPrioritizationBpmnConstantValueTest.TestTimerStartEventPriorityOnActivity.bpmn20.xml").Deploy();

            IJob job = managementService.CreateJobQuery().First();

            // then the timer start job has the priority defined in the process definition
            Assert.AreEqual(1515, job.Priority);

            // cleanup
            repositoryService.DeleteDeployment(deployment.Id, true);
        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/job/boundaryTimerJobPrioProcess.bpmn20.xml" })]
        public virtual void TestBoundaryTimerEventPriority()
        {
            // given an active boundary event timer
            runtimeService.StartProcessInstanceByKey("boundaryTimerJobPrioProcess");

            IJob job = managementService.CreateJobQuery().First();

            // then the job has the priority specified in the BPMN XML
            Assert.AreEqual(20, job.Priority);
        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/job/eventSubprocessTimerJobPrioProcess.bpmn20.xml" })]
        public virtual void TestEventSubprocessTimerPriority()
        {
            // given an active event subprocess timer
            runtimeService.StartProcessInstanceByKey("eventSubprocessTimerJobPrioProcess");

            IJob job = managementService.CreateJobQuery().First();

            // then the job has the priority specified in the BPMN XML
            Assert.AreEqual(25, job.Priority);
        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/job/intermediateSignalAsyncProcess.bpmn20.xml", "resources/bpmn/job/intermediateSignalCatchJobPrioProcess.bpmn20.xml" })]
        public virtual void TestAsyncSignalThrowingEventActivityPriority()
        {
            // given a receiving process instance with two subscriptions
            runtimeService.StartProcessInstanceByKey("intermediateSignalCatchJobPrioProcess");

            // and a process instance that executes an async signal throwing event
            runtimeService.StartProcessInstanceByKey("intermediateSignalJobPrioProcess");

            IExecution signal1Execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "signal1").First();
            IJob signal1Job = managementService.CreateJobQuery(c => c.ExecutionId == signal1Execution.Id).First();

            IExecution signal2Execution = runtimeService.CreateExecutionQuery(c => c.ActivityId == "signal2").First();
            IJob signal2Job = managementService.CreateJobQuery(c => c.ExecutionId == signal2Execution.Id).First();

            // then the jobs have the priority as specified for the receiving events, not the throwing
            Assert.AreEqual(8, signal1Job.Priority);
            Assert.AreEqual(4, signal2Job.Priority);

        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/job/intermediateSignalAsyncProcess.bpmn20.xml", "resources/bpmn/job/signalStartJobPrioProcess.bpmn20.xml" })]
        public virtual void TestAsyncSignalThrowingEventSignalStartActivityPriority()
        {
            //Todo: excution为空，原因为查明

            // given a process instance that executes an async signal throwing event
            runtimeService.StartProcessInstanceByKey("intermediateSignalJobPrioProcess");

            // then there is an async job for the signal start event with the priority defined in the BPMN XML
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());
            IJob signalStartJob = managementService.CreateJobQuery().First();
            Assert.NotNull(signalStartJob);
            Assert.AreEqual(4, signalStartJob.Priority);

        }

        //[Test]
        [Deployment(Resources = new string[] { "resources/bpmn/job/miBodyAsyncProcess.bpmn20.xml" })]
        public virtual void FAILING_testMultiInstanceBodyActivityPriority()
        {
            // Todo: 此用例java源码自身无法通过
            // given a process instance that executes an async mi body
            runtimeService.StartProcessInstanceByKey("miBodyAsyncPriorityProcess");

            // then there is a job that has the priority as defined on the activity
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());
            IJob miBodyJob = managementService.CreateJobQuery().First();
            Assert.NotNull(miBodyJob);
            Assert.AreEqual(5, miBodyJob.Priority);
        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/job/miInnerAsyncProcess.bpmn20.xml" })]
        public virtual void TestMultiInstanceInnerActivityPriority()
        {
            // given a process instance that executes an async mi inner activity
            runtimeService.StartProcessInstanceByKey("miBodyAsyncPriorityProcess");

            // then there are three jobs that have the priority as defined on the activity (TODO: or should it be MI characteristics?)
            IList<IJob> jobs = managementService.CreateJobQuery().ToList();

            Assert.AreEqual(3, jobs.Count);
            foreach (IJob job in jobs)
            {
                Assert.NotNull(job);
                Assert.AreEqual(5, job.Priority);
            }
        }
    }

}