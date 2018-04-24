//using System.Collections.Generic;
//using System.Linq;
//using ESS.FW.Bpm.Engine.Tests.Api.Runtime.Migration;
//using ESS.FW.Bpm.Engine.Variable;
//using ESS.FW.Bpm.Model.Bpmn;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.Bpmn.Event.Conditional
//{
//    [TestFixture]
//    public class EventSubProcessStartConditionalEventTest : AbstractConditionalEventTestCase
//    {
//        protected internal virtual void deployConditionalEventSubProcess(IBpmnModelInstance model, bool isInterrupting)
//        {
//            DeployConditionalEventSubProcess(model, ConditionalEventProcessKey, isInterrupting);
//        }


//        // Todo: ConditionalEventDefinitionBuilder.ConditionalEventDefinitionDone()
//        protected internal override void DeployConditionalEventSubProcess(IBpmnModelInstance model, string parentId,
//            bool isInterrupting)
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = ModifiableBpmnModelInstance.Modify(model).AddSubProcessTo(parentId).Id("eventSubProcess").TriggerByEvent()//.EmbeddedSubProcess().StartEvent().interrupting(isInterrupting).ConditionalEventDefinition(CONDITIONAL_EVENT).Condition(CONDITION_EXPR).ConditionalEventDefinitionDone().UserTask("taskAfterCond").Name(TASK_AFTER_CONDITION).EndEvent().Done();
//            IBpmnModelInstance modelInstance = ModifiableBpmnModelInstance.Modify(model)
//                .AddSubProcessTo(parentId)
//                .Id("eventSubProcess")
//                .TriggerByEvent()
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                //.Interrupting(isInterrupting)
//                //.ConditionalEventDefinition(ConditionalEvent)
//                //.Condition(ConditionExpr)
//                //.ConditionalEventDefinitionDone()
//                //.UserTask("taskAfterCond")
//                //.Name(TaskAfterCondition)
//                //.EndEvent()
//                .Done();

//            Engine.ManageDeployment(RepositoryService.CreateDeployment()
//                .AddModelInstance(ConditionalModel, modelInstance)
//                .Deploy());
//        }

//        //private class ConditionalProcessVarSpecificationAnonymousInnerClass2 : ConditionalEventWithSpecificVariableEventTest.ConditionalProcessVarSpecification
//        //{
//        //    private readonly ConditionalEventWithSpecificVariableEventTest outerInstance;

//        //    public ConditionalProcessVarSpecificationAnonymousInnerClass2(ConditionalEventWithSpecificVariableEventTest outerInstance)
//        //    {
//        //        this.outerInstance = outerInstance;
//        //    }

//        //    public virtual IBpmnModelInstance getProcessWithVarName(bool interrupting, string condition)
//        //    {
//        //        return ModifiableBpmnModelInstance.Modify(TASK_MODEL).AddSubProcessTo(ConditionalEventProcessKey).TriggerByEvent()//.EmbeddedSubProcess().StartEvent().interrupting(interrupting).ConditionalEventDefinition(CONDITIONAL_EVENT).Condition(condition).CamundaVariableName(VARIABLE_NAME).ConditionalEventDefinitionDone().UserTask().Name(TaskAfterCondition).EndEvent().Done();
//        //    }

//        //    public virtual IBpmnModelInstance getProcessWithVarNameAndEvents(bool interrupting, string varEvent)
//        //    {
//        //        return ModifiableBpmnModelInstance.Modify(TASK_MODEL).AddSubProcessTo(ConditionalEventProcessKey).TriggerByEvent()//.EmbeddedSubProcess().StartEvent().interrupting(interrupting).ConditionalEventDefinition(CONDITIONAL_EVENT).Condition(CONDITION_EXPR).CamundaVariableName(VARIABLE_NAME).CamundaVariableEvents(varEvent).ConditionalEventDefinitionDone().UserTask().Name(TaskAfterCondition).EndEvent().Done();
//        //    }

//        //    public virtual IBpmnModelInstance getProcessWithVarEvents(bool interrupting, string varEvent)
//        //    {
//        //        return ModifiableBpmnModelInstance.Modify(TASK_MODEL).AddSubProcessTo(ConditionalEventProcessKey).TriggerByEvent()//.EmbeddedSubProcess().StartEvent().interrupting(interrupting).ConditionalEventDefinition(CONDITIONAL_EVENT).Condition(CONDITION_EXPR).CamundaVariableEvents(varEvent).ConditionalEventDefinitionDone().UserTask().Name(TaskAfterCondition).EndEvent().Done();
//        //    }

//        //    public override string ToString()
//        //    {
//        //        return "ConditionalStartEventWithVarEvents";
//        //    }
//        //}

//        [Test]
//        [Deployment]
//        public virtual void testFalseCondition()
//        {
//            //given process with event sub process conditional start event
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when variable is set on task with condition
//            TaskService.SetVariable(task.Id, VariableName, 1);

//            //then execution stays at IUser task
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskBeforeCondition, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void testLoop()
//        {
//            // given
//            var processInstance = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            var task = TaskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "Task_1")
//                .First();

//            // when
//            TaskService.Complete(task.Id);

//            //then process instance will be in endless loop
//            //to end the instance we have a conditional branch in the java delegate
//            //after 3 instantiations the variable will be set to the instantiation Count
//            //execution stays in task 2
//            TasksAfterVariableIsSet = TaskService.CreateTaskQuery()
//                .ToList();
//            Assert.AreEqual(1, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual("Task_2", TasksAfterVariableIsSet[0].TaskDefinitionKey);
//            Assert.AreEqual(3, RuntimeService.GetVariable(processInstance.Id, VariableName));
//        }

//        [Test]
//        public virtual void testNonInterruptingConditionalSuspendedProcess()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskWithConditionId).EndEvent().Done();
//            var modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask(TaskWithConditionId)
//                .EndEvent()
//                .Done();


//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, false);

//            // given suspended process
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            RuntimeService.SuspendProcessInstanceById(procInst.Id);

//            //when wrong variable is set
//            RuntimeService.SetVariable(procInst.Id, VariableName + 1, 1);

//            //then nothing happens
//            Assert.True(RuntimeService.CreateProcessInstanceQuery()
//                .First()
//                .IsSuspended);

//            //when variable which triggers condition is set
//            //then exception is expected
//            try
//            {
//                RuntimeService.SetVariable(procInst.Id, VariableName, 1);
//                Assert.Fail("Should Assert.Fail!");
//            }
//            catch (SuspendedEntityInteractionException)
//            {
//                //expected
//            }
//            RuntimeService.ActivateProcessInstanceById(procInst.Id);
//            TasksAfterVariableIsSet = TaskService.CreateTaskQuery()
//                .ToList();
//        }

//        // Todo: ModifiableBpmnModelInstance.Modify()
//        //[Test]
//        //public virtual void testTriggerAnotherEventSubprocess()
//        //{
//        //    //given process with IUser task
//        //    IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskWithConditionId).Name(TaskWithCondition).EndEvent().Done();

//        //    //and event sub process with true condition
//        //    modelInstance = ModifiableBpmnModelInstance.Modify(modelInstance).AddSubProcessTo(ConditionalEventProcessKey)
//        //        .TriggerByEvent()//.EmbeddedSubProcess().StartEvent().interrupting(true).ConditionalEventDefinition()
//        //        .Condition(TRUE_CONDITION).ConditionalEventDefinitionDone().UserTask(AbstractConditionalEventTestCase.TaskAfterConditionId + 1).Name(TaskAfterCondition + 1).EndEvent().Done();
//        //    //a second event sub process
//        //    deployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, false);

//        //    //when
//        //    IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//        //    IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id);

//        //    //then first event sub process is on starting of process instance triggered
//        //    ITask task = taskQuery.First();
//        //    Assert.AreEqual(TaskAfterCondition + 1, task.Name);

//        //    //when variable is set, second condition becomes true -> but since first event sub process has
//        //    // interrupt the process instance the second event sub process can't be triggered
//        //    runtimeService.SetVariable(processInstance.Id, VARIABLE_NAME, 1);
//        //    tasksAfterVariableIsSet = taskQuery.ToList();
//        //    Assert.AreEqual(1, tasksAfterVariableIsSet.Count);
//        //    Assert.AreEqual(TaskAfterCondition + 1, tasksAfterVariableIsSet[0].Name);
//        //}

//        // Todo: ModifiableBpmnModelInstance.Modify()
//        //[Test]
//        //public virtual void testNonInterruptingTriggerAnotherEventSubprocess()
//        //{
//        //    IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskWithConditionId).Name(TaskWithCondition).EndEvent().Done();

//        //    //first event sub process
//        //    modelInstance = ModifiableBpmnModelInstance.Modify(modelInstance).AddSubProcessTo(ConditionalEventProcessKey).Id("eventSubProcess1").TriggerByEvent()//.EmbeddedSubProcess().StartEvent().interrupting(false).ConditionalEventDefinition().Condition(TRUE_CONDITION).ConditionalEventDefinitionDone().UserTask("taskAfterCond1").Name(TaskAfterCondition + 1).EndEvent().Done();

//        //    deployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, false);

//        //    //given process with two event sub processes

//        //    //when process is started
//        //    IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//        //    IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id);

//        //    //then first event sub process is triggered because condition is true
//        //    ITask task = taskQuery.Where(c=>c.Name==TaskAfterCondition + 1).First();
//        //    Assert.NotNull(task);
//        //    Assert.AreEqual(2, taskService.CreateTaskQuery().Count());

//        //    //when variable is set, second condition becomes true -> second event sub process is triggered
//        //    runtimeService.SetVariable(processInstance.Id, "variable", 1);
//        //    task = taskService.CreateTaskQuery(c=>c.Name ==TaskAfterCondition).First();
//        //    Assert.NotNull(task);
//        //    tasksAfterVariableIsSet = taskService.CreateTaskQuery().ToList();
//        //    Assert.AreEqual(4, tasksAfterVariableIsSet.Count);
//        //}

//        [Test]
//        [Deployment]
//        public virtual void testNonInterruptingSetMultipleVariableInDelegate()
//        {
//            // when
//            RuntimeService.StartProcessInstanceByKey("process");

//            // then
//            TasksAfterVariableIsSet = TaskService.CreateTaskQuery()
//                .ToList();
//            Assert.AreEqual(5, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(3, TaskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "Task_3")
//                .Count());
//        }

//        [Test]
//        public virtual void testNonInterruptingSetMultipleVariables()
//        {
//            var modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask(TaskWithConditionId)
//                .Name(TaskWithCondition)
//                .EndEvent()
//                .Done();

//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, false);

//            //given
//            var processInstance = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== processInstance.Id);
//            var task = taskQuery.First();

//            //when multiple variable are set on task execution
//            var variables = Variable.Variables.CreateVariables();
//            variables.PutValue("variable", 1);
//            variables.PutValue("variable1", 1);
//            RuntimeService.SetVariables(task.ExecutionId, variables);

//            //then event sub process should be triggered more than once
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(3, TasksAfterVariableIsSet.Count);
//        }

//        [Test]
//        public virtual void testNonInterruptingSetVariableInCallActivity()
//        {
//            Engine.ManageDeployment(RepositoryService.CreateDeployment()
//                .AddModelInstance(ConditionalModel, DelegatedProcess)
//                .Deploy());

//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).CallActivity(TaskWithConditionId).CalledElement(DELEGATED_PROCESS_KEY).UserTask().Name(TASK_AFTER_SERVICE_TASK).EndEvent().Done();
//            var modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .CallActivity(TaskWithConditionId)
//                .CalledElement(DelegatedProcessKey)
//                .UserTask()
//                .Name(TaskAfterServiceTask)
//                .EndEvent()
//                .Done();

//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, false);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then service task in call activity sets variable
//            //conditional event is not triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterServiceTask, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void testNonInterruptingSetVariableInDelegate()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask().Name(TaskBeforeCondition).ServiceTask().CamundaClass(SetVariableDelegate.class.GetName()).UserTask().EndEvent().Done();
//            //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
//            var modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask()
//                .Name(TaskBeforeCondition)
//                .ServiceTask()
//                .CamundaClass(typeof(SetVariableDelegate).FullName)
//                .UserTask()
//                .EndEvent()
//                .Done();
//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, false);

//            // given process with event sub process conditional start event and service task with delegate class which sets a variable
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);
//            Assert.AreEqual(1, ConditionEventSubscriptionQuery
//                .Count());

//            //when task before service task is completed
//            TaskService.Complete(task.Id);

//            //then service task with delegated code is called and variable is set
//            //-> non interrupting conditional event is triggered
//            //execution stays at IUser task after condition and after service task
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(2, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());
//        }

//        [Test]
//        public virtual void testNonInterruptingSetVariableInExpression()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask().Name(TaskBeforeCondition).ServiceTask(TaskWithConditionId).CamundaExpression("${execution.SetVariable(\"variable\", 1)}").UserTask().Name(TASK_AFTER_SERVICE_TASK).EndEvent().Done();
//            var modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask()
//                .Name(TaskBeforeCondition)
//                .ServiceTask(TaskWithConditionId)
//                .CamundaExpression("${execution.SetVariable(\"variable\", 1)}")
//                .UserTask()
//                .Name(TaskAfterServiceTask)
//                .EndEvent()
//                .Done();
//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, false);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task before service task is completed
//            TaskService.Complete(task.Id);

//            //then service task with expression is called and variable is set
//            //-> non interrupting conditional event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(2, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());
//        }

//        [Test]
//        public virtual void testNonInterruptingSetVariableInInMappingOfCallActivity()
//        {
//            Engine.ManageDeployment(RepositoryService.CreateDeployment()
//                .AddModelInstance(ConditionalModel, DelegatedProcess)
//                .Deploy());

//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).CallActivity(TaskWithConditionId).CalledElement(DELEGATED_PROCESS_KEY).CamundaIn(VARIABLE_NAME, VARIABLE_NAME).UserTask().Name(TASK_AFTER_OUTPUT_MAPPING).EndEvent().Done();
//            var modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .CallActivity(TaskWithConditionId)
//                .CalledElement(DelegatedProcessKey)
//                .CamundaIn(VariableName, VariableName)
//                .UserTask()
//                .Name(TaskAfterOutputMapping)
//                .EndEvent()
//                .Done();
//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, false);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then in mapping from call activity sets variable
//            //-> interrupting conditional event is not triggered, since variable is only locally
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterOutputMapping, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void testNonInterruptingSetVariableInInputMapping()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask().Name(TaskBeforeCondition).ServiceTask(TaskWithConditionId).CamundaInputParameter(VARIABLE_NAME, "1").CamundaExpression(TRUE_CONDITION).UserTask().Name(TASK_AFTER_SERVICE_TASK).EndEvent().Done();
//            var modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask()
//                .Name(TaskBeforeCondition)
//                .ServiceTask(TaskWithConditionId)
//                .CamundaInputParameter(VariableName, "1")
//                .CamundaExpression(TrueCondition)
//                .UserTask()
//                .Name(TaskAfterServiceTask)
//                .EndEvent()
//                .Done();
//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, false);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task before service task is completed
//            TaskService.Complete(task.Id);

//            //then service task with input mapping is called and variable is set
//            //-> non interrupting conditional event is not triggered
//            //since variable is only locally
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterServiceTask, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void testNonInterruptingSetVariableInInputMappingOfSubProcess()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).StartEvent().UserTask().Name(TASK_BEFORE_CONDITION).SubProcess(SUB_PROCESS_ID).CamundaInputParameter(VARIABLE_NAME, "1")//.EmbeddedSubProcess().StartEvent().UserTask().Name(TASK_IN_SUB_PROCESS_ID).EndEvent().SubProcessDone().EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask()
//                .Name(TaskBeforeCondition)
//                //.SubProcess(SubProcessId)
//                //.CamundaInputParameter(VariableName, "1")
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask()
//                .Name(TaskInSubProcessId)
//                .EndEvent()
//                //.SubProcessDone()
//                .EndEvent()
//                .Done();
//            DeployConditionalEventSubProcess(modelInstance, SubProcessId, false);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task before service task is completed
//            TaskService.Complete(task.Id);

//            //then input mapping from sub process sets variable
//            //-> non interrupting conditional event is triggered via default evaluation behavior
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(2, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());
//        }

//        [Test]
//        public virtual void testNonInterruptingSetVariableInOutMappingOfCallActivity()
//        {
//            Engine.ManageDeployment(RepositoryService.CreateDeployment()
//                .AddModelInstance(ConditionalModel, DelegatedProcess)
//                .Deploy());

//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).CallActivity(TaskWithConditionId).CalledElement(DELEGATED_PROCESS_KEY).CamundaOut(VARIABLE_NAME, VARIABLE_NAME).UserTask().Name(TASK_AFTER_OUTPUT_MAPPING).EndEvent().Done();
//            var modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .CallActivity(TaskWithConditionId)
//                .CalledElement(DelegatedProcessKey)
//                .CamundaOut(VariableName, VariableName)
//                .UserTask()
//                .Name(TaskAfterOutputMapping)
//                .EndEvent()
//                .Done();
//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, false);


//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task before service task is completed
//            TaskService.Complete(task.Id);

//            //then out mapping of call activity sets a variable
//            //-> non interrupting conditional event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(2, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());
//        }

//        [Test]
//        public virtual void testNonInterruptingSetVariableInOutputMapping()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).CamundaOutputParameter(VARIABLE_NAME, "1").UserTask().EndEvent().Done();
//            var modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .CamundaOutputParameter(VariableName, "1")
//                .UserTask()
//                .EndEvent()
//                .Done();
//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, false);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then output mapping from IUser task sets variable
//            //-> non interrupting conditional event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(2, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());
//        }

//        [Test]
//        public virtual void testNonInterruptingSetVariableInOutputMappingOfCallActivity()
//        {
//            Engine.ManageDeployment(RepositoryService.CreateDeployment()
//                .AddModelInstance(ConditionalModel, DelegatedProcess)
//                .Deploy());

//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).CallActivity(TaskWithConditionId).CalledElement(DELEGATED_PROCESS_KEY).CamundaOutputParameter(VARIABLE_NAME, "1").UserTask().EndEvent().Done();
//            var modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .CallActivity(TaskWithConditionId)
//                .CalledElement(DelegatedProcessKey)
//                .CamundaOutputParameter(VariableName, "1")
//                .UserTask()
//                .EndEvent()
//                .Done();
//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, false);


//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then output mapping from call activity sets variable
//            //-> non interrupting conditional event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(2, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());
//        }


//        [Test]
//        public virtual void testNonInterruptingSetVariableInSubProcessInDelegatedCode()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).StartEvent().UserTask(TASK_BEFORE_CONDITION_ID).Name(TASK_BEFORE_CONDITION).SubProcess(SUB_PROCESS_ID)//.EmbeddedSubProcess().StartEvent().ServiceTask().CamundaExpression(EXPR_SET_VARIABLE).UserTask().Name(TASK_AFTER_SERVICE_TASK).EndEvent().SubProcessDone().EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                //.SubProcess(SubProcessId)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .ServiceTask()
//                .CamundaExpression(ExprSetVariable)
//                .UserTask()
//                .Name(TaskAfterServiceTask)
//                .EndEvent()
//                //.SubProcessDone()
//                .EndEvent()
//                .Done();
//            DeployConditionalEventSubProcess(modelInstance, SubProcessId, false);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then service task in sub process sets variable
//            //conditional event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(2, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());
//        }

//        [Test]
//        public virtual void testNonInterruptingSetVariableInSubProcessInDelegatedCodeConditionOnPI()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).StartEvent().UserTask(TASK_BEFORE_CONDITION_ID).Name(TASK_BEFORE_CONDITION).SubProcess(SUB_PROCESS_ID)//.EmbeddedSubProcess().StartEvent().ServiceTask().CamundaExpression(EXPR_SET_VARIABLE).UserTask().Name(TASK_AFTER_SERVICE_TASK).EndEvent().SubProcessDone().EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                //.SubProcess(SubProcessId)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .ServiceTask()
//                .CamundaExpression(ExprSetVariable)
//                .UserTask()
//                .Name(TaskAfterServiceTask)
//                .EndEvent()
//                //.SubProcessDone()
//                .EndEvent()
//                .Done();

//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, false);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then service task in sub process sets variable
//            //conditional event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(2, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());
//        }

//        [Test]
//        [Deployment]
//        public virtual void testNonInterruptingVariableCondition()
//        {
//            //given process with event sub process conditional start event
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);
//            Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());

//            //when variable is set on task with condition
//            TaskService.SetVariable(task.Id, VariableName, 1);

//            //then execution is at IUser task after conditional start event
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(2, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());
//        }

//        [Test]
//        public virtual void testSetVariableInCallActivity()
//        {
//            Engine.ManageDeployment(RepositoryService.CreateDeployment()
//                .AddModelInstance(ConditionalModel, DelegatedProcess)
//                .Deploy());

//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).CallActivity(TaskWithConditionId).CalledElement(DELEGATED_PROCESS_KEY).UserTask().Name(TASK_AFTER_SERVICE_TASK).EndEvent().Done();
//            var modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .CallActivity(TaskWithConditionId)
//                .CalledElement(DelegatedProcessKey)
//                .UserTask()
//                .Name(TaskAfterServiceTask)
//                .EndEvent()
//                .Done();

//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, true);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then service task in call activity sets variable
//            //conditional event is not triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterServiceTask, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void testSetVariableInDelegate()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask().Name(TaskBeforeCondition).ServiceTask().CamundaClass(SetVariableDelegate.class.GetName()).EndEvent().Done();
//            //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
//            var modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask()
//                .Name(TaskBeforeCondition)
//                .ServiceTask()
//                .CamundaClass(typeof(SetVariableDelegate).FullName)
//                .EndEvent()
//                .Done();
//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, true);

//            // given process with event sub process conditional start event and service task with delegate class which sets a variable
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);
//            Assert.AreEqual(1, ConditionEventSubscriptionQuery
//                .Count());

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then service task with delegated code is called and variable is set
//            //-> conditional event is triggered and execution stays at IUser task after condition
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterCondition, TasksAfterVariableIsSet[0].Name);
//            Assert.AreEqual(0, ConditionEventSubscriptionQuery
//                .Count());
//        }

//        [Test]
//        public virtual void testSetVariableInExpression()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask().Name(TaskBeforeCondition).ServiceTask(TaskWithConditionId).CamundaExpression("${execution.SetVariable(\"variable\", 1)}").UserTask().Name(TASK_AFTER_SERVICE_TASK).EndEvent().Done();
//            var modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask()
//                .Name(TaskBeforeCondition)
//                .ServiceTask(TaskWithConditionId)
//                .CamundaExpression("${execution.SetVariable(\"variable\", 1)}")
//                .UserTask()
//                .Name(TaskAfterServiceTask)
//                .EndEvent()
//                .Done();
//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, true);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then service task with expression is called and variable is set
//            //-> interrupting conditional event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterCondition, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void testSetVariableInInMappingOfCallActivity()
//        {
//            Engine.ManageDeployment(RepositoryService.CreateDeployment()
//                .AddModelInstance(ConditionalModel, DelegatedProcess)
//                .Deploy());

//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).CallActivity(TaskWithConditionId).CalledElement(DELEGATED_PROCESS_KEY).CamundaIn(VARIABLE_NAME, VARIABLE_NAME).UserTask().Name(TASK_AFTER_OUTPUT_MAPPING).EndEvent().Done();
//            var modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .CallActivity(TaskWithConditionId)
//                .CalledElement(DelegatedProcessKey)
//                .CamundaIn(VariableName, VariableName)
//                .UserTask()
//                .Name(TaskAfterOutputMapping)
//                .EndEvent()
//                .Done();

//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, true);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then in mapping from call activity sets variable
//            //-> interrupting conditional event is not triggered, since variable is only locally
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterOutputMapping, TasksAfterVariableIsSet[0].Name);
//        }

//        // Todo: ModifiableBpmnModelInstance.Modify()
//        //[Test]
//        //public virtual void testSetVariableInDelegateWithSynchronousEvent()
//        //{
//        //    //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
//        //    IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//        //        .StartEvent().UserTask().Name(TaskBeforeCondition).ServiceTask().CamundaClass(typeof(SetVariableDelegate).FullName).EndEvent().Done();

//        //    modelInstance = ModifiableBpmnModelInstance.Modify(modelInstance).AddSubProcessTo(ConditionalEventProcessKey).TriggerByEvent()//.EmbeddedSubProcess().StartEvent().interrupting(true).ConditionalEventDefinition(CONDITIONAL_EVENT).Condition(CONDITION_EXPR).ConditionalEventDefinitionDone().EndEvent().Done();

//        //    engine.ManageDeployment(repositoryService.CreateDeployment().AddModelInstance(CONDITIONAL_MODEL, modelInstance).Deploy());

//        //    // given
//        //    IProcessInstance procInst = runtimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//        //    IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==procInst.Id);
//        //    ITask task = taskQuery.First();

//        //    //when task is completed
//        //    taskService.Complete(task.Id);

//        //    //then service task with delegated code is called and variable is set
//        //    //-> conditional event is triggered and process instance ends
//        //    tasksAfterVariableIsSet = taskQuery.ToList();
//        //    Assert.AreEqual(0, tasksAfterVariableIsSet.Count);
//        //    Assert.AreEqual(0, conditionEventSubscriptionQuery.Count());
//        //    Assert.IsNull(runtimeService.CreateProcessInstanceQuery().First());
//        //}


//        // Todo: ModifiableBpmnModelInstance.Modify()
//        //[Test]
//        //public virtual void testNonInterruptingSetVariableInDelegateWithSynchronousEvent()
//        //{
//        //    //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
//        //    IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//        //        .StartEvent().UserTask().Name(TaskBeforeCondition).ServiceTask().CamundaClass(typeof(SetVariableDelegate).FullName).UserTask().EndEvent().Done();

//        //    modelInstance = ModifiableBpmnModelInstance.Modify(modelInstance).AddSubProcessTo(ConditionalEventProcessKey).TriggerByEvent()//.EmbeddedSubProcess().StartEvent().interrupting(false).ConditionalEventDefinition(CONDITIONAL_EVENT).Condition(CONDITION_EXPR).ConditionalEventDefinitionDone().EndEvent().Done();

//        //    engine.ManageDeployment(repositoryService.CreateDeployment().AddModelInstance(CONDITIONAL_MODEL, modelInstance).Deploy());

//        //    // given process with event sub process conditional start event and service task with delegate class which sets a variable
//        //    IProcessInstance procInst = runtimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//        //    IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==procInst.Id);
//        //    ITask task = taskQuery.First();
//        //    Assert.NotNull(task);
//        //    Assert.AreEqual(TaskBeforeCondition, task.Name);
//        //    Assert.AreEqual(1, conditionEventSubscriptionQuery.Count());

//        //    //when task before service task is completed
//        //    taskService.Complete(task.Id);

//        //    //then service task with delegated code is called and variable is set
//        //    //-> non interrupting conditional event is triggered
//        //    //execution stays at IUser task after service task
//        //    tasksAfterVariableIsSet = taskQuery.ToList();
//        //    Assert.AreEqual(1, tasksAfterVariableIsSet.Count);
//        //    Assert.AreEqual(1, conditionEventSubscriptionQuery.Count());
//        //}

//        [Test]
//        public virtual void testSetVariableInInputMapping()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask().Name(TaskBeforeCondition).ServiceTask(TaskWithConditionId).CamundaInputParameter(VARIABLE_NAME, "1").CamundaExpression(TRUE_CONDITION).UserTask().Name(TASK_AFTER_SERVICE_TASK).EndEvent().Done();
//            var modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask()
//                .Name(TaskBeforeCondition)
//                .ServiceTask(TaskWithConditionId)
//                .CamundaInputParameter(VariableName, "1")
//                .CamundaExpression(TrueCondition)
//                .UserTask()
//                .Name(TaskAfterServiceTask)
//                .EndEvent()
//                .Done();
//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, true);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then service task with input mapping is called and variable is set
//            //-> interrupting conditional event is not triggered
//            //since variable is only locally
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterServiceTask, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void testSetVariableInInputMappingOfSubProcess()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).StartEvent().UserTask().Name(TASK_BEFORE_CONDITION).SubProcess(SUB_PROCESS_ID).CamundaInputParameter(VARIABLE_NAME, "1")//.EmbeddedSubProcess().StartEvent("startSubProcess").UserTask().Name(TASK_IN_SUB_PROCESS_ID).EndEvent().SubProcessDone().EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask()
//                .Name(TaskBeforeCondition)
//                .SubProcess(SubProcessId)
//                .CamundaInputParameter(VariableName, "1")
//                ////.EmbeddedSubProcess()
//                //.StartEvent("startSubProcess")
//                .UserTask()
//                .Name(TaskInSubProcessId)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();
//            DeployConditionalEventSubProcess(modelInstance, SubProcessId, true);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then input mapping from sub process sets variable
//            //-> interrupting conditional event is triggered by default evaluation behavior
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterCondition, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void testSetVariableInOutMappingOfCallActivity()
//        {
//            Engine.ManageDeployment(RepositoryService.CreateDeployment()
//                .AddModelInstance(ConditionalModel, DelegatedProcess)
//                .Deploy());

//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).CallActivity(TaskWithConditionId).CalledElement(DELEGATED_PROCESS_KEY).CamundaOut(VARIABLE_NAME, VARIABLE_NAME).UserTask().Name(TASK_AFTER_OUTPUT_MAPPING).EndEvent().Done();
//            var modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .CallActivity(TaskWithConditionId)
//                .CalledElement(DelegatedProcessKey)
//                .CamundaOut(VariableName, VariableName)
//                .UserTask()
//                .Name(TaskAfterOutputMapping)
//                .EndEvent()
//                .Done();
//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, true);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then out mapping from call activity sets variable
//            //-> interrupting conditional event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterCondition, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void testSetVariableInOutputMapping()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).CamundaOutputParameter(VARIABLE_NAME, "1").UserTask().EndEvent().Done();
//            var modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .CamundaOutputParameter(VariableName, "1")
//                .UserTask()
//                .EndEvent()
//                .Done();
//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, true);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then output mapping from IUser task sets variable
//            //-> interrupting conditional event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterCondition, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void testSetVariableInOutputMappingOfCallActivity()
//        {
//            Engine.ManageDeployment(RepositoryService.CreateDeployment()
//                .AddModelInstance(ConditionalModel, DelegatedProcess)
//                .Deploy());

//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).CallActivity(TaskWithConditionId).CalledElement(DELEGATED_PROCESS_KEY).CamundaOutputParameter(VARIABLE_NAME, "1").UserTask().EndEvent().Done();
//            var modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .CallActivity(TaskWithConditionId)
//                .CalledElement(DelegatedProcessKey)
//                .CamundaOutputParameter(VariableName, "1")
//                .UserTask()
//                .EndEvent()
//                .Done();
//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, true);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then output mapping from call activity sets variable
//            //-> interrupting conditional event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterCondition, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void testSetVariableInSubProcessInDelegatedCode()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).StartEvent().UserTask(TASK_BEFORE_CONDITION_ID).Name(TASK_BEFORE_CONDITION).SubProcess(SUB_PROCESS_ID)//.EmbeddedSubProcess().StartEvent().ServiceTask().CamundaExpression(EXPR_SET_VARIABLE).UserTask().Name(TASK_AFTER_SERVICE_TASK).EndEvent().SubProcessDone().EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .SubProcess(SubProcessId)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .ServiceTask()
//                .CamundaExpression(ExprSetVariable)
//                .UserTask()
//                .Name(TaskAfterServiceTask)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();

//            DeployConditionalEventSubProcess(modelInstance, SubProcessId, true);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then service task in sub process sets variable
//            //conditional event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterCondition, TasksAfterVariableIsSet[0].Name);
//        }


//        [Test]
//        public virtual void testSetVariableInSubProcessInDelegatedCodeConditionOnPI()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).StartEvent().UserTask(TASK_BEFORE_CONDITION_ID).Name(TASK_BEFORE_CONDITION).SubProcess(SUB_PROCESS_ID)//.EmbeddedSubProcess().StartEvent().ServiceTask().CamundaExpression(EXPR_SET_VARIABLE).UserTask().Name(TASK_AFTER_SERVICE_TASK).EndEvent().SubProcessDone().EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .SubProcess(SubProcessId)
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .ServiceTask()
//                .CamundaExpression(ExprSetVariable)
//                .UserTask()
//                .Name(TaskAfterServiceTask)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent()
//                .Done();
//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, true);

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then service task in sub process sets variable
//            //conditional event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterCondition, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        [Deployment(
//            new[]
//            {
//                "resources/bpmn/event/conditional/EventSubProcessStartConditionalEventTest.TestSubProcessVariableCondition.bpmn20.xml"
//            })]
//        public virtual void testSubProcessSetVariableOnExecutionCondition()
//        {
//            //given process with event sub process conditional start event and IUser task in sub process
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when local variable is set on task execution
//            RuntimeService.SetVariableLocal(task.ExecutionId, VariableName, 1);

//            //then execution stays at IUser task
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskBeforeCondition, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        [Deployment(
//            new[]
//            {
//                "resources/bpmn/event/conditional/EventSubProcessStartConditionalEventTest.TestSubProcessVariableCondition.bpmn20.xml"
//            })]
//        public virtual void testSubProcessSetVariableOnProcessInstanceCondition()
//        {
//            //given process with event sub process conditional start event and IUser task in sub process
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when variable is set on process instance
//            RuntimeService.SetVariable(procInst.Id, VariableName, 1);

//            //then execution is at IUser task after conditional start event
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterCondition, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        [Deployment(
//            new[]
//            {
//                "resources/bpmn/event/conditional/EventSubProcessStartConditionalEventTest.TestSubProcessVariableCondition.bpmn20.xml"
//            })]
//        public virtual void testSubProcessSetVariableOnTaskCondition()
//        {
//            //given process with event sub process conditional start event and IUser task in sub process
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when variable is set on task, variable is propagated to process instance
//            TaskService.SetVariable(task.Id, VariableName, 1);

//            //then execution is at IUser task after conditional start event
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterCondition, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        [Deployment]
//        public virtual void testSubProcessVariableCondition()
//        {
//            //given process with event sub process conditional start event and IUser task in sub process
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when local variable is set on task with condition
//            TaskService.SetVariableLocal(task.Id, VariableName, 1);

//            //then execution stays at IUser task
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskBeforeCondition, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void testSuspendedProcess()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskWithConditionId).EndEvent().Done();
//            var modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent()
//                .UserTask(TaskWithConditionId)
//                .EndEvent()
//                .Done();

//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, true);

//            // given suspended process
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            RuntimeService.SuspendProcessInstanceById(procInst.Id);

//            //when wrong variable is set
//            RuntimeService.SetVariable(procInst.Id, VariableName + 1, 1);

//            //then nothing happens
//            Assert.True(RuntimeService.CreateProcessInstanceQuery()
//                .First()
//                .IsSuspended);

//            //when variable which triggers condition is set
//            //then exception is expected
//            try
//            {
//                RuntimeService.SetVariable(procInst.Id, VariableName, 1);
//                Assert.Fail("Should Assert.Fail!");
//            }
//            catch (SuspendedEntityInteractionException)
//            {
//                //expected
//            }
//            RuntimeService.ActivateProcessInstanceById(procInst.Id);
//            TasksAfterVariableIsSet = TaskService.CreateTaskQuery()
//                .ToList();
//        }

//        [Test]
//        [Deployment]
//        public virtual void testTrueCondition()
//        {
//            //given process with event sub process conditional start event

//            //when process instance is started with true condition
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            //then event sub process is triggered via default evaluation behavior
//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterCondition, TasksAfterVariableIsSet[0].Name);
//            Assert.AreEqual(0, ConditionEventSubscriptionQuery.Count());
//        }

//        [Test]
//        [Deployment]
//        public virtual void testVariableCondition()
//        {
//            //given process with event sub process conditional start event
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when variable is set on task with condition
//            TaskService.SetVariable(task.Id, VariableName, 1);

//            //then execution is at IUser task after conditional start event
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterCondition, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        [Deployment(
//            new[]
//            {
//                "resources/bpmn/event/conditional/EventSubProcessStartConditionalEventTest.TestVariableCondition.bpmn20.xml"
//            })]
//        public virtual void testVariableConditionAndStartingWithVar()
//        {
//            //given process with event sub process conditional start event
//            IDictionary<string, object> vars = Variable.Variables.CreateVariables();
//            vars[VariableName] = 1;

//            //when starting process with variable
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey, vars);

//            //then event sub process is triggered via default evaluation behavior
//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterCondition, TasksAfterVariableIsSet[0].Name);
//            Assert.AreEqual(0, ConditionEventSubscriptionQuery.Count());
//        }

//        [Test]
//        [Deployment(
//            new[]
//            {
//                "resources/bpmn/event/conditional/EventSubProcessStartConditionalEventTest.TestVariableCondition.bpmn20.xml"
//            })]
//        public virtual void testWrongVariableCondition()
//        {
//            //given process with event sub process conditional start event
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);
//            Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());

//            //when variable is set on task with condition
//            TaskService.SetVariable(task.Id, VariableName + 1, 1);

//            //then execution stays at IUser task
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskBeforeCondition, TasksAfterVariableIsSet[0].Name);
//            Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());
//        }

//        //}
//        //    Assert.AreEqual(TaskAfterCondition, tasksAfterVariableIsSet[0].Name);
//        //    Assert.AreEqual(1, tasksAfterVariableIsSet.Count);
//        //    tasksAfterVariableIsSet = taskService.CreateTaskQuery().ToList();
//        //    //and service task in event sub process triggers again sub process
//        //    //event sub process is triggered

//        //    //then variable is set
//        //    taskService.Complete(task.Id);

//        //    //when task is completed
//        //    Assert.AreEqual(TaskWithCondition, task.Name);
//        //    ITask task = taskQuery.First();
//        //    IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id);


//        //    IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//        //    engine.ManageDeployment(repositoryService.CreateDeployment().AddModelInstance(CONDITIONAL_MODEL, modelInstance).Deploy());
//        //        .UserTask().Name(TaskAfterCondition).EndEvent().Done();
//        //        .Condition(CONDITION_EXPR).ConditionalEventDefinitionDone().ServiceTask().CamundaClass(typeof(LoopDelegate).FullName)
//        //        //.EmbeddedSubProcess().StartEvent().interrupting(true).ConditionalEventDefinition(CONDITIONAL_EVENT)
//        //    modelInstance = ModifiableBpmnModelInstance.Modify(modelInstance).AddSubProcessTo(ConditionalEventProcessKey).TriggerByEvent()

//        //    //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
//        //    IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskWithConditionId).Name(TaskWithCondition).ServiceTask().CamundaClass(typeof(SetVariableDelegate).FullName).EndEvent().Done();
//        //    //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
//        //{
//        //public virtual void testSetVariableInTriggeredEventSubProcess()
//        //[Test]

//        // Todo: ModifiableBpmnModelInstance.Modify()
//    }
//}