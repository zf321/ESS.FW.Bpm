//using System.Collections.Generic;
//using System.Linq;
//using ESS.FW.Bpm.Engine.Delegate;
//using ESS.FW.Bpm.Model.Bpmn;
//using ESS.FW.Bpm.Model.Bpmn.instance;
//using ESS.FW.Bpm.Model.Bpmn.instance.camunda;
//using NUnit.Framework;
//using ITask = ESS.FW.Bpm.Engine.Task.ITask;

//namespace ESS.FW.Bpm.Engine.Tests.Bpmn.Event.Conditional
//{
//    // Todo:RunWithAttribuite
//    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//    //ORIGINAL LINE: @RunWith(Parameterized.class) public class ConditionalEventTriggeredByExecutionListenerTest extends AbstractConditionalEventTestCase
//    [TestFixture]
//    public class ConditionalEventTriggeredByExecutionListenerTest : AbstractConditionalEventTestCase
//    {
//        protected internal const string TaskAfterConditionalBoundaryEvent = "ITask after conditional boundary event";
//        protected internal const string TaskAfterConditionalStartEvent = "ITask after conditional start event";
//        protected internal const string StartEventId = "startEventId";
//        protected internal const string EndEventId = "endEventId";

//        public interface IConditionalEventProcessSpecifier
//        {
//            IBpmnModelInstance SpecifyConditionalProcess(IBpmnModelInstance modelInstance, bool isInterrupting);
//            void AssertTaskNames(IList<ITask> tasks, bool isInterrupting, bool isAyncBefore);
//            int ExpectedSubscriptions();
//            int ExpectedTaskCount();
//        }

//        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Parameterized.Parameters(TestContext.CurrentContext.Test.Name  = "{index}: {0}") public static java.util.Collection<Object[]> data()
//        //public static ICollection<object[]> data()
//        //{
//        //JAVA TO C# CONVERTER TODO Resources.Task: The following anonymous inner class could not be converted:
//        //		return java.util.(new Object[][]{ {new ConditionalEventProcessSpecifier()
//        //	{
//        //		@@Override public IBpmnModelInstance specifyConditionalProcess(IBpmnModelInstance modelInstance, boolean isInterrupting)
//        //		{
//        //		  return ModifiableBpmnModelInstance.Modify(modelInstance).AddSubProcessTo(ConditionalEventProcessKey).TriggerByEvent()//.EmbeddedSubProcess().StartEvent().interrupting(isInterrupting).ConditionalEventDefinition().Condition(CONDITION_EXPR).ConditionalEventDefinitionDone().UserTask().Name(TaskAfterCondition).EndEvent().Done();
//        //		}
//        //
//        //		@@Override public void AssertTaskNames(List<ITask> tasks, boolean isInterrupting, boolean isAyncBefore)
//        //		{
//        //			if (isInterrupting || isAyncBefore)
//        //			{
//        //			  ConditionalEventTriggeredByExecutionListenerTest.AssertTaskNames(tasks, TaskAfterCondition);
//        //			}
//        //			else
//        //			{
//        //			  ConditionalEventTriggeredByExecutionListenerTest.AssertTaskNames(tasks, TaskWithCondition, TaskAfterCondition);
//        //			}
//        //		}
//        //
//        //		@@Override public int expectedSubscriptions()
//        //		{
//        //		  return 1;
//        //		}
//        //
//        //		@@Override public int expectedTaskCount()
//        //		{
//        //		  return 2;
//        //		}
//        //
//        //		@@Override public String toString()
//        //		{
//        //		  return "ConditionalEventSubProcess";
//        //		}
//        //	  }
//        //  }
//        // ,
//        // {
//        //	  new ConditionalEventProcessSpecifierAnonymousInnerClass2(this)
//        // }
//        //}
//        //);
//        //}

//        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Parameterized.Parameter public ConditionalEventProcessSpecifier specifier;
//        public IConditionalEventProcessSpecifier Specifier;

//        [Test]
//        public void TestNonInterruptingSetVariableInEndListener()
//        {
//            var modelInstance =
//                Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                    .StartEvent(StartEventId)
//                    .UserTask(TaskBeforeConditionId)
//                    .Name(TaskBeforeCondition)
//                    .CamundaExecutionListenerExpression(ExecutionListenerFields.EventNameEnd, ExprSetVariable)
//                    .UserTask(TaskWithConditionId)
//                    .Name(TaskWithCondition)
//                    .EndEvent(EndEventId)
//                    .Done();
//            modelInstance = Specifier.SpecifyConditionalProcess(modelInstance, false);
//            Engine.ManageDeployment(
//                RepositoryService.CreateDeployment()
//                    .AddModelInstance(ConditionalModel, modelInstance)
//                    .Deploy());

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);

//            //when task is completed
//            TaskService.Complete(taskQuery.First()
//                .Id);

//            //then end listener sets variable
//            //non interrupting boundary event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(Specifier.ExpectedTaskCount(), TasksAfterVariableIsSet.Count);
//            Specifier.AssertTaskNames(TasksAfterVariableIsSet, false, false);
//        }

//        [Test]
//        public void TestNonInterruptingSetVariableInStartListener()
//        {
//            var modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent(StartEventId)
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .UserTask(TaskWithConditionId)
//                .CamundaExecutionListenerExpression(ExecutionListenerFields.EventNameStart, ExprSetVariable)
//                .Name(TaskWithCondition)
//                .EndEvent(EndEventId)
//                .Done();
//            modelInstance = Specifier.SpecifyConditionalProcess(modelInstance, false);
//            Engine.ManageDeployment(
//                RepositoryService.CreateDeployment()
//                    .AddModelInstance(ConditionalModel, modelInstance)
//                    .Deploy());

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);
//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);

//            //when task is completed
//            TaskService.Complete(taskQuery.First()
//                .Id);

//            //then start listener sets variable
//            //non interrupting boundary event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(Specifier.ExpectedTaskCount(), TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(Specifier.ExpectedSubscriptions(), ConditionEventSubscriptionQuery
//                .Count());
//            Specifier.AssertTaskNames(TasksAfterVariableIsSet, false, false);
//        }

//        [Test]
//        public void TestNonInterruptingSetVariableInTakeListener()
//        {
//            var modelInstance =
//                Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                    .StartEvent(StartEventId)
//                    .UserTask(TaskBeforeConditionId)
//                    .Name(TaskBeforeCondition)
//                    .SequenceFlowId(FlowId)
//                    .UserTask(TaskWithConditionId)
//                    .Name(TaskWithCondition)
//                    .EndEvent(EndEventId)
//                    .Done();
//            var listener = modelInstance.NewInstance<ICamundaExecutionListener>(typeof(ICamundaExecutionListener));
//            listener.CamundaEvent = ExecutionListenerFields.EventNameTake;
//            listener.CamundaExpression = ExprSetVariable;
//            modelInstance.GetModelElementById<ISequenceFlow>(FlowId)
//                .Builder()
//                .AddExtensionElement(listener);
//            modelInstance = Specifier.SpecifyConditionalProcess(modelInstance, false);
//            Engine.ManageDeployment(
//                RepositoryService.CreateDeployment()
//                    .AddModelInstance(ConditionalModel, modelInstance)
//                    .Deploy());

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then take listener sets variable
//            //non interrupting boundary event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(Specifier.ExpectedTaskCount(), TasksAfterVariableIsSet.Count);
//            Assert.AreEqual(Specifier.ExpectedSubscriptions(), ConditionEventSubscriptionQuery.Count());
//            Specifier.AssertTaskNames(TasksAfterVariableIsSet, false, false);
//        }

//        [Test]
//        public void TestNonInterruptingSetVariableInTakeListenerWithAsyncBefore()
//        {
//            var modelInstance =
//                Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                    .StartEvent(StartEventId)
//                    .UserTask(TaskBeforeConditionId)
//                    .Name(TaskBeforeCondition)
//                    .SequenceFlowId(FlowId)
//                    .UserTask(TaskWithConditionId)
//                    .Name(TaskWithCondition)
//                    .CamundaAsyncBefore()
//                    .EndEvent(EndEventId)
//                    .Done();
//            var listener = modelInstance.NewInstance<ICamundaExecutionListener>(typeof(ICamundaExecutionListener));
//            listener.CamundaEvent = ExecutionListenerFields.EventNameTake;
//            listener.CamundaExpression = ExprSetVariable;
//            modelInstance.GetModelElementById<ISequenceFlow>(FlowId)
//                .Builder()
//                .AddExtensionElement(listener);
//            modelInstance = Specifier.SpecifyConditionalProcess(modelInstance, false);
//            Engine.ManageDeployment(
//                RepositoryService.CreateDeployment()
//                    .AddModelInstance(ConditionalModel, modelInstance)
//                    .Deploy());

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);

//            //when task is completed
//            TaskService.Complete(taskQuery.First()
//                .Id);

//            //then take listener sets variable
//            //non interrupting boundary event is triggered
//            Specifier.AssertTaskNames(taskQuery.ToList(), false, true);

//            //and job was created
//            var job = Engine.ManagementService.CreateJobQuery()
//                .First();
//            Assert.NotNull(job);
//            Assert.AreEqual(1, ConditionEventSubscriptionQuery.Count());

//            //when job is executed task is created
//            Engine.ManagementService.ExecuteJob(job.Id);
//            //when tasks are completed
//            foreach (var task in taskQuery.ToList())
//                TaskService.Complete(task.Id);

//            //then no task exist and process instance is ended
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Assert.AreEqual(0, TasksAfterVariableIsSet.Count);
//            Assert.IsNull(RuntimeService.CreateProcessInstanceQuery()
//                .First());
//        }

//        [Test]
//        public void testNonInterruptingSetVariableOnParentScopeInEndListener()
//        {
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent(StartEventId)
//                .SubProcess()
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .CamundaExecutionListenerExpression(ExecutionListenerFields.EventNameEnd, ExprSetVariableOnParent)
//                .UserTask(TaskWithConditionId)
//                .Name(TaskWithCondition)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent(EndEventId)
//                .Done();

//            modelInstance = Specifier.SpecifyConditionalProcess(modelInstance, false);
//            Engine.ManageDeployment(
//                RepositoryService.CreateDeployment()
//                    .AddModelInstance(ConditionalModel, modelInstance)
//                    .Deploy());

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then end listener sets variable
//            //non interrupting boundary event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Specifier.AssertTaskNames(TasksAfterVariableIsSet, false, false);
//        }

//        [Test]
//        public void testNonInterruptingSetVariableOnParentScopeInStartListener()
//        {
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent(StartEventId)
//                .SubProcess()
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .UserTask(TaskWithConditionId)
//                .Name(TaskWithCondition)
//                .CamundaExecutionListenerExpression(ExecutionListenerFields.EventNameStart, ExprSetVariableOnParent)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent(EndEventId)
//                .Done();
//            modelInstance = Specifier.SpecifyConditionalProcess(modelInstance, false);
//            Engine.ManageDeployment(
//                RepositoryService.CreateDeployment()
//                    .AddModelInstance(ConditionalModel, modelInstance)
//                    .Deploy());

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then start listener sets variable
//            //non interrupting boundary event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Specifier.AssertTaskNames(TasksAfterVariableIsSet, false, false);
//        }

//        [Test]
//        public void testNonInterruptingSetVariableOnParentScopeInTakeListener()
//        {
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent(StartEventId)
//                .SubProcess()
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .SequenceFlowId(FlowId)
//                .UserTask(TaskWithConditionId)
//                .Name(TaskWithCondition)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent(EndEventId)
//                .Done();

//            var listener = modelInstance.NewInstance<ICamundaExecutionListener>(typeof(ICamundaExecutionListener));
//            listener.CamundaEvent = ExecutionListenerFields.EventNameTake;
//            listener.CamundaExpression = ExprSetVariableOnParent;
//            modelInstance.GetModelElementById<ISequenceFlow>(FlowId)
//                .Builder()
//                .AddExtensionElement(listener);
//            modelInstance = Specifier.SpecifyConditionalProcess(modelInstance, false);
//            Engine.ManageDeployment(
//                RepositoryService.CreateDeployment()
//                    .AddModelInstance(ConditionalModel, modelInstance)
//                    .Deploy());

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then start listener sets variable
//            //non interrupting boundary event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Specifier.AssertTaskNames(TasksAfterVariableIsSet, false, false);
//        }

//        [Test]
//        public void TestSetVariableInEndListener()
//        {
//            var modelInstance =
//                Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                    .StartEvent(StartEventId)
//                    .UserTask(TaskBeforeConditionId)
//                    .Name(TaskBeforeCondition)
//                    .CamundaExecutionListenerExpression(ExecutionListenerFields.EventNameEnd, ExprSetVariable)
//                    .UserTask(TaskWithConditionId)
//                    .Name(TaskWithCondition)
//                    .EndEvent(EndEventId)
//                    .Done();
//            modelInstance = Specifier.SpecifyConditionalProcess(modelInstance, true);
//            Engine.ManageDeployment(
//                RepositoryService.CreateDeployment()
//                    .AddModelInstance(ConditionalModel, modelInstance)
//                    .Deploy());

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then end listener sets variable
//            //conditional event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Specifier.AssertTaskNames(TasksAfterVariableIsSet, true, false);
//        }


//        [Test]
//        public void TestSetVariableInStartListener()
//        {
//            var modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent(StartEventId)
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .UserTask(TaskWithConditionId)
//                .Name(TaskWithCondition)
//                .CamundaExecutionListenerExpression(ExecutionListenerFields.EventNameStart, ExprSetVariable)
//                .EndEvent(EndEventId)
//                .Done();
//            modelInstance = Specifier.SpecifyConditionalProcess(modelInstance, true);
//            Engine.ManageDeployment(
//                RepositoryService.CreateDeployment()
//                    .AddModelInstance(ConditionalModel, modelInstance)
//                    .Deploy());

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then start listener sets variable
//            //conditional event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Specifier.AssertTaskNames(TasksAfterVariableIsSet, true, false);
//        }

//        [Test]
//        public void TestSetVariableInTakeListener()
//        {
//            var modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent(StartEventId)
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .SequenceFlowId(FlowId)
//                .UserTask(TaskWithConditionId)
//                .Name(TaskWithCondition)
//                .EndEvent(EndEventId)
//                .Done();
//            var listener = modelInstance.NewInstance<ICamundaExecutionListener>(typeof(ICamundaExecutionListener));
//            listener.CamundaEvent = ExecutionListenerFields.EventNameTake;
//            listener.CamundaExpression = ExprSetVariable;
//            modelInstance.GetModelElementById<ISequenceFlow>(FlowId)
//                .Builder()
//                .AddExtensionElement(listener);
//            modelInstance = Specifier.SpecifyConditionalProcess(modelInstance, true);
//            Engine.ManageDeployment(
//                RepositoryService.CreateDeployment()
//                    .AddModelInstance(ConditionalModel, modelInstance)
//                    .Deploy());

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then take listener sets variable
//            //conditional event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Specifier.AssertTaskNames(TasksAfterVariableIsSet, true, false);
//        }

//        [Test]
//        public void TestSetVariableInTakeListenerWithAsyncBefore()
//        {
//            var modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent(StartEventId)
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .SequenceFlowId(FlowId)
//                .UserTask(TaskWithConditionId)
//                .Name(TaskWithCondition)
//                .CamundaAsyncBefore()
//                .EndEvent(EndEventId)
//                .Done();
//            var listener = modelInstance.NewInstance<ICamundaExecutionListener>(typeof(ICamundaExecutionListener));
//            listener.CamundaEvent = ExecutionListenerFields.EventNameTake;
//            listener.CamundaExpression = ExprSetVariable;
//            modelInstance.GetModelElementById<ISequenceFlow>(FlowId)
//                .Builder()
//                .AddExtensionElement(listener);
//            modelInstance = Specifier.SpecifyConditionalProcess(modelInstance, true);
//            Engine.ManageDeployment(
//                RepositoryService.CreateDeployment()
//                    .AddModelInstance(ConditionalModel, modelInstance)
//                    .Deploy());

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then take listener sets variable
//            //conditional event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Specifier.AssertTaskNames(TasksAfterVariableIsSet, true, false);
//        }

//        [Test]
//        public void testSetVariableOnParentScopeInEndListener()
//        {
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent(StartEventId)
//                .SubProcess()
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .CamundaExecutionListenerExpression(ExecutionListenerFields.EventNameEnd, ExprSetVariableOnParent)
//                .UserTask(TaskWithConditionId)
//                .Name(TaskWithCondition)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent(EndEventId)
//                .Done();

//            modelInstance = Specifier.SpecifyConditionalProcess(modelInstance, true);
//            Engine.ManageDeployment(
//                RepositoryService.CreateDeployment()
//                    .AddModelInstance(ConditionalModel, modelInstance)
//                    .Deploy());

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then end listener sets variable
//            //conditional event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Specifier.AssertTaskNames(TasksAfterVariableIsSet, true, false);
//        }

//        [Test]
//        public void testSetVariableOnParentScopeInStartListener()
//        {
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent(StartEventId)
//                .SubProcess()
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .UserTask(TaskWithConditionId)
//                .Name(TaskWithCondition)
//                .CamundaExecutionListenerExpression(ExecutionListenerFields.EventNameStart, ExprSetVariableOnParent)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent(EndEventId)
//                .Done();

//            modelInstance = Specifier.SpecifyConditionalProcess(modelInstance, true);
//            Engine.ManageDeployment(
//                RepositoryService.CreateDeployment()
//                    .AddModelInstance(ConditionalModel, modelInstance)
//                    .Deploy());

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then start listener sets variable
//            //conditional event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Specifier.AssertTaskNames(TasksAfterVariableIsSet, true, false);
//        }

//        [Test]
//        public void testSetVariableOnParentScopeInTakeListener()
//        {
//            IBpmnModelInstance modelInstance = Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalEventProcessKey)
//                .StartEvent(StartEventId)
//                .SubProcess()
//                ////.EmbeddedSubProcess()
//                //.StartEvent()
//                .UserTask(TaskBeforeConditionId)
//                .Name(TaskBeforeCondition)
//                .SequenceFlowId(FlowId)
//                .UserTask(TaskWithConditionId)
//                .Name(TaskWithCondition)
//                .EndEvent()
//                .SubProcessDone()
//                .EndEvent(EndEventId)
//                .Done();
//            var listener = modelInstance.NewInstance<ICamundaExecutionListener>(typeof(ICamundaExecutionListener));
//            listener.CamundaEvent = ExecutionListenerFields.EventNameTake;
//            listener.CamundaExpression = ExprSetVariableOnParent;
//            modelInstance.GetModelElementById<ISequenceFlow>(FlowId)
//                .Builder()
//                .AddExtensionElement(listener);
//            modelInstance = Specifier.SpecifyConditionalProcess(modelInstance, true);
//            Engine.ManageDeployment(
//                RepositoryService.CreateDeployment()
//                    .AddModelInstance(ConditionalModel, modelInstance)
//                    .Deploy());

//            // given
//            var procInst = RuntimeService.StartProcessInstanceByKey(ConditionalEventProcessKey);

//            var taskQuery = TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== procInst.Id);
//            var task = taskQuery.First();
//            Assert.NotNull(task);
//            Assert.AreEqual(TaskBeforeCondition, task.Name);

//            //when task is completed
//            TaskService.Complete(task.Id);

//            //then start listener sets variable
//            //conditional event is triggered
//            TasksAfterVariableIsSet = taskQuery.ToList();
//            Specifier.AssertTaskNames(TasksAfterVariableIsSet, true, false);
//        }
//    }
//}