using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.instance;
using NUnit.Framework;

namespace Engine.Tests.History
{
    /// <summary>
    ///    
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryActivity)]

    [TestFixture]
    public class HistoricProcessInstanceStateTest
    {
        public const string TERMINATION = "termination";
        public const string PROCESS_ID = "process1";
        public const string REASON = "very important reason";
        private readonly bool InstanceFieldsInitialized;

        public ProcessEngineRule processEngineRule = new ProvidedProcessEngineRule();
        public ProcessEngineTestRule processEngineTestRule;

        public HistoricProcessInstanceStateTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            processEngineTestRule = new ProcessEngineTestRule(processEngineRule);
            //ruleChain = RuleChain.outerRule(processEngineTestRule).around(processEngineRule);
        }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(processEngineTestRule).around(processEngineRule);
        //public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.ExpectedException thrown = org.junit.Rules.ExpectedException.None();
        //public ExpectedException thrown = ExpectedException.None();

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTerminatedInternalWithGateway()
        public virtual void testTerminatedInternalWithGateway()
        {
            var instance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_ID)
                .StartEvent()
                .ParallelGateway()
                .EndEvent() /*.MoveToLastGateway()*/
                .EndEvent(TERMINATION)
                .Done();
            initEndEvent(instance, TERMINATION);
            var processDefinition = processEngineTestRule.DeployAndGetDefinition(instance);
            processEngineRule.RuntimeService.StartProcessInstanceById(processDefinition.Id);
            var entity = getHistoricProcessInstanceWithAssertion(processDefinition);
            Assert.That(entity.State, Is.EqualTo(HistoricProcessInstanceFields.StateCompleted));
        }

        [Test]
        public virtual void testCompletedOnEndEvent()
        {
            var instance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_ID)
                .StartEvent()
                .EndEvent()
                .Done();
            var processDefinition = processEngineTestRule.DeployAndGetDefinition(instance);
            processEngineRule.RuntimeService.StartProcessInstanceById(processDefinition.Id);
            var entity = getHistoricProcessInstanceWithAssertion(processDefinition);
            Assert.That(entity.State, Is.EqualTo(HistoricProcessInstanceFields.StateCompleted));
        }

        [Test]
        public virtual void testCompletionWithSuspension()
        {
            var instance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_ID)
                .StartEvent()
                .UserTask()
                .EndEvent()
                .Done();
            var processDefinition = processEngineTestRule.DeployAndGetDefinition(instance);
            var processInstance = processEngineRule.RuntimeService.StartProcessInstanceById(processDefinition.Id);
            var entity = getHistoricProcessInstanceWithAssertion(processDefinition);
            Assert.That(entity.State, Is.EqualTo(HistoricProcessInstanceFields.StateActive));

            //suspend
            processEngineRule.RuntimeService.UpdateProcessInstanceSuspensionState()
                .ByProcessInstanceId(processInstance.Id)
                .Suspend();

            entity = getHistoricProcessInstanceWithAssertion(processDefinition);
            Assert.That(entity.State, Is.EqualTo(HistoricProcessInstanceFields.StateSuspended));

            //activate
            processEngineRule.RuntimeService.UpdateProcessInstanceSuspensionState()
                .ByProcessInstanceId(processInstance.Id)
                .Activate();

            entity = getHistoricProcessInstanceWithAssertion(processDefinition);
            Assert.That(entity.State, Is.EqualTo(HistoricProcessInstanceFields.StateActive));

            //complete task
            processEngineRule.TaskService.Complete(processEngineRule.TaskService.CreateTaskQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First()
                .Id);

            //make sure happy path ended
            entity = getHistoricProcessInstanceWithAssertion(processDefinition);
            Assert.That(entity.State, Is.EqualTo(HistoricProcessInstanceFields.StateCompleted));
        }

        [Test]
        public virtual void testSuspensionByProcessDefinition()
        {
            var instance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_ID)
                .StartEvent()
                .UserTask()
                .EndEvent()
                .Done();
            var processDefinition = processEngineTestRule.DeployAndGetDefinition(instance);
            var processInstance1 = processEngineRule.RuntimeService.StartProcessInstanceById(processDefinition.Id);

            var processInstance2 = processEngineRule.RuntimeService.StartProcessInstanceById(processDefinition.Id);

            //suspend all
            processEngineRule.RuntimeService.UpdateProcessInstanceSuspensionState()
                .ByProcessDefinitionId(processDefinition.Id)
                .Suspend();

            var hpi1 = processEngineRule.HistoryService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId== processInstance1.Id)
                .First();

            var hpi2 = processEngineRule.HistoryService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId== processInstance2.Id)
                .First();

            Assert.That(hpi1.State, Is.EqualTo(HistoricProcessInstanceFields.StateSuspended));
            Assert.That(hpi2.State, Is.EqualTo(HistoricProcessInstanceFields.StateSuspended));

            //activate all
            processEngineRule.RuntimeService.UpdateProcessInstanceSuspensionState()
                .ByProcessDefinitionKey(processDefinition.Key)
                .Activate();

            hpi1 = processEngineRule.HistoryService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId== processInstance1.Id)
                .First();

            hpi2 = processEngineRule.HistoryService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId== processInstance2.Id)
                .First();

            Assert.That(hpi1.State, Is.EqualTo(HistoricProcessInstanceFields.StateActive));
            Assert.That(hpi2.State, Is.EqualTo(HistoricProcessInstanceFields.StateActive));
        }


        [Test]
        public virtual void testCancellationState()
        {
            var instance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_ID)
                .StartEvent()
                .UserTask()
                .EndEvent()
                .Done();
            var processDefinition = processEngineTestRule.DeployAndGetDefinition(instance);
            var processInstance = processEngineRule.RuntimeService.StartProcessInstanceById(processDefinition.Id);
            var entity = getHistoricProcessInstanceWithAssertion(processDefinition);
            Assert.That(entity.State, Is.EqualTo(HistoricProcessInstanceFields.StateActive));

            //same call as in ProcessInstanceResourceImpl
            processEngineRule.RuntimeService.DeleteProcessInstance(processInstance.Id, REASON, false, true);
            entity = getHistoricProcessInstanceWithAssertion(processDefinition);
            Assert.That(entity.State, Is.EqualTo(HistoricProcessInstanceFields.StateExternallyTerminated));
        }

        [Test]
        public virtual void testSateOfScriptTaskProcessWithTransactionCommitAndException()
        {
            var instance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_ID)
                .StartEvent();
                instance.CamundaAsyncAfter()
                .ScriptTask()
                .ScriptText("throw new RuntimeException()")
                .ScriptFormat("groovy");
           var r= instance
                .EndEvent()
                .Done();
            //add wait state
            var processDefinition = processEngineTestRule.DeployAndGetDefinition(r);


            try
            {
                var pi = processEngineRule.RuntimeService.StartProcessInstanceById(processDefinition.Id);
                processEngineRule.ManagementService.ExecuteJob(processEngineRule.ManagementService.CreateJobQuery()
                    /*.Executable()*/
                    .First()
                    .Id);
                Assert.Fail("exception expected");
            }
            catch (System.Exception)
            {
                //expected
            }

            Assert.That(processEngineRule.RuntimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                
                .Count, Is.EqualTo(1));
            var entity = getHistoricProcessInstanceWithAssertion(processDefinition);
            Assert.That(entity.State, Is.EqualTo(HistoricProcessInstanceFields.StateActive));
        }

        [Test]
        public virtual void testErrorEndEvent()
        {
            var process1 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_ID)
                .StartEvent();

            process1.EndEvent();
            process1
                .Error("1");
                var r=process1.Done();

            var processDefinition = processEngineTestRule.DeployAndGetDefinition(r);
            processEngineRule.RuntimeService.StartProcessInstanceById(processDefinition.Id);
            var entity = getHistoricProcessInstanceWithAssertion(processDefinition);
            Assert.That(entity.State, Is.EqualTo(HistoricProcessInstanceFields.StateCompleted));
        }

        [Test][Deployment( new [] {"resources/history/HistoricProcessInstanceStateTest.TestWithCallActivity.bpmn"})]
        public virtual void testWithCallActivity()
        {
            processEngineRule.RuntimeService.StartProcessInstanceByKey("Main_Process");
            Assert.That(processEngineRule.RuntimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                
                .Count, Is.EqualTo(0));

            var entity1 = processEngineRule.HistoryService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionKey=="Main_Process")
                .First();

            var entity2 = processEngineRule.HistoryService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionKey=="Sub_Process")
                .First();

            Assert.That(entity1, Is.Not.Null);
            Assert.That(entity2, Is.Not.Null);
            Assert.That(entity1.State, Is.EqualTo(HistoricProcessInstanceFields.StateCompleted));
            Assert.That(entity2.State, Is.EqualTo(HistoricProcessInstanceFields.StateInternallyTerminated));
        }

        private IHistoricProcessInstance getHistoricProcessInstanceWithAssertion(IProcessDefinition processDefinition)
        {
            var entities = processEngineRule.HistoryService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionId==processDefinition.Id)
                
                .ToList();
            Assert.That(entities, Is.Not.Null);
            Assert.That(entities.Count, Is.EqualTo(1));
            return entities[0];
        }

        protected internal static void initEndEvent(IBpmnModelInstance modelInstance, string endEventId)
        {
            var endEvent = modelInstance.GetModelElementById/*<IEndEvent>*/(endEventId);
            var terminateDefinition =
                modelInstance.NewInstance<ITerminateEventDefinition>(typeof(ITerminateEventDefinition));
            endEvent.AddChildElement(terminateDefinition);
        }
    }
}