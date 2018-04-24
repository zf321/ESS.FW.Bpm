//using System.Linq;
//using ESS.FW.Bpm.Engine.Delegate;
//using ESS.FW.Bpm.Model.Bpmn;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.Bpmn.Event.Conditional
//{
//    [TestFixture]
//    public class MixedConditionalEventTest : AbstractConditionalEventTestCase
//    {
//        protected internal const string TaskAfterConditionalBoundaryEvent = "ITask after conditional boundary event";
//        protected internal const string TaskAfterConditionalStartEvent = "ITask after conditional start event";

//        protected internal const string TaskAfterCondStartEventInSubProcess =
//            "ITask after cond start event in sub process";

//        protected internal const string TaskAfterCondBounEventInSubProcess =
//            "ITask after cond bound event in sub process";

//        // Todo:ModifiableBpmnModelInstance.Modify()
//        protected internal virtual IBpmnModelInstance AddBoundaryEvent(IBpmnModelInstance modelInstance,
//            string activityId, string userTaskName, bool isInterrupting)
//        {
//            //return ModifiableBpmnModelInstance.Modify(modelInstance).ActivityBuilder(activityId).BoundaryEvent().CancelActivity(isInterrupting).ConditionalEventDefinition().Condition(CONDITION_EXPR).ConditionalEventDefinitionDone().UserTask().Name(userTaskName).EndEvent().Done();
//            return null;
//        }

//        // Todo:ModifiableBpmnModelInstance.Modify()
//        protected internal virtual IBpmnModelInstance AddEventSubProcess(IBpmnModelInstance model, string parentId,
//            string userTaskName, bool isInterrupting)
//        {
//            //return ModifiableBpmnModelInstance.Modify(model).AddSubProcessTo(parentId).TriggerByEvent()//.EmbeddedSubProcess().StartEvent().interrupting(isInterrupting).ConditionalEventDefinition().Condition(CONDITION_EXPR).ConditionalEventDefinitionDone().UserTask().Name(userTaskName).EndEvent().Done();
//            return null;
//        }

//        protected internal virtual void DeployMixedProcess(IBpmnModelInstance model, string parentId,
//            bool isInterrupting)
//        {
//            DeployMixedProcess(model, parentId, TaskWithConditionId, isInterrupting);
//        }

//        protected internal virtual void DeployMixedProcess(IBpmnModelInstance model, string parentId, string activityId,
//            bool isInterrupting)
//        {
//            var modelInstance = AddEventSubProcess(model, parentId, TaskAfterConditionalStartEvent, isInterrupting);
//            modelInstance = AddBoundaryEvent(modelInstance, activityId, TaskAfterConditionalBoundaryEvent,
//                isInterrupting);
//            Engine.ManageDeployment(RepositoryService.CreateDeployment()
//                .AddModelInstance(ConditionalModel, modelInstance)
//                .Deploy());
//        }


//        [Test]
//        [Deployment]
//        public virtual void TestCompactedExecutionTree()
//        {
//            //given process with concurrent execution and conditional events
//            RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            //when task before cancel is completed
//            TaskService.Complete(TaskService.CreateTaskQuery(c=>c.Name ==TaskBeforeCondition)
//                .First()
//                .Id);

//            //then conditional events are triggered
//            TasksAfterVariableIsSet = TaskService.CreateTaskQuery()
//                .ToList();
//            Assert.AreEqual(1, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(TaskAfterConditionalStartEvent, TasksAfterVariableIsSet[0].Name);
//        }


//        [Test]
//        [Deployment]
//        public virtual void TestCompensationWithConditionalEvents()
//        {
//            //given process with compensation and conditional events
//            var processInstance = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            var task = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== processInstance.Id)
//                .First();
//            Assert.NotNull(task);
//            Assert.AreEqual("Before Cancel", task.Name);

//            //when task before cancel is completed
//            TaskService.Complete(task.Id);

//            //then compensation is triggered -> which triggers conditional events
//            TasksAfterVariableIsSet = TaskService.CreateTaskQuery()
//                .ToList();
//            Assert.AreEqual(4, TasksAfterVariableIsSet.Count);
//        }


//        [Test]
//        public virtual void testNonInterruptingSetVariableInOutMappingOfCallActivity()
//        {
//            Engine.ManageDeployment(RepositoryService.CreateDeployment()
//                .AddModelInstance(ConditionalModel, DelegatedProcess)
//                .Deploy());

//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .SubProcess(SubProcessId)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .CallActivity(TaskWithConditionId)
//                .CalledElement(DelegatedProcessKey)
//                .CamundaOut(VariableName, VariableName)
//                .UserTask()
//                .Name(TaskAfterOutputMapping)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();

//            modelInstance = AddEventSubProcess(modelInstance, SubProcessId, TaskAfterCondStartEventInSubProcess, false);
//            modelInstance = AddBoundaryEvent(modelInstance, TaskWithConditionId, TaskAfterCondBounEventInSubProcess,
//                false);
//            DeployMixedProcess(modelInstance, ConditionalEventProcessKey, SubProcessId, false);


//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task before service task is completed
//            TaskService.Complete(task.Id);

//            //then out mapping of call activity sets a variable
//            //-> all non interrupting conditional events are triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(5, TasksAfterVariableIsSet.Count);
//            //three subscriptions: event sub process in sub process and on process instance level and boundary event of sub process
//            Assert.AreEqual(3, ConditionEventSubscriptionQuery.Count());
//        }

//        [Test]
//        public virtual void testNonInterruptingSetVariableOnEndExecutionListenerInSubProcess()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().SubProcess(SubProcessId)//.EmbeddedSubProcess().StartEvent().UserTask(TaskWithConditionId).CamundaExecutionListenerExpression(org.Camunda.bpm.Engine.Delegate.ExecutionListenerFields.EventNameEnd, ExprSetVariable).Name(TaskWithCondition).UserTask().Name(TaskAfterOutputMapping).EndEvent().SubProcessDone().EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .SubProcess(SubProcessId)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskWithConditionId)
//                .CamundaExecutionListenerExpression(ExecutionListenerFields.EventNameEnd, ExprSetVariable)
//                .Name(TaskWithCondition)
//                .UserTask()
//                .Name(TaskAfterOutputMapping)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();

//            DeployMixedProcess(modelInstance, SubProcessId, false);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);

//            //when task before is completed
//            TaskService.Complete(taskQuery.First()
//                .Id);

//            //then conditional boundary should not triggered but conditional start event
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(2, TasksAfterVariableIsSet.Count);
//            AssertTaskNames(TasksAfterVariableIsSet, TaskAfterConditionalStartEvent, TaskAfterOutputMapping);
//        }


//        [Test]
//        public virtual void testNonInterruptingSetVariableOnEndExecutionListenerInSubProcessWithBoundary()
//        {
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .SubProcess(SubProcessId)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskWithConditionId)
//                .CamundaExecutionListenerExpression(ExecutionListenerFields.EventNameEnd, ExprSetVariable)
//                .Name(TaskWithCondition)
//                .UserTask()
//                .Name(TaskAfterOutputMapping)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();

//            modelInstance = AddEventSubProcess(modelInstance, SubProcessId, TaskAfterCondStartEventInSubProcess, false);
//            DeployMixedProcess(modelInstance, ConditionalEventProcessKey, SubProcessId, false);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);

//            //when task before is completed
//            TaskService.Complete(taskQuery.First()
//                .Id);

//            //then all conditional events are triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(4, TasksAfterVariableIsSet.Count);
//        }


//        [Test]
//        public virtual void TestNonInterruptingSetVariableOnInputMapping()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).UserTask(TaskWithConditionId).CamundaInputParameter(VariableName, "1").Name(TaskWithCondition).EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .UserTask(TaskWithConditionId)
//                .CamundaInputParameter(VariableName, "1")
//                .Name(TaskWithCondition)
//                .EndEvent()
//                .Done();

//            DeployMixedProcess(modelInstance, ConditionalEventProcessKey, false);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);

//            //when task before is completed
//            TaskService.Complete(taskQuery.First()
//                .Id);

//            //then conditional boundary event should not triggered also not conditional start event
//            //since variable is only local
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskWithCondition, TasksAfterVariableIsSet[0].Name);
//        }


//        [Test]
//        public virtual void testNonInterruptingSetVariableOnInputMappingInSubProcess()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().SubProcess(SubProcessId)//.EmbeddedSubProcess().StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).UserTask(TaskWithConditionId).CamundaInputParameter(VariableName, "1").Name(TaskWithCondition).EndEvent().SubProcessDone().EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .SubProcess(SubProcessId)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .UserTask(TaskWithConditionId)
//                .CamundaInputParameter(VariableName, "1")
//                .Name(TaskWithCondition)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();

//            DeployMixedProcess(modelInstance, SubProcessId, false);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);

//            //when task before is completed
//            TaskService.Complete(taskQuery.First()
//                .Id);

//            //then conditional boundary event should not triggered also not conditional start event
//            //since variable is only local
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskWithCondition, TasksAfterVariableIsSet[0].Name);
//        }


//        [Test]
//        public virtual void testNonInterruptingSetVariableOnInputMappingInSubProcessWithBoundary()
//        {
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .SubProcess(SubProcessId)
//                .CamundaInputParameter(VariableName, "1")
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskWithConditionId)
//                .Name(TaskWithCondition)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();

//            modelInstance = AddEventSubProcess(modelInstance, SubProcessId, TaskAfterCondStartEventInSubProcess, false);
//            DeployMixedProcess(modelInstance, ConditionalEventProcessKey, SubProcessId, false);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);

//            //when input mapping sets variable

//            //then conditional boundary event should triggered and also conditional start event in sub process
//            //via the default evaluation behavior but not the global event sub process
//            //since variable is only local
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(3, TasksAfterVariableIsSet.Count);
//            AssertTaskNames(TasksAfterVariableIsSet, TaskAfterCondStartEventInSubProcess,
//                TaskAfterConditionalBoundaryEvent, TaskWithCondition);
//        }

//        [Test]
//        public virtual void TestNonInterruptingSetVariableOnOutputMapping()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskWithConditionId).CamundaOutputParameter(VariableName, "1").Name(TaskWithCondition).UserTask().Name(TaskAfterOutputMapping).EndEvent().Done();
//            var modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask(TaskWithConditionId)
//                .CamundaOutputParameter(VariableName, "1")
//                .Name(TaskWithCondition)
//                .UserTask()
//                .Name(TaskAfterOutputMapping)
//                .EndEvent()
//                .Done();

//            DeployMixedProcess(modelInstance, ConditionalEventProcessKey, false);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);

//            //when task before is completed
//            TaskService.Complete(taskQuery.First()
//                .Id);

//            //then conditional boundary should not triggered but conditional start event
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(2, TasksAfterVariableIsSet.Count);
//            AssertTaskNames(TasksAfterVariableIsSet, TaskAfterConditionalStartEvent, TaskAfterOutputMapping);
//        }


//        [Test]
//        public virtual void testNonInterruptingSetVariableOnOutputMappingInSubProcess()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().SubProcess(SubProcessId)//.EmbeddedSubProcess().StartEvent().UserTask(TaskWithConditionId).CamundaOutputParameter(VariableName, "1").Name(TaskWithCondition).UserTask().Name(TaskAfterOutputMapping).EndEvent().SubProcessDone().EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .SubProcess(SubProcessId)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskWithConditionId)
//                .CamundaOutputParameter(VariableName, "1")
//                .Name(TaskWithCondition)
//                .UserTask()
//                .Name(TaskAfterOutputMapping)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();

//            DeployMixedProcess(modelInstance, SubProcessId, false);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);

//            //when task before is completed
//            TaskService.Complete(taskQuery.First()
//                .Id);

//            //then conditional boundary should not triggered but conditional start event
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(2, TasksAfterVariableIsSet.Count);
//            AssertTaskNames(TasksAfterVariableIsSet, TaskAfterConditionalStartEvent, TaskAfterOutputMapping);
//        }

//        [Test]
//        public virtual void testNonInterruptingSetVariableOnOutputMappingInSubProcessWithBoundary()
//        {
//            var modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .SubProcess(SubProcessId) /*//.EmbeddedSubProcess().StartEvent()*/
//                .UserTask(TaskWithConditionId)
//                .CamundaOutputParameter(VariableName, "1")
//                .Name(TaskWithCondition)
//                .UserTask()
//                .Name(TaskAfterOutputMapping)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();

//            modelInstance = AddEventSubProcess(modelInstance, SubProcessId, TaskAfterCondStartEventInSubProcess, false);
//            DeployMixedProcess(modelInstance, ConditionalEventProcessKey, SubProcessId, false);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);

//            //when task before is completed
//            TaskService.Complete(taskQuery.First()
//                .Id);

//            //then all conditional events are triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(4, TasksAfterVariableIsSet.Count);
//        }


//        [Test]
//        public virtual void testNonInterruptingSetVariableOnStartExecutionListenerInSubProcess()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().SubProcess(SubProcessId)//.EmbeddedSubProcess().StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).UserTask(TaskWithConditionId).CamundaExecutionListenerExpression(org.Camunda.bpm.Engine.Delegate.ExecutionListenerFields.EventNameStart, ExprSetVariable).Name(TaskWithCondition).EndEvent().SubProcessDone().EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .SubProcess(SubProcessId)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .UserTask(TaskWithConditionId)
//                .CamundaExecutionListenerExpression(ExecutionListenerFields.EventNameStart, ExprSetVariable)
//                .Name(TaskWithCondition)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();

//            DeployMixedProcess(modelInstance, SubProcessId, false);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);

//            //when task before is completed
//            TaskService.Complete(taskQuery.First()
//                .Id);

//            //then conditional boundary should triggered via default evaluation behavior
//            //and conditional start event via delayed events
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(3, TasksAfterVariableIsSet.Count);
//            AssertTaskNames(TasksAfterVariableIsSet, TaskAfterConditionalStartEvent, TaskAfterConditionalBoundaryEvent,
//                TaskWithCondition);
//        }


//        [Test]
//        public virtual void testNonInterruptingSetVariableOnStartExecutionListenerInSubProcessWithBoundary()
//        {
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .SubProcess(SubProcessId)
//                .CamundaExecutionListenerExpression(ExecutionListenerFields.EventNameStart, ExprSetVariable)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskWithConditionId)
//                .Name(TaskWithCondition)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();

//            modelInstance = AddEventSubProcess(modelInstance, SubProcessId, TaskAfterCondStartEventInSubProcess, false);
//            DeployMixedProcess(modelInstance, ConditionalEventProcessKey, SubProcessId, false);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);

//            //when start listener sets variable

//            //then conditional boundary and event sub process inside the sub process should triggered via default evaluation behavior
//            //and global conditional start event via delayed events
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(4, TasksAfterVariableIsSet.Count);
//            AssertTaskNames(TasksAfterVariableIsSet, TaskAfterConditionalStartEvent, TaskAfterCondStartEventInSubProcess,
//                TaskAfterConditionalBoundaryEvent, TaskWithCondition);
//        }

//        [Test]
//        public virtual void testSetVariableInOutMappingOfCallActivity()
//        {
//            Engine.ManageDeployment(RepositoryService.CreateDeployment()
//                .AddModelInstance(ConditionalModel, DelegatedProcess)
//                .Deploy());

//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .SubProcess(SubProcessId)
//                /*//.EmbeddedSubProcess().StartEvent()*/
//                .CallActivity(TaskWithConditionId)
//                .CalledElement(DelegatedProcessKey)
//                .CamundaOut(VariableName, VariableName)
//                .UserTask()
//                .Name(TaskAfterOutputMapping)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();

//            modelInstance = AddEventSubProcess(modelInstance, SubProcessId, TaskAfterCondStartEventInSubProcess, true);
//            modelInstance = AddBoundaryEvent(modelInstance, TaskWithConditionId, TaskAfterCondBounEventInSubProcess,
//                true);
//            DeployMixedProcess(modelInstance, ConditionalEventProcessKey, SubProcessId, true);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then out mapping from call activity sets variable
//            //-> interrupting conditional start event on process instance level is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterConditionalStartEvent, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void testSetVariableOnEndExecutionListenerInSubProcess()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().SubProcess(SubProcessId)//.EmbeddedSubProcess().StartEvent().UserTask(TaskWithConditionId).CamundaExecutionListenerExpression(org.Camunda.bpm.Engine.Delegate.ExecutionListenerFields.EventNameEnd, ExprSetVariable).Name(TaskWithCondition).EndEvent().SubProcessDone().EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .SubProcess(SubProcessId)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskWithConditionId)
//                .CamundaExecutionListenerExpression(ExecutionListenerFields.EventNameEnd, ExprSetVariable)
//                .Name(TaskWithCondition)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();

//            DeployMixedProcess(modelInstance, SubProcessId, true);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();

//            //when task before is completed
//            TaskService.Complete(task.Id);

//            //then conditional boundary should not triggered but conditional start event
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterConditionalStartEvent, TasksAfterVariableIsSet[0].Name);
//        }


//        [Test]
//        public virtual void testSetVariableOnEndExecutionListenerInSubProcessWithBoundary()
//        {
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .SubProcess(SubProcessId)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskWithConditionId)
//                .CamundaExecutionListenerExpression(ExecutionListenerFields.EventNameEnd, ExprSetVariable)
//                .Name(TaskWithCondition)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();

//            modelInstance = AddEventSubProcess(modelInstance, SubProcessId, TaskAfterCondStartEventInSubProcess, true);
//            DeployMixedProcess(modelInstance, ConditionalEventProcessKey, SubProcessId, true);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();

//            //when task before is completed
//            TaskService.Complete(task.Id);

//            //then conditional boundary should not triggered but conditional start event
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterConditionalStartEvent, TasksAfterVariableIsSet[0].Name);
//        }

//        // io mapping ////////////////////////////////////////////////////////////////////////////////////////////////////////


//        [Test]
//        public virtual void TestSetVariableOnInputMapping()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).UserTask(TaskWithConditionId).CamundaInputParameter(VariableName, "1").Name(TaskWithCondition).EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .UserTask(TaskWithConditionId)
//                .CamundaInputParameter(VariableName, "1")
//                .Name(TaskWithCondition)
//                .EndEvent()
//                .Done();

//            DeployMixedProcess(modelInstance, ConditionalEventProcessKey, true);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();

//            //when task before is completed
//            TaskService.Complete(task.Id);

//            //then conditional boundary event should triggered with the default evaluation behavior
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterConditionalBoundaryEvent, TasksAfterVariableIsSet[0].Name);
//        }


//        [Test]
//        public virtual void testSetVariableOnInputMappingInSubProcess()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().SubProcess(SubProcessId)//.EmbeddedSubProcess().StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).UserTask(TaskWithConditionId).CamundaInputParameter(VariableName, "1").Name(TaskWithCondition).EndEvent().SubProcessDone().EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .SubProcess(SubProcessId)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .UserTask(TaskWithConditionId)
//                .CamundaInputParameter(VariableName, "1")
//                .Name(TaskWithCondition)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();

//            DeployMixedProcess(modelInstance, SubProcessId, true);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();

//            //when task before is completed
//            TaskService.Complete(task.Id);

//            //then conditional boundary event should triggered via default evaluation behavior
//            //but conditional start event should not
//            //since variable is only local
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterConditionalBoundaryEvent, TasksAfterVariableIsSet[0].Name);
//        }


//        [Test]
//        public virtual void testSetVariableOnInputMappingInSubProcessWithBoundary()
//        {
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .SubProcess(SubProcessId)
//                .CamundaInputParameter(VariableName, "1")
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskWithConditionId)
//                .Name(TaskWithCondition)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();

//            modelInstance = AddEventSubProcess(modelInstance, SubProcessId, TaskAfterCondStartEventInSubProcess, true);
//            DeployMixedProcess(modelInstance, ConditionalEventProcessKey, SubProcessId, true);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);

//            //when input mapping sets variable

//            //then conditional boundary event should triggered from the default evaluation behavior
//            // The event sub process inside the sub process should not since the scope is lower than from the boundary.
//            // The global event sub process should not since the variable is only locally.
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(1, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(TaskAfterConditionalBoundaryEvent, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void TestSetVariableOnOutputMapping()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskWithConditionId).CamundaOutputParameter(VariableName, "1").Name(TaskWithCondition).EndEvent().Done();
//            var modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask(TaskWithConditionId)
//                .CamundaOutputParameter(VariableName, "1")
//                .Name(TaskWithCondition)
//                .EndEvent()
//                .Done();

//            DeployMixedProcess(modelInstance, ConditionalEventProcessKey, true);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();

//            //when task before is completed
//            TaskService.Complete(task.Id);

//            //then conditional boundary should not triggered but conditional start event
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterConditionalStartEvent, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void testSetVariableOnOutputMappingInSubProcess()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().SubProcess(SubProcessId)//.EmbeddedSubProcess().StartEvent().UserTask(TaskWithConditionId).CamundaOutputParameter(VariableName, "1").Name(TaskWithCondition).EndEvent().SubProcessDone().EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .SubProcess(SubProcessId)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskWithConditionId)
//                .CamundaOutputParameter(VariableName, "1")
//                .Name(TaskWithCondition)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();

//            DeployMixedProcess(modelInstance, SubProcessId, true);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();

//            //when task before is completed
//            TaskService.Complete(task.Id);

//            //then conditional boundary should not triggered but conditional start event
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterConditionalStartEvent, TasksAfterVariableIsSet[0].Name);
//        }


//        [Test]
//        public virtual void testSetVariableOnOutputMappingInSubProcessWithBoundary()
//        {
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .SubProcess(SubProcessId)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskWithConditionId)
//                .CamundaOutputParameter(VariableName, "1")
//                .Name(TaskWithCondition)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();

//            modelInstance = AddEventSubProcess(modelInstance, SubProcessId, TaskAfterCondStartEventInSubProcess, true);
//            DeployMixedProcess(modelInstance, ConditionalEventProcessKey, SubProcessId, true);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();

//            //when task before is completed
//            TaskService.Complete(task.Id);

//            //then conditional boundary should not triggered but conditional start event
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterConditionalStartEvent, TasksAfterVariableIsSet[0].Name);
//        }

//        // sub process testing with event sub process and conditional start event and boundary event on IUser task
//        // execution listener in sub process //////////////////////////////////////////////////////////////////////////////////


//        [Test]
//        public virtual void testSetVariableOnStartExecutionListenerInSubProcess()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().SubProcess(SubProcessId)//.EmbeddedSubProcess().StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).UserTask(TaskWithConditionId).CamundaExecutionListenerExpression(org.Camunda.bpm.Engine.Delegate.ExecutionListenerFields.EventNameStart, ExprSetVariable).Name(TaskWithCondition).EndEvent().SubProcessDone().EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .SubProcess(SubProcessId)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .UserTask(TaskWithConditionId)
//                .CamundaExecutionListenerExpression(ExecutionListenerFields.EventNameStart, ExprSetVariable)
//                .Name(TaskWithCondition)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();

//            DeployMixedProcess(modelInstance, SubProcessId, true);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();

//            //when task before is completed
//            TaskService.Complete(task.Id);

//            //then conditional boundary should not triggered but conditional start event
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterConditionalStartEvent, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void testSetVariableOnStartExecutionListenerInSubProcessWithBoundary()
//        {
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .SubProcess(SubProcessId)
//                .CamundaExecutionListenerExpression(ExecutionListenerFields.EventNameStart, ExprSetVariable)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskWithConditionId)
//                .Name(TaskWithCondition)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();

//            modelInstance = AddEventSubProcess(modelInstance, SubProcessId, TaskAfterCondStartEventInSubProcess, true);
//            DeployMixedProcess(modelInstance, ConditionalEventProcessKey, SubProcessId, true);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);

//            //when start listener sets variable

//            //then conditional boundary should not triggered but conditional start event
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterConditionalStartEvent, TasksAfterVariableIsSet[0].Name);
//        }
//    }
//}