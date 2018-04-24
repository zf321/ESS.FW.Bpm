//using System.Collections.Generic;
//using System.Linq;
//using ESS.FW.Bpm.Engine.Delegate;
//using ESS.FW.Bpm.Engine.Runtime;
//using ESS.FW.Bpm.Engine.Task;
//using ESS.FW.Bpm.Engine.Tests.Api.Runtime.Migration;
//using ESS.FW.Bpm.Engine.Variable;
//using ESS.FW.Bpm.Model.Bpmn;
//using ESS.FW.Bpm.Model.Bpmn.builder;
//using ESS.FW.Bpm.Model.Bpmn.instance;
//using ESS.FW.Bpm.Model.Bpmn.instance.camunda;
//using NUnit.Framework;
//using ITask = ESS.FW.Bpm.Engine.Task.ITask;

//namespace ESS.FW.Bpm.Engine.Tests.Bpmn.Event.Conditional
//{
//    [TestFixture]
//    public class BoundaryConditionalEventTest : AbstractConditionalEventTestCase
//    {
//        [Test]
//        [Deployment]
//        public virtual void testTrueCondition()
//        {
//            //given process with boundary conditional event

//            //when process is started and execution arrives IUser task with boundary event
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            //then default evaluation behavior triggers boundary event
//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterCondition, TasksAfterVariableIsSet[0].Name);
//            Assert.AreEqual(0, ConditionEventSubscriptionQuery.Count());
//        }

//        [Test]
//        [Deployment]
//        public virtual void testNonInterruptingTrueCondition()
//        {
//            //given process with boundary conditional event

//            //when process is started and execution arrives activity with boundary event
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            //then default evaluation behavior triggers conditional event
//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(2, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());
//        }

//        [Test]
//        [Deployment]
//        public virtual void testFalseCondition()
//        {
//            //given process with boundary conditional event
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery();
//            ITask task = taskQuery.Where(c => c.ProcessInstanceId == procInst.Id).First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskWithCondition, task.Name);

//            //when variable is set on task execution
//            TaskService.SetVariable(task.Id, VariableName, 1);

//            //then execution stays in task with boundary condition
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskWithCondition, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        [Deployment]
//        public virtual void testVariableCondition()
//        {
//            //given process with boundary conditional event
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskWithCondition, task.Name);

//            //when local variable is set on task with condition
//            TaskService.SetVariableLocal(task.Id, VariableName, 1);

//            //then execution should remain on task
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskWithCondition, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        [Deployment(new string[] { "resources/bpmn/event/conditional/BoundaryConditionalEventTest.TestVariableCondition.bpmn20.xml" })]
//        public virtual void testVariableSetOnExecutionCondition()
//        {
//            //given process with boundary conditional event
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskWithCondition, task.Name);

//            //when variable is set on task execution
//            TaskService.SetVariable(task.Id, VariableName, 1);

//            //then execution ends
//            IExecution execution = RuntimeService.CreateExecutionQuery(c => c.ProcessInstanceId == procInst.Id && c.ActivityId == TaskWithConditionId).First();
//            Assert.IsNull(execution);

//            //and execution is at IUser task after boundary event
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterCondition, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        [Deployment]
//        public virtual void testNonInterruptingVariableCondition()
//        {
//            //given process with boundary conditional event
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskWithCondition, task.Name);

//            //when variable is set on task with condition
//            TaskService.SetVariable(task.Id, VariableName, 1);

//            //then execution is at IUser task after boundary event and in the task with the boundary event
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(2, TasksAfterVariableIsSet.Count);
//        }

//        [Test]
//        [Deployment(new string[] { "resources/bpmn/event/conditional/BoundaryConditionalEventTest.TestVariableCondition.bpmn20.xml" })]
//        public virtual void testWrongVariableCondition()
//        {
//            //given process with boundary conditional event
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskWithCondition, task.Name);
//            Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());

//            //when wrong variable is set on task execution
//            TaskService.SetVariable(task.Id, VariableName + 1, 1);

//            //then execution stays at IUser task with condition
//            task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskWithCondition, task.Name);
//            Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());

//            //when correct variable is set
//            TaskService.SetVariable(task.Id, VariableName, 1);

//            //then execution is on IUser task after condition
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterCondition, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        [Deployment]
//        public virtual void testParallelVariableCondition()
//        {
//            //given process with parallel IUser tasks and boundary conditional event
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            IList<ITask> tasks = taskQuery.ToList();
//            Assert.AreEqual(2, tasks.Count);
//            Assert.AreEqual(2, ConditionEventSubscriptionQuery.Count());

//            ITask task = tasks[0];

//            //when local variable is set on task
//            TaskService.SetVariableLocal(task.Id, VariableName, 1);

//            //then nothing happens
//            tasks = taskQuery.ToList();
//            Assert.AreEqual(2, tasks.Count);

//            //when local variable is set on task execution
//            RuntimeService.SetVariableLocal(task.ExecutionId, VariableName, 1);

//            //then boundary event is triggered of this task and task ends (subscription is deleted)
//            //other execution stays in other task
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(1, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());
//        }

//        [Test]
//        [Deployment(new string[] { "resources/bpmn/event/conditional/BoundaryConditionalEventTest.TestParallelVariableCondition.bpmn20.xml" })]
//        public virtual void testParallelSetVariableOnTaskCondition()
//        {
//            //given process with parallel IUser tasks and boundary conditional event
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            IList<ITask> tasks = taskQuery.ToList();
//            Assert.AreEqual(2, tasks.Count);

//            ITask task = tasks[0];

//            //when variable is set on execution
//            TaskService.SetVariable(task.Id, VariableName, 1);

//            //then both boundary event are triggered and process instance ends
//            IList<IExecution> executions = RuntimeService.CreateExecutionQuery(c => c.ProcessInstanceId == procInst.Id).ToList();
//            Assert.AreEqual(0, executions.Count);

//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(0, TasksAfterVariableIsSet.Count);
//        }

//        [Test]
//        [Deployment(new string[] { "resources/bpmn/event/conditional/BoundaryConditionalEventTest.TestParallelVariableCondition.bpmn20.xml" })]
//        public virtual void testParallelSetVariableOnExecutionCondition()
//        {
//            //given process with parallel IUser tasks and boundary conditional event
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            IList<ITask> tasks = taskQuery.ToList();
//            Assert.AreEqual(2, tasks.Count);

//            //when variable is set on execution
//            //taskService.SetVariable(task.GetId(), VARIABLE_NAME, 1);
//            RuntimeService.SetVariable(procInst.Id, VariableName, 1);

//            //then both boundary events are triggered and process instance ends
//            IList<IExecution> executions = RuntimeService.CreateExecutionQuery(c => c.ProcessInstanceId == procInst.Id).ToList();
//            Assert.AreEqual(0, executions.Count);

//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(0, TasksAfterVariableIsSet.Count);
//        }

//        [Test]
//        [Deployment]
//        public virtual void testSubProcessVariableCondition()
//        {
//            //given process with boundary conditional event on sub process
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskInSubProcess, task.Name);
//            Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());

//            //when local variable is set on task with condition
//            TaskService.SetVariableLocal(task.Id, VariableName, 1);

//            //then execution stays on IUser task
//            IList<IExecution> executions = RuntimeService.CreateExecutionQuery(c => c.ProcessInstanceId == procInst.Id).ToList();
//            Assert.AreEqual(2, executions.Count);
//            Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());

//            //when local variable is set on task execution
//            RuntimeService.SetVariableLocal(task.ExecutionId, VariableName, 1);

//            //then process instance ends
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(0, TasksAfterVariableIsSet.Count);
//        }

//        [Test]
//        [Deployment(new string[] { "resources/bpmn/event/conditional/BoundaryConditionalEventTest.TestSubProcessVariableCondition.bpmn20.xml" })]
//        public virtual void testSubProcessSetVariableOnTaskCondition()
//        {
//            //given process with boundary conditional event on sub process
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskInSubProcess, task.Name);

//            //when variable is set on task execution with condition
//            TaskService.SetVariable(task.Id, VariableName, 1);

//            //then process instance ends
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(0, TasksAfterVariableIsSet.Count);
//        }

//        [Test]
//        [Deployment(new string[] { "resources/bpmn/event/conditional/BoundaryConditionalEventTest.TestSubProcessVariableCondition.bpmn20.xml" })]
//        public virtual void testSubProcessSetVariableOnExecutionCondition()
//        {
//            //given process with boundary conditional event on sub process
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskInSubProcess, task.Name);

//            //when variable is set on task execution with condition
//            RuntimeService.SetVariable(task.ExecutionId, VariableName, 1);

//            //then process instance ends
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(0, TasksAfterVariableIsSet.Count);
//        }

//        [Test]
//        [Deployment]
//        public virtual void testNonInterruptingSubProcessVariableCondition()
//        {
//            //given process with boundary conditional event on sub process
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskInSubProcess, task.Name);

//            //when variable is set on task with condition
//            TaskService.SetVariable(task.Id, VariableName, 1);

//            //then execution stays on IUser task and at task after condition
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(2, TasksAfterVariableIsSet.Count);
//        }

//        [Test]
//        [Deployment]
//        public virtual void testCleanUpConditionalEventSubscriptions()
//        {
//            //given process with boundary conditional event
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();

//            Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then conditional subscription should be deleted
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(1, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(0, ConditionEventSubscriptionQuery.Count());
//        }

//        // Todo: AbstractActivityBuilder<,>
//        //protected internal virtual void deployBoundaryEventProcess(AbstractActivityBuilder<,> builder, bool isInterrupting)
//        //{
//        //    deployBoundaryEventProcess(builder, ConditionExpr, isInterrupting);
//        //}

//        // Todo: AbstractActivityBuilder<,>
//        //protected internal virtual void deployBoundaryEventProcess(AbstractActivityBuilder<,> builder, string conditionExpr, bool isInterrupting)
//        //{
//        //    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//        //    //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = builder.BoundaryEvent().cancelActivity(isInterrupting).ConditionalEventDefinition(CONDITIONAL_EVENT).Condition(conditionExpr).ConditionalEventDefinitionDone().UserTask().Name(TASK_AFTER_CONDITION).EndEvent().Done();
//        //    IBpmnModelInstance modelInstance = builder.BoundaryEvent().CancelActivity(isInterrupting).ConditionalEventDefinition(ConditionalEvent)
//        //        .Condition(conditionExpr).ConditionalEventDefinitionDone().UserTask().Name(TaskAfterCondition).EndEvent().Done();

//        //    Engine.ManageDeployment(RepositoryService.CreateDeployment().AddModelInstance(ConditionalModel, modelInstance).Deploy());
//        //}

//        [Test]
//        public virtual void testSetVariableInDelegate()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask().Name(TaskBeforeCondition).ServiceTask(TaskWithConditionId).CamundaClass(SetVariableDelegate.class.GetName()).EndEvent().Done();
//            //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask().Name(TaskBeforeCondition).ServiceTask(TaskWithConditionId).CamundaClass(typeof(SetVariableDelegate).FullName).EndEvent().Done();
//            DeployConditionalBoundaryEventProcess(modelInstance, TaskWithConditionId, true);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then service task with delegated code is called and variable is set
//            //-> conditional event is triggered and execution stays at IUser task after condition
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterCondition, TasksAfterVariableIsSet[0].Name);
//            Assert.AreEqual(0, ConditionEventSubscriptionQuery.Count());
//        }

//        [Test]
//        public virtual void testNonInterruptingSetVariableInDelegate()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask().Name(TaskBeforeCondition).ServiceTask(TaskWithConditionId).CamundaClass(SetVariableDelegate.class.GetName()).UserTask().EndEvent().Done();
//            //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask().Name(TaskBeforeCondition).ServiceTask(TaskWithConditionId).CamundaClass(typeof(SetVariableDelegate).FullName).UserTask().EndEvent().Done();
//            DeployConditionalBoundaryEventProcess(modelInstance, TaskWithConditionId, false);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task before service task is completed
//            TaskService.Complete(task.Id);

//            //then service task with delegated code is called and variable is set
//            //-> non interrupting conditional event is triggered
//            //execution stays at IUser task after condition and after service task
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(2, TasksAfterVariableIsSet.Count);
//        }

//        // Todo: AbstractConditionalEventDefinitionBuilder<>.ConditionalEventDefinitionDone()
//        //[Test]
//        //public virtual void testSetVariableInDelegateWithSynchronousEvent()
//        //{
//        //    JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
//        //    IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//        //        .StartEvent().UserTask().Name(TaskBeforeCondition).ServiceTask(TaskWithConditionId).CamundaClass(typeof(SetVariableDelegate).FullName).EndEvent().Done();

//        //    modelInstance = ModifiableBpmnModelInstance.Modify(modelInstance).serviceTaskBuilder(TaskWithConditionId).BoundaryEvent()
//        //        .CancelActivity(true).ConditionalEventDefinition(ConditionalEvent).Condition(ConditionExpr).ConditionalEventDefinitionDone().Done();

//        //    Engine.ManageDeployment(RepositoryService.CreateDeployment().AddModelInstance(ConditionalModel, modelInstance).Deploy());

//        //    given
//        //   IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//        //    IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId ==procInst.Id);

//        //    when task is completed
//        //    TaskService.Complete(taskQuery.First().Id);

//        //    then service task with delegated code is called and variable is set
//        //    -> conditional event is triggered and process instance ends
//        //    TasksAfterVariableIsSet = taskQuery.ToList();
//        //Assert.AreEqual(0, TasksAfterVariableIsSet.Count);
//        //Assert.AreEqual(0, ConditionEventSubscriptionQuery.Count());
//        //Assert.IsNull(RuntimeService.CreateProcessInstanceQuery().First());
//        //}


//        // Todo: AbstractConditionalEventDefinitionBuilder<>.ConditionalEventDefinitionDone()
//        //[Test]
//        //public virtual void testNonInterruptingSetVariableInDelegateWithSynchronousEvent()
//        //{
//        //    //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
//        //    IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//        //        .StartEvent().UserTask()
//        //        .Name(TaskBeforeCondition).ServiceTask(TaskWithConditionId)
//        //        .CamundaClass(typeof(SetVariableDelegate).FullName).UserTask().EndEvent().Done();

//        //    modelInstance = ModifiableBpmnModelInstance.Modify(modelInstance).serviceTaskBuilder(TaskWithConditionId).BoundaryEvent()
//        //        .CancelActivity(false).ConditionalEventDefinition(ConditionalEvent).Condition(ConditionExpr).ConditionalEventDefinitionDone().Done();

//        //    Engine.ManageDeployment(RepositoryService.CreateDeployment().AddModelInstance(ConditionalModel, modelInstance).Deploy());

//        //    // given process with event sub process conditional start event and service task with delegate class which sets a variable
//        //    IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//        //    IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId ==procInst.Id);

//        //    //when task before service task is completed
//        //    TaskService.Complete(taskQuery.First().Id);

//        //    //then service task with delegated code is called and variable is set
//        //    //-> non interrupting conditional event is triggered
//        //    //execution stays at IUser task after service task
//        //    TasksAfterVariableIsSet = taskQuery.ToList();
//        //    Assert.AreEqual(1, TasksAfterVariableIsSet.Count);
//        //    Assert.AreEqual(0, ConditionEventSubscriptionQuery.Count());
//        //}

//        [Test]
//        public virtual void testSetVariableInInputMapping()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask().Name(TaskBeforeCondition).ServiceTask(TaskWithConditionId).CamundaInputParameter(VARIABLE_NAME, "1").CamundaExpression(TRUE_CONDITION).EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent().UserTask()
//                .Name(TaskBeforeCondition).ServiceTask(TaskWithConditionId).CamundaInputParameter(VariableName, "1")
//                .CamundaExpression(TrueCondition).EndEvent().Done();

//            DeployConditionalBoundaryEventProcess(modelInstance, TaskWithConditionId, true);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            // input mapping does trigger boundary event with help of default evaluation behavior and process ends regularly
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterCondition, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void testNonInterruptingSetVariableInInputMapping()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask().Name(TaskBeforeCondition).ServiceTask(TaskWithConditionId).CamundaInputParameter(VARIABLE_NAME, "1").CamundaExpression(TRUE_CONDITION).UserTask().Name(TASK_AFTER_SERVICE_TASK).EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent().UserTask().Name(TaskBeforeCondition).ServiceTask(TaskWithConditionId).CamundaInputParameter(VariableName, "1")
//                .CamundaExpression(TrueCondition).UserTask().Name(TaskAfterServiceTask).EndEvent().Done();

//            DeployConditionalBoundaryEventProcess(modelInstance, TaskWithConditionId, false);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task before service task is completed
//            TaskService.Complete(task.Id);

//            // then the variable is set in an input mapping
//            // -> non interrupting conditional event is not triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterServiceTask, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void testSetVariableInExpression()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask().Name(TaskBeforeCondition).ServiceTask(TaskWithConditionId).CamundaExpression(EXPR_SET_VARIABLE).EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent().UserTask().Name(TaskBeforeCondition).ServiceTask(TaskWithConditionId)
//                .CamundaExpression(ExprSetVariable).EndEvent().Done();

//            DeployConditionalBoundaryEventProcess(modelInstance, TaskWithConditionId, true);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
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
//        public virtual void testNonInterruptingSetVariableInExpression()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask().Name(TaskBeforeCondition).ServiceTask(TaskWithConditionId).CamundaExpression(EXPR_SET_VARIABLE).UserTask().Name(TASK_AFTER_SERVICE_TASK).EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent().UserTask().Name(TaskBeforeCondition).ServiceTask(TaskWithConditionId)
//                .CamundaExpression(ExprSetVariable).UserTask().Name(TaskAfterServiceTask).EndEvent().Done();

//            DeployConditionalBoundaryEventProcess(modelInstance, TaskWithConditionId, false);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task before service task is completed
//            TaskService.Complete(task.Id);

//            //then service task with expression is called and variable is set
//            //->non interrupting conditional event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(2, TasksAfterVariableIsSet.Count);
//        }

//        // Todo: EmbeddedSubProcessBuilder
//        //[Test]
//        //public virtual void testSetVariableInInputMappingOfSubProcess()
//        //{
//        //    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//        //    //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).StartEvent().UserTask().Name(TASK_BEFORE_CONDITION).SubProcess(SUB_PROCESS_ID).CamundaInputParameter(VARIABLE_NAME, "1")//.EmbeddedSubProcess().StartEvent().UserTask().Name(TASK_IN_SUB_PROCESS_ID).EndEvent().SubProcessDone().EndEvent().Done();
//        //    IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//        //        .StartEvent().UserTask().Name(TaskBeforeCondition).SubProcess(SubProcessId)
//        //        .CamundaInputParameter(VariableName, "1")//.EmbeddedSubProcess()
//        //        .StartEvent().UserTask().Name(TaskInSubProcessId)
//        //        .EndEvent().SubProcessDone().EndEvent().Done();

//        //    DeployConditionalBoundaryEventProcess(modelInstance, SubProcessId, true);

//        //    // given
//        //    IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//        //    IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId ==procInst.Id);
//        //    ITask task = taskQuery.First();
//        //    Assert.NotNull(task);
//        //    Assert.AreEqual(TaskBeforeCondition, task.Name);

//        //    //when task is completed
//        //    TaskService.Complete(task.Id);

//        //    // Then input mapping from sub process sets variable,
//        //    // interrupting conditional event is triggered by default evaluation behavior
//        //    TasksAfterVariableIsSet = taskQuery.ToList();
//        //    Assert.AreEqual(TaskAfterCondition, TasksAfterVariableIsSet[0].Name);
//        //    Assert.AreEqual(0, ConditionEventSubscriptionQuery.Count());
//        //}

//        // Todo: EmbeddedSubProcessBuilder
//        //[Test]
//        //public virtual void testNonInterruptingSetVariableInInputMappingOfSubProcess()
//        //{
//        //    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//        //    //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).StartEvent().UserTask().Name(TASK_BEFORE_CONDITION).SubProcess(SUB_PROCESS_ID).CamundaInputParameter(VARIABLE_NAME, "1")//.EmbeddedSubProcess().StartEvent().UserTask().Name(TASK_IN_SUB_PROCESS_ID).EndEvent().SubProcessDone().EndEvent().Done();
//        //    IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask()
//        //        .Name(TaskBeforeCondition).SubProcess(SubProcessId).CamundaInputParameter(VariableName, "1")
//        //        //.EmbeddedSubProcess()
//        //        .StartEvent().UserTask().Name(TaskInSubProcessId).EndEvent().SubProcessDone().EndEvent().Done();

//        //    DeployConditionalBoundaryEventProcess(modelInstance, SubProcessId, false);

//        //    // given
//        //    IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//        //    IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId ==procInst.Id);
//        //    ITask task = taskQuery.First();
//        //    Assert.NotNull(task);
//        //    Assert.AreEqual(TaskBeforeCondition, task.Name);

//        //    //when task before is completed
//        //    TaskService.Complete(task.Id);

//        //    // Then input mapping from sub process sets variable, but
//        //    // non interrupting conditional event is not triggered
//        //    TasksAfterVariableIsSet = taskQuery.ToList();
//        //    Assert.AreEqual(TaskInSubProcessId, TasksAfterVariableIsSet[0].Name);
//        //    Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());
//        //}

//        // Todo: EmbeddedSubProcessBuilder
//        //[Test]
//        //public virtual void testSetVariableInStartListenerOfSubProcess()
//        //{
//        //    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//        //    //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).StartEvent().UserTask().Name(TASK_BEFORE_CONDITION).SubProcess(SUB_PROCESS_ID).CamundaExecutionListenerExpression(org.Camunda.bpm.Engine.Delegate.ExecutionListenerFields.EventNameStart, EXPR_SET_VARIABLE)//.EmbeddedSubProcess().StartEvent().UserTask().Name(TASK_IN_SUB_PROCESS_ID).EndEvent().SubProcessDone().EndEvent().Done();
//        //    IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask()
//        //        .Name(TaskBeforeCondition).SubProcess(SubProcessId)
//        //        .CamundaExecutionListenerExpression(ExecutionListenerFields.EventNameStart, ExprSetVariable)//.EmbeddedSubProcess()
//        //        .StartEvent().UserTask().Name(TaskInSubProcessId).EndEvent().SubProcessDone().EndEvent().Done();

//        //    DeployConditionalBoundaryEventProcess(modelInstance, SubProcessId, true);

//        //    // given
//        //    IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//        //    IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId ==procInst.Id);
//        //    ITask task = taskQuery.First();
//        //    Assert.NotNull(task);
//        //    Assert.AreEqual(TaskBeforeCondition, task.Name);

//        //    //when task is completed
//        //    TaskService.Complete(task.Id);

//        //    // Then start listener from sub process sets variable,
//        //    // interrupting conditional event is triggered by default evaluation behavior
//        //    TasksAfterVariableIsSet = taskQuery.ToList();
//        //    Assert.AreEqual(TaskAfterCondition, TasksAfterVariableIsSet[0].Name);
//        //    Assert.AreEqual(0, ConditionEventSubscriptionQuery.Count());
//        //}

//        // Todo: EmbeddedSubProcessBuilder
//        //[Test]
//        //public virtual void testNonInterruptingSetVariableInStartListenerOfSubProcess()
//        //{
//        //    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//        //    //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).StartEvent().UserTask().Name(TASK_BEFORE_CONDITION).SubProcess(SUB_PROCESS_ID).CamundaExecutionListenerExpression(org.Camunda.bpm.Engine.Delegate.ExecutionListenerFields.EventNameStart, EXPR_SET_VARIABLE)//.EmbeddedSubProcess().StartEvent().UserTask().Name(TASK_IN_SUB_PROCESS_ID).EndEvent().SubProcessDone().EndEvent().Done();
//        //    IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//        //        .StartEvent().UserTask().Name(TaskBeforeCondition).SubProcess(SubProcessId)
//        //        .CamundaExecutionListenerExpression(ExecutionListenerFields.EventNameStart, ExprSetVariable)
//        //        //.EmbeddedSubProcess().StartEvent().UserTask().Name(TaskInSubProcessId).EndEvent().SubProcessDone().EndEvent().Done();
//        //    DeployConditionalBoundaryEventProcess(modelInstance, SubProcessId, false);

//        //    // given
//        //    IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//        //    IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId ==procInst.Id);
//        //    ITask task = taskQuery.First();
//        //    Assert.NotNull(task);
//        //    Assert.AreEqual(TaskBeforeCondition, task.Name);

//        //    //when task before is completed
//        //    TaskService.Complete(task.Id);

//        //    // Then start listener from sub process sets variable,
//        //    // non interrupting conditional event is triggered by default evaluation behavior
//        //    TasksAfterVariableIsSet = taskQuery.ToList();
//        //    Assert.AreEqual(2, TasksAfterVariableIsSet.Count);
//        //    Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());
//        //}

//        [Test]
//        public virtual void testSetVariableInOutputMapping()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).CamundaOutputParameter(VARIABLE_NAME, "1").UserTask().Name(TASK_AFTER_OUTPUT_MAPPING).EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).CamundaOutputParameter(VariableName, "1")
//                .UserTask().Name(TaskAfterOutputMapping).EndEvent().Done();

//            DeployConditionalBoundaryEventProcess(modelInstance, TaskBeforeConditionId, true);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then output mapping sets variable
//            //boundary event is not triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterOutputMapping, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void testSetVariableInOutputMappingWithBoundary()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).CamundaOutputParameter(VARIABLE_NAME, "1").UserTask(TaskWithConditionId).Name(TaskWithCondition).EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).CamundaOutputParameter(VariableName, "1")
//                .UserTask(TaskWithConditionId).Name(TaskWithCondition).EndEvent().Done();

//            DeployConditionalBoundaryEventProcess(modelInstance, TaskWithConditionId, true);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then output mapping sets variable
//            //boundary event is triggered by default evaluation behavior
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterCondition, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void testNonInterruptingSetVariableInOutputMapping()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).CamundaOutputParameter(VARIABLE_NAME, "1").UserTask(TaskWithConditionId).Name(TaskWithCondition).EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent().UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition).CamundaOutputParameter(VariableName, "1").UserTask(TaskWithConditionId)
//                .Name(TaskWithCondition).EndEvent().Done();
//            DeployConditionalBoundaryEventProcess(modelInstance, TaskWithConditionId, false);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task with output mapping is completed
//            TaskService.Complete(task.Id);

//            //then output mapping sets variable
//            //boundary event is triggered from default evaluation behavior
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(2, TasksAfterVariableIsSet.Count);
//        }

//        [Test]
//        public virtual void testNonInterruptingSetVariableInOutputMappingWithBoundary()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskWithConditionId).Name(TaskWithCondition).CamundaOutputParameter(VARIABLE_NAME, "1").UserTask().Name(TASK_AFTER_OUTPUT_MAPPING).EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent().UserTask(TaskWithConditionId)
//                .Name(TaskWithCondition).CamundaOutputParameter(VariableName, "1").UserTask()
//                .Name(TaskAfterOutputMapping).EndEvent().Done();

//            DeployConditionalBoundaryEventProcess(modelInstance, TaskWithConditionId, false);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskWithCondition, task.Name);

//            //when task with output mapping is completed
//            TaskService.Complete(task.Id);

//            //then output mapping sets variable
//            //boundary event is not triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterOutputMapping, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void testSetVariableInOutputMappingOfCallActivity()
//        {
//            Engine.ManageDeployment(RepositoryService.CreateDeployment().AddModelInstance(ConditionalModel, DelegatedProcess).Deploy());

//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).CallActivity(TaskWithConditionId).CalledElement(DELEGATED_PROCESS_KEY).CamundaOutputParameter(VARIABLE_NAME, "1").UserTask().Name(TASK_AFTER_OUTPUT_MAPPING).EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).CallActivity(TaskWithConditionId)
//                .CalledElement(DelegatedProcessKey).CamundaOutputParameter(VariableName, "1")
//                .UserTask().Name(TaskAfterOutputMapping).EndEvent().Done();

//            DeployConditionalBoundaryEventProcess(modelInstance, TaskWithConditionId, true);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then output mapping from call activity sets variable
//            //-> interrupting conditional event is not triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterOutputMapping, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void testNonInterruptingSetVariableInOutputMappingOfCallActivity()
//        {
//            Engine.ManageDeployment(RepositoryService.CreateDeployment().AddModelInstance(ConditionalModel, DelegatedProcess).Deploy());

//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).CallActivity(TaskWithConditionId).CalledElement(DELEGATED_PROCESS_KEY).CamundaOutputParameter(VARIABLE_NAME, "1").UserTask().Name(TASK_AFTER_OUTPUT_MAPPING).EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).CallActivity(TaskWithConditionId)
//                .CalledElement(DelegatedProcessKey).CamundaOutputParameter(VariableName, "1").UserTask()
//                .Name(TaskAfterOutputMapping).EndEvent().Done();

//            DeployConditionalBoundaryEventProcess(modelInstance, TaskWithConditionId, false);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task before service task is completed
//            TaskService.Complete(task.Id);

//            //then out mapping of call activity sets a variable
//            //-> non interrupting conditional event is not triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterOutputMapping, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void testSetVariableInOutMappingOfCallActivity()
//        {
//            Engine.ManageDeployment(RepositoryService.CreateDeployment().AddModelInstance(ConditionalModel, DelegatedProcess).Deploy());

//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).CallActivity(TaskWithConditionId).CalledElement(DELEGATED_PROCESS_KEY).CamundaOut(VARIABLE_NAME, VARIABLE_NAME).UserTask().Name(TASK_AFTER_OUTPUT_MAPPING).EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent().UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition).CallActivity(TaskWithConditionId)
//                .CalledElement(DelegatedProcessKey).CamundaOut(VariableName, VariableName).UserTask()
//                .Name(TaskAfterOutputMapping).EndEvent().Done();
//            DeployConditionalBoundaryEventProcess(modelInstance, TaskWithConditionId, true);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
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
//        public virtual void testNonInterruptingSetVariableInOutMappingOfCallActivity()
//        {
//            Engine.ManageDeployment(RepositoryService.CreateDeployment().AddModelInstance(ConditionalModel, DelegatedProcess).Deploy());

//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).CallActivity(TaskWithConditionId).CalledElement(DELEGATED_PROCESS_KEY).CamundaOut(VARIABLE_NAME, VARIABLE_NAME).UserTask().Name(TASK_AFTER_OUTPUT_MAPPING).EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).CallActivity(TaskWithConditionId)
//                .CalledElement(DelegatedProcessKey).CamundaOut(VariableName, VariableName)
//                .UserTask().Name(TaskAfterOutputMapping).EndEvent().Done();

//            DeployConditionalBoundaryEventProcess(modelInstance, TaskWithConditionId, false);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task before service task is completed
//            TaskService.Complete(task.Id);

//            //then out mapping of call activity sets a variable
//            //-> non interrupting conditional event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(2, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(0, ConditionEventSubscriptionQuery.Count());
//        }

//        [Test]
//        public virtual void testSetVariableInInMappingOfCallActivity()
//        {
//            Engine.ManageDeployment(RepositoryService.CreateDeployment().AddModelInstance(ConditionalModel, DelegatedProcess).Deploy());

//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).CallActivity(TaskWithConditionId).CalledElement(DELEGATED_PROCESS_KEY).CamundaIn(VARIABLE_NAME, VARIABLE_NAME).UserTask().Name(TASK_AFTER_OUTPUT_MAPPING).EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent().UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition).CallActivity(TaskWithConditionId)
//                .CalledElement(DelegatedProcessKey).CamundaIn(VariableName, VariableName).UserTask().Name(TaskAfterOutputMapping).EndEvent().Done();
//            DeployConditionalBoundaryEventProcess(modelInstance, TaskWithConditionId, true);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
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
//        public virtual void testNonInterruptingSetVariableInInMappingOfCallActivity()
//        {
//            Engine.ManageDeployment(RepositoryService.CreateDeployment().AddModelInstance(ConditionalModel, DelegatedProcess).Deploy());

//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).CallActivity(TaskWithConditionId).CalledElement(DELEGATED_PROCESS_KEY).CamundaIn(VARIABLE_NAME, VARIABLE_NAME).UserTask().Name(TASK_AFTER_OUTPUT_MAPPING).EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent().UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition).CallActivity(TaskWithConditionId)
//                .CalledElement(DelegatedProcessKey).CamundaIn(VariableName, VariableName)
//                .UserTask().Name(TaskAfterOutputMapping).EndEvent().Done();

//            DeployConditionalBoundaryEventProcess(modelInstance, TaskWithConditionId, false);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
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
//        public virtual void testSetVariableInStartListener()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).UserTask(TaskWithConditionId).Name(TaskWithCondition).CamundaExecutionListenerExpression(org.Camunda.bpm.Engine.Delegate.ExecutionListenerFields.EventNameStart, EXPR_SET_VARIABLE).EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent().UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition).UserTask(TaskWithConditionId)
//                .Name(TaskWithCondition).CamundaExecutionListenerExpression(ExecutionListenerFields.EventNameStart, ExprSetVariable).EndEvent().Done();
//            DeployConditionalBoundaryEventProcess(modelInstance, TaskWithConditionId, true);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then start listener sets variable
//            //boundary event is triggered by default evaluation behavior
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterCondition, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void testNonInterruptingSetVariableInStartListener()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).UserTask(TaskWithConditionId).Name(TaskWithCondition).CamundaExecutionListenerExpression(org.Camunda.bpm.Engine.Delegate.ExecutionListenerFields.EventNameStart, EXPR_SET_VARIABLE).EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition).UserTask(TaskWithConditionId).Name(TaskWithCondition).CamundaExecutionListenerExpression(ExecutionListenerFields.EventNameStart, ExprSetVariable).EndEvent().Done();
//            DeployConditionalBoundaryEventProcess(modelInstance, TaskWithConditionId, false);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then start listener sets variable
//            //non interrupting boundary event is not triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskWithCondition, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void testSetVariableInTakeListener()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).SequenceFlowId(FlowId).UserTask(TaskWithConditionId).Name(TaskWithCondition).EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition)
//                .SequenceFlowId(FlowId).UserTask(TaskWithConditionId).Name(TaskWithCondition).EndEvent().Done();
//            ICamundaExecutionListener listener = modelInstance.NewInstance<ICamundaExecutionListener>(typeof(ICamundaExecutionListener));
//            listener.CamundaEvent = ExecutionListenerFields.EventNameTake;
//            listener.CamundaExpression = ExprSetVariable;
//            modelInstance.GetModelElementById<ISequenceFlow>(FlowId).Builder().AddExtensionElement(listener);
//            DeployConditionalBoundaryEventProcess(modelInstance, TaskWithConditionId, true);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then take listener sets variable
//            //non interrupting boundary event is triggered with default evaluation behavior
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterCondition, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void testNonInterruptingSetVariableInTakeListener()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).SequenceFlowId(FlowId).UserTask(TaskWithConditionId).Name(TaskWithCondition).EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).SequenceFlowId(FlowId).UserTask(TaskWithConditionId).Name(TaskWithCondition).EndEvent().Done();
//            ICamundaExecutionListener listener = modelInstance.NewInstance<ICamundaExecutionListener>(typeof(ICamundaExecutionListener));
//            listener.CamundaEvent = ExecutionListenerFields.EventNameTake;
//            listener.CamundaExpression = ExprSetVariable;
//            modelInstance.GetModelElementById<ISequenceFlow>(FlowId).Builder().AddExtensionElement(listener);
//            DeployConditionalBoundaryEventProcess(modelInstance, TaskWithConditionId, false);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then take listener sets variable
//            //non interrupting boundary event is triggered with default evaluation behavior
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(2, TasksAfterVariableIsSet.Count);
//        }

//        [Test]
//        public virtual void testSetVariableInEndListener()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskWithConditionId).Name(TaskWithCondition).CamundaExecutionListenerExpression(org.Camunda.bpm.Engine.Delegate.ExecutionListenerFields.EventNameEnd, EXPR_SET_VARIABLE).UserTask().Name(AFTER_TASK).EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskWithConditionId).Name(TaskWithCondition).CamundaExecutionListenerExpression(ExecutionListenerFields.EventNameEnd, ExprSetVariable).UserTask().Name(AfterTask).EndEvent().Done();
//            DeployConditionalBoundaryEventProcess(modelInstance, TaskWithConditionId, true);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskWithCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then end listener sets variable
//            //conditional event is not triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(AfterTask, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void testNonInterruptingSetVariableInEndListener()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskWithConditionId).Name(TaskWithCondition).CamundaExecutionListenerExpression(org.Camunda.bpm.Engine.Delegate.ExecutionListenerFields.EventNameEnd, EXPR_SET_VARIABLE).UserTask().Name(AFTER_TASK).EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskWithConditionId).Name(TaskWithCondition).CamundaExecutionListenerExpression(ExecutionListenerFields.EventNameEnd, ExprSetVariable).UserTask().Name(AfterTask).EndEvent().Done();
//            DeployConditionalBoundaryEventProcess(modelInstance, TaskWithConditionId, false);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskWithCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then end listener sets variable
//            //non interrupting boundary event is not triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(AfterTask, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void testSetVariableInMultiInstance()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).UserTask(TaskWithConditionId).MultiInstance().Cardinality("3").Parallel().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).UserTask(TaskWithConditionId).MultiInstance().Cardinality("3").Parallel().Done();
//            DeployConditionalBoundaryEventProcess(modelInstance, TaskWithConditionId, "${nrOfInstances == 3}", true);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then multi instance is created
//            //and boundary event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterCondition, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void testNonInterruptingSetVariableInMultiInstance()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).UserTask(TaskWithConditionId).MultiInstance().Cardinality("3").Parallel().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).UserTask(TaskWithConditionId).MultiInstance().Cardinality("3").Parallel().Done();
//            DeployConditionalBoundaryEventProcess(modelInstance, TaskWithConditionId, "${nrOfInstances == 3}", false);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then multi instance is created
//            //and boundary event is triggered for each multi instance creation
//            //IList<ITask> multiInstanceTasks = taskQuery.TaskDefinitionKey(TaskWithConditionId).ToList();
//            //Assert.AreEqual(3, multiInstanceTasks.Count);
//            //Assert.AreEqual(3, TaskService.CreateTaskQuery(c=>c.Name ==TaskAfterCondition).Count());

//            ////when multi instances are completed
//            //foreach (ITask multiInstanceTask in multiInstanceTasks)
//            //{
//            //    TaskService.Complete(multiInstanceTask.Id);
//            //}

//            ////then non boundary events are triggered
//            //TasksAfterVariableIsSet = TaskService.CreateTaskQuery().ToList();
//            //Assert.AreEqual(9, TasksAfterVariableIsSet.Count);
//            //Assert.AreEqual(0, ConditionEventSubscriptionQuery.Count());
//        }

//        [Test]
//        public virtual void testSetVariableInSeqMultiInstance()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).UserTask(TaskWithConditionId).MultiInstance().Cardinality("3").Sequential().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).UserTask(TaskWithConditionId).MultiInstance().Cardinality("3").Sequential().Done();
//            DeployConditionalBoundaryEventProcess(modelInstance, TaskWithConditionId, "${true}", true);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then multi instance is created
//            //and boundary event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterCondition, TasksAfterVariableIsSet[0].Name);
//        }

//        [Test]
//        public virtual void testNonInterruptingSetVariableInSeqMultiInstance()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).UserTask(TaskWithConditionId).MultiInstance().Cardinality("3").Sequential().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).UserTask(TaskWithConditionId).MultiInstance().Cardinality("3").Sequential().Done();
//            DeployConditionalBoundaryEventProcess(modelInstance, TaskWithConditionId, "${true}", false);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then multi instance is created
//            //and boundary event is triggered for each multi instance creation and also from the default evaluation behavior
//            //since the condition is true. That means one time from the default behavior and 4 times for the variables which are set:
//            //nrOfInstances, nrOfCompletedInstances, nrOfActiveInstances, loopCounter
//            //for (int i = 0; i < 3; i++)
//            //{
//            //    ITask multiInstanceTask = taskQuery.TaskDefinitionKey(TaskWithConditionId).First();
//            //    Assert.NotNull(multiInstanceTask);
//            //    Assert.AreEqual(i == 0 ? 5 : 5 + i * 2, TaskService.CreateTaskQuery(c=>c.Name ==TaskAfterCondition).Count());
//            //    TaskService.Complete(multiInstanceTask.Id);
//            //}

//            //then non boundary events are triggered 9 times
//            TasksAfterVariableIsSet = TaskService.CreateTaskQuery().ToList();
//            Assert.AreEqual(9, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(0, ConditionEventSubscriptionQuery.Count());
//        }

//        [Test]
//        public virtual void testSetVariableInCallActivity()
//        {
//            Engine.ManageDeployment(RepositoryService.CreateDeployment().AddModelInstance(ConditionalModel, DelegatedProcess).Deploy());

//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).CallActivity(TaskWithConditionId).CalledElement(DELEGATED_PROCESS_KEY).UserTask().Name(TASK_AFTER_SERVICE_TASK).EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition)
//                .CallActivity(TaskWithConditionId).CalledElement(DelegatedProcessKey).UserTask().Name(TaskAfterServiceTask).EndEvent().Done();
//            DeployConditionalBoundaryEventProcess(modelInstance, TaskWithConditionId, true);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
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
//        public virtual void testNonInterruptingSetVariableInCallActivity()
//        {
//            Engine.ManageDeployment(RepositoryService.CreateDeployment().AddModelInstance(ConditionalModel, DelegatedProcess).Deploy());

//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).CallActivity(TaskWithConditionId).CalledElement(DELEGATED_PROCESS_KEY).UserTask().Name(TASK_AFTER_SERVICE_TASK).EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).CallActivity(TaskWithConditionId)
//                .CalledElement(DelegatedProcessKey).UserTask().Name(TaskAfterServiceTask).EndEvent().Done();

//            DeployConditionalBoundaryEventProcess(modelInstance, TaskWithConditionId, false);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then service task in call activity sets variable
//            //conditional event is not triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterServiceTask, TasksAfterVariableIsSet[0].Name);
//        }

//        // Todo: EmbeddedSubProcessBuilder
//        //[Test]
//        //public virtual void testSetVariableInSubProcessInDelegatedCode()
//        //{

//        //    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//        //    //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).StartEvent().UserTask(TASK_BEFORE_CONDITION_ID).Name(TASK_BEFORE_CONDITION).SubProcess(SUB_PROCESS_ID)//.EmbeddedSubProcess().StartEvent().ServiceTask().CamundaExpression(EXPR_SET_VARIABLE).UserTask().Name(TASK_AFTER_SERVICE_TASK).EndEvent().SubProcessDone().EndEvent().Done();
//        //    IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//        //        .StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).SubProcess(SubProcessId)
//        //        //.EmbeddedSubProcess().StartEvent()
//        //        .ServiceTask().CamundaExpression(ExprSetVariable).UserTask().Name(TaskAfterServiceTask).EndEvent().SubProcessDone().EndEvent().Done();

//        //    DeployConditionalBoundaryEventProcess(modelInstance, SubProcessId, true);

//        //    // given
//        //    IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//        //    IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId ==procInst.Id);
//        //    ITask task = taskQuery.First();
//        //    Assert.NotNull(task);
//        //    Assert.AreEqual(TaskBeforeCondition, task.Name);

//        //    //when task is completed
//        //    TaskService.Complete(task.Id);

//        //    //then service task in sub process sets variable
//        //    //conditional event is triggered
//        //    TasksAfterVariableIsSet = taskQuery.ToList();
//        //    Assert.AreEqual(TaskAfterCondition, TasksAfterVariableIsSet[0].Name);
//        //}

//        // Todo: EmbeddedSubProcessBuilder
//        //[Test]
//        //public virtual void testNonInterruptingSetVariableInSubProcessInDelegatedCode()
//        //{

//        //    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//        //    //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(CONDITIONAL_EVENT_PROCESS_KEY).StartEvent().UserTask(TASK_BEFORE_CONDITION_ID).Name(TASK_BEFORE_CONDITION).SubProcess(SUB_PROCESS_ID)//.EmbeddedSubProcess().StartEvent().ServiceTask().CamundaExpression(EXPR_SET_VARIABLE).UserTask().Name(TASK_AFTER_SERVICE_TASK).EndEvent().SubProcessDone().EndEvent().Done();
//        //    IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//        //        .StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).SubProcess(SubProcessId)
//        //        //.EmbeddedSubProcess().StartEvent()
//        //        .ServiceTask().CamundaExpression(ExprSetVariable).UserTask().Name(TaskAfterServiceTask).EndEvent().SubProcessDone().EndEvent().Done();

//        //    DeployConditionalBoundaryEventProcess(modelInstance, SubProcessId, false);

//        //    // given
//        //    IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//        //    IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId ==procInst.Id);
//        //    ITask task = taskQuery.First();
//        //    Assert.NotNull(task);
//        //    Assert.AreEqual(TaskBeforeCondition, task.Name);

//        //    //when task is completed
//        //    TaskService.Complete(task.Id);

//        //    //then service task in sub process sets variable
//        //    //conditional event is triggered
//        //    TasksAfterVariableIsSet = taskQuery.ToList();
//        //    Assert.AreEqual(2, TasksAfterVariableIsSet.Count);
//        //    Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());
//        //}

//        // Todo: AbstractConditionalEventDefinitionBuilder<>.ConditionalEventDefinitionDone()
//        //[Test]
//        //public virtual void testSetMultipleVariables()
//        //{

//        //    // given
//        //    IBpmnModelInstance modelInstance = ModifiableBpmnModelInstance.Modify(TaskModel).userTaskBuilder(TaskBeforeConditionId)
//        //        .BoundaryEvent().CancelActivity(true).ConditionalEventDefinition("event1").Condition("${variable1 == 1}")
//        //        .ConditionalEventDefinitionDone().UserTask("afterBoundary1").EndEvent().MoveToActivity(TaskBeforeConditionId).BoundaryEvent()
//        //        .cancelActivity(true).ConditionalEventDefinition("event2").Condition("${variable2 == 1}").ConditionalEventDefinitionDone()
//        //        .UserTask("afterBoundary2").EndEvent().Done();

//        //    Engine.ManageDeployment(RepositoryService.CreateDeployment().AddModelInstance(ConditionalModel, modelInstance).Deploy());
//        //    RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey, Variable.Variables.CreateVariables().PutValue("variable1", "44").PutValue("variable2", "44"));
//        //    ITask task = TaskService.CreateTaskQuery().First();

//        //    // when
//        //    TaskService.SetVariables(task.Id, Variable.Variables.CreateVariables().PutValue("variable1", 1).PutValue("variable2", 1));

//        //    // then
//        //    Assert.AreEqual(1, TaskService.CreateTaskQuery().Count());
//        //    TasksAfterVariableIsSet = TaskService.CreateTaskQuery().ToList();
//        //    string TaskDefinitionKey = TasksAfterVariableIsSet[0].TaskDefinitionKey;
//        //    Assert.True("afterBoundary1".Equals(TaskDefinitionKey) || "afterBoundary2".Equals(TaskDefinitionKey));
//        //}

//        [Test]
//        [Deployment]
//        public virtual void testTrueConditionWithExecutionListener()
//        {
//            //given process with boundary conditional event

//            //when process is started and execution arrives activity with boundary event
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            //then default evaluation behavior triggers conditional event
//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(TaskAfterCondition, TasksAfterVariableIsSet[0].Name);
//            Assert.AreEqual(0, ConditionEventSubscriptionQuery.Count());
//        }

//        [Test]
//        public virtual void testSuspendedProcess()
//        {

//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskWithConditionId).EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent().UserTask(TaskWithConditionId).EndEvent().Done();

//            DeployConditionalBoundaryEventProcess(modelInstance, TaskWithConditionId, true);

//            // given suspended process
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            RuntimeService.SuspendProcessInstanceById(procInst.Id);

//            //when wrong variable is set
//            RuntimeService.SetVariable(procInst.Id, VariableName + 1, 1);

//            //then nothing happens
//            Assert.True(RuntimeService.CreateProcessInstanceQuery().First().IsSuspended);

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
//            TasksAfterVariableIsSet = TaskService.CreateTaskQuery().ToList();
//        }

//        [Test]
//        public virtual void testNonInterruptingConditionalSuspendedProcess()
//        {

//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskWithConditionId).EndEvent().Done();
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent().UserTask(TaskWithConditionId).EndEvent().Done();

//            DeployConditionalBoundaryEventProcess(modelInstance, TaskWithConditionId, false);

//            // given suspended process
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            RuntimeService.SuspendProcessInstanceById(procInst.Id);

//            //when wrong variable is set
//            RuntimeService.SetVariable(procInst.Id, VariableName + 1, 1);

//            //then nothing happens
//            Assert.True(RuntimeService.CreateProcessInstanceQuery().First().IsSuspended);

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
//            TasksAfterVariableIsSet = TaskService.CreateTaskQuery().ToList();
//        }
//    }

//}