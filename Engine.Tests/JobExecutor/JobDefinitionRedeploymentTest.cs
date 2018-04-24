using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Management;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    /// <summary>
    ///     Redeploy process definition and Assert that no new job definitions were created.
    /// </summary>
    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class JobDefinitionRedeploymentTest
    [TestFixture]
    public class JobDefinitionRedeploymentTest
    {
        [SetUp]
        public void SetUpEngineRule()
        {
            try
            {
                if (Rule.ProcessEngine == null)
                    Rule.InitializeProcessEngine();

                Rule.InitializeServices();

                Rule.Starting();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        [SetUp]
        public virtual void InitServices()
        {
            ManagementService = Rule.ManagementService;
            RepositoryService = Rule.RepositoryService;
            RuntimeService = Rule.RuntimeService;
            ProcessEngineConfiguration = (ProcessEngineConfigurationImpl)Rule.ProcessEngine.ProcessEngineConfiguration;
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameters(name = "{index}: process definition = {0}") public static java.util.Collection<Object[]> data()
        public static ICollection<object[]> Data()
        {
            return new[]
            {
                new object[] {"resources/jobexecutor/JobDefinitionDeploymentTest.TestTimerStartEvent.bpmn20.xml"},
                new object[] {"resources/jobexecutor/JobDefinitionDeploymentTest.TestTimerBoundaryEvent.bpmn20.xml"},
                new object[] {"resources/jobexecutor/JobDefinitionDeploymentTest.TestMultipleTimerBoundaryEvents.bpmn20.xml"},
                new object[] {"resources/jobexecutor/JobDefinitionDeploymentTest.TestEventBasedGateway.bpmn20.xml"},
                new object[] {"resources/jobexecutor/JobDefinitionDeploymentTest.TestTimerIntermediateEvent.bpmn20.xml"},
                new object[] {"resources/jobexecutor/JobDefinitionDeploymentTest.TestAsyncContinuation.bpmn20.xml"},
                new object[]{ "resources/jobexecutor/JobDefinitionDeploymentTest.TestAsyncContinuationOfMultiInstance.bpmn20.xml"},
                new object[]{ "resources/jobexecutor/JobDefinitionDeploymentTest.TestAsyncContinuationOfActivityWrappedInMultiInstance.bpmn20.xml"}
            };
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameter public String processDefinitionResource;
        public string ProcessDefinitionResource;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public ProcessEngineRule rule = new util.ProvidedProcessEngineRule();
        public ProcessEngineRule Rule = new ProvidedProcessEngineRule();

        protected internal IManagementService ManagementService;
        protected internal IRepositoryService RepositoryService;
        protected internal IRuntimeService RuntimeService;
        protected internal ProcessEngineConfigurationImpl ProcessEngineConfiguration;

        [Test]
        public virtual void TestJobDefinitionsAfterRedeploment()
        {
            ProcessDefinitionResource = Data().ElementAt(0).ElementAt(0).ToString();
            // initially there are no job definitions:
            Assert.AreEqual(0, ManagementService.CreateJobDefinitionQuery().Count());

            // initial deployment
            var deploymentId =
                RepositoryService.CreateDeployment().AddClasspathResource(ProcessDefinitionResource).Deploy().Id;

            var processDefinition = RepositoryService.CreateProcessDefinitionQuery()
                .SingleOrDefault(c => c.DeploymentId == deploymentId);
            Assert.NotNull(processDefinition);

            // this parses the process and created the IJob definitions:
            var jobDefinitions = ManagementService.CreateJobDefinitionQuery().ToList();
            var jobDefinitionIds = GetJobDefinitionIds(jobDefinitions);

            // now clear the cache:
            ProcessEngineConfiguration.DeploymentCache.DiscardProcessDefinitionCache();

            // if we start an instance of the process, the process will be parsed again:
            RuntimeService.StartProcessInstanceByKey(processDefinition.Key);

            // no new definitions were created
            Assert.AreEqual(jobDefinitions.Count, ManagementService.CreateJobDefinitionQuery().Count());

            // the job has the correct definitionId set:
            var jobs = ManagementService.CreateJobQuery().ToList();
            foreach (var job in jobs)
                Assert.True(jobDefinitionIds.Contains(job.JobDefinitionId));

            // Delete the deployment
            RepositoryService.DeleteDeployment(deploymentId, true);
        }

        protected internal virtual ISet<string> GetJobDefinitionIds(IList<IJobDefinition> jobDefinitions)
        {
            ISet<string> definitionIds = new HashSet<string>();
            foreach (var definition in jobDefinitions)
                definitionIds.Add(definition.Id);
            return definitionIds;
        }
    }
}