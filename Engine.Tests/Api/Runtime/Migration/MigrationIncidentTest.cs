using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationIncidentTest
    {
        public const string FAIL_CALLED_PROC_KEY = "calledProc";

        public const string FAIL_CALL_PROC_KEY = "oneFailingServiceTaskProcess";


        public const string NEW_CALLED_PROC_KEY = "newCalledProc";

        public const string NEW_CALL_PROC_KEY = "newServiceTaskProcess";
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
        public static readonly IBpmnModelInstance FAIL_CALLED_PROC =
            ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(FAIL_CALLED_PROC_KEY)
                .StartEvent("start")
                .ServiceTask("task")
                //.CamundaAsyncBefore()
                .CamundaClass(typeof(FailingDelegate).FullName)
                .EndEvent("end")
                .Done();

        public static readonly IBpmnModelInstance FAIL_CALL_ACT_JOB_PROC =
            ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(FAIL_CALL_PROC_KEY)
                .StartEvent("start")
                .CallActivity("calling")
                .CalledElement(FAIL_CALLED_PROC_KEY)
                .EndEvent("end")
                .Done();

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
        public static readonly IBpmnModelInstance NEW_CALLED_PROC =
            ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(NEW_CALLED_PROC_KEY)
                .StartEvent("start")
                .ServiceTask("taskV2")
                //.CamundaAsyncBefore()
                .CamundaClass(typeof(NewDelegate).FullName)
                .EndEvent("end")
                .Done();

        public static readonly IBpmnModelInstance NEW_CALL_ACT_PROC =
            ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(NEW_CALL_PROC_KEY)
                .StartEvent("start")
                .CallActivity("callingV2")
                .CalledElement(NEW_CALLED_PROC_KEY)
                .EndEvent("end")
                .Done();

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain chain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testHelper);
        //public RuleChain chain;


        public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        private readonly bool InstanceFieldsInitialized;
        public ProcessEngineTestRule testHelper;

        public MigrationIncidentTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            testHelper = new ProcessEngineTestRule(engineRule);
            //chain = RuleChain.outerRule(engineRule).around(testHelper);
        }

        [Test][Deployment(new string[] {"resources/api/runtime/migration/calledProcess.bpmn", "resources/api/runtime/migration/callingProcess.bpmn", "resources/api/runtime/migration/callingProcess_v2.bpmn"}) ]
        public virtual void testCallActivityExternalTaskIncidentMigration()
        {
            // Given we create a new process instance
            var callingProcess = engineRule.RepositoryService.CreateProcessDefinitionQuery(c=>c.Key =="callingProcess")
                .First();
            var processInstance = engineRule.RuntimeService.StartProcessInstanceById(callingProcess.Id);

            //var task = engineRule.ExternalTaskService.FetchAndLock(1, "foo")
            //    //.Topic("foo", 1000L)
            //    .Execute()[0];
            // creating an incident in the called and calling process
            //engineRule.ExternalTaskService.HandleFailure(task.Id, "foo", "error", 0, 1000L);

            var incidentInCallingProcess = engineRule.RuntimeService.CreateIncidentQuery(c=>c.ProcessDefinitionId==callingProcess.Id)
                .First();

            // when
            var callingProcessV2 = engineRule.RepositoryService.CreateProcessDefinitionQuery(c=>c.Key =="callingProcessV2")
                .First();

            var migrationPlan = engineRule.RuntimeService.CreateMigrationPlan(callingProcess.Id, callingProcessV2.Id)
                .MapEqualActivities()
                .MapActivities("CallActivity", "CallActivityV2")
                .Build();

            engineRule.RuntimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstance.Id)
                .Execute();

            // then
            var incidentAfterMigration = engineRule.RuntimeService.CreateIncidentQuery(c=> c.Id==incidentInCallingProcess.Id)
                .First();
            Assert.AreEqual(callingProcessV2.Id, incidentAfterMigration.ProcessDefinitionId);
            Assert.AreEqual("CallActivityV2", incidentAfterMigration.ActivityId);
        }


        [Test][Deployment(new string[] {"resources/api/runtime/migration/calledProcess.bpmn", "resources/api/runtime/migration/calledProcess_v2.bpmn"})]
        public virtual void testExternalTaskIncidentMigration()
        {
            // Given we create a new process instance
            var callingProcess = engineRule.RepositoryService.CreateProcessDefinitionQuery(c=>c.Key =="calledProcess")
                .First();
            var processInstance = engineRule.RuntimeService.StartProcessInstanceById(callingProcess.Id);

            //var task = engineRule.ExternalTaskService.FetchAndLock(1, "foo")
            //    .Topic("foo", 1000L)
            //    .Execute()[0];
            //// creating an incident in the called and calling process
            //engineRule.ExternalTaskService.HandleFailure(task.Id, "foo", "error", 0, 1000L);

            var incidentInCallingProcess = engineRule.RuntimeService.CreateIncidentQuery(c=>c.ProcessDefinitionId==callingProcess.Id)
                .First();

            // when
            var callingProcessV2 = engineRule.RepositoryService.CreateProcessDefinitionQuery(c=>c.Key =="calledProcessV2")
                .First();

            var migrationPlan = engineRule.RuntimeService.CreateMigrationPlan(callingProcess.Id, callingProcessV2.Id)
                .MapEqualActivities()
                .MapActivities("ServiceTask_1p58ywb", "ServiceTask_V2")
                .Build();

            engineRule.RuntimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstance.Id)
                .Execute();

            // then
            var incidentAfterMigration = engineRule.RuntimeService.CreateIncidentQuery(c=> c.Id==incidentInCallingProcess.Id)
                .First();
            Assert.AreEqual(callingProcessV2.Id, incidentAfterMigration.ProcessDefinitionId);
            Assert.AreEqual("ServiceTask_V2", incidentAfterMigration.ActivityId);
        }


        [Test]
        public virtual void testCallActivityJobIncidentMigration()
        {
            // Given we deploy process definitions
            testHelper.Deploy(FAIL_CALLED_PROC, FAIL_CALL_ACT_JOB_PROC, NEW_CALLED_PROC, NEW_CALL_ACT_PROC);

            var failingProcess = engineRule.RepositoryService.CreateProcessDefinitionQuery(c=>c.Key ==FAIL_CALL_PROC_KEY)
                .First();

            var newProcess = engineRule.RepositoryService.CreateProcessDefinitionQuery(c=>c.Key ==NEW_CALL_PROC_KEY)
                .First();

            //create process instance and execute job which fails
            var processInstance = engineRule.RuntimeService.StartProcessInstanceByKey(FAIL_CALL_PROC_KEY);
            testHelper.ExecuteAvailableJobs();

            var incidentInCallingProcess = engineRule.RuntimeService.CreateIncidentQuery(c=>c.ProcessDefinitionId==failingProcess.Id)
                .First();

            // when
            var migrationPlan = engineRule.RuntimeService.CreateMigrationPlan(failingProcess.Id, newProcess.Id)
                .MapEqualActivities()
                .MapActivities("calling", "callingV2")
                .Build();

            engineRule.RuntimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstance.Id)
                .Execute();

            // then
            var incidentAfterMigration = engineRule.RuntimeService.CreateIncidentQuery(c=> c.Id==incidentInCallingProcess.Id)
                .First();
            Assert.AreEqual(newProcess.Id, incidentAfterMigration.ProcessDefinitionId);
            Assert.AreEqual("callingV2", incidentAfterMigration.ActivityId);
        }


        [Test]
        public virtual void testJobIncidentMigration()
        {
            // Given we deploy process definitions
            testHelper.Deploy(FAIL_CALLED_PROC, NEW_CALLED_PROC);

            var failingProcess = engineRule.RepositoryService.CreateProcessDefinitionQuery(c=>c.Key ==FAIL_CALLED_PROC_KEY)
                .First();

            var newProcess = engineRule.RepositoryService.CreateProcessDefinitionQuery(c=>c.Key ==NEW_CALLED_PROC_KEY)
                .First();

            //create process instance and execute job which fails
            var processInstance = engineRule.RuntimeService.StartProcessInstanceByKey(FAIL_CALLED_PROC_KEY);
            testHelper.ExecuteAvailableJobs();

            var incidentInCallingProcess = engineRule.RuntimeService.CreateIncidentQuery(c=>c.ProcessDefinitionId==failingProcess.Id)
                .First();

            // when
            var migrationPlan = engineRule.RuntimeService.CreateMigrationPlan(failingProcess.Id, newProcess.Id)
                .MapEqualActivities()
                .MapActivities("task", "taskV2")
                .Build();

            engineRule.RuntimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstance.Id)
                .Execute();

            // then
            var incidentAfterMigration = engineRule.RuntimeService.CreateIncidentQuery(c=> c.Id==incidentInCallingProcess.Id)
                .First();
            Assert.AreEqual(newProcess.Id, incidentAfterMigration.ProcessDefinitionId);
            Assert.AreEqual("taskV2", incidentAfterMigration.ActivityId);
        }


        public class NewDelegate : IJavaDelegate
        {
            public void Execute(IBaseDelegateExecution execution)
            {
            }
        }
    }
}