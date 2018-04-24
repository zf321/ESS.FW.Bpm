using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Bpmn.TaskListener.Util;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Model.Bpmn.builder;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.TaskListener
{

    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class TaskListenerTest: ProvidedProcessEngineRule
    {
        private bool InstanceFieldsInitialized = false;

        public TaskListenerTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            testRule = new ProcessEngineTestRule(this);
            //Junit 暂时屏蔽
            // //ruleChain = RuleChain.outerRule(engineRule).around(testRule);
        }


        //public ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        public ProcessEngineTestRule testRule;

        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testRule);
        //Junit 暂时屏蔽
        ////public RuleChain ruleChain;

        //protected internal IRuntimeService runtimeService;
        //protected internal ITaskService taskService;
        //protected internal IHistoryService historyService;
        //protected internal ProcessEngineConfigurationImpl processEngineConfiguration;

        [SetUp]
        public virtual void setUp()
        {
            //runtimeService = engineRule.RuntimeService;
            //taskService = engineRule.TaskService;
            //historyService = engineRule.HistoryService;
            //processEngineConfiguration = engineRule.ProcessEngineConfiguration;
        }

        [Test]
        [Deployment("resources/bpmn/tasklistener/TaskListenerTest.bpmn20.xml")]

        public virtual void testTaskCreateListener()
        {
            var p = runtimeService.StartProcessInstanceByKey("taskListenerProcess");
            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId == p.Id).FirstOrDefault();
            Assert.AreEqual("Schedule meeting", task.Name);
            Assert.AreEqual("TaskCreateListener is listening!", task.Description);
        }

        [Test]
        [Deployment("resources/bpmn/tasklistener/TaskListenerTest.bpmn20.xml")]
        public virtual void testTaskCompleteListener()
        {
            TaskDeleteListener.Clear();
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("taskListenerProcess");
            Assert.AreEqual(null, runtimeService.GetVariable(processInstance.Id, "greeting"));
            Assert.AreEqual(null, runtimeService.GetVariable(processInstance.Id, "expressionValue"));

            // Completing first task will change the description
            ITask task = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id).First();
            taskService.Complete(task.Id);

            // Check that the completion did not execute the Delete listener
            Assert.AreEqual(0, TaskDeleteListener.EventCounter);
            Assert.IsNull(TaskDeleteListener.LastTaskDefinitionKey);
            Assert.IsNull(TaskDeleteListener.LastDeleteReason);

            Assert.AreEqual("Hello from The Process", runtimeService.GetVariable(processInstance.Id, "greeting"));
            Assert.AreEqual("Act", runtimeService.GetVariable(processInstance.Id, "shortName"));
        }

        [Test]
        [Deployment("resources/bpmn/tasklistener/TaskListenerTest.bpmn20.xml")]
        public virtual void testTaskDeleteListenerByProcessDeletion()
        {
            TaskDeleteListener.Clear();
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("taskListenerProcess");

            Assert.AreEqual(0, TaskDeleteListener.EventCounter);
            Assert.IsNull(TaskDeleteListener.LastTaskDefinitionKey);
            Assert.IsNull(TaskDeleteListener.LastDeleteReason);

            // Delete process instance to Delete task
            ITask task = taskService.CreateTaskQuery().First();
            runtimeService.DeleteProcessInstance(processInstance.ProcessInstanceId, "test Delete task listener");

            Assert.AreEqual(1, TaskDeleteListener.EventCounter);
            Assert.AreEqual(task.TaskDefinitionKey, TaskDeleteListener.LastTaskDefinitionKey);
            Assert.AreEqual("test Delete task listener", TaskDeleteListener.LastDeleteReason);
        }

        [Test]
        [Deployment("resources/bpmn/tasklistener/TaskListenerTest.bpmn20.xml")]
        public virtual void testTaskDeleteListenerByBoundaryEvent()
        {
            TaskDeleteListener.Clear();
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("taskListenerProcess");

            Assert.AreEqual(0, TaskDeleteListener.EventCounter);
            Assert.IsNull(TaskDeleteListener.LastTaskDefinitionKey);
            Assert.IsNull(TaskDeleteListener.LastDeleteReason);

            // correlate message to Delete task
            ITask task = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id).First();
            runtimeService.CorrelateMessage("message");

            Assert.AreEqual(1, TaskDeleteListener.EventCounter);
            Assert.AreEqual(task.TaskDefinitionKey, TaskDeleteListener.LastTaskDefinitionKey);
            Assert.AreEqual("deleted", TaskDeleteListener.LastDeleteReason);
        }


        [Test]
        [Deployment("resources/bpmn/tasklistener/TaskListenerTest.bpmn20.xml")]
        public virtual void testTaskListenerWithExpression()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("taskListenerProcess");
            Assert.AreEqual(null, runtimeService.GetVariable(processInstance.Id, "greeting2"));

            // Completing first task will change the description
            ITask task = taskService.CreateTaskQuery().First();
            taskService.Complete(task.Id);

            Assert.AreEqual("Write meeting notes", runtimeService.GetVariable(processInstance.Id, "greeting2"));
        }

        [Test]
        [Deployment]
        public virtual void testScriptListener()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            ITask task = taskService.CreateTaskQuery().First();
            Assert.NotNull(task);

            Assert.True((bool)runtimeService.GetVariable(processInstance.Id, "create"));

            taskService.SetAssignee(task.Id, "test");
            Assert.True((bool)runtimeService.GetVariable(processInstance.Id, "assignment"));

            taskService.Complete(task.Id);
            Assert.True((bool)runtimeService.GetVariable(processInstance.Id, "complete"));

            task = taskService.CreateTaskQuery().First();
            Assert.NotNull(task);

            runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            if (processEngineConfiguration.HistoryLevel.Id >= HistoryLevelFields.HistoryLevelAudit.Id)//HISTORYLEVEL_AUDIT
            {
                var variable = HistoryService.CreateHistoricVariableInstanceQuery()/*.VariableName("Delete")*/.First();
                Assert.NotNull(variable);
                Assert.True((bool)variable.Value);
            }
        }

        [Test]
        [Deployment(new string[] {
    "org/camunda/bpm/engine/test/bpmn/tasklistener/TaskListenerTest.testScriptResourceListener.bpmn20.xml",
    "org/camunda/bpm/engine/test/bpmn/tasklistener/taskListener.groovy"
        })]
        public virtual void testScriptResourceListener()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            ITask task = taskService.CreateTaskQuery().First();
            Assert.NotNull(task);

            Assert.True((bool)runtimeService.GetVariable(processInstance.Id, "create"));

            taskService.SetAssignee(task.Id, "test");
            Assert.True((bool)runtimeService.GetVariable(processInstance.Id, "assignment"));

            taskService.Complete(task.Id);
            Assert.True((bool)runtimeService.GetVariable(processInstance.Id, "complete"));

            task = taskService.CreateTaskQuery().First();
            Assert.NotNull(task);

            runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            if (processEngineConfiguration.HistoryLevel.Id >= HistoryLevelFields.HistoryLevelAudit.Id)
            {
                var variable = HistoryService.CreateHistoricVariableInstanceQuery()/*.VariableName("Delete")*/.First();
                Assert.NotNull(variable);
                Assert.True((bool)variable.Value);
            }
        }


        
        [Test]
        public virtual void testCompleteTaskInCreateTaskListener()
        {
            // given process with IUser task and task create listener
            var modelInstance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("startToEnd").StartEvent().UserTask();
            modelInstance.CamundaTaskListenerClass(TaskListenerFields.EventnameCreate, typeof(TaskCreateListener)
                        .FullName).Name<AbstractUserTaskBuilder>("userTask");
            var r = modelInstance.EndEvent().Done();
            testRule.Deploy(r);

            // when process is started and IUser task completed in task create listener
            runtimeService.StartProcessInstanceByKey("startToEnd");

            // then task is successfully completed without an exception
            Assert.IsNull(taskService.CreateTaskQuery().First());
        }

        [Test]
        public virtual void testCompleteTaskInCreateTaskListenerWithIdentityLinks()
        {

            var modelInstance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("startToEnd")
                    .StartEvent().UserTask();

            modelInstance.CamundaTaskListenerClass(TaskListenerFields.EventnameCreate, typeof(TaskCreateListener)
                .FullName).Name<AbstractUserTaskBuilder>("userTask");
            var r = modelInstance.CamundaCandidateUsers((new string[] { "users1", "user2" }))
                 .CamundaCandidateGroups((new string[] { "group1", "group2" })).EndEvent().Done();
            testRule.Deploy(r);

            // when process is started and IUser task completed in task create listener
            runtimeService.StartProcessInstanceByKey("startToEnd");

            // then task is successfully completed without an exception
            Assert.IsNull(taskService.CreateTaskQuery().First());
        }


        [Test]
        public virtual void testActivityInstanceIdOnDeleteInCalledProcess()
        {
            // given
            RecorderTaskListener.clear();

            var callActivityProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("calling").StartEvent().CallActivity().CalledElement("called").EndEvent().Done();


            var calledProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("called").StartEvent().UserTask().CamundaTaskListenerClass(TaskListenerFields.EventnameCreate, typeof(RecorderTaskListener).FullName).CamundaTaskListenerClass(TaskListenerFields.EventnameDelete, typeof(RecorderTaskListener).FullName).EndEvent().Done();
            testRule.Deploy(callActivityProcess, calledProcess);
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("calling");

            // when
            runtimeService.DeleteProcessInstance(processInstance.Id, null);

            // then
            IList<RecorderTaskListener.RecordedTaskEvent> recordedEvents = RecorderTaskListener.RecordedEvents;
            Assert.AreEqual(2, recordedEvents.Count);
            string createActivityInstanceId = recordedEvents[0].ActivityInstanceId;
            string deleteActivityInstanceId = recordedEvents[1].ActivityInstanceId;

            Assert.AreEqual(createActivityInstanceId, deleteActivityInstanceId);
        }

        [Test]
        public virtual void testVariableAccessOnDeleteInCalledProcess()
        {
            // given
            VariablesCollectingListener.reset();

            var callActivityProcess =
                ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("calling")
                    .StartEvent()
                    .CallActivity()
                    .CalledElement("called")
                    .EndEvent()
                    .Done();

            //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var calledProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("called").StartEvent().UserTask()
                .CamundaTaskListenerClass(TaskListenerFields.EventnameDelete, typeof(VariablesCollectingListener).FullName)
                .EndEvent().Done();
            //Junit暂时屏蔽
            //testRule.Deploy(callActivityProcess, calledProcess);
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("calling",
                ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("foo", "bar"));

            // when
            runtimeService.DeleteProcessInstance(processInstance.Id, null);

            // then
            var collectedVariables = VariablesCollectingListener.CollectedVariables;
            Assert.NotNull(collectedVariables);
            Assert.AreEqual(1, collectedVariables.Count); //.Count());

            //原代码： Assert.AreEqual("bar", collectedVariables.Get("foo"));
            Assert.IsTrue(collectedVariables.Contains(new KeyValuePair<string, object>("foo", "bar")));
        }

        

    }
    public class TaskCreateListener : ITaskListener
    {
        public virtual void Notify(IDelegateTask delegateTask)
        {
            delegateTask.Complete();
        }
    }
    public class VariablesCollectingListener : ITaskListener
    {

        protected internal static IVariableMap collectedVariables;

        public static IVariableMap CollectedVariables
        {
            get
            {
                return collectedVariables;
            }
        }

        public static void reset()
        {
            collectedVariables = null;
        }

        public void Notify(IDelegateTask delegateTask)
        {
            collectedVariables = delegateTask.VariablesTyped;
        }

    }


}