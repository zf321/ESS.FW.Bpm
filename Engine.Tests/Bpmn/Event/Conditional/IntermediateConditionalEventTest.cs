using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Conditional
{
    [TestFixture]
    public class IntermediateConditionalEventTest : AbstractConditionalEventTestCase
    {

        protected internal const string EVENT_BASED_GATEWAY_ID = "egw";
        protected internal const string PARALLEL_GATEWAY_ID = "parallelGateway";
        protected internal const string TASK_BEFORE_SERVICE_TASK_ID = "taskBeforeServiceTask";
        protected internal const string TASK_BEFORE_EVENT_BASED_GW_ID = "taskBeforeEGW";

        public override void CheckIfProcessCanBeFinished()
        {
            //override since check is not needed in intermediate test suite
        }

        [Test]
        public virtual void testFalseCondition()
        {
            //given process with intermediate conditional event
            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery();
            ITask task = taskQuery.Where(c=>c.ProcessInstanceId==procInst.Id).First();
            Assert.NotNull(task);
            Assert.AreEqual(TaskBeforeCondition, task.Name);

            //when task before condition is completed
            TaskService.Complete(task.Id);

            //then next wait state is on conditional event, since condition is false
            //and a condition event subscription is create
            IExecution execution = RuntimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==procInst.Id&& c.ActivityId ==ConditionalEvent).First();
            Assert.NotNull(execution);
            Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());
        }

        [Test]
        [Deployment]
        public virtual void testTrueCondition()
        {
            //given process with intermediate conditional event
            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId ==procInst.Id);
            ITask task = taskQuery.First();
            Assert.NotNull(task);
            Assert.AreEqual(TaskBeforeCondition, task.Name);

            //when task before condition is completed
            TaskService.Complete(task.Id);

            //then next wait state is on IUser task after conditional event, since condition was true
            IExecution execution = RuntimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==procInst.Id&& c.ActivityId ==ConditionalEvent).First();
            Assert.IsNull(execution);

            task = taskQuery.First();
            Assert.NotNull(task);
            Assert.AreEqual(TaskAfterCondition, task.Name);
        }

        [Test]
        [Deployment]
        public virtual void testVariableValue()
        {
            //given process with intermediate conditional event and variable with wrong value
            IDictionary<string, object> variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables();
            variables[VariableName] = 0;
            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey, variables);

            //wait state is on conditional event, since condition is false
            IExecution execution = RuntimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==procInst.Id&& c.ActivityId ==ConditionalEvent).First();
            Assert.NotNull(execution);
            Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());

            //when variable is set to correct value
            RuntimeService.SetVariable(execution.Id, VariableName, 1);

            //then process instance is completed, since condition was true
            execution = RuntimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==procInst.Id&& c.ActivityId ==ConditionalEvent).First();
            Assert.IsNull(execution);

            procInst = RuntimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == ConditionalEventProcessKey).First();
            Assert.IsNull(procInst);
            Assert.AreEqual(0, ConditionEventSubscriptionQuery.Count());
        }

        [Test]
        [Deployment]
        public virtual void testParallelVariableValue()
        {
            //given process with intermediate conditional event and variable with wrong value
            IDictionary<string, object> variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables();
            variables[VariableName] = 0;
            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey, variables);
            IExecution execution1 = RuntimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==procInst.Id&& c.ActivityId ==ConditionalEvent + 1).First();

            IExecution execution2 = RuntimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==procInst.Id&& c.ActivityId ==ConditionalEvent + 2).First();
            Assert.AreEqual(2, ConditionEventSubscriptionQuery.Count());

            //when variable is set to correct value
            RuntimeService.SetVariable(execution1.Id, VariableName, 1);

            //then execution of first conditional event is completed
            execution1 = RuntimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==procInst.Id&& c.ActivityId ==ConditionalEvent + 1).First();
            Assert.IsNull(execution1);
            Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());

            //when second variable is set to correct value
            RuntimeService.SetVariable(execution2.Id, VariableName, 2);

            //then execution and process instance is ended, since both conditions was true
            execution2 = RuntimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==procInst.Id&& c.ActivityId ==ConditionalEvent + 2).First();
            Assert.IsNull(execution2);
            procInst = RuntimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == ConditionalEventProcessKey).First();
            Assert.IsNull(procInst);
            Assert.AreEqual(0, ConditionEventSubscriptionQuery.Count());
        }


        [Test]
        [Deployment]
        public virtual void testParallelVariableValueEqualConditions()
        {
            //given process with intermediate conditional event and variable with wrong value
            IDictionary<string, object> variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables();
            variables[VariableName] = 0;
            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey, variables);

            //when variable is set to correct value
            RuntimeService.SetVariable(procInst.Id, VariableName, 1);

            //then process instance is ended, since both conditions are true
            procInst = RuntimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == ConditionalEventProcessKey).First();
            Assert.IsNull(procInst);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/conditional/IntermediateConditionalEventTest.TestParallelVariableValue.bpmn20.xml" })]
        public virtual void testParallelVariableSetValueOnParent()
        {
            //given process with intermediate conditional event and variable with wrong value
            IDictionary<string, object> variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables();
            variables[VariableName] = 0;
            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey, variables);

            //when variable is set to correct value
            RuntimeService.SetVariable(procInst.Id, VariableName, 1);

            //then execution of conditional event is completed
            IExecution execution = RuntimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==procInst.Id&& c.ActivityId ==ConditionalEvent + 1).First();
            Assert.IsNull(execution);

            //when second variable is set to correct value
            RuntimeService.SetVariable(procInst.Id, VariableName, 2);

            //then execution and process instance is ended, since both conditions was true
            execution = RuntimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==procInst.Id&& c.ActivityId ==ConditionalEvent + 2).First();
            Assert.IsNull(execution);
            procInst = RuntimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == ConditionalEventProcessKey).First();
            Assert.IsNull(procInst);
        }

        [Test]
        [Deployment]
        public virtual void testSubProcessVariableValue()
        {
            //given process with intermediate conditional event and variable with wrong value
            IDictionary<string, object> variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables();
            variables[VariableName] = 0;
            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey, variables);
            IExecution execution = RuntimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==procInst.Id&& c.ActivityId ==ConditionalEvent).First();
            Assert.NotNull(execution);

            //when variable is set to correct value
            RuntimeService.SetVariableLocal(execution.Id, VariableName, 1);

            //then execution and process instance is ended, since condition was true
            execution = RuntimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==procInst.Id&& c.ActivityId ==ConditionalEvent).First();
            Assert.IsNull(execution);
            procInst = RuntimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == ConditionalEventProcessKey).First();
            Assert.IsNull(procInst);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/conditional/IntermediateConditionalEventTest.TestSubProcessVariableValue.bpmn20.xml" })]
        public virtual void testSubProcessVariableSetValueOnParent()
        {
            //given process with intermediate conditional event and variable with wrong value
            IDictionary<string, object> variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables();
            variables[VariableName] = 0;
            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey, variables);

            //when variable is set to correct value
            RuntimeService.SetVariable(procInst.Id, VariableName, 1);

            //then process instance is ended, since condition was true
            procInst = RuntimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == ConditionalEventProcessKey).First();
            Assert.IsNull(procInst);
        }

        [Test]
        [Deployment]
        public virtual void testCleanUpConditionalEventSubscriptions()
        {
            //given process with intermediate conditional event and variable with wrong value
            IDictionary<string, object> variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables();
            variables[VariableName] = 0;
            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey, variables);

            //wait state is on conditional event, since condition is false
            IExecution execution = RuntimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==procInst.Id&& c.ActivityId ==ConditionalEvent).First();
            Assert.NotNull(execution);

            //condition subscription is created
            Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());

            //when variable is set to correct value
            RuntimeService.SetVariable(execution.Id, VariableName, 1);

            //then execution is on next IUser task and the subscription is deleted
            ITask task = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId ==procInst.Id).First();
            Assert.NotNull(task);
            Assert.AreEqual(TaskAfterCondition, task.Name);
            Assert.AreEqual(0, ConditionEventSubscriptionQuery.Count());

            //and task can be completed which ends process instance
            TaskService.Complete(task.Id);
            Assert.IsNull(TaskService.CreateTaskQuery().First());
            Assert.IsNull(RuntimeService.CreateProcessInstanceQuery().First());
        }

        // Todo: AbstractConditionalEventDefinitionBuilder.ConditionalEventDefinitionDone<>()
        //[Test]
        //public virtual void testVariableConditionWithVariableName()
        //{

        //    //given process with boundary conditional event and defined variable name
        //    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //    //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().IntermediateCatchEvent(CONDITIONAL_EVENT).ConditionalEventDefinition().Condition(CONDITION_EXPR).CamundaVariableName(VARIABLE_NAME).ConditionalEventDefinitionDone().UserTask().Name(TaskAfterCondition).EndEvent().Done();
        //    IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
        //        .StartEvent().IntermediateCatchEvent(CONDITIONAL_EVENT).ConditionalEventDefinition().Condition(CONDITION_EXPR).CamundaVariableName(VARIABLE_NAME)
        //        .ConditionalEventDefinitionDone().UserTask().Name(TaskAfterCondition).EndEvent().Done();

        //    engine.ManageDeployment(repositoryService.CreateDeployment().AddModelInstance(CONDITIONAL_MODEL, modelInstance).Deploy());

        //    IProcessInstance procInst = runtimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
        //    IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==procInst.Id);
        //    IExecution execution = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==procInst.Id&& c.ActivityId ==CONDITIONAL_EVENT).First();
        //    Assert.NotNull(execution);
        //    Assert.AreEqual(1, conditionEventSubscriptionQuery.Count());

        //    //when variable with name `variable1` is set on execution
        //    runtimeService.SetVariable(procInst.Id, VARIABLE_NAME + 1, 1);

        //    //then nothing happens
        //    execution = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==procInst.Id&& c.ActivityId ==CONDITIONAL_EVENT).First();
        //    Assert.NotNull(execution);
        //    Assert.AreEqual(1, conditionEventSubscriptionQuery.Count());

        //    //when variable with name `variable` is set on execution
        //    runtimeService.SetVariable(procInst.Id, VARIABLE_NAME, 1);

        //    //then execution is at IUser task after conditional intermediate event
        //    ITask task = taskQuery.First();
        //    Assert.AreEqual(TaskAfterCondition, task.Name);
        //    Assert.AreEqual(0, conditionEventSubscriptionQuery.Count());

        //    //and task can be completed which ends process instance
        //    taskService.Complete(task.Id);
        //    Assert.IsNull(taskService.CreateTaskQuery().First());
        //    Assert.IsNull(runtimeService.CreateProcessInstanceQuery().First());
        //}

        // Todo: AbstractConditionalEventDefinitionBuilder.ConditionalEventDefinitionDone<>()
        //[Test]
        //public virtual void testVariableConditionWithVariableEvent()
        //{

        //    //given process with boundary conditional event and defined variable name and event
        //    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //    //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().IntermediateCatchEvent(CONDITIONAL_EVENT).ConditionalEventDefinition().Condition(CONDITION_EXPR).CamundaVariableEvents(CONDITIONAL_VAR_EVENT_UPDATE).ConditionalEventDefinitionDone().UserTask().Name(TaskAfterCondition).EndEvent().Done();
        //    IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
        //        .StartEvent().IntermediateCatchEvent(CONDITIONAL_EVENT).ConditionalEventDefinition().Condition(CONDITION_EXPR)
        //        .CamundaVariableEvents(CONDITIONAL_VAR_EVENT_UPDATE)
        //        .ConditionalEventDefinitionDone().UserTask().Name(TaskAfterCondition).EndEvent().Done();

        //    engine.ManageDeployment(repositoryService.CreateDeployment().AddModelInstance(CONDITIONAL_MODEL, modelInstance).Deploy());

        //    IDictionary<string, object> variables = Variable.Variables.CreateVariables();
        //    variables[VARIABLE_NAME + 1] = 0;
        //    IProcessInstance procInst = runtimeService.StartProcessInstanceByKey(ConditionalEventProcessKey, variables);
        //    IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==procInst.Id);
        //    IExecution execution = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==procInst.Id&& c.ActivityId ==CONDITIONAL_EVENT).First();
        //    Assert.NotNull(execution);
        //    Assert.AreEqual(1, conditionEventSubscriptionQuery.Count());


        //    //when variable with name `variable` is set on execution
        //    runtimeService.SetVariable(procInst.Id, VARIABLE_NAME, 1);

        //    //then nothing happens
        //    execution = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==procInst.Id&& c.ActivityId ==CONDITIONAL_EVENT).First();
        //    Assert.NotNull(execution);
        //    Assert.AreEqual(1, conditionEventSubscriptionQuery.Count());

        //    //when variable with name `variable1` is updated
        //    runtimeService.SetVariable(procInst.Id, VARIABLE_NAME + 1, 1);

        //    //then execution is at IUser task after conditional intermediate event
        //    ITask task = taskQuery.First();
        //    Assert.AreEqual(TaskAfterCondition, task.Name);
        //    Assert.AreEqual(0, conditionEventSubscriptionQuery.Count());

        //    //and task can be completed which ends process instance
        //    taskService.Complete(task.Id);
        //    Assert.IsNull(taskService.CreateTaskQuery().First());
        //    Assert.IsNull(runtimeService.CreateProcessInstanceQuery().First());
        //}

        // Todo: AbstractConditionalEventDefinitionBuilder.ConditionalEventDefinitionDone<>()
        //[Test]
        //public virtual void testVariableConditionWithVariableNameAndEvent()
        //{

        //    //given process with boundary conditional event and defined variable name and event
        //    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //    //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().IntermediateCatchEvent(CONDITIONAL_EVENT).ConditionalEventDefinition().Condition(CONDITION_EXPR).CamundaVariableName(VARIABLE_NAME).CamundaVariableEvents(CONDITIONAL_VAR_EVENT_UPDATE).ConditionalEventDefinitionDone().UserTask().Name(TaskAfterCondition).EndEvent().Done();
        //    IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
        //        .StartEvent().IntermediateCatchEvent(CONDITIONAL_EVENT).ConditionalEventDefinition().Condition(CONDITION_EXPR)
        //        .CamundaVariableName(VARIABLE_NAME).CamundaVariableEvents(CONDITIONAL_VAR_EVENT_UPDATE)
        //        .ConditionalEventDefinitionDone().UserTask().Name(TaskAfterCondition).EndEvent().Done();

        //    engine.ManageDeployment(repositoryService.CreateDeployment().AddModelInstance(CONDITIONAL_MODEL, modelInstance).Deploy());
        //    IProcessInstance procInst = runtimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
        //    IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==procInst.Id);
        //    IExecution execution = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==procInst.Id&& c.ActivityId ==CONDITIONAL_EVENT).First();
        //    Assert.NotNull(execution);
        //    Assert.AreEqual(1, conditionEventSubscriptionQuery.Count());


        //    //when variable with name `variable` is set on execution
        //    runtimeService.SetVariable(procInst.Id, VARIABLE_NAME, 1);

        //    //then nothing happens
        //    execution = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==procInst.Id&& c.ActivityId ==CONDITIONAL_EVENT).First();
        //    Assert.NotNull(execution);
        //    Assert.AreEqual(1, conditionEventSubscriptionQuery.Count());

        //    //when variable with name `variable` is updated
        //    runtimeService.SetVariable(procInst.Id, VARIABLE_NAME, 1);

        //    //then execution is at IUser task after conditional intermediate event
        //    ITask task = taskQuery.First();
        //    Assert.AreEqual(TaskAfterCondition, task.Name);
        //    Assert.AreEqual(0, conditionEventSubscriptionQuery.Count());

        //    //and task can be completed which ends process instance
        //    taskService.Complete(task.Id);
        //    Assert.IsNull(taskService.CreateTaskQuery().First());
        //    Assert.IsNull(runtimeService.CreateProcessInstanceQuery().First());
        //}

        // Todo: AbstractConditionalEventDefinitionBuilder.ConditionalEventDefinitionDone<>()
        //[Test]
        //public virtual void testSuspendedProcess()
        //{

        //    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //    //ORIGINAL LINE: final org.Camunda.bpm.Model.bpmn.IBpmnModelInstance modelInstance = org.Camunda.bpm.Model.bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().IntermediateCatchEvent(CONDITIONAL_EVENT).ConditionalEventDefinition().Condition(CONDITION_EXPR).ConditionalEventDefinitionDone().EndEvent().Done();
        //    IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().IntermediateCatchEvent(CONDITIONAL_EVENT)
        //        .ConditionalEventDefinition().Condition(CONDITION_EXPR)
        //        .ConditionalEventDefinitionDone().EndEvent().Done();

        //    engine.ManageDeployment(repositoryService.CreateDeployment().AddModelInstance(CONDITIONAL_MODEL, modelInstance).Deploy());
        //    // given suspended process
        //    IProcessInstance procInst = runtimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
        //    runtimeService.SuspendProcessInstanceById(procInst.Id);

        //    //when wrong variable is set
        //    runtimeService.SetVariable(procInst.Id, VARIABLE_NAME + 1, 1);

        //    //then nothing happens
        //    Assert.True(runtimeService.CreateProcessInstanceQuery().First().Suspended);

        //    //when variable which triggers condition is set
        //    //then exception is expected
        //    try
        //    {
        //        runtimeService.SetVariable(procInst.Id, VARIABLE_NAME, 1);
        //        Assert.Fail("Should Assert.Fail!");
        //    }
        //    catch (SuspendedEntityInteractionException)
        //    {
        //        //expected
        //    }
        //    runtimeService.ActivateProcessInstanceById(procInst.Id);
        //    tasksAfterVariableIsSet = taskService.CreateTaskQuery().ToList();
        //}

        // Todo: AbstractConditionalEventDefinitionBuilder.ConditionalEventDefinitionDone<>()
        //[Test]
        //public virtual void testEventBasedGateway()
        //{
        //    IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
        //        .StartEvent().EventBasedGateway().Id(EVENT_BASED_GATEWAY_ID).IntermediateCatchEvent(CONDITIONAL_EVENT)
        //        .ConditionalEventDefinition().Condition(CONDITION_EXPR)
        //        .ConditionalEventDefinitionDone().UserTask().Name(TaskAfterCondition).EndEvent().Done();

        //    engine.ManageDeployment(repositoryService.CreateDeployment().AddModelInstance(CONDITIONAL_MODEL, modelInstance).Deploy());

        //    //given
        //    IProcessInstance procInst = runtimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
        //    IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==procInst.Id);
        //    Assert.AreEqual(1, conditionEventSubscriptionQuery.Count());
        //    IExecution execution = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==procInst.Id&& c.ActivityId ==EVENT_BASED_GATEWAY_ID).First();
        //    Assert.NotNull(execution);

        //    //when variable is set on execution
        //    runtimeService.SetVariable(procInst.Id, VARIABLE_NAME, 1);

        //    //then execution is at IUser task after intermediate conditional event
        //    ITask task = taskQuery.First();
        //    Assert.AreEqual(TaskAfterCondition, task.Name);
        //    Assert.AreEqual(0, conditionEventSubscriptionQuery.Count());
        //}

        // Todo: AbstractConditionalEventDefinitionBuilder.ConditionalEventDefinitionDone<>()
        //[Test]
        //public virtual void testEventBasedGatewayTrueCondition()
        //{
        //    IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
        //        .StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).EventBasedGateway().Id(EVENT_BASED_GATEWAY_ID)
        //        .IntermediateCatchEvent(CONDITIONAL_EVENT).ConditionalEventDefinition().Condition(TRUE_CONDITION)
        //        .ConditionalEventDefinitionDone().UserTask().Name(TaskAfterCondition).EndEvent().Done();

        //    engine.ManageDeployment(repositoryService.CreateDeployment().AddModelInstance(CONDITIONAL_MODEL, modelInstance).Deploy());

        //    //given
        //    IProcessInstance procInst = runtimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
        //    IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==procInst.Id);
        //    ITask task = taskQuery.First();

        //    //when task before condition is completed
        //    taskService.Complete(task.Id);

        //    //then next wait state is on IUser task after conditional event, since condition was true
        //    IExecution execution = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==procInst.Id&& c.ActivityId ==EVENT_BASED_GATEWAY_ID).First();
        //    Assert.IsNull(execution);

        //    task = taskQuery.First();
        //    Assert.NotNull(task);
        //    Assert.AreEqual(TaskAfterCondition, task.Name);
        //}

        // Todo: AbstractConditionalEventDefinitionBuilder.ConditionalEventDefinitionDone<>()
        //[Test]
        //public virtual void testEventBasedGatewayWith2ConditionsOneIsTrue()
        //{
        //    IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
        //        .StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).EventBasedGateway().Id(EVENT_BASED_GATEWAY_ID)
        //        .IntermediateCatchEvent().ConditionalEventDefinition().Condition(CONDITION_EXPR)
        //        .ConditionalEventDefinitionDone().UserTask().Name(TaskAfterCondition + 1).EndEvent().MoveToLastGateway().IntermediateCatchEvent(CONDITIONAL_EVENT).ConditionalEventDefinition().Condition(TRUE_CONDITION).ConditionalEventDefinitionDone().UserTask().Name(TaskAfterCondition + 2).EndEvent().Done();

        //    engine.ManageDeployment(repositoryService.CreateDeployment().AddModelInstance(CONDITIONAL_MODEL, modelInstance).Deploy());

        //    //given
        //    IProcessInstance procInst = runtimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
        //    IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==procInst.Id);
        //    ITask task = taskQuery.First();

        //    //when task before condition is completed
        //    taskService.Complete(task.Id);

        //    //then next wait state is on IUser task after true conditional event
        //    IExecution execution = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==procInst.Id&& c.ActivityId ==EVENT_BASED_GATEWAY_ID).First();
        //    Assert.IsNull(execution);

        //    task = taskQuery.First();
        //    Assert.NotNull(task);
        //    Assert.AreEqual(TaskAfterCondition + 2, task.Name);
        //}

        // Todo: AbstractConditionalEventDefinitionBuilder.ConditionalEventDefinitionDone<>()
        //[Test]
        //public virtual void testEventBasedGatewayWith2VarConditions()
        //{
        //    IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
        //        .StartEvent().EventBasedGateway().Id(EVENT_BASED_GATEWAY_ID).IntermediateCatchEvent().ConditionalEventDefinition().Condition(CONDITION_EXPR)
        //        .ConditionalEventDefinitionDone().UserTask().Name(TaskAfterCondition + 1).EndEvent().MoveToLastGateway().IntermediateCatchEvent(CONDITIONAL_EVENT).ConditionalEventDefinition().Condition("${var==2}").ConditionalEventDefinitionDone().UserTask().Name(TaskAfterCondition + 2).EndEvent().Done();

        //    engine.ManageDeployment(repositoryService.CreateDeployment().AddModelInstance(CONDITIONAL_MODEL, modelInstance).Deploy());

        //    //given
        //    IProcessInstance procInst = runtimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
        //    IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==procInst.Id);
        //    IExecution execution = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==procInst.Id&& c.ActivityId ==EVENT_BASED_GATEWAY_ID).First();
        //    Assert.NotNull(execution);

        //    //when wrong value of variable `var` is set
        //    runtimeService.SetVariable(procInst.Id, "var", 1);

        //    //then nothing happens
        //    execution = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==procInst.Id&& c.ActivityId ==EVENT_BASED_GATEWAY_ID).First();
        //    Assert.NotNull(execution);
        //    Assert.AreEqual(0, taskQuery.Count());

        //    //when right value is set
        //    runtimeService.SetVariable(procInst.Id, "var", 2);

        //    //then next wait state is on IUser task after second conditional event
        //    ITask task = taskQuery.First();
        //    Assert.NotNull(task);
        //    Assert.AreEqual(TaskAfterCondition + 2, task.Name);
        //}

        // Todo: AbstractConditionalEventDefinitionBuilder.ConditionalEventDefinitionDone<>()
        //protected internal virtual void deployParallelProcessWithEventBasedGateway()
        //{
        //    //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
        //    IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
        //        .StartEvent().ParallelGateway().Id(PARALLEL_GATEWAY_ID).UserTask(TASK_BEFORE_EVENT_BASED_GW_ID).EventBasedGateway()
        //        .Id(EVENT_BASED_GATEWAY_ID).IntermediateCatchEvent().ConditionalEventDefinition().Condition(CONDITION_EXPR)
        //        .ConditionalEventDefinitionDone().UserTask().Name(TaskAfterCondition).EndEvent().MoveToNode(PARALLEL_GATEWAY_ID).UserTask(TASK_BEFORE_SERVICE_TASK_ID).ServiceTask().CamundaClass(typeof(SetVariableDelegate).FullName).EndEvent().Done();

        //    engine.ManageDeployment(repositoryService.CreateDeployment().AddModelInstance(CONDITIONAL_MODEL, modelInstance).Deploy());
        //}

        //[Test]
        //public virtual void testParallelProcessWithSetVariableBeforeReachingEventBasedGW()
        //{
        //    deployParallelProcessWithEventBasedGateway();
        //    //given
        //    IProcessInstance procInst = runtimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
        //    IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==procInst.Id);
        //    ITask taskBeforeEGW = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey ==TASK_BEFORE_EVENT_BASED_GW_ID).First();
        //    ITask taskBeforeServiceTask = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey ==TASK_BEFORE_SERVICE_TASK_ID).First();

        //    //when task before service task is completed and after that task before event based gateway
        //    taskService.Complete(taskBeforeServiceTask.Id);
        //    taskService.Complete(taskBeforeEGW.Id);

        //    //then variable is set before event based gateway is reached
        //    //on reaching event based gateway condition of conditional event is also evaluated to true
        //    ITask task = taskQuery.First();
        //    Assert.NotNull(task);
        //    Assert.AreEqual(TaskAfterCondition, task.Name);
        //    //completing this task ends process instance
        //    taskService.Complete(task.Id);
        //    Assert.IsNull(taskQuery.First());
        //    Assert.IsNull(runtimeService.CreateProcessInstanceQuery().First());
        //}

        //[Test]
        //public virtual void testParallelProcessWithSetVariableAfterReachingEventBasedGW()
        //{
        //    deployParallelProcessWithEventBasedGateway();
        //    //given
        //    IProcessInstance procInst = runtimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
        //    IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==procInst.Id);
        //    ITask taskBeforeEGW = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey ==TASK_BEFORE_EVENT_BASED_GW_ID).First();
        //    ITask taskBeforeServiceTask = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey ==TASK_BEFORE_SERVICE_TASK_ID).First();

        //    //when task before event based gateway is completed and after that task before service task
        //    taskService.Complete(taskBeforeEGW.Id);
        //    taskService.Complete(taskBeforeServiceTask.Id);

        //    //then event based gateway is reached and executions stays there
        //    //variable is set after reaching event based gateway
        //    //after setting variable the conditional event is triggered and evaluated to true
        //    ITask task = taskQuery.First();
        //    Assert.NotNull(task);
        //    Assert.AreEqual(TaskAfterCondition, task.Name);
        //    //completing this task ends process instance
        //    taskService.Complete(task.Id);
        //    Assert.IsNull(taskQuery.First());
        //    Assert.IsNull(runtimeService.CreateProcessInstanceQuery().First());
        //}
    }

}