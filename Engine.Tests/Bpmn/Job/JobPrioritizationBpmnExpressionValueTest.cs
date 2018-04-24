using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Job
{

    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class JobPrioritizationBpmnExpressionValueTest : PluggableProcessEngineTestCase
    {

        protected internal const long EXPECTED_DEFAULT_PRIORITY = 123;
        protected internal const long EXPECTED_DEFAULT_PRIORITY_ON_RESOLUTION_FAILURE = 296;

        protected internal long originalDefaultPriority;
        protected internal long originalDefaultPriorityOnFailure;
        public JobPrioritizationBpmnExpressionValueTest()
        {
            this.SetUpAfterEvent += SetUp;
            this.TearDownAfterEvent += TearDown;
        }

        //[SetUp]
        protected internal virtual void SetUp()
        {
            originalDefaultPriority = DefaultJobPriorityProvider.DEFAULT_PRIORITY;
            originalDefaultPriorityOnFailure = DefaultJobPriorityProvider.DEFAULT_PRIORITY_ON_RESOLUTION_FAILURE;

            DefaultJobPriorityProvider.DEFAULT_PRIORITY = EXPECTED_DEFAULT_PRIORITY;
            DefaultJobPriorityProvider.DEFAULT_PRIORITY_ON_RESOLUTION_FAILURE = EXPECTED_DEFAULT_PRIORITY_ON_RESOLUTION_FAILURE;
        }

        //[TearDown]
        protected new void TearDown()
        {
            // reset default priorities
            DefaultJobPriorityProvider.DEFAULT_PRIORITY = originalDefaultPriority;
            DefaultJobPriorityProvider.DEFAULT_PRIORITY_ON_RESOLUTION_FAILURE = originalDefaultPriorityOnFailure;
        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/job/jobPrioExpressionProcess.bpmn20.xml" })]
        public virtual void TestConstantValueExpressionPrioritization()
        {
            // when
            runtimeService.CreateProcessInstanceByKey("jobPrioExpressionProcess").StartBeforeActivity("task2").Execute();

            // then
            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);
            Assert.AreEqual(15, job.Priority);
        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/job/jobPrioExpressionProcess.bpmn20.xml" })]
        public virtual void TestConstantValueHashExpressionPrioritization()
        {
            // when
            runtimeService.CreateProcessInstanceByKey("jobPrioExpressionProcess").StartBeforeActivity("task4").Execute();

            // then
            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);
            Assert.AreEqual(16, job.Priority);
        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/job/jobPrioExpressionProcess.bpmn20.xml" })]
        public virtual void TestVariableValueExpressionPrioritization()
        {
            // when
            runtimeService.CreateProcessInstanceByKey("jobPrioExpressionProcess").StartBeforeActivity("task1").SetVariable("priority", 22).Execute();

            // then
            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);
            Assert.AreEqual(22, job.Priority);
        }

        /// <summary>
        /// Can't distinguish this case from the cases we have to tolerate due to CAM-4207
        /// </summary>
        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/job/jobPrioExpressionProcess.bpmn20.xml" })]
        public virtual void FAILING_testVariableValueExpressionPrioritizationFailsWhenVariableMisses()
        {
            // when
            try
            {
                runtimeService.CreateProcessInstanceByKey("jobPrioExpressionProcess").StartBeforeActivity("task1").Execute();
                //Assert.Fail("this should not succeed since the priority variable is not defined");
            }
            catch (ProcessEngineException e)
            {

                AssertTextPresentIgnoreCase("Unknown property used in expression: ${priority}. " + "Cause: Cannot resolve identifier 'priority'", e.Message);
            }
        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/job/jobPrioExpressionProcess.bpmn20.xml" })]
        public virtual void TestExecutionExpressionPrioritization()
        {
            // when
            runtimeService.CreateProcessInstanceByKey("jobPrioExpressionProcess").StartBeforeActivity("task1").SetVariable("priority", 25).Execute();

            // then
            IJob job = managementService.CreateJobQuery().First();
            Assert.NotNull(job);
            Assert.AreEqual(25, job.Priority);
        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/job/jobPrioExpressionProcess.bpmn20.xml" })]
        public virtual void TestExpressionEvaluatesToNull()
        {
            // when
            try
            {
                runtimeService.CreateProcessInstanceByKey("jobPrioExpressionProcess").StartBeforeActivity("task3").SetVariable("priority", null).Execute();
                //Assert.Fail("this should not succeed since the priority variable is not defined");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresentIgnoreCase("Priority value is not an Integer", e.Message);
            }
        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/job/jobPrioExpressionProcess.bpmn20.xml" })]
        public virtual void TestExpressionEvaluatesToNonNumericalValue()
        {
            // when
            try
            {
                runtimeService.CreateProcessInstanceByKey("jobPrioExpressionProcess").StartBeforeActivity("task3").SetVariable("priority", "aNonNumericalVariableValue").Execute();
                //Assert.Fail("this should not succeed since the priority must be integer");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresentIgnoreCase("Priority value is not an Integer", e.Message);
            }
        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/job/jobPrioExpressionProcess.bpmn20.xml" })]
        public virtual void TestExpressionEvaluatesToNonIntegerValue()
        {
            // when
            try
            {
                runtimeService.CreateProcessInstanceByKey("jobPrioExpressionProcess").StartBeforeActivity("task3").SetVariable("priority", 4.2d).ExecuteWithVariablesInReturn();
                Assert.Fail("this should not succeed since the priority must be integer");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresentIgnoreCase("Priority value must be either Short, Integer, or Long", e.Message);
            }
        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/job/jobPrioExpressionProcess.bpmn20.xml" })]
        public virtual void TestConcurrentLocalVariablesAreAccessible()
        {
            // when
            runtimeService.CreateProcessInstanceByKey("jobPrioExpressionProcess").StartBeforeActivity("task2").StartBeforeActivity("task1").SetVariableLocal("priority", 14).ExecuteWithVariablesInReturn(); // this is a local variable on the
                                                                                                                                                                                                             // concurrent execution entering the activity

            // Todo: JobManager.FindJobsByQueryCriteria方法的表关联查询。
            // then
            IJob job = managementService.CreateJobQuery(c=>c.ActivityId == "task1").First();
            Assert.NotNull(job);
            Assert.AreEqual(14, job.Priority);
        }

        /// <summary>
        /// This test case Asserts that a non-resolving expression does not Assert.Fail job creation;
        /// This is a unit test scenario, where simply the variable misses (in general a human-made error), but
        /// the actual case covered by the behavior are missing beans (e.g. in the case the engine can't perform a
        /// context switch)
        /// </summary>
        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/job/jobPrioExpressionProcess.bpmn20.xml" })]
        public virtual void TestDefaultPriorityWhenBeanMisses()
        {
            // creating a job with a priority that can't be resolved does not Assert.Fail entirely but uses a default priority
            runtimeService.CreateProcessInstanceByKey("jobPrioExpressionProcess").StartBeforeActivity("task1").Execute();

            // then
            IJob job = managementService.CreateJobQuery().First();
            Assert.AreEqual(EXPECTED_DEFAULT_PRIORITY_ON_RESOLUTION_FAILURE, job.Priority);
        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/job/jobPrioExpressionProcess.bpmn20.xml" })]
        public virtual void TestDisableGracefulDegradation()
        {
            try
            {
                processEngineConfiguration.EnableGracefulDegradationOnContextSwitchFailure = false;

                try
                {
                    runtimeService.CreateProcessInstanceByKey("jobPrioExpressionProcess").StartBeforeActivity("task1").Execute();
                    //Assert.Fail("should not succeed due to missing variable");
                }
                catch (ProcessEngineException e)
                {
                    AssertTextPresentIgnoreCase("unknown property used in expression", e.Message);
                }

            }
            finally
            {
                processEngineConfiguration.EnableGracefulDegradationOnContextSwitchFailure = true;
            }
        }
        [Test]
        public virtual void TestDefaultEngineConfigurationSetting()
        {
            ProcessEngineConfigurationImpl config = new StandaloneInMemProcessEngineConfiguration();

            Assert.True(config.EnableGracefulDegradationOnContextSwitchFailure);
        }

    }

}