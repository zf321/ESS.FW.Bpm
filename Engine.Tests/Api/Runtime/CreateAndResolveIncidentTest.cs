using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Incident;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    [TestFixture]
    public class CreateAndResolveIncidentTest:ProcessEngineTestRule
    {
        private bool InstanceFieldsInitialized = false;

        public CreateAndResolveIncidentTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            engineRule = new ProvidedProcessEngineRule(processEngineBootstrapRule);
            //ruleChain = RuleChain.OuterRule(processEngineBootstrapRule).Around(engineRule).Around(testRule);
        }


        protected internal ProcessEngineBootstrapRule processEngineBootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

        private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
        {
            public ProcessEngineBootstrapRuleAnonymousInnerClass()
            {
            }

            public override ProcessEngineConfiguration ConfigureEngine(ProcessEngineConfigurationImpl configuration)
            {
                configuration.CustomIncidentHandlers = new[] {((IIncidentHandler)new CustomIncidentHandler())};
                return configuration;
            }
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
        //public ExpectedException thrown = ExpectedException.None();

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(processEngineBootstrapRule).around(engineRule).around(testRule);
        //public RuleChain ruleChain;

        protected internal IRuntimeService runtimeService;
        protected internal IHistoryService historyService;
        [SetUp]
        public virtual void Init()
        {
            runtimeService = engineRule.RuntimeService;
            historyService = engineRule.HistoryService;
        }
        
        [Test]
        public virtual void CreateIncident()
        {
            // given
            Deploy(ProcessModels.TwoTasksProcess);
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("Process");

            // when
            IIncident incident = runtimeService.CreateIncident("foo", processInstance.Id, "aa", "bar");

            // then
            IIncident incident2 = runtimeService.CreateIncidentQuery(c=>c.ExecutionId == processInstance.Id).FirstOrDefault();
            Assert.Equals(incident2.Id, incident.Id);
            Assert.Equals("foo", incident2.IncidentType);
            Assert.Equals("aa", incident2.Configuration);
            Assert.Equals("bar", incident2.IncidentMessage);
            Assert.Equals(processInstance.Id, incident2.ExecutionId);
        }

        [Test]
        public virtual void CreateIncidentWithNullExecution()
        {

            try
            {
                runtimeService.CreateIncident("foo", null, "userTask1", "bar");
                Assert.Fail("exception expected");
            }
            catch (BadUserRequestException e)
            {
                Assert.That(e.Message, Does.Contain("Execution id cannot be null"));
            }
        }

        [Test]
        public virtual void CreateIncidentWithNullIncidentType()
        {
            try
            {
                runtimeService.CreateIncident(null, "processInstanceId", "foo", "bar");
                Assert.Fail("Exception expected");
            }
            catch (BadUserRequestException e)
            {
                Assert.That(e.Message, Does.Contain("incidentType is null"));
            }
        }

        [Test]
        public virtual void CreateIncidentWithNonExistingExecution()
        {

            try
            {
                runtimeService.CreateIncident("foo", "aaa", "bbb", "bar");
                Assert.Fail("exception expected");
            }
            catch (BadUserRequestException e)
            {
                Assert.That(e.Message, Does.Contain("Cannot find an execution with executionId 'aaa'"));
            }
        }

        [Test]
        public virtual void ResolveIncident()
        {
            // given
            Deploy(ProcessModels.TwoTasksProcess);
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("Process");
            IIncident incident = runtimeService.CreateIncident("foo", processInstance.Id, "userTask1", "bar");

            // when
            runtimeService.ResolveIncident(incident.Id);

            // then
            IIncident incident2 = runtimeService.CreateIncidentQuery(c=>c.ExecutionId ==processInstance.Id).FirstOrDefault();
            Assert.Null(incident2);
        }

        [Test]
        public virtual void ResolveUnexistingIncident()
        {
            try
            {
                runtimeService.ResolveIncident("foo");
                Assert.Fail("Exception expected");
            }
            catch (NotFoundException e)
            {
                Assert.That(e.Message, Does.Contain("Cannot find an incident with id 'foo'"));
            }
        }

        [Test]
        public virtual void ResolveNullIncident()
        {
            try
            {
                runtimeService.ResolveIncident(null);
                Assert.Fail("Exception expected");
            }
            catch (BadUserRequestException e)
            {
                Assert.That(e.Message, Does.Contain("incidentId is null"));
            }
        }

        [Test]
        public virtual void ResolveIncidentOfTypeFailedJob()
        {
            // given
            Deploy("resources/api/mgmt/IncidentTest.testShouldCreateOneIncident.bpmn");
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("failingProcess");

            // when
            IList<IJob> jobs = engineRule.ManagementService.CreateJobQuery().ToList()/*.WithRetriesLeft().List()*/;

            foreach (IJob job in jobs)
            {
                engineRule.ManagementService.SetJobRetries(job.Id, 1);
                try
                {
                    engineRule.ManagementService.ExecuteJob(job.Id);
                }
                catch (System.Exception ex)
                {
                }
            }

            // then
            IIncident incident = runtimeService.CreateIncidentQuery(c=>c.ProcessDefinitionId ==processInstance.Id).FirstOrDefault();
            try
            {
                runtimeService.ResolveIncident(incident.Id);
                Assert.Fail("Exception expected");
            }
            catch (BadUserRequestException e)
            {
                Assert.That(e.Message, Does.Contain("Cannot resolve an incident of type failedJob"));
            }
        }

        [Test]
        public virtual void CreateIncidentWithIncidentHandler()
        {
            // given
            Deploy(ProcessModels.TwoTasksProcess);
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("Process");

            // when
            IIncident incident = runtimeService.CreateIncident("custom", processInstance.Id, "configuration");

            // then
            Assert.NotNull(incident);

            IIncident incident2 = runtimeService.CreateIncidentQuery().FirstOrDefault();
            Assert.NotNull(incident2);
            Assert.Equals(incident, incident2);
            Assert.Equals("custom", incident.IncidentType);
            Assert.Equals("configuration", incident.Configuration);
        }

        [Test]
        public virtual void ResolveIncidentWithIncidentHandler()
        {
            // given
            Deploy(ProcessModels.TwoTasksProcess);
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("Process");
            runtimeService.CreateIncident("custom", processInstance.Id, "configuration");
            IIncident incident = runtimeService.CreateIncidentQuery().FirstOrDefault();

            // when
            runtimeService.ResolveIncident(incident.Id);

            // then
            incident = runtimeService.CreateIncidentQuery().First();
            Assert.Null(incident);
        }

        public class CustomIncidentHandler : IIncidentHandler
        {

            internal string incidentType = "custom";

            public string IncidentHandlerType
            {
                get
                {
                    return incidentType;
                }
            }

            public  void HandleIncident(IncidentContext context, string message)
            {
                IncidentEntity.CreateAndInsertIncident(incidentType, context, message);
            }

            public  void ResolveIncident(IncidentContext context)
            {
                DeleteIncident(context);
            }

            public  void DeleteIncident(IncidentContext context)
            {
                IList<IIncident> incidents = ESS.FW.Bpm.Engine.context.Impl.Context.CommandContext.IncidentManager.FindIncidentByConfigurationAndIncidentType(context.Configuration, incidentType);
                ((IncidentEntity)incidents[0]).Delete();
            }

        }
    }
}
