using System;
using System.Linq;
using Engine.Tests.Api.Runtime.Migration;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Api.Runtime.Util;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Cfg.Multitenancy;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    /// <summary>
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class RestartProcessInstanceSyncTest
    {
        [SetUp]
        public virtual void init()
        {
            runtimeService = engineRule.RuntimeService;
            taskService = engineRule.TaskService;
            defaultTenantIdProvider = engineRule.ProcessEngineConfiguration.TenantIdProvider;
        }

        [TearDown]
        public virtual void reset()
        {
            engineRule.ProcessEngineConfiguration.SetTenantIdProvider(defaultTenantIdProvider);
        }

        protected internal ITenantIdProvider defaultTenantIdProvider;


        protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        private readonly bool InstanceFieldsInitialized;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.ExpectedException thrown = org.junit.Rules.ExpectedException.None();
        //public ExpectedException thrown = ExpectedException.None();

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testRule);
        //public RuleChain ruleChain;

        protected internal IRuntimeService runtimeService;
        protected internal ITaskService taskService;
        protected internal ProcessEngineTestRule testRule;

        public RestartProcessInstanceSyncTest()
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

        [Test]
        public virtual void shouldRestartSimpleProcessInstance()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var processInstance = runtimeService.StartProcessInstanceByKey("Process");

            var task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== processInstance.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();
            // process instance was deleted
            runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            // when
            runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask")
                .SetProcessInstanceIds(processInstance.Id)
                .Execute();

            // then
            var restartedProcessInstance = runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();
            var restartedTask = engineRule.TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== restartedProcessInstance.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();
            Assert.AreEqual(task.TaskDefinitionKey, restartedTask.TaskDefinitionKey);
        }

        [Test]
        public virtual void shouldRestartProcessInstanceWithTwoTasks()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);
            var processInstance = runtimeService.StartProcessInstanceByKey("Process");

            // the first task is completed
            var userTask1 = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== processInstance.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();
            taskService.Complete(userTask1.Id);
            var userTask2 = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== processInstance.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();
            // Delete process instance
            runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            // when
            runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask2")
                .SetProcessInstanceIds(processInstance.Id)
                .Execute();

            // then
            var restartedProcessInstance = runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();
            var restartedTask = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== restartedProcessInstance.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();
            Assert.AreEqual(userTask2.TaskDefinitionKey, restartedTask.TaskDefinitionKey);

            var updatedTree = runtimeService.GetActivityInstance(restartedProcessInstance.Id);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(restartedProcessInstance.Id, updatedTree.ProcessInstanceId);
            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processDefinition.Id)
                    .Activity("userTask2")
                    .Done());
        }

        [Test]
        public virtual void shouldRestartProcessInstanceWithParallelGateway()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);
            var processInstance = runtimeService.StartProcessInstanceByKey("Process");
            runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            // when
            runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask1")
                .StartBeforeActivity("userTask2")
                .SetProcessInstanceIds(processInstance.Id)
                .Execute();

            // then
            var restartedProcessInstance = runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();
            var updatedTree = runtimeService.GetActivityInstance(restartedProcessInstance.Id);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(restartedProcessInstance.Id, updatedTree.ProcessInstanceId);
            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processDefinition.Id)
                    .Activity("userTask1")
                    .Activity("userTask2")
                    .Done());
        }

        [Test]
        public virtual void shouldRestartProcessInstanceWithSubProcess()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.SubprocessProcess);
            var processInstance = runtimeService.StartProcessInstanceByKey("Process");
            runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            // when
            runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("subProcess")
                .SetProcessInstanceIds(processInstance.Id)
                .Execute();

            // then
            var restartedProcessInstance = runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();
            var updatedTree = runtimeService.GetActivityInstance(restartedProcessInstance.Id);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(restartedProcessInstance.Id, updatedTree.ProcessInstanceId);
            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processDefinition.Id)
                    .BeginScope("subProcess")
                    .Activity("userTask")
                    .Done());
        }

        [Test]
        public virtual void shouldRestartProcessInstanceWithVariables()
        {
            // given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var instance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("Process")
                .StartEvent()
                .UserTask("userTask1")
                .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd,
                    typeof(SetVariableExecutionListenerImpl).FullName)
                .UserTask("userTask2")
                .EndEvent()
                .Done();

            var processDefinition = testRule.DeployAndGetDefinition(instance);
            var processInstance = runtimeService.StartProcessInstanceByKey("Process");
            // variable is set at the beginning
            runtimeService.SetVariable(processInstance.Id, "var", "bar");

            // variable is changed
            var task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== processInstance.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();
            taskService.Complete(task.Id);

            runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            // when
            runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask1")
                .SetProcessInstanceIds(processInstance.Id)
                .Execute();

            // then
            var restartedProcessInstance = runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();
            var variableInstance = runtimeService.CreateVariableInstanceQuery(c=> c.ProcessInstanceId== restartedProcessInstance.Id)
                .First();

            Assert.AreEqual(variableInstance.ExecutionId, restartedProcessInstance.Id);
            Assert.AreEqual("var", variableInstance.Name);
            Assert.AreEqual("foo", variableInstance.Value);
        }

        [Test]
        public virtual void shouldRestartProcessInstanceWithInitialVariables()
        {
            // given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var instance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("Process")
                .StartEvent("startEvent")
                .UserTask("userTask1")
                .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd,
                    typeof(SetVariableExecutionListenerImpl).FullName)
                .UserTask("userTask2")
                .EndEvent()
                .Done();

            var processDefinition = testRule.DeployAndGetDefinition(instance);
            // initial variable
            var processInstance = runtimeService.StartProcessInstanceByKey("Process", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("var", "bar"));

            // variable update
            var task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== processInstance.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();
            taskService.Complete(task.Id);

            // Delete process instance
            runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            // when
            runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask1")
                .InitialSetOfVariables()
                .SetProcessInstanceIds(processInstance.Id)
                .Execute();

            // then
            var restartedProcessInstance = runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();
            var variableInstance = runtimeService.CreateVariableInstanceQuery(c=> c.ProcessInstanceId== restartedProcessInstance.Id)
                .First();

            Assert.AreEqual(variableInstance.ExecutionId, restartedProcessInstance.Id);
            Assert.AreEqual("var", variableInstance.Name);
            Assert.AreEqual("bar", variableInstance.Value);
        }

        [Test]
        public virtual void shouldNotSetLocalVariables()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.SubprocessProcess);
            var processInstance = runtimeService.StartProcessInstanceByKey("Process");

            var subProcess = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == processInstance.Id&& c.ActivityId =="userTask")
                .First();
            runtimeService.SetVariableLocal(subProcess.Id, "local", "foo");
            runtimeService.SetVariable(processInstance.Id, "var", "bar");

            runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            // when
            runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask")
                .SetProcessInstanceIds(processInstance.Id)
                .Execute();

            // then
            var restartedProcessInstance = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId ==processDefinition.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();
            var variables = runtimeService.CreateVariableInstanceQuery(c=> c.ProcessInstanceId== restartedProcessInstance.Id)
                
                .ToList();
            Assert.AreEqual(1, variables.Count);
            Assert.AreEqual("var", variables[0].Name);
            Assert.AreEqual("bar", variables[0].Value);
        }

        [Test]
        public virtual void shouldNotSetInitialVersionOfLocalVariables()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.SubprocessProcess);
            var processInstance = runtimeService.StartProcessInstanceByKey("Process", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("var", "bar"));

            var subProcess = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == processInstance.Id&& c.ActivityId =="userTask")
                .First();
            runtimeService.SetVariableLocal(subProcess.Id, "local", "foo");

            runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            // when
            runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask")
                .SetProcessInstanceIds(processInstance.Id)
                .InitialSetOfVariables()
                .Execute();

            // then
            var restartedProcessInstance = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == processDefinition.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();
            var variables = runtimeService.CreateVariableInstanceQuery(c=> c.ProcessInstanceId== restartedProcessInstance.Id)
                
                .ToList();
            Assert.AreEqual(1, variables.Count);
            Assert.AreEqual("var", variables[0].Name);
            Assert.AreEqual("bar", variables[0].Value);
        }

        [Test]
        public virtual void shouldNotSetInitialVersionOfVariables()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.SubprocessProcess);
            var processInstance = runtimeService.StartProcessInstanceByKey("Process", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("var", "bar"));
            runtimeService.SetVariable(processInstance.Id, "bar", "foo");

            runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            // when
            runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask")
                .SetProcessInstanceIds(processInstance.Id)
                .InitialSetOfVariables()
                .Execute();

            // then
            var restartedProcessInstance = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == processDefinition.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();
            var variables = runtimeService.CreateVariableInstanceQuery(c=> c.ProcessInstanceId== restartedProcessInstance.Id)
                
                .ToList();
            Assert.AreEqual(1, variables.Count);
            Assert.AreEqual("var", variables[0].Name);
            Assert.AreEqual("bar", variables[0].Value);
        }

        [Test]
        public virtual void shouldRestartProcessInstanceUsingHistoricProcessInstanceQuery()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);
            var processInstance = runtimeService.StartProcessInstanceByKey("Process");
            runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            // when
            var historicProcessInstanceQuery = engineRule.HistoryService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionId==processDefinition.Id);

            runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask1")
                .SetHistoricProcessInstanceQuery(historicProcessInstanceQuery)
                .Execute();

            // then
            var restartedProcessInstance = runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();

            var updatedTree = runtimeService.GetActivityInstance(restartedProcessInstance.Id);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(restartedProcessInstance.Id, updatedTree.ProcessInstanceId);
            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processDefinition.Id)
                    .Activity("userTask1")
                    .Done());
        }

        [Test]
        public virtual void restartProcessInstanceWithNullProcessDefinitionId()
        {
            try
            {
                runtimeService.RestartProcessInstances(null)
                    .Execute();
                Assert.Fail("exception expected");
            }
            catch (BadUserRequestException e)
            {
                Assert.That(e.Message, Does.Contain("processDefinitionId is null"));
            }
        }

        [Test]
        public virtual void restartProcessInstanceWithoutProcessInstanceIds()
        {
            try
            {
                runtimeService.RestartProcessInstances("foo")
                    .StartAfterActivity("bar")
                    .Execute();
                Assert.Fail("exception expected");
            }
            catch (BadUserRequestException e)
            {
                Assert.That(e.Message, Does.Contain("processInstanceIds is empty"));
            }
        }

        [Test]
        public virtual void restartProcessInstanceWithoutInstructions()
        {
            try
            {
                runtimeService.RestartProcessInstances("foo")
                    .SetProcessInstanceIds("bar")
                    .Execute();
                Assert.Fail("exception expected");
            }
            catch (BadUserRequestException e)
            {
                Assert.That(e.Message, Does.Contain("Restart instructions cannot be empty"));
            }
        }

        [Test]
        public virtual void restartProcessInstanceWithNullProcessInstanceId()
        {
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            try
            {
                runtimeService.RestartProcessInstances(processDefinition.Id)
                    .StartAfterActivity("bar")
                    .SetProcessInstanceIds((string) null)
                    .Execute();
                Assert.Fail("exception expected");
            }
            catch (BadUserRequestException e)
            {
                Assert.That(e.Message, Does.Contain("Process instance ids cannot be null"));
            }
        }

        [Test]
        public virtual void restartNotExistingProcessInstance()
        {
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            try
            {
                runtimeService.RestartProcessInstances(processDefinition.Id)
                    .StartBeforeActivity("bar")
                    .SetProcessInstanceIds("aaa")
                    .Execute();
                Assert.Fail("exception expected");
            }
            catch (BadUserRequestException e)
            {
                Assert.That(e.Message, Does.Contain("Historic process instance cannot be found"));
            }
        }

        [Test]
        public virtual void restartProcessInstanceWithNotMatchingProcessDefinition()
        {
            var instance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("Process2")
                .StartEvent()
                .UserTask()
                .EndEvent()
                .Done();
            var processDefinition = testRule.DeployAndGetDefinition(instance);
            var processDefinition2 = testRule.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);
            var processInstance = runtimeService.StartProcessInstanceByKey("Process");
            runtimeService.DeleteProcessInstance(processInstance.Id, null);
            try
            {
                runtimeService.RestartProcessInstances(processDefinition.Id)
                    .StartBeforeActivity("userTask")
                    .SetProcessInstanceIds(processInstance.Id)
                    .Execute();
                Assert.Fail("exception expected");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message,
                    Does.Contain("Its process definition '" + processDefinition2.Id +
                                        "' does not match given process definition '" + processDefinition.Id + "'"));
            }
        }

        [Test]
        public virtual void shouldRestartProcessInstanceWithoutBusinessKey()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);
            var processInstance = runtimeService.StartProcessInstanceByKey("Process", "businessKey", (string) null);
            runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            // when
            runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask1")
                .SetProcessInstanceIds(processInstance.Id)
                .SetWithoutBusinessKey()
                .Execute();

            // then
            var restartedProcessInstance = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId==processDefinition.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();
            Assert.IsNull(restartedProcessInstance.BusinessKey);
        }

        [Test]
        public virtual void shouldRestartProcessInstanceWithBusinessKey()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);
            var processInstance = runtimeService.StartProcessInstanceByKey("Process", "businessKey", (string) null);
            runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            // when
            runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask1")
                .SetProcessInstanceIds(processInstance.Id)
                .Execute();

            // then
            var restartedProcessInstance = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId==processDefinition.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();
            Assert.NotNull(restartedProcessInstance.BusinessKey);
            Assert.AreEqual("businessKey", restartedProcessInstance.BusinessKey);
        }

        [Test]
        public virtual void shouldRestartProcessInstanceWithoutCaseInstanceId()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);
            var processInstance = runtimeService.StartProcessInstanceByKey("Process", null, "caseInstanceId");
            runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            // when
            runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask1")
                .SetProcessInstanceIds(processInstance.Id)
                .Execute();

            // then
            var restartedProcessInstance = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId ==processDefinition.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();
            Assert.IsNull(restartedProcessInstance.CaseInstanceId);
        }

        [Test]
        public virtual void shouldRestartProcessInstanceWithTenant()
        {
            // given
            var processDefinition = testRule.DeployForTenantAndGetDefinition("tenantId", ProcessModels.TwoTasksProcess);
            var processInstance = runtimeService.StartProcessInstanceByKey("Process");

            runtimeService.DeleteProcessInstance(processInstance.Id, "test");
            // when
            runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask1")
                .SetProcessInstanceIds(processInstance.Id)
                .Execute();

            // then
            var restartedProcessInstance = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == processDefinition.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();
            Assert.NotNull(restartedProcessInstance.TenantId);
            Assert.AreEqual(processInstance.TenantId, restartedProcessInstance.TenantId);
        }

        [Test]
        public virtual void shouldSkipCustomListeners()
        {
            // given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var processDefinition =
                testRule.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.TwoTasksProcess)
                    //.ActivityBuilder("userTask1")
                    //.CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart,
                    //    typeof(IncrementCounterListener).FullName)
                    //.Done()
                );
            var processInstance = runtimeService.StartProcessInstanceByKey("Process");

            runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            IncrementCounterListener.Counter = 0;
            // when
            runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask1")
                .SetProcessInstanceIds(processInstance.Id)
                .SetSkipCustomListeners()
                .Execute();

            // then
            Assert.AreEqual(0, IncrementCounterListener.Counter);
        }

        [Test]
        public virtual void shouldSkipIoMappings()
        {
            // given
            var processDefinition =
                testRule.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.TwoTasksProcess)
                    //.ActivityBuilder("userTask1")
                    //.CamundaInputParameter("foo", "bar")
                    //.Done()
                );
            var processInstance = runtimeService.StartProcessInstanceByKey("Process");

            runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            // when
            runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask1")
                .SetSkipIoMappings()
                .SetProcessInstanceIds(processInstance.Id)
                .Execute();

            // then
            var task1Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "userTask1")
                .First();
            Assert.NotNull(task1Execution);
            Assert.IsNull(runtimeService.GetVariable(task1Execution.Id, "foo"));
        }

        [Test]
        public virtual void shouldRetainTenantIdOfSharedProcessDefinition()
        {
            // given
            engineRule.ProcessEngineConfiguration.SetTenantIdProvider(new TestTenantIdProvider());

            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);
            Assert.AreEqual(processInstance.TenantId, TestTenantIdProvider.TENANT_ID);
            runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            // when
            runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity(ProcessModels.UserTaskId)
                .SetProcessInstanceIds(processInstance.Id)
                .Execute();

            // then
            var restartedInstance = runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode&& c.ProcessDefinitionId == processDefinition.Id)
                .First();

            Assert.NotNull(restartedInstance);
            Assert.AreEqual(restartedInstance.TenantId, TestTenantIdProvider.TENANT_ID);
        }

        [Test]
        public virtual void shouldSkipTenantIdProviderOnRestart()
        {
            // given
            engineRule.ProcessEngineConfiguration.SetTenantIdProvider(new TestTenantIdProvider());

            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);
            Assert.AreEqual(processInstance.TenantId, TestTenantIdProvider.TENANT_ID);
            runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            // set tenant id provider to Assert.Fail to verify it is not called during instantiation
            engineRule.ProcessEngineConfiguration.SetTenantIdProvider(new FailingTenantIdProvider());

            // when
            runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity(ProcessModels.UserTaskId)
                .SetProcessInstanceIds(processInstance.Id)
                .Execute();

            // then
            var restartedInstance = runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode&& c.ProcessDefinitionId == processDefinition.Id)
                .First();

            Assert.NotNull(restartedInstance);
            Assert.AreEqual(restartedInstance.TenantId, TestTenantIdProvider.TENANT_ID);
        }

        [Test]
        public virtual void shouldNotSetInitialVariablesIfThereIsNoUniqueStartActivity()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);

            var processInstance = runtimeService.CreateProcessInstanceById(processDefinition.Id)
                .StartBeforeActivity("userTask1")
                .StartBeforeActivity("userTask2")
                .SetVariable("foo", "bar")
                .Execute();

            runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            // when
            runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask1")
                .InitialSetOfVariables()
                .SetProcessInstanceIds(processInstance.Id)
                .Execute();

            // then
            var restartedProcessInstance = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId==processDefinition.Id)
                .First();
            var variables = runtimeService.CreateVariableInstanceQuery(c=> c.ProcessInstanceId== restartedProcessInstance.Id)
                
                .ToList();
            Assert.AreEqual(0, variables.Count);
        }

        [Test]
        //[ExpectedException(typeof(ProcessEngineException))]
        public virtual void shouldNotRestartActiveProcessInstance()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);
            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);

            // then
            //thrown.Expect(typeof(ProcessEngineException));

            // when
            runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask1")
                .InitialSetOfVariables()
                .SetProcessInstanceIds(processInstance.Id)
                .Execute();
        }

        public class SetVariableExecutionListenerImpl : IDelegateListener<IBaseDelegateExecution>
        {
            public void Notify(IBaseDelegateExecution execution)
            {
                execution.SetVariable("var", "foo");
            }
        }

        public class TestTenantIdProvider : FailingTenantIdProvider
        {
            internal const string TENANT_ID = "testTenantId";

            public string provideTenantIdForProcessInstance(TenantIdProviderProcessInstanceContext ctx)
            {
                return TENANT_ID;
            }
        }

        public class FailingTenantIdProvider : ITenantIdProvider
        {
            public string ProvideTenantIdForProcessInstance(TenantIdProviderProcessInstanceContext ctx)
            {
                throw new NotSupportedException();
            }

            public string ProvideTenantIdForCaseInstance(TenantIdProviderCaseInstanceContext ctx)
            {
                throw new NotSupportedException();
            }

            public string ProvideTenantIdForHistoricDecisionInstance(TenantIdProviderHistoricDecisionInstanceContext ctx)
            {
                throw new NotSupportedException();
            }
        }
    }
}