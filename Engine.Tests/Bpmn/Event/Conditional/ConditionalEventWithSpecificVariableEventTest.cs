using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Conditional
{
    //// Todo: RunWithAttribute
    ////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    ////ORIGINAL LINE: @RunWith(Parameterized.class) public class ConditionalEventWithSpecificVariableEventTest extends AbstractConditionalEventTestCase
    [TestFixture]
    public class ConditionalEventWithSpecificVariableEventTest : AbstractConditionalEventTestCase
    {
        public interface IConditionalProcessVarSpecification
        {
            IBpmnModelInstance GetProcessWithVarName(bool interrupting, String condition);
            IBpmnModelInstance GetProcessWithVarNameAndEvents(bool interrupting, String varEvent);
            IBpmnModelInstance GetProcessWithVarEvents(bool interrupting, String varEvent);
        }

        //Todo: data()
        //@Parameterized.Parameters(name = "{0}")
        //public static Collection<Object[]> data()
        //{
        //return Arrays.asList(new Object[][] 
        //{
        //  {
        //    //conditional boundary event
        //    new ConditionalProcessVarSpecification()
        //    {
        //        @Override
        //        public IBpmnModelInstance getProcessWithVarName(boolean interrupting, String condition)
        //        {
        //            return ModifiableBpmnModelInstance.Modify(TASK_MODEL)
        //              .UserTaskBuilder(TaskBeforeConditionId)
        //              .BoundaryEvent()
        //              .CancelActivity(interrupting)
        //              .ConditionalEventDefinition(CONDITIONAL_EVENT)
        //              .Condition(condition)
        //              .CamundaVariableName(VARIABLE_NAME)
        //              .ConditionalEventDefinitionDone()
        //              .UserTask()
        //              .Name(TaskAfterCondition)
        //              .EndEvent()
        //              .Done();
        //        }

        //        @Override
        //        public IBpmnModelInstance getProcessWithVarNameAndEvents(boolean interrupting, String varEvent)
        //        {
        //            return ModifiableBpmnModelInstance.Modify(TASK_MODEL)
        //              .UserTaskBuilder(TaskBeforeConditionId)
        //              .BoundaryEvent()
        //              .CancelActivity(interrupting)
        //              .ConditionalEventDefinition(CONDITIONAL_EVENT)
        //              .Condition(CONDITION_EXPR)
        //              .CamundaVariableName(VARIABLE_NAME)
        //              .CamundaVariableEvents(varEvent)
        //              .ConditionalEventDefinitionDone()
        //              .UserTask()
        //              .Name(TaskAfterCondition)
        //              .EndEvent()
        //              .Done();
        //        }

        //        @Override
        //        public IBpmnModelInstance getProcessWithVarEvents(boolean interrupting, String varEvent)
        //        {
        //            return ModifiableBpmnModelInstance.Modify(TASK_MODEL)
        //              .UserTaskBuilder(TaskBeforeConditionId)
        //              .BoundaryEvent()
        //              .CancelActivity(interrupting)
        //              .ConditionalEventDefinition(CONDITIONAL_EVENT)
        //              .Condition(CONDITION_EXPR)
        //              .CamundaVariableEvents(varEvent)
        //              .ConditionalEventDefinitionDone()
        //              .UserTask()
        //              .Name(TaskAfterCondition)
        //              .EndEvent()
        //              .Done();
        //        }

        //        @Override
        //        public String toString()
        //    {
        //        return "ConditionalBoundaryEventWithVarEvents";
        //    }
        //    }
        //},

        //conditional start event of event sub process
        //  {
        //        new ConditionalProcessVarSpecification()
        //        {
        //            @Override
        //            public IBpmnModelInstance getProcessWithVarName(boolean interrupting, String condition)
        //            {
        //            return ModifiableBpmnModelInstance.Modify(TASK_MODEL)
        //              .AddSubProcessTo(ConditionalEventProcessKey)
        //              .TriggerByEvent()
        //              //.EmbeddedSubProcess()
        //              .StartEvent()
        //              .interrupting(interrupting)
        //              .ConditionalEventDefinition(CONDITIONAL_EVENT)
        //              .Condition(condition)
        //              .CamundaVariableName(VARIABLE_NAME)
        //              .ConditionalEventDefinitionDone()
        //              .UserTask()
        //              .Name(TaskAfterCondition)
        //              .EndEvent()
        //              .Done();
        //        }

        //            @Override
        //            public IBpmnModelInstance getProcessWithVarNameAndEvents(boolean interrupting, String varEvent)
        //            {
        //            return ModifiableBpmnModelInstance.Modify(TASK_MODEL)
        //              .AddSubProcessTo(ConditionalEventProcessKey)
        //              .TriggerByEvent()
        //              //.EmbeddedSubProcess()
        //              .StartEvent()
        //              .interrupting(interrupting)
        //              .ConditionalEventDefinition(CONDITIONAL_EVENT)
        //              .Condition(CONDITION_EXPR)
        //              .CamundaVariableName(VARIABLE_NAME)
        //              .CamundaVariableEvents(varEvent)
        //              .ConditionalEventDefinitionDone()
        //              .UserTask()
        //              .Name(TaskAfterCondition)
        //              .EndEvent()
        //              .Done();
        //        }

        //             @Override
        //            public IBpmnModelInstance getProcessWithVarEvents(boolean interrupting, String varEvent)
        //            {
        //            return ModifiableBpmnModelInstance.Modify(TASK_MODEL)
        //              .AddSubProcessTo(ConditionalEventProcessKey)
        //              .TriggerByEvent()
        //              //.EmbeddedSubProcess()
        //              .StartEvent()
        //              .interrupting(interrupting)
        //              .ConditionalEventDefinition(CONDITIONAL_EVENT)
        //              .Condition(CONDITION_EXPR)
        //              .CamundaVariableEvents(varEvent)
        //              .ConditionalEventDefinitionDone()
        //              .UserTask()
        //              .Name(TaskAfterCondition)
        //              .EndEvent()
        //              .Done();
        //        }

        //            @Override
        //            public String toString()
        //            {
        //            return "ConditionalStartEventWithVarEvents";
        //        }
        //        }
        //}
        //}



        // Todo: @Parameterized.Parameter
        public IConditionalProcessVarSpecification Specifier;

        [Test]
        public void TestVariableConditionWithVariableName()
        {

            //given process with boundary conditional event and defined variable name
            IBpmnModelInstance modelInstance = Specifier.GetProcessWithVarName(true, ConditionExpr);
            Engine.ManageDeployment(RepositoryService.CreateDeployment().AddModelInstance(ConditionalModel, modelInstance).Deploy());

            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId ==procInst.Id);
            ITask task = taskQuery.First();
            Assert.IsNotNull(task);
            //AssertNotNull(task);

            //when variable with name `variable1` is set on execution
            TaskService.SetVariable(task.Id, VariableName + 1, 1);

            //then nothing happens
            task = taskQuery.First();
            Assert.IsNotNull(task);
            Assert.Equals(TaskBeforeCondition, task.Name);
            Assert.Equals(1, ConditionEventSubscriptionQuery.Count());


            //when variable with name `variable` is set on execution
            TaskService.SetVariable(task.Id, VariableName, 1);

            //then execution is at user task after conditional event
            TasksAfterVariableIsSet = taskQuery.ToList();
            Assert.Equals(TaskAfterCondition, TasksAfterVariableIsSet.ElementAt(0).Name);
            Assert.Equals(0, ConditionEventSubscriptionQuery.Count());

        }

        [Test]
        public void TestVariableConditionWithVariableNameAndEvent()
        {

            //given process with boundary conditional event and defined variable name and event
            IBpmnModelInstance modelInstance = Specifier.GetProcessWithVarNameAndEvents(true, ConditionalVarEventUpdate);
            Engine.ManageDeployment(RepositoryService.CreateDeployment().AddModelInstance(ConditionalModel, modelInstance).Deploy());

            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId ==procInst.Id);
            ITask task = taskQuery.First();
            Assert.IsNotNull(task);


            //when variable with name `variable` is set on execution
            TaskService.SetVariable(task.Id, VariableName, 1);

            //then nothing happens
            task = taskQuery.First();
            Assert.IsNotNull(task);
            Assert.Equals(TaskBeforeCondition, task.Name);
            Assert.Equals(1, ConditionEventSubscriptionQuery.Count());


            //when variable with name `variable` is updated
            TaskService.SetVariable(task.Id, VariableName, 1);

            //then execution is at user task after conditional event
            TasksAfterVariableIsSet = taskQuery.ToList();
            Assert.Equals(TaskAfterCondition, TasksAfterVariableIsSet.ElementAt(0).Name);
            Assert.Equals(0, ConditionEventSubscriptionQuery.Count());
        }

        [Test]
        public void TestNonInterruptingVariableConditionWithVariableName()
        {

            //given process with non interrupting boundary conditional event and defined variable name and true condition
            IBpmnModelInstance modelInstance = Specifier.GetProcessWithVarName(false, TrueCondition);
            Engine.ManageDeployment(RepositoryService.CreateDeployment().AddModelInstance(ConditionalModel, modelInstance).Deploy());

            //when process is started
            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId ==procInst.Id);

            //then first event is triggered since condition is true
            IList<ITask> tasks = taskQuery.ToList();
            Assert.Equals(2, tasks.Count);

            //when variable with name `variable1` is set on execution
            RuntimeService.SetVariable(procInst.Id, VariableName + 1, 1);

            //then nothing happens
            tasks = taskQuery.ToList();
            Assert.Equals(2, tasks.Count);
            Assert.Equals(1, ConditionEventSubscriptionQuery.Count());

            //when variable with name `variable` is set, updated and deleted
            RuntimeService.SetVariable(procInst.Id, VariableName, 1); //create
            RuntimeService.SetVariable(procInst.Id, VariableName, 1); //update
            RuntimeService.RemoveVariable(procInst.Id, VariableName); //delete

            //then execution is for four times at user task after conditional event
            //one from default behavior and three times from the variable events
            Assert.Equals(4, TaskService.CreateTaskQuery(c=>c.Name ==TaskAfterCondition).Count());
            TasksAfterVariableIsSet = taskQuery.ToList();
            Assert.Equals(5, TasksAfterVariableIsSet.Count);
            Assert.Equals(1, ConditionEventSubscriptionQuery.Count());
        }

        [Test]
        public void TestNonInterruptingVariableConditionWithVariableNameAndEvents()
        {

            //given process with non interrupting boundary conditional event and defined variable name and events
            IBpmnModelInstance modelInstance = Specifier.GetProcessWithVarNameAndEvents(false, ConditionalVarEvents);
            Engine.ManageDeployment(RepositoryService.CreateDeployment().AddModelInstance(ConditionalModel, modelInstance).Deploy());

            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId ==procInst.Id);
            ITask task = taskQuery.First();
            Assert.IsNotNull(task);

            //when variable with name `variable` is set, updated and deleted
            TaskService.SetVariable(task.Id, VariableName, 1); //create
            TaskService.SetVariable(task.Id, VariableName, 1); //update
            TaskService.RemoveVariable(task.Id, VariableName); //delete

            //then execution is for two times at user task after conditional start event
            Assert.Equals(2, TaskService.CreateTaskQuery(c=>c.Name ==TaskAfterCondition).Count());
            TasksAfterVariableIsSet = taskQuery.ToList();
            Assert.Equals(3, TasksAfterVariableIsSet.Count);
            Assert.Equals(1, ConditionEventSubscriptionQuery.Count());
        }


        [Test]
        public void TestVariableConditionWithVariableEvent()
        {

            //given process with boundary conditional event and defined variable event
            IBpmnModelInstance modelInstance = Specifier.GetProcessWithVarEvents(true, ConditionalVarEventUpdate);
            Engine.ManageDeployment(RepositoryService.CreateDeployment().AddModelInstance(ConditionalModel, modelInstance).Deploy());

            IDictionary<string, object> variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables();
            variables[VariableName + 1] = 0;
            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey, variables);
            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId ==procInst.Id);
            ITask task = taskQuery.First();
            Assert.IsNotNull(task);

            //when variable with name `variable` is set on execution
            RuntimeService.SetVariable(procInst.Id, VariableName, 1);

            //then nothing happens
            task = taskQuery.First();
            Assert.IsNotNull(task);
            Assert.Equals(TaskBeforeCondition, task.Name);
            Assert.Equals(1, ConditionEventSubscriptionQuery.Count());

            //when variable with name `variable1` is updated
            RuntimeService.SetVariable(procInst.Id, VariableName + 1, 1);

            //then execution is at user task after conditional intermediate event
            TasksAfterVariableIsSet = taskQuery.ToList();
            Assert.Equals(TaskAfterCondition, TasksAfterVariableIsSet.ElementAt(0).Name);
            Assert.Equals(0, ConditionEventSubscriptionQuery.Count());
        }

        [Test]
        public void TestNonInterruptingVariableConditionWithVariableEvent()
        {

            //given process with non interrupting boundary conditional event and defined variable event
            IBpmnModelInstance modelInstance = Specifier.GetProcessWithVarEvents(false, ConditionalVarEventUpdate);
            Engine.ManageDeployment(RepositoryService.CreateDeployment().AddModelInstance(ConditionalModel, modelInstance).Deploy());

            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId ==procInst.Id);
            ITask task = taskQuery.First();
            Assert.IsNotNull(task);

            //when variable with name `variable` is set
            TaskService.SetVariable(task.Id, VariableName, 1); //create

            //then nothing happens
            task = taskQuery.First();
            Assert.IsNotNull(task);

            //when variable is updated twice
            TaskService.SetVariable(task.Id, VariableName, 1); //update
            TaskService.SetVariable(task.Id, VariableName, 1); //update

            //then execution is for two times at user task after conditional event
            Assert.Equals(2, taskQuery.Where(c=>c.Name==TaskAfterCondition).Count());
            TasksAfterVariableIsSet = TaskService.CreateTaskQuery().ToList();
            Assert.Equals(3, TasksAfterVariableIsSet.Count);
            Assert.Equals(1, ConditionEventSubscriptionQuery.Count());
        }

    }    
}