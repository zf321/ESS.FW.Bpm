//using System;
//using System.Collections.Generic;
//using System.Linq;
//using ESS.FW.Bpm.Engine.Delegate;
//using ESS.FW.Bpm.Engine.Runtime;
//using ESS.FW.Bpm.Engine.Task;
//using ESS.FW.Bpm.Model.Bpmn;
//using ESS.FW.Bpm.Model.Bpmn.instance;
//using ESS.FW.Bpm.Model.Bpmn.instance.camunda;
//using NUnit.Framework;
//using ITask = ESS.FW.Bpm.Engine.Task.ITask;


//namespace ESS.FW.Bpm.Engine.Tests.Bpmn.Event.Conditional
//{
//    // Todo: RunWithAttribute
//    //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//    //ORIGINAL LINE: @RunWith(Parameterized.class) public class TriggerConditionalEventFromDelegationCodeTest extends AbstractConditionalEventTestCase
//    [TestFixture]
//    public class TriggerConditionalEventFromDelegationCodeTest : AbstractConditionalEventTestCase
//    {
//        public interface ConditionalEventProcessSpecifier
//        {
//            Type DelegateClass { get; }
//            int ExpectedInterruptingCount { get; }
//            int ExpectedNonInterruptingCount { get; }
//            string Condition { get; }
//        }


//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Parameterized.Parameters(TestContext.CurrentContext.Test.Name  = "{index}: {0}") public static java.util.Collection<Object[]> data()
//        //public static ICollection<object[]> data()
//        //{
//        //JAVA TO C# CONVERTER TODO TASK: The following anonymous inner class could not be converted:
//        //		return java.util.(new Object[][]{ {new ConditionalEventProcessSpecifier()
//        //	{
//        //		@@Override public Class getDelegateClass()
//        //		{
//        //		  return SetVariableDelegate.class;
//        //		}
//        //
//        //		@@Override public int getExpectedInterruptingCount()
//        //		{
//        //		  return 1;
//        //		}
//        //
//        //		@@Override public int getExpectedNonInterruptingCount()
//        //		{
//        //		  return 1;
//        //		}
//        //
//        //		@@Override public String getCondition()
//        //		{
//        //		  return CONDITION_EXPR;
//        //		}
//        //
//        //		@@Override public String toString()
//        //		{
//        //		  return "SetSingleVariableInDelegate";
//        //		}
//        //	  }
//        //  }
//        //	 ,
//        //	 {
//        //	  new ConditionalEventProcessSpecifierAnonymousInnerClass2(this)
//        //	 }
//        //}
//        //);
//        //}

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Parameterized.Parameter public ConditionalEventProcessSpecifier specifier;

//        public ConditionalEventProcessSpecifier specifier;

//        [Test]
//        public void testSetVariableInStartListener()
//        {
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).UserTask(TaskWithConditionId)
//                .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart, specifier.DelegateClass.Name).EndEvent().Done();
//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, specifier.Condition, true);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then start listener sets variable
//            //conditional event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(specifier.ExpectedInterruptingCount, taskQuery.Where(c => c.Name == TaskAfterCondition).Count());
//        }

//        [Test]
//        public void testNonInterruptingSetVariableInStartListener()
//        {
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent()
//                .UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).UserTask(TaskWithConditionId)
//                .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart, specifier.DelegateClass.Name).Name(TaskWithCondition).EndEvent().Done();

//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, specifier.Condition, false);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);

//            //when task is completed
//            TaskService.Complete(taskQuery.First().Id);

//            //then start listener sets variable
//            //non interrupting boundary event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(1 + specifier.ExpectedNonInterruptingCount, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(specifier.ExpectedNonInterruptingCount, taskQuery.Where(c => c.Name == TaskAfterCondition).Count());
//        }

//        [Test]
//        public void testSetVariableInTakeListener()
//        {
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent().UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).SequenceFlowId(FlowId).UserTask(TaskWithConditionId).EndEvent().Done();
//            ICamundaExecutionListener listener = modelInstance.NewInstance<ICamundaExecutionListener>(typeof(ICamundaExecutionListener));
//            listener.CamundaEvent = ExecutionListenerFields.EventNameTake;
//            listener.CamundaClass = specifier.DelegateClass.Name;
//            modelInstance.GetModelElementById<ISequenceFlow>(FlowId).Builder().AddExtensionElement(listener);
//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, specifier.Condition, true);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then take listener sets variable
//            //conditional event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(specifier.ExpectedInterruptingCount, taskQuery.Where(c => c.Name == TaskAfterCondition).Count());
//        }

//        [Test]
//        public void testNonInterruptingSetVariableInTakeListener()
//        {
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent()
//                .UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).SequenceFlowId(FlowId).UserTask(TaskWithConditionId).EndEvent().Done();
//            ICamundaExecutionListener listener = modelInstance.NewInstance<ICamundaExecutionListener>(typeof(ICamundaExecutionListener));
//            listener.CamundaEvent = ExecutionListenerFields.EventNameTake;
//            listener.CamundaClass = specifier.DelegateClass.Name;
//            modelInstance.GetModelElementById<ISequenceFlow>(FlowId).Builder().AddExtensionElement(listener);
//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, specifier.Condition, false);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then take listener sets variable
//            //non interrupting boundary event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(1 + specifier.ExpectedNonInterruptingCount, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(specifier.ExpectedNonInterruptingCount, taskQuery.Where(c => c.Name == TaskAfterCondition).Count());
//        }

//        [Test]
//        public void testSetVariableInTakeListenerWithAsyncBefore()
//        {
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent()
//                .UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).SequenceFlowId(FlowId).UserTask(TaskWithConditionId)
//                .CamundaAsyncBefore().EndEvent().Done();
//            ICamundaExecutionListener listener = modelInstance.NewInstance<ICamundaExecutionListener>(typeof(ICamundaExecutionListener));
//            listener.CamundaEvent = ExecutionListenerFields.EventNameTake;
//            listener.CamundaClass = specifier.DelegateClass.Name;
//            modelInstance.GetModelElementById<ISequenceFlow>(FlowId).Builder().AddExtensionElement(listener);
//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, specifier.Condition, true);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then take listener sets variable
//            //conditional event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(specifier.ExpectedInterruptingCount, taskQuery.Where(c => c.Name == TaskAfterCondition).Count());
//        }

//        [Test]
//        public void testNonInterruptingSetVariableInTakeListenerWithAsyncBefore()
//        {
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent()
//                .UserTask(TaskBeforeConditionId).Name(TaskBeforeCondition).SequenceFlowId(FlowId).UserTask(TaskWithConditionId).CamundaAsyncBefore().EndEvent().Done();
//            ICamundaExecutionListener listener = modelInstance.NewInstance<ICamundaExecutionListener>(typeof(ICamundaExecutionListener));
//            listener.CamundaEvent = ExecutionListenerFields.EventNameTake;
//            listener.CamundaClass = specifier.DelegateClass.Name;
//            modelInstance.GetModelElementById<ISequenceFlow>(FlowId).Builder().AddExtensionElement(listener);
//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, specifier.Condition, false);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);

//            //when task is completed
//            TaskService.Complete(taskQuery.First().Id);

//            //then take listener sets variable
//            //non interrupting boundary event is triggered
//            Assert.AreEqual(specifier.ExpectedNonInterruptingCount, TaskService.CreateTaskQuery(c => c.Name == TaskAfterCondition).Count());

//            //and job was created
//            IJob job = Engine.ManagementService.CreateJobQuery().First();
//            Assert.NotNull(job);


//            //when job is executed task is created
//            Engine.ManagementService.ExecuteJob(job.Id);
//            //when all tasks are completed
//            Assert.AreEqual(specifier.ExpectedNonInterruptingCount + 1, taskQuery.Count());
//            foreach (ITask task in taskQuery.ToList())
//            {
//                TaskService.Complete(task.Id);
//            }

//            //then no task exist and process instance is ended
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(0, TasksAfterVariableIsSet.Count);
//            Assert.IsNull(RuntimeService.CreateProcessInstanceQuery().First());
//        }

//        [Test]
//        public void testSetVariableInEndListener()
//        {
//            var usertask = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent().UserTask(TaskBeforeConditionId);
//            usertask.Name(TaskBeforeCondition);
//            IBpmnModelInstance modelInstance = usertask.CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd, specifier.DelegateClass.Name).UserTask(TaskWithConditionId).EndEvent().Done();
//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, specifier.Condition, true);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then end listener sets variable
//            //conditional event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(specifier.ExpectedInterruptingCount, taskQuery.Where(c => c.Name == TaskAfterCondition).Count());
//        }

//        [Test]
//        public void testNonInterruptingSetVariableInEndListener()
//        {
//            //IBpmnModelInstance modelInstance = 
//            var userTask = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//             .StartEvent().UserTask(TaskBeforeConditionId);
//            userTask.Name(TaskBeforeCondition);
//            var userTask2 = userTask.CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd, specifier.DelegateClass.Name).UserTask(TaskWithConditionId);
//            userTask2.Name(TaskWithCondition);
//            IBpmnModelInstance modelInstance = userTask2.EndEvent().Done();
//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, specifier.Condition, false);

//            // given
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);

//            //when task is completed
//            TaskService.Complete(taskQuery.First().Id);

//            //then end listener sets variable
//            //non interrupting event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(1 + specifier.ExpectedNonInterruptingCount, TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(specifier.ExpectedNonInterruptingCount, taskQuery.Where(c => c.Name == TaskAfterCondition).Count());
//        }

//        [Test]
//        public void testSetVariableInStartAndEndListener()
//        {
//            //given process with start and end listener on IUser task
//            var usertask = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey).StartEvent().UserTask(TaskBeforeConditionId);
//            usertask.Name(TaskBeforeCondition);
//            IBpmnModelInstance modelInstance = usertask.CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart, specifier.DelegateClass.Name).CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd, specifier.DelegateClass.Name).UserTask(TaskWithConditionId).EndEvent().Done();
//            DeployConditionalEventSubProcess(modelInstance, ConditionalEventProcessKey, specifier.Condition, true);

//            //when process is started
//            IProcessInstance procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            //then start listener sets variable and
//            //execution stays in task after conditional event in event sub process
//            IQueryable<ITask> taskQuery = TaskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);
//            ITask task = taskQuery.First();
//            Assert.AreEqual(TaskAfterCondition, task.Name);
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(specifier.ExpectedInterruptingCount, taskQuery.Where(c => c.Name == TaskAfterCondition).Count());
//        }
//    }

//}