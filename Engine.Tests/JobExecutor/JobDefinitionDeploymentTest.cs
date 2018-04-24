using System.Linq;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    /// <summary>
    ///     These testcases verify that job definitions are created upon deployment of the process definition.
    /// </summary>
    [TestFixture]
    public class JobDefinitionDeploymentTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment]
        public virtual void TestTimerStartEvent()
        {
            // given
            var processDefinition = repositoryService.CreateProcessDefinitionQuery().First(c=>c.Key == "testProcess");
            var jobDefinition =
                managementService.CreateJobDefinitionQuery(c=>c.ProcessDefinitionKey=="testProcess").First();

            // then Assert
            Assert.NotNull(jobDefinition);
            Assert.AreEqual(TimerStartEventJobHandler.TYPE, jobDefinition.JobType);
            Assert.AreEqual("theStart", jobDefinition.ActivityId);
            Assert.AreEqual("Date: 2036-11-14T11:12:22", jobDefinition.JobConfiguration);
            Assert.AreEqual(processDefinition.Id, jobDefinition.ProcessDefinitionId);

            // there exists a job with the correct job definition id:
            var timerStartJob = managementService.CreateJobQuery().First();
            Assert.AreEqual(jobDefinition.Id, timerStartJob.JobDefinitionId);
        }

        [Test] [Deployment ]
        public virtual void TestTimerBoundaryEvent()
        {
            // given
            var processDefinition = repositoryService.CreateProcessDefinitionQuery().First(c=>c.Key == "testProcess");
            var jobDefinition =
                managementService.CreateJobDefinitionQuery(c=>c.ProcessDefinitionKey=="testProcess").First();

            // then Assert
            Assert.NotNull(jobDefinition);
            Assert.AreEqual(TimerExecuteNestedActivityJobHandler.TYPE, jobDefinition.JobType);
            Assert.AreEqual("theBoundaryEvent", jobDefinition.ActivityId);
            Assert.AreEqual("Date: 2036-11-14T11:12:22", jobDefinition.JobConfiguration);
            Assert.AreEqual(processDefinition.Id, jobDefinition.ProcessDefinitionId);
        }

        [Test][Deployment ]
        public virtual void TestMultipleTimerBoundaryEvents()
        {
            // given
            var processDefinition = repositoryService.CreateProcessDefinitionQuery().First(c=>c.Key == "testProcess");
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery(c=>c.ProcessDefinitionKey=="testProcess");

            // then Assert
            Assert.AreEqual(2, jobDefinitionQuery.Count());

            var jobDefinition = jobDefinitionQuery.Where(c=>c.ActivityId == "theBoundaryEvent1").First();
            Assert.NotNull(jobDefinition);
            Assert.AreEqual(TimerExecuteNestedActivityJobHandler.TYPE, jobDefinition.JobType);
            Assert.AreEqual("theBoundaryEvent1", jobDefinition.ActivityId);
            Assert.AreEqual("Date: 2036-11-14T11:12:22", jobDefinition.JobConfiguration);
            Assert.AreEqual(processDefinition.Id, jobDefinition.ProcessDefinitionId);

            jobDefinition = jobDefinitionQuery.Where(c=>c.ActivityId == "theBoundaryEvent2").First();
            Assert.NotNull(jobDefinition);
            Assert.AreEqual(TimerExecuteNestedActivityJobHandler.TYPE, jobDefinition.JobType);
            Assert.AreEqual("theBoundaryEvent2", jobDefinition.ActivityId);
            Assert.AreEqual("Duration: PT5M", jobDefinition.JobConfiguration);
            Assert.AreEqual(processDefinition.Id, jobDefinition.ProcessDefinitionId);
        }

        [Test]
        [Deployment]
        public virtual void TestEventBasedGateway()
        {
            // given
            var processDefinition = repositoryService.CreateProcessDefinitionQuery().First(c=>c.Key== "testProcess");
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery(c=>c.ProcessDefinitionKey=="testProcess");

            // then Assert
            Assert.AreEqual(2, jobDefinitionQuery.Count());

            var jobDefinition = jobDefinitionQuery.Where(c=>c.ActivityId == "timer1").First();
            Assert.NotNull(jobDefinition);
            Assert.AreEqual(TimerCatchIntermediateEventJobHandler.TYPE, jobDefinition.JobType);
            Assert.AreEqual("timer1", jobDefinition.ActivityId);
            //Assert.AreEqual("DURATION: PT5M", jobDefinition.JobConfiguration);
            Assert.AreEqual("Duration: PT5M", jobDefinition.JobConfiguration);
            Assert.AreEqual(processDefinition.Id, jobDefinition.ProcessDefinitionId);

            jobDefinition = jobDefinitionQuery.Where(c=>c.ActivityId == "timer2").First();
            Assert.NotNull(jobDefinition);
            Assert.AreEqual(TimerCatchIntermediateEventJobHandler.TYPE, jobDefinition.JobType);
            Assert.AreEqual("timer2", jobDefinition.ActivityId);
            //Assert.AreEqual("DURATION: PT10M", jobDefinition.JobConfiguration);
            Assert.AreEqual("Duration: PT10M", jobDefinition.JobConfiguration);
            Assert.AreEqual(processDefinition.Id, jobDefinition.ProcessDefinitionId);
        }

        [Test]
        [Deployment]
        public virtual void TestTimerIntermediateEvent()
        {
            // given
            var processDefinition = repositoryService.CreateProcessDefinitionQuery().First(c=>c.Key== "testProcess");
            var jobDefinition =
                managementService.CreateJobDefinitionQuery(c=>c.ProcessDefinitionKey=="testProcess").First();

            // then Assert
            Assert.NotNull(jobDefinition);
            Assert.AreEqual(TimerCatchIntermediateEventJobHandler.TYPE, jobDefinition.JobType);
            Assert.AreEqual("timer", jobDefinition.ActivityId);
            Assert.AreEqual("Duration: PT5M", jobDefinition.JobConfiguration);
            Assert.AreEqual(processDefinition.Id, jobDefinition.ProcessDefinitionId);
        }

        [Test]
        [Deployment]
        public virtual void TestAsyncContinuation()
        {
            // given
            var processDefinition = repositoryService.CreateProcessDefinitionQuery().First(c=>c.Key== "testProcess");
            var jobDefinition =
                managementService.CreateJobDefinitionQuery(c=>c.ProcessDefinitionKey=="testProcess").First();

            // then Assert
            Assert.NotNull(jobDefinition);
            Assert.AreEqual(AsyncContinuationJobHandler.TYPE, jobDefinition.JobType);
            Assert.AreEqual("theService", jobDefinition.ActivityId);
            Assert.AreEqual(MessageJobDeclaration.AsyncBefore, jobDefinition.JobConfiguration);
            Assert.AreEqual(processDefinition.Id, jobDefinition.ProcessDefinitionId);
        }

        [Test]
        [Deployment]
        public virtual void TestAsyncContinuationOfMultiInstance()
        {
            // given
            var processDefinition = repositoryService.CreateProcessDefinitionQuery().First(c=>c.Key== "testProcess");
            var jobDefinition =
                managementService.CreateJobDefinitionQuery(c=>c.ProcessDefinitionKey=="testProcess").First();

            // then Assert
            Assert.NotNull(jobDefinition);
            Assert.AreEqual(AsyncContinuationJobHandler.TYPE, jobDefinition.JobType);
            Assert.AreEqual("theService" + BpmnParse.MultiInstanceBodyIdSuffix, jobDefinition.ActivityId);
            Assert.AreEqual(MessageJobDeclaration.AsyncAfter, jobDefinition.JobConfiguration);
            Assert.AreEqual(processDefinition.Id, jobDefinition.ProcessDefinitionId);
        }

        [Test]
        [Deployment]
        public virtual void TestAsyncContinuationOfActivityWrappedInMultiInstance()
        {
            // given
            var processDefinition = repositoryService.CreateProcessDefinitionQuery().First(c=>c.Key == "testProcess");
            var jobDefinition =
                managementService.CreateJobDefinitionQuery(c=>c.ProcessDefinitionKey=="testProcess").First();

            // then Assert
            Assert.NotNull(jobDefinition);
            Assert.AreEqual(AsyncContinuationJobHandler.TYPE, jobDefinition.JobType);
            Assert.AreEqual("theService", jobDefinition.ActivityId);
            Assert.AreEqual(MessageJobDeclaration.AsyncAfter, jobDefinition.JobConfiguration);
            //Todo: JobDefinition的Clear
            Assert.AreEqual(processDefinition.Id, jobDefinition.ProcessDefinitionId);
        }

        [Test]
        [Deployment(new string[]{"resources/jobexecutor/JobDefinitionDeploymentTest.TestAsyncContinuation.bpmn20.xml", "resources/jobexecutor/JobDefinitionDeploymentTest.TestMultipleProcessesWithinDeployment.bpmn20.xml"}) ]
        public virtual void TestMultipleProcessDeployment()
        {
            var query = managementService.CreateJobDefinitionQuery();
            var jobDefinitions = query.ToList();
            Assert.AreEqual(3, jobDefinitions.Count);

            Assert.AreEqual(1, query.Where(c=>c.ProcessDefinitionKey== "testProcess").Count());
            Assert.AreEqual(2, query.Where(c => c.ProcessDefinitionKey == "anotherTestProcess").Count());
        }
    }
}