using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Common.Cache;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Persistence.Deploy.Cache;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;
using ProcessEngineBootstrapRule = Engine.Tests.Util.ProcessEngineBootstrapRule;

namespace Engine.Tests.Api.Cfg
{


    /// <summary>
    /// @author Johannes Heinemann
    /// </summary>
    public class DeploymentCacheCfgTest
    {
        private bool InstanceFieldsInitialized = false;

        public DeploymentCacheCfgTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            testRule = new ProcessEngineTestRule(cacheFactoryEngineRule);
            //ruleChain = RuleChain.outerRule(cacheFactoryBootstrapRule).around(cacheFactoryEngineRule).around(testRule);
        }


        protected internal static ProcessEngineBootstrapRule cacheFactoryBootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

        private class ProcessEngineBootstrapRuleAnonymousInnerClass : Util.ProcessEngineBootstrapRule
        {
            public ProcessEngineBootstrapRuleAnonymousInnerClass()
            {
            }

            public override ProcessEngineConfiguration ConfigureEngine(ProcessEngineConfigurationImpl configuration)
            {
                // apply configuration options here
                configuration.CacheCapacity = 2;
                configuration.CacheFactory = new MyCacheFactory();
                configuration.EnableFetchProcessDefinitionDescription = false;
                return configuration;
            }
        }

        protected internal ProvidedProcessEngineRule cacheFactoryEngineRule = new ProvidedProcessEngineRule(cacheFactoryBootstrapRule);

        protected internal ProcessEngineTestRule testRule;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(cacheFactoryBootstrapRule).around(cacheFactoryEngineRule).around(testRule);
        //public RuleChain ruleChain;
        internal IRepositoryService repositoryService;
        internal ProcessEngineConfigurationImpl processEngineConfiguration;
        internal IRuntimeService runtimeService;
        internal ITaskService taskService;
        internal IManagementService managementService;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Before public void initialize()
        public virtual void initialize()
        {
            repositoryService = cacheFactoryEngineRule.RepositoryService;
            processEngineConfiguration = cacheFactoryEngineRule.ProcessEngineConfiguration;
            runtimeService = cacheFactoryEngineRule.RuntimeService;
            taskService = cacheFactoryEngineRule.TaskService;
            managementService = cacheFactoryEngineRule.ManagementService;
        }

        [Test]
        public virtual void testPlugInOwnCacheImplementation()
        {

            // given
            DeploymentCache deploymentCache = processEngineConfiguration.DeploymentCache;

            // when
            ICache<string, ProcessDefinitionEntity> cache = deploymentCache.ProcessDefinitionCache;

            // then
            Assert.That(cache, Is.InstanceOf(typeof(MyCacheImplementation<string, ProcessDefinitionEntity>)));
        }

        [Test]
        public virtual void testDefaultCacheRemovesElementWhenMaxSizeIsExceeded()
        {
            // The engine rule sets the maximum number of elements of the to 2.
            // Accordingly, one process should not be contained in the cache anymore at the end.

            // given
            IList<IBpmnModelInstance> modelInstances = createProcesses(3);
            deploy(modelInstances);
            string processDefinitionIdZero = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "Process0").First().Id;
            string processDefinitionIdOne = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "Process1").First().Id;
            string processDefinitionIdTwo = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "Process2").First().Id;

            // when
            DeploymentCache deploymentCache = processEngineConfiguration.DeploymentCache;

            // then
            int numberOfProcessesInCache = 0;
            numberOfProcessesInCache += deploymentCache.ProcessDefinitionCache.Get(processDefinitionIdZero) == null ? 0 : 1;
            numberOfProcessesInCache += deploymentCache.ProcessDefinitionCache.Get(processDefinitionIdOne) == null ? 0 : 1;
            numberOfProcessesInCache += deploymentCache.ProcessDefinitionCache.Get(processDefinitionIdTwo) == null ? 0 : 1;

            Assert.AreEqual(2, numberOfProcessesInCache);
        }

        [Test]
        public virtual void testDisableQueryOfProcessDefinitionAddModelInstancesToDeploymentCache()
        {

            // given
            deploy(ProcessModels.OneTaskProcessWithDocumentation);
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey(ProcessModels.ProcessKey);

            // when
            var id = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == ProcessModels.ProcessKey).First().Id;

            // then
            DeploymentCache deploymentCache = processEngineConfiguration.DeploymentCache;
            IBpmnModelInstance modelInstance = deploymentCache.BpmnModelInstanceCache.Get(pi.ProcessDefinitionId);
            Assert.IsNull(modelInstance);
        }

        [Test]
        public virtual void testEnableQueryOfProcessDefinitionAddModelInstancesToDeploymentCache()
        {

            // given
            deploy(ProcessModels.OneTaskProcessWithDocumentation);
            processEngineConfiguration.EnableFetchProcessDefinitionDescription = true;
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey(ProcessModels.ProcessKey);

            // when
            var id = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == ProcessModels.ProcessKey).First().Id;

            // then
            DeploymentCache deploymentCache = processEngineConfiguration.DeploymentCache;
            IBpmnModelInstance modelInstance = deploymentCache.BpmnModelInstanceCache.Get(pi.ProcessDefinitionId);
            Assert.NotNull(modelInstance);
        }

        [Test]
        public virtual void testDescriptionIsNullWhenFetchProcessDefinitionDescriptionIsDisabled()
        {

            // given
            deploy(ProcessModels.OneTaskProcessWithDocumentation);
            runtimeService.StartProcessInstanceByKey(ProcessModels.ProcessKey);

            // when
            IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == ProcessModels.ProcessKey).First();

            // then
            Assert.IsNull(processDefinition.Description);
        }

        [Test]
        public virtual void testDescriptionIsAvailableWhenFetchProcessDefinitionDescriptionIsEnabled()
        {

            // given
            deploy(ProcessModels.OneTaskProcessWithDocumentation);
            processEngineConfiguration.EnableFetchProcessDefinitionDescription = true;
            runtimeService.StartProcessInstanceByKey(ProcessModels.ProcessKey);

            // when
            IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == ProcessModels.ProcessKey).First();

            // then
            Assert.NotNull(processDefinition.Description);
            Assert.AreEqual("This is a documentation!", processDefinition.Description);
        }

        [Test]
        public virtual void testLoadProcessDefinitionsFromDBWhenNotExistingInCacheAnymore()
        {

            // given more processes to deploy than capacity in the cache
            int numberOfProcessesToDeploy = 10;
            IList<IBpmnModelInstance> modelInstances = createProcesses(numberOfProcessesToDeploy);
            deploy(modelInstances);

            // when we start a process that was already removed from the cache
            Assert.NotNull(repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "Process0").First());
            runtimeService.StartProcessInstanceByKey("Process0");

            // then we should be able to complete the process
            ITask task = taskService.CreateTaskQuery().First();
            taskService.Complete(task.Id);

        }

        [Test]
        public virtual void testSequentialCallActivityCall()
        {

            // given a number process definitions which call each other by call activities (0->1->2->0->4),
            // which stops after the first repetition of 0 in 4
            IList<IBpmnModelInstance> modelInstances = createSequentialCallActivityProcess();
            deploy(modelInstances);

            // when we start the first process 0
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["NextProcess"] = "Process1";
            runtimeService.StartProcessInstanceByKey("Process0", variables);

            // then we should be able to complete the task in process 4
            ITask task = taskService.CreateTaskQuery().First();
            taskService.Complete(task.Id);
        }

        [Test]
        public virtual void testSequentialCallActivityCallAsynchronously()
        {

            // given a number process definitions which call each other by call activities (0->1->2->0->4),
            // which stops after the first repetition of 0 in 4
            IList<IBpmnModelInstance> modelInstances = createSequentialCallActivityProcessAsync();
            deploy(modelInstances);

            // when we start the first process 0
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["NextProcess"] = "Process1";
            runtimeService.StartProcessInstanceByKey("Process0", variables);
            IJob job = managementService.CreateJobQuery().First();
            managementService.ExecuteJob(job.Id);

            // when we reach process 0 a second time, we have to start that job as well
            job = managementService.CreateJobQuery().First();
            managementService.ExecuteJob(job.Id);

            // then we should be able to complete the task in process 4
            ITask task = taskService.CreateTaskQuery().First();
            taskService.Complete(task.Id);
        }

        [Test]
        public virtual void testSequentialCallActivityAsynchronousWithUnfinishedExecution()
        {

            // given a number process definitions which call each other by call activities (0->1->2->0->4),
            // which stops after the first repetition of 0
            IList<IBpmnModelInstance> modelInstances = createSequentialCallActivityProcessAsync();
            IDeployment deployment = deploy(modelInstances);

            // when we start the first process 0
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["NextProcess"] = "Process1";
            runtimeService.StartProcessInstanceByKey("Process0", variables);
            IJob job = managementService.CreateJobQuery().First();
            managementService.ExecuteJob(job.Id);

            // then deleting the deployment should still be possible
            repositoryService.DeleteDeployment(deployment.Id, true);
        }

        protected internal virtual IList<IBpmnModelInstance> createSequentialCallActivityProcess()
        {
            ICollection<IBpmnModelInstance> modelInstances = new LinkedList<IBpmnModelInstance>();

            modelInstances.Add(CallActivityModels.oneBpmnCallActivityProcessAsExpression(0));
            modelInstances.Add(CallActivityModels.oneBpmnCallActivityProcessPassingVariables(1, 2));
            modelInstances.Add(CallActivityModels.oneBpmnCallActivityProcessPassingVariables(2, 0));
            modelInstances.Add(ProcessModels.SetOneTaskProcess(3));

            return modelInstances as IList<IBpmnModelInstance>;
        }

        protected internal virtual IList<IBpmnModelInstance> createSequentialCallActivityProcessAsync()
        {
            ICollection<IBpmnModelInstance> modelInstances = new LinkedList<IBpmnModelInstance>();

            modelInstances.Add(CallActivityModels.oneBpmnCallActivityProcessAsExpressionAsync(0));
            modelInstances.Add(CallActivityModels.oneBpmnCallActivityProcessPassingVariables(1, 2));
            modelInstances.Add(CallActivityModels.oneBpmnCallActivityProcessPassingVariables(2, 0));
            modelInstances.Add(ProcessModels.SetOneTaskProcess(3));

            return modelInstances as IList<IBpmnModelInstance>;
        }

        protected internal virtual IDeployment deploy(IList<IBpmnModelInstance> modelInstances)
        {
            IDeploymentBuilder deploymentbuilder = processEngineConfiguration.RepositoryService.CreateDeployment();

            for (int i = 0; i < modelInstances.Count; i++)
            {
                deploymentbuilder.AddModelInstance("process" + i + ".bpmn", modelInstances[i]);
            }

            return testRule.Deploy(deploymentbuilder);
        }

        protected internal virtual IDeployment deploy(IBpmnModelInstance modelInstance)
        {
            IDeploymentBuilder deploymentbuilder = processEngineConfiguration.RepositoryService.CreateDeployment();
            deploymentbuilder.AddModelInstance("process0.bpmn", modelInstance);
            return testRule.Deploy(deploymentbuilder);
        }

        protected internal virtual IList<IBpmnModelInstance> createProcesses(int numberOfProcesses)
        {

            IList<IBpmnModelInstance> result = new List<IBpmnModelInstance>(numberOfProcesses);
            for (int i = 0; i < numberOfProcesses; i++)
            {
                result.Add(ProcessModels.SetOneTaskProcess(i));
            }
            return result;
        }


    }
}