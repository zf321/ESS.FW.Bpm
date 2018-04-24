using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Repository.Impl;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy
{
    public class MultiTenancyRepositoryServiceTest
    {
        protected internal const string TENANT_TWO = "tenant2";
        protected internal const string TENANT_ONE = "tenant1";
        protected internal const string CMMN = "resources/cmmn/deployment/CmmnDeploymentTest.TestSimpleDeployment.cmmn";
        protected internal const string DMN = "resources/api/multitenancy/simpleDecisionTable.Dmn";

        protected internal static readonly IBpmnModelInstance emptyProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess()
            .Done();

        protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        private readonly bool InstanceFieldsInitialized;
        protected internal ProcessEngineConfiguration processEngineConfiguration;


        protected internal IRepositoryService repositoryService;

        protected internal ProcessEngineTestRule testRule;

        public MultiTenancyRepositoryServiceTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            testRule = new ProcessEngineTestRule(engineRule);
            //ruleChain = RuleChain.outerRule(engineRule).around(testRule);
        }

        [SetUp]
        public virtual void init()
        {
            processEngineConfiguration = engineRule.ProcessEngineConfiguration;
            repositoryService = engineRule.RepositoryService;
        }

        [Test]
        public virtual void deploymentWithoutTenantId()
        {
            createDeploymentBuilder()
                .Deploy();

            var deployment = repositoryService.CreateDeploymentQuery()
                .First();

            Assert.That(deployment != null);
            Assert.That(deployment.TenantId, Is.EqualTo(null));
        }


        [Test]
        public virtual void deploymentWithTenantId()
        {
            createDeploymentBuilder()
                .TenantId(TENANT_ONE)
                .Deploy();

            var deployment = repositoryService.CreateDeploymentQuery()
                .First();

            Assert.That(deployment != null);
            Assert.That(deployment.TenantId, Is.EqualTo(TENANT_ONE));
        }


        [Test]
        public virtual void processDefinitionVersionWithTenantId()
        {
            createDeploymentBuilder()
                .TenantId(TENANT_ONE)
                .Deploy();

            createDeploymentBuilder()
                .TenantId(TENANT_ONE)
                .Deploy();

            createDeploymentBuilder()
                .TenantId(TENANT_TWO)
                .Deploy();

            IList<IProcessDefinition> processDefinitions = repositoryService.CreateProcessDefinitionQuery()
                /*.OrderByTenantId()*/
                /*.Asc()*/
                //.OrderByProcessDefinitionVersion()
                /*.Asc()*/
                .ToList();

            Assert.That(processDefinitions.Count, Is.EqualTo(3));
            // process definition was deployed twice for tenant one
            Assert.That(processDefinitions[0].Version, Is.EqualTo(1));
            Assert.That(processDefinitions[1].Version, Is.EqualTo(2));
            // process definition version of tenant two have to be independent from tenant one
            Assert.That(processDefinitions[2].Version, Is.EqualTo(1));
        }

        [Test]
        public virtual void deploymentWithDuplicateFilteringForSameTenant()
        {
            // given: a deployment with tenant ID
            createDeploymentBuilder()
                .EnableDuplicateFiltering(false)
                .Name("twice")
                .TenantId(TENANT_ONE)
                .Deploy();

            // if the same process is deployed with the same tenant ID again
            createDeploymentBuilder()
                .EnableDuplicateFiltering(false)
                .Name("twice")
                .TenantId(TENANT_ONE)
                .Deploy();

            // then it does not create a new deployment
            Assert.That(repositoryService.CreateDeploymentQuery()
                .Count(), Is.EqualTo(1L));
        }


        [Test]
        public virtual void deploymentWithDuplicateFilteringForDifferentTenants()
        {
            // given: a deployment with tenant ID
            createDeploymentBuilder()
                .EnableDuplicateFiltering(false)
                .Name("twice")
                .TenantId(TENANT_ONE)
                .Deploy();

            // if the same process is deployed with the another tenant ID
            createDeploymentBuilder()
                .EnableDuplicateFiltering(false)
                .Name("twice")
                .TenantId(TENANT_TWO)
                .Deploy();

            // then a new deployment is created
            Assert.That(repositoryService.CreateDeploymentQuery()
                .Count(), Is.EqualTo(2L));
        }


        [Test]
        public virtual void deploymentWithDuplicateFilteringIgnoreDeploymentForNoTenant()
        {
            // given: a deployment without tenant ID
            createDeploymentBuilder()
                .EnableDuplicateFiltering(false)
                .Name("twice")
                .Deploy();

            // if the same process is deployed with tenant ID
            createDeploymentBuilder()
                .EnableDuplicateFiltering(false)
                .Name("twice")
                .TenantId(TENANT_ONE)
                .Deploy();

            // then a new deployment is created
            Assert.That(repositoryService.CreateDeploymentQuery()
                .Count(), Is.EqualTo(2L));
        }


        [Test]
        public virtual void deploymentWithDuplicateFilteringIgnoreDeploymentForTenant()
        {
            // given: a deployment with tenant ID
            createDeploymentBuilder()
                .EnableDuplicateFiltering(false)
                .Name("twice")
                .TenantId(TENANT_ONE)
                .Deploy();

            // if the same process is deployed without tenant ID
            createDeploymentBuilder()
                .EnableDuplicateFiltering(false)
                .Name("twice")
                .Deploy();

            // then a new deployment is created
            Assert.That(repositoryService.CreateDeploymentQuery()
                .Count(), Is.EqualTo(2L));
        }


        [Test]
        public virtual void getPreviousProcessDefinitionWithTenantId()
        {
            testRule.DeployForTenant(TENANT_ONE, emptyProcess);
            testRule.DeployForTenant(TENANT_ONE, emptyProcess);
            testRule.DeployForTenant(TENANT_ONE, emptyProcess);

            testRule.DeployForTenant(TENANT_TWO, emptyProcess);
            testRule.DeployForTenant(TENANT_TWO, emptyProcess);

            IList<IProcessDefinition> latestProcessDefinitions = repositoryService.CreateProcessDefinitionQuery()
                /*.LatestVersion()*/
                /*.OrderByTenantId()*/
                /*.Asc()*/
                .ToList();

            var previousDefinitionTenantOne =
                getPreviousDefinition((ProcessDefinitionEntity) latestProcessDefinitions[0]);
            var previousDefinitionTenantTwo =
                getPreviousDefinition((ProcessDefinitionEntity) latestProcessDefinitions[1]);

            Assert.That(previousDefinitionTenantOne.Version, Is.EqualTo(2));
            Assert.That(previousDefinitionTenantOne.TenantId, Is.EqualTo(TENANT_ONE));

            Assert.That(previousDefinitionTenantTwo.Version, Is.EqualTo(1));
            Assert.That(previousDefinitionTenantTwo.TenantId, Is.EqualTo(TENANT_TWO));
        }


        [Test]
        public virtual void getPreviousCaseDefinitionWithTenantId()
        {
            testRule.DeployForTenant(TENANT_ONE, CMMN);
            testRule.DeployForTenant(TENANT_ONE, CMMN);
            testRule.DeployForTenant(TENANT_ONE, CMMN);

            testRule.DeployForTenant(TENANT_TWO, CMMN);
            testRule.DeployForTenant(TENANT_TWO, CMMN);

            IList<ICaseDefinition> latestCaseDefinitions = repositoryService.CreateCaseDefinitionQuery()
                ///*.LatestVersion()*/
                ///*.OrderByTenantId()*/
                ///*.Asc()*/
                .ToList();

            //CaseDefinitionEntity previousDefinitionTenantOne = getPreviousDefinition((ICaseDefinitionEntity) latestCaseDefinitions[0]);
            //CaseDefinitionEntity previousDefinitionTenantTwo = getPreviousDefinition((CaseDefinitionEntity) latestCaseDefinitions[1]);

            //Assert.That(previousDefinitionTenantOne.Version, Is.EqualTo(2));
            //Assert.That(previousDefinitionTenantOne.TenantId, Is.EqualTo(TENANT_ONE));

            //Assert.That(previousDefinitionTenantTwo.Version, Is.EqualTo(1));
            //Assert.That(previousDefinitionTenantTwo.TenantId, Is.EqualTo(TENANT_TWO));
        }


        [Test]
        public virtual void getPreviousDecisionDefinitionWithTenantId()
        {
            testRule.DeployForTenant(TENANT_ONE, DMN);
            testRule.DeployForTenant(TENANT_ONE, DMN);
            testRule.DeployForTenant(TENANT_ONE, DMN);

            testRule.DeployForTenant(TENANT_TWO, DMN);
            testRule.DeployForTenant(TENANT_TWO, DMN);

            IList<IDecisionDefinition> latestDefinitions = repositoryService.CreateDecisionDefinitionQuery()
                ///*.LatestVersion()*/
                ///*.OrderByTenantId()*/
                ///*.Asc()*/
                .ToList();

            var previousDefinitionTenantOne = getPreviousDefinition((DecisionDefinitionEntity) latestDefinitions[0]);
            var previousDefinitionTenantTwo = getPreviousDefinition((DecisionDefinitionEntity) latestDefinitions[1]);

            Assert.That(previousDefinitionTenantOne.Version, Is.EqualTo(2));
            Assert.That(previousDefinitionTenantOne.TenantId, Is.EqualTo(TENANT_ONE));

            Assert.That(previousDefinitionTenantTwo.Version, Is.EqualTo(1));
            Assert.That(previousDefinitionTenantTwo.TenantId, Is.EqualTo(TENANT_TWO));
        }


        protected internal virtual IResourceDefinitionEntity getPreviousDefinition(
                IResourceDefinitionEntity definitionEntity)
            // org.Camunda.bpm.Engine.impl.Repository.ResourceDefinitionEntity
        {
            return
                ((ProcessEngineConfigurationImpl) processEngineConfiguration).CommandExecutorTxRequired.Execute(
                    new CommandAnonymousInnerClass(this, definitionEntity));
        }

        protected internal virtual IDeploymentBuilder createDeploymentBuilder()
        {
            return repositoryService.CreateDeployment()
                .AddModelInstance("testProcess.bpmn", emptyProcess);
        }

        [TearDown]
        public virtual void tearDown()
        {
            foreach (var deployment in repositoryService.CreateDeploymentQuery()
                .ToList())
                repositoryService.DeleteDeployment(deployment.Id, true);
        }

        private class CommandAnonymousInnerClass : ICommand<IResourceDefinitionEntity>
        {
            private readonly MultiTenancyRepositoryServiceTest outerInstance;

            private readonly IResourceDefinitionEntity definitionEntity;

            public CommandAnonymousInnerClass(MultiTenancyRepositoryServiceTest outerInstance,
                IResourceDefinitionEntity definitionEntity)
            {
                this.outerInstance = outerInstance;
                this.definitionEntity = definitionEntity;
            }


            public IResourceDefinitionEntity Execute(CommandContext commandContext)
            {
                return definitionEntity.PreviousDefinition;
            }
        }
    }
}