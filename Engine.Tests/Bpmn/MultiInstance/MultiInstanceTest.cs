using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Bpmn.Event.Error;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Value;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.MultiInstance
{
    /// <summary>
    ///     
    /// </summary>
    [TestFixture]
    public class MultiInstanceTest : PluggableProcessEngineTestCase
    {
        [Test][Deployment("resources/bpmn/multiinstance/MultiInstanceTest.sequentialUserTasks.bpmn20.xml") ]
        public virtual void testSequentialUserTasks()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("miSequentialUserTasks",
                CollectionUtil.SingletonMap("nrOfLoops", 3));
            var procId = processInstance.Id;

            // now there is now 1 activity instance below the pi:
            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            var expectedTree = ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                .BeginMiBody("miTasks")
                .Activity("miTasks")
                .Done();
            ActivityInstanceAssert.That(tree)
                .HasStructure(expectedTree);

            var task = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("My Task", task.Name);
            Assert.AreEqual("kermit_0", task.Assignee);
            taskService.Complete(task.Id);

            tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(expectedTree);


            task = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("My Task", task.Name);
            Assert.AreEqual("kermit_1", task.Assignee);
            taskService.Complete(task.Id);

            tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(expectedTree);

            task = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("My Task", task.Name);
            Assert.AreEqual("kermit_2", task.Assignee);
            taskService.Complete(task.Id);

            Assert.IsNull(taskService.CreateTaskQuery()
                .FirstOrDefault());
            AssertProcessEnded(procId);
        }

        [Test]
        [Deployment("resources/bpmn/multiinstance/MultiInstanceTest.sequentialUserTasks.bpmn20.xml")]
        public virtual void testSequentialUserTasksHistory()
        {
            runtimeService.StartProcessInstanceByKey("miSequentialUserTasks",
                CollectionUtil.SingletonMap("nrOfLoops", 4)); //.Id;
            for (var i = 0; i < 4; i++)
                taskService.Complete(taskService.CreateTaskQuery()
                    .First()
                    .Id);

            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                IList<IHistoricActivityInstance> historicActivityInstances =
                    historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityType == "userTask")
                        .ToList();
                Assert.AreEqual(4, historicActivityInstances.Count);
                foreach (var hai in historicActivityInstances)
                {
                    Assert.NotNull(hai.ActivityId);
                    Assert.NotNull(hai.ActivityName);
                    Assert.NotNull(hai.StartTime);
                    Assert.NotNull(hai.EndTime);
                    Assert.NotNull(hai.Assignee);
                }
            }

            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelActivity)
            {
                IList<IHistoricTaskInstance> historicTaskInstances = historyService.CreateHistoricTaskInstanceQuery()
                    .ToList();
                Assert.AreEqual(4, historicTaskInstances.Count);
                foreach (var ht in historicTaskInstances)
                {
                    Assert.NotNull(ht.Assignee);
                    Assert.NotNull(ht.StartTime);
                    Assert.NotNull(ht.EndTime);
                }
            }
        }


        [Test]
        [Deployment("resources/bpmn/multiinstance/MultiInstanceTest.sequentialUserTasks.bpmn20.xml")]
        public virtual void testSequentialUserTasksWithTimer()
        {
            var procId =
                runtimeService.StartProcessInstanceByKey("miSequentialUserTasks",
                        CollectionUtil.SingletonMap("nrOfLoops", 3))
                    .Id;

            // Complete 1 tasks
            taskService.Complete(taskService.CreateTaskQuery()
                .First()
                .Id);

            // Fire timer
            var timer = managementService.CreateJobQuery()
                .First();
            managementService.ExecuteJob(timer.Id);

            var taskAfterTimer = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("taskAfterTimer", taskAfterTimer.TaskDefinitionKey);
            taskService.Complete(taskAfterTimer.Id);
            AssertProcessEnded(procId);
        }


        [Test]
        [Deployment("resources/bpmn/multiinstance/MultiInstanceTest.sequentialUserTasks.bpmn20.xml")]
        public virtual void testSequentialUserTasksCompletionCondition()
        {
            var procId = runtimeService.StartProcessInstanceByKey("miSequentialUserTasks",
                    CollectionUtil.SingletonMap("nrOfLoops", 10))
                .Id;

            // 10 tasks are to be created, but completionCondition stops them at 5
            for (var i = 0; i < 5; i++)
            {
                var task = taskService.CreateTaskQuery()
                    .First();
                taskService.Complete(task.Id);
            }
            Assert.IsNull(taskService.CreateTaskQuery()
                .FirstOrDefault());
            AssertProcessEnded(procId);
        }


        [Test][Deployment]
        public virtual void testNestedSequentialUserTasks()
        {
            var procId = runtimeService.StartProcessInstanceByKey("miNestedSequentialUserTasks")
                .Id;

            for (var i = 0; i < 3; i++)
            {
                ITask task = taskService.CreateTaskQuery(c=>c.AssigneeWithoutCascade == "kermit")
                    .First();
                Assert.AreEqual("My Task", task.Name);
                var processInstance = runtimeService.GetActivityInstance(procId);
                var instancesForActivitiyId = GetInstancesForActivityId(processInstance, "miTasks");
                Assert.AreEqual(1, instancesForActivitiyId.Count);
                taskService.Complete(task.Id);
            }

            AssertProcessEnded(procId);
        }


        [Test]
        [Deployment]
        public virtual void testParallelUserTasks()
        {
            var procInst = runtimeService.StartProcessInstanceByKey("miParallelUserTasks");
            var procId = procInst.Id;

            IList<ITask> tasks = taskService.CreateTaskQuery()
                /*.OrderByTaskName()*/
                /*.Asc()*/
                .ToList().OrderBy(m=>m.Name).ToList();
            Assert.AreEqual(3, tasks.Count);
            Assert.AreEqual("My Task 0", tasks[0].Name);
            Assert.AreEqual("My Task 1", tasks[1].Name);
            Assert.AreEqual("My Task 2", tasks[2].Name);

            var processInstance = runtimeService.GetActivityInstance(procId);
            Assert.AreEqual(3, processInstance.GetActivityInstances("miTasks")
                .Length);

            taskService.Complete(tasks[0].Id);

            processInstance = runtimeService.GetActivityInstance(procId);

            Assert.AreEqual(2, processInstance.GetActivityInstances("miTasks")
                .Length);

            taskService.Complete(tasks[1].Id);

            processInstance = runtimeService.GetActivityInstance(procId);
            Assert.AreEqual(1, processInstance.GetActivityInstances("miTasks")
                .Length);

            taskService.Complete(tasks[2].Id);
            AssertProcessEnded(procId);
        }


        [Test]
        [Deployment]
        public virtual void testParallelReceiveTasks()
        {
            var procInst = runtimeService.StartProcessInstanceByKey("miParallelReceiveTasks");
            var procId = procInst.Id;

            Assert.AreEqual(3, runtimeService.CreateEventSubscriptionQuery()
                .Count());

            IList<IExecution> receiveTaskExecutions =
                runtimeService.CreateExecutionQuery(c => c.ActivityId == "miTasks")
                    .ToList();

            foreach (var execution in receiveTaskExecutions)
                runtimeService.MessageEventReceived("message", execution.Id);
            AssertProcessEnded(procId);
        }

        [Test][Deployment( "resources/bpmn/multiinstance/MultiInstanceTest.TestParallelReceiveTasks.bpmn20.xml")]
        public virtual void testParallelReceiveTasksAssertEventSubscriptionRemoval()
        {
            runtimeService.StartProcessInstanceByKey("miParallelReceiveTasks");

            Assert.AreEqual(3, runtimeService.CreateEventSubscriptionQuery()
                .Count());

            IList<IExecution> receiveTaskExecutions =
                runtimeService.CreateExecutionQuery(c => c.ActivityId == "miTasks")
                    .ToList();

            // signal one of the instances
            runtimeService.MessageEventReceived("message", receiveTaskExecutions[0].Id);

            // now there should be two subscriptions left
            Assert.AreEqual(2, runtimeService.CreateEventSubscriptionQuery()
                .Count());
        }

        [Test][Deployment(new string[] {"resources/bpmn/multiinstance/MultiInstanceTest.TestParallelUserTasks.bpmn20.xml"}) ]
        public virtual void testParallelUserTasksHistory()
        {
            var pi = runtimeService.StartProcessInstanceByKey("miParallelUserTasks");
            foreach (var task in taskService.CreateTaskQuery()
                .ToList())
                taskService.Complete(task.Id);

            // Validate history
            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                IList<IHistoricTaskInstance> historicTaskInstances = historyService.CreateHistoricTaskInstanceQuery()
                    /*.OrderByTaskAssignee()*/
                    /*.Asc()*/
                    .ToList().OrderBy(m=>m.Name).ToList();
                for (var i = 0; i < historicTaskInstances.Count; i++)
                {
                    var hi = historicTaskInstances[i];
                    Assert.NotNull(hi.StartTime);
                    Assert.NotNull(hi.EndTime);
                    Assert.AreEqual("kermit_" + i, hi.Assignee);
                }

                var multiInstanceBodyInstance =
                    historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityId == "miTasks#multiInstanceBody")
                        .First();
                Assert.NotNull(multiInstanceBodyInstance);
                Assert.AreEqual(pi.Id, multiInstanceBodyInstance.ParentActivityInstanceId);

                IList<IHistoricActivityInstance> historicActivityInstances =
                    historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityType == "userTask")
                        .ToList();
                Assert.AreEqual(3, historicActivityInstances.Count);
                foreach (var hai in historicActivityInstances)
                {
                    Assert.NotNull(hai.StartTime);
                    Assert.NotNull(hai.EndTime);
                    Assert.NotNull(hai.Assignee);
                    Assert.AreEqual("userTask", hai.ActivityType);
                    Assert.AreEqual(multiInstanceBodyInstance.Id, hai.ParentActivityInstanceId);
                    Assert.NotNull(hai.TaskId);
                }
            }
        }


        [Test]
        [Deployment]
        public virtual void testParallelUserTasksWithTimer()
        {
            var procId = runtimeService.StartProcessInstanceByKey("miParallelUserTasksWithTimer")
                .Id;

            IList<ITask> tasks = taskService.CreateTaskQuery()
                .ToList();
            taskService.Complete(tasks[0].Id);

            // Fire timer
            var timer = managementService.CreateJobQuery()
                .First();
            managementService.ExecuteJob(timer.Id);

            var taskAfterTimer = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("taskAfterTimer", taskAfterTimer.TaskDefinitionKey);
            taskService.Complete(taskAfterTimer.Id);
            AssertProcessEnded(procId);
        }


        [Test]//${nrOfCompletedInstances/nrOfInstances >= 0.5}
        [Deployment]
        public virtual void testParallelUserTasksCompletionCondition()
        {
            throw new NotSupportedException("${nrOfCompletedInstances/nrOfInstances >= 0.5}");
            var procId = runtimeService.StartProcessInstanceByKey("miParallelUserTasksCompletionCondition")
                .Id;
            IList<ITask> tasks = taskService.CreateTaskQuery()
                .ToList();
            Assert.AreEqual(5, tasks.Count);

            // Completing 3 tasks gives 50% of tasks completed, which triggers completionCondition
            for (var i = 0; i < 3; i++)
            {
                Assert.AreEqual(5 - i, taskService.CreateTaskQuery()
                    .Count());
                taskService.Complete(tasks[i].Id);
            }
            AssertProcessEnded(procId);
        }


        [Test]
        [Deployment("resources/bpmn/multiinstance/MultiInstanceTest.testParallelUserTasksBasedOnCollection.bpmn20.xml")]
        public virtual void testEmptyCollectionInMI()
        {
            IList<string> assigneeList = new List<string>();
            var procId = runtimeService.StartProcessInstanceByKey("miParallelUserTasksBasedOnCollection"
                    , CollectionUtil.SingletonMap("assigneeList", assigneeList))
                .Id;

            Assert.AreEqual(0, taskService.CreateTaskQuery()
                .Count());
            AssertProcessEnded(procId);
            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                IList<IHistoricActivityInstance> activities = historyService.CreateHistoricActivityInstanceQuery(c=>c.ProcessInstanceId ==procId)
                    /*.OrderByActivityId()*/
                    /*.Asc()*/
                    .ToList().OrderBy(m=>m.ActivityId).ToList();
                Assert.AreEqual(3, activities.Count);
                Assert.AreEqual("miTasks#multiInstanceBody", activities[0].ActivityId);
                Assert.AreEqual("theEnd", activities[1].ActivityId);
                Assert.AreEqual("theStart", activities[2].ActivityId);
            }
        }


        //[Test]
        public virtual void FAILING_testParallelUserTasksBasedOnCollectionExpression()
        {
            DelegateEvent.ClearEvents();

            runtimeService.StartProcessInstanceByKey("process", Variables.CreateVariables()
                .PutValue("myBean", new DelegateBean()));

            var recordedEvents = DelegateEvent.Events;
            Assert.AreEqual(2, recordedEvents.Count);

            Assert.AreEqual("miTasks#multiInstanceBody", recordedEvents[0].CurrentActivityId);
            Assert.AreEqual("miTasks#multiInstanceBody", recordedEvents[1].CurrentActivityId); // or miTasks

            DelegateEvent.ClearEvents();
        }


        [Test]
        [Deployment]
        public virtual void testParallelUserTasksCustomExtensions()
        {
            IDictionary<string, object> vars = new Dictionary<string, object>();
            IList<string> assigneeList = new List<string> {"kermit", "gonzo", "fozzie"};
            vars["assigneeList"] = assigneeList;
            runtimeService.StartProcessInstanceByKey("miSequentialUserTasks", vars);

            foreach (var assignee in assigneeList)
            {
                var task = taskService.CreateTaskQuery()
                    .First();
                Assert.AreEqual(assignee, task.Assignee);
                taskService.Complete(task.Id);
            }
        }


        [Test]
        [Deployment]
        public virtual void testParallelUserTasksExecutionAndTaskListeners()
        {
            runtimeService.StartProcessInstanceByKey("miParallelUserTasks");
            IList<ITask> tasks = taskService.CreateTaskQuery()
                .ToList();
            foreach (var task in tasks)
                taskService.Complete(task.Id);

            var waitState = runtimeService.CreateExecutionQuery()
                .First();
            Assert.AreEqual(3, runtimeService.GetVariable(waitState.Id, "taskListenerCounter"));
            Assert.AreEqual(3, runtimeService.GetVariable(waitState.Id, "executionListenerCounter"));
        }


        [Test]//不支持groovy语法
        //[Deployment("resources/bpmn/multiinstance/MultiInstanceTest.testSequentialScriptTasks.bpmn20.xml")]
        public virtual void testSequentialScriptTasksNoStackOverflow()
        {
            throw new NotSupportedException("不支持groovy语法");
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["sum"] = 0;
            vars["nrOfLoops"] = 200;
            runtimeService.StartProcessInstanceByKey("miSequentialScriptTask", vars);
            var waitStateExecution = runtimeService.CreateExecutionQuery()
                .First();
            var sum = (int?) runtimeService.GetVariable(waitStateExecution.Id, "sum"); //.Value;
            Assert.AreEqual(19900, sum);
        }


        [Test]//不支持groovy语法
        //[Deployment("resources/bpmn/multiinstance/MultiInstanceTest.testSequentialScriptTasks.bpmn20.xml")]
        public virtual void testSequentialScriptTasksHistory()
        {
            throw new NotSupportedException("不支持groovy语法");
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["sum"] = 0;
            vars["nrOfLoops"] = 7;
            runtimeService.StartProcessInstanceByKey("miSequentialScriptTask", vars);

            // Validate history
            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                IList<IHistoricActivityInstance> historicInstances =
                    historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityType == "scriptTask")
                        ///*.OrderByActivityId()*/
                        ///*.Asc()*/
                        .ToList().OrderBy(m=>m.ActivityId).ToList();
                Assert.AreEqual(7, historicInstances.Count);
                for (var i = 0; i < 7; i++)
                {
                    var hai = historicInstances[i];
                    Assert.AreEqual("scriptTask", hai.ActivityType);
                    Assert.NotNull(hai.StartTime);
                    Assert.NotNull(hai.EndTime);
                }
            }
        }


        [Test]//不支持groovy语法
        //[Deployment]
        public virtual void testSequentialScriptTasksCompletionCondition()
        {
            throw new NotSupportedException("不支持groovy语法");
            runtimeService.StartProcessInstanceByKey("miSequentialScriptTaskCompletionCondition"); //.Id;
            var waitStateExecution = runtimeService.CreateExecutionQuery()
                .First();
            var sum = (int?) runtimeService.GetVariable(waitStateExecution.Id, "sum"); //.Value;
            Assert.AreEqual(5, sum);
        }

        [Test]//不支持groovy语法
        //[Deployment]
        public virtual void testParallelScriptTasks()
        {
            throw new NotSupportedException("不支持groovy语法");
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["sum"] = 0;
            vars["nrOfLoops"] = 10;
            runtimeService.StartProcessInstanceByKey("miParallelScriptTask", vars);
            var waitStateExecution = runtimeService.CreateExecutionQuery()
                .First();
            var sum = (int?) runtimeService.GetVariable(waitStateExecution.Id, "sum"); //.Value;
            Assert.AreEqual(45, sum);
        }

        [Test]//不支持groovy语法
        //[Deployment(new string[] {"resources/bpmn/multiinstance/MultiInstanceTest.TestParallelScriptTasks.bpmn20.xml"}) ]
        public virtual void testParallelScriptTasksHistory()
        {
            throw new NotSupportedException("不支持groovy语法");
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["sum"] = 0;
            vars["nrOfLoops"] = 4;
            runtimeService.StartProcessInstanceByKey("miParallelScriptTask", vars);
            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                IList<IHistoricActivityInstance> historicActivityInstances =
                    historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityType == "scriptTask")
                        .ToList();
                Assert.AreEqual(4, historicActivityInstances.Count);
                foreach (var hai in historicActivityInstances)
                {
                    Assert.NotNull(hai.StartTime);
                    Assert.NotNull(hai.StartTime);
                }
            }
        }


        [Test]//不支持groovy语法
        //[Deployment]
        public virtual void testParallelScriptTasksCompletionCondition()
        {
            throw new NotSupportedException("不支持groovy语法");
            runtimeService.StartProcessInstanceByKey("miParallelScriptTaskCompletionCondition");
            var waitStateExecution = runtimeService.CreateExecutionQuery()
                .First();
            var sum = (int?) runtimeService.GetVariable(waitStateExecution.Id, "sum"); //.Value;
            Assert.AreEqual(2, sum);
        }

        [Test]//不支持groovy语法
        //[Deployment("resources/bpmn/multiinstance/MultiInstanceTest.TestParallelScriptTasksCompletionCondition.bpmn20.xml") ]
        public virtual void testParallelScriptTasksCompletionConditionHistory()
        {
            throw new NotSupportedException("不支持groovy语法");
            runtimeService.StartProcessInstanceByKey("miParallelScriptTaskCompletionCondition");
            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                IList<IHistoricActivityInstance> historicActivityInstances =
                    historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityType == "scriptTask")
                        .ToList();
                Assert.AreEqual(2, historicActivityInstances.Count);
            }
        }


        [Test]
        [Deployment]
        public virtual void testSequentialSubProcess()
        {
            var procId = runtimeService.StartProcessInstanceByKey("miSequentialSubprocess")
                .Id;

            List<ITask> query = taskService.CreateTaskQuery().ToList().OrderBy(m=>m.Name).ToList()
                /*.OrderByTaskName()*/
                /*.Asc()*/;
            for (var i = 0; i < 4; i++)
            {
                IList<ITask> tasks = query.ToList();
                Assert.AreEqual(2, tasks.Count);

                Assert.AreEqual("task one", tasks[0].Name);
                Assert.AreEqual("task two", tasks[1].Name);

                taskService.Complete(tasks[0].Id);
                taskService.Complete(tasks[1].Id);

                if (i != 3)
                {
                    var activities = runtimeService.GetActiveActivityIds(procId);
                    Assert.NotNull(activities);
                    Assert.AreEqual(2, activities.Count);
                }
            }

            AssertProcessEnded(procId);
        }


        [Test]
        public virtual void testSequentialSubProcessEndEvent()
        {
            // ACT-1185: end-event in subprocess causes inactivated execution
            var procId = runtimeService.StartProcessInstanceByKey("miSequentialSubprocess")
                .Id;

            IQueryable<ITask> query = taskService.CreateTaskQuery()
                /*.OrderByTaskName()*/
                /*.Asc()*/;
            for (var i = 0; i < 4; i++)
            {
                IList<ITask> tasks = query.ToList();
                Assert.AreEqual(1, tasks.Count);

                Assert.AreEqual("task one", tasks[0].Name);

                taskService.Complete(tasks[0].Id);

                // Last run, the execution no longer exists
                if (i != 3)
                {
                    var activities = runtimeService.GetActiveActivityIds(procId);
                    Assert.NotNull(activities);
                    Assert.AreEqual(1, activities.Count);
                }
            }

            AssertProcessEnded(procId);
        }


        [Test][Deployment(new string[] {"resources/bpmn/multiinstance/MultiInstanceTest.TestSequentialSubProcess.bpmn20.xml"})]
        public virtual void testSequentialSubProcessHistory()
        {
            runtimeService.StartProcessInstanceByKey("miSequentialSubprocess");
            for (var i = 0; i < 4; i++)
            {
                IList<ITask> tasks = taskService.CreateTaskQuery()
                    .ToList();
                taskService.Complete(tasks[0].Id);
                taskService.Complete(tasks[1].Id);
            }

            // Validate history
            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                IList<IHistoricActivityInstance> onlySubProcessInstances =
                    historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityType == "subProcess")
                        .ToList();
                Assert.AreEqual(4, onlySubProcessInstances.Count);

                IList<IHistoricActivityInstance> historicInstances =
                    historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityType == "subProcess")
                        .ToList();
                Assert.AreEqual(4, historicInstances.Count);
                foreach (var hai in historicInstances)
                {
                    Assert.NotNull(hai.StartTime);
                    Assert.NotNull(hai.EndTime);
                }

                historicInstances =
                    historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityType == "userTask")
                        .ToList();
                Assert.AreEqual(8, historicInstances.Count);
                foreach (var hai in historicInstances)
                {
                    Assert.NotNull(hai.StartTime);
                    Assert.NotNull(hai.EndTime);
                }
            }
        }

        [Test][Deployment(new string[] {"resources/bpmn/multiinstance/MultiInstanceTest.TestParallelSubProcess.bpmn20.xml"})]
        public virtual void testParallelSubProcessHistory()
        {
            var pi = runtimeService.StartProcessInstanceByKey("miParallelSubprocess");

            // Validate history
            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                IList<IHistoricActivityInstance> historicActivityInstances =
                    historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityId == "miSubProcess")
                        .ToList();
                Assert.AreEqual(2, historicActivityInstances.Count);
                foreach (var hai in historicActivityInstances)
                {
                    Assert.NotNull(hai.StartTime);
                    // now end time is null
                    Assert.IsNull(hai.EndTime);
                    Assert.NotNull(pi.Id, hai.ParentActivityInstanceId);
                }
            }

            foreach (var task in taskService.CreateTaskQuery()
                .ToList())
                taskService.Complete(task.Id);

            // Validate history
            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                IList<IHistoricActivityInstance> historicActivityInstances =
                    historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityId == "miSubProcess")
                        .ToList();
                Assert.AreEqual(2, historicActivityInstances.Count);
                foreach (var hai in historicActivityInstances)
                {
                    Assert.NotNull(hai.StartTime);
                    Assert.NotNull(hai.EndTime);
                    Assert.NotNull(pi.Id, hai.ParentActivityInstanceId);
                }
            }
        }


        [Test]
        public virtual void testParallelSubProcessWithTimer()
        {
            var procId = runtimeService.StartProcessInstanceByKey("miParallelSubprocessWithTimer")
                .Id;
            IList<ITask> tasks = taskService.CreateTaskQuery()
                .ToList();
            Assert.AreEqual(6, tasks.Count);

            // Complete two tasks
            taskService.Complete(tasks[0].Id);
            taskService.Complete(tasks[1].Id);

            // Fire timer
            var timer = managementService.CreateJobQuery()
                .First();
            managementService.ExecuteJob(timer.Id);

            var taskAfterTimer = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("taskAfterTimer", taskAfterTimer.TaskDefinitionKey);
            taskService.Complete(taskAfterTimer.Id);

            AssertProcessEnded(procId);
        }


        [Test]
        public virtual void testParallelSubProcessCompletionCondition()
        {
            var procId = runtimeService.StartProcessInstanceByKey("miParallelSubprocessCompletionCondition")
                .Id;

            IList<ITask> tasks = taskService.CreateTaskQuery()
                .ToList();
            Assert.AreEqual(4, tasks.Count);

            // get activities of a single subprocess
            var taskActivities = runtimeService.GetActivityInstance(procId)
                .GetActivityInstances("miSubProcess")[0].ChildActivityInstances;

            foreach (var taskActivity in taskActivities)
            {
                ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId==taskActivity.Id)
                    .First();
                taskService.Complete(task.Id);
            }

            AssertProcessEnded(procId);
        }

        [Test][Deployment(new string[] {"resources/bpmn/multiinstance/MultiInstanceTest.TestParallelSubProcessAllAutomatic.bpmn20.xml"}) ]
        public virtual void testParallelSubProcessAllAutomaticCompletionCondition()
        {
            var procId =
                runtimeService.StartProcessInstanceByKey("miParallelSubprocessAllAutomatics",
                        CollectionUtil.SingletonMap("nrOfLoops", 10))
                    .Id;
            var waitState = runtimeService.CreateExecutionQuery()
                .First();
            Assert.AreEqual(12, runtimeService.GetVariable(waitState.Id, "sum"));

            runtimeService.Signal(waitState.Id);
            AssertProcessEnded(procId);
        }

        [Test][Deployment(new string[] {"resources/bpmn/multiinstance/MultiInstanceTest.TestSequentialCallActivity.bpmn20.xml", "resources/bpmn/multiinstance/MultiInstanceTest.ExternalSubProcess.bpmn20.xml"})]
        public virtual void testSequentialCallActivity()
        {
            var procId = runtimeService.StartProcessInstanceByKey("miSequentialCallActivity")
                .Id;

            for (var i = 0; i < 3; i++)
            {
                IList<ITask> tasks = taskService.CreateTaskQuery()
                    /*.OrderByTaskName()*/
                    /*.Asc()*/
                    .ToList();
                Assert.AreEqual(2, tasks.Count);
                Assert.AreEqual("task one", tasks[0].Name);
                Assert.AreEqual("task two", tasks[1].Name);
                taskService.Complete(tasks[0].Id);
                taskService.Complete(tasks[1].Id);
            }

            AssertProcessEnded(procId);
        }

        [Test][Deployment( "resources/bpmn/multiinstance/MultiInstanceTest.TestSequentialCallActivityWithList.bpmn20.xml") ]
        public virtual void testSequentialCallActivityWithList()
        {
            var list = new List<string>();
            list.Add("one");
            list.Add("two");

            var variables = new Dictionary<string, object>();
            variables["list"] = list;

            var procId = runtimeService.StartProcessInstanceByKey("parentProcess", variables)
                .Id;

            ITask task1 = taskService.CreateTaskQuery()
                //.ProcessVariableValueEquals("element", "one")
                .First();
            ITask task2 = taskService.CreateTaskQuery()
                //.ProcessVariableValueEquals("element", "two")
                .First();

            Assert.NotNull(task1);
            Assert.NotNull(task2);

            var subVariables = new Dictionary<string, object>();
            subVariables["x"] = "y";

            taskService.Complete(task1.Id, subVariables);
            taskService.Complete(task2.Id, subVariables);

            ITask task3 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey=="midProcess")
                .First();
            Assert.NotNull(task3);
            taskService.Complete(task3.Id);

            ITask task4 = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey == "parentProcess")
                .First();
            Assert.NotNull(task4);
            taskService.Complete(task4.Id);

            AssertProcessEnded(procId);
        }

        [Test][Deployment(new []{"resources/bpmn/multiinstance/MultiInstanceTest.TestSequentialCallActivityWithTimer.bpmn20.xml", "resources/bpmn/multiinstance/MultiInstanceTest.ExternalSubProcess.bpmn20.xml" }) ]
        public virtual void testSequentialCallActivityWithTimer()
        {
            var procId = runtimeService.StartProcessInstanceByKey("miSequentialCallActivityWithTimer")
                .Id;

            // Complete first subprocess
            IList<ITask> tasks = taskService.CreateTaskQuery()
                /*.OrderByTaskName()*/
                /*.Asc()*/
                .ToList();
            Assert.AreEqual(2, tasks.Count);
            Assert.AreEqual("task one", tasks[0].Name);
            Assert.AreEqual("task two", tasks[1].Name);
            taskService.Complete(tasks[0].Id);
            taskService.Complete(tasks[1].Id);

            // Fire timer
            var timer = managementService.CreateJobQuery()
                .First();
            managementService.ExecuteJob(timer.Id);

            var taskAfterTimer = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("taskAfterTimer", taskAfterTimer.TaskDefinitionKey);
            taskService.Complete(taskAfterTimer.Id);

            AssertProcessEnded(procId);
        }

        [Test][Deployment(new []{"resources/bpmn/multiinstance/MultiInstanceTest.TestParallelCallActivity.bpmn20.xml", "resources/bpmn/multiinstance/MultiInstanceTest.ExternalSubProcess.bpmn20.xml" })]
        public virtual void testParallelCallActivity()
        {
            var procId = runtimeService.StartProcessInstanceByKey("miParallelCallActivity")
                .Id;
            IList<ITask> tasks = taskService.CreateTaskQuery()
                .ToList();
            Assert.AreEqual(12, tasks.Count);
            for (var i = 0; i < tasks.Count; i++)
                taskService.Complete(tasks[i].Id);

            AssertProcessEnded(procId);
        }

        [Test][Deployment(new []{"resources/bpmn/multiinstance/MultiInstanceTest.TestParallelCallActivity.bpmn20.xml", "resources/bpmn/multiinstance/MultiInstanceTest.ExternalSubProcess.bpmn20.xml" }) ]
        public virtual void testParallelCallActivityHistory()
        {
            runtimeService.StartProcessInstanceByKey("miParallelCallActivity");
            IList<ITask> tasks = taskService.CreateTaskQuery()
                .ToList();
            Assert.AreEqual(12, tasks.Count);
            for (var i = 0; i < tasks.Count; i++)
                taskService.Complete(tasks[i].Id);

            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                // Validate historic processes
                IList<IHistoricProcessInstance> historicProcessInstances =
                    historyService.CreateHistoricProcessInstanceQuery()
                        .ToList();
                Assert.AreEqual(7, historicProcessInstances.Count); // 6 subprocesses + main process
                foreach (var hpi in historicProcessInstances)
                {
                    Assert.NotNull(hpi.StartTime);
                    Assert.NotNull(hpi.EndTime);
                }

                // Validate historic activities
                IList<IHistoricActivityInstance> historicActivityInstances =
                    historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityType == "callActivity")
                        .ToList();
                Assert.AreEqual(6, historicActivityInstances.Count);
                foreach (var hai in historicActivityInstances)
                {
                    Assert.NotNull(hai.StartTime);
                    Assert.NotNull(hai.EndTime);
                }
            }

            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelActivity)
            {
                // Validate historic tasks
                IList<IHistoricTaskInstance> historicTaskInstances = historyService.CreateHistoricTaskInstanceQuery()
                    .ToList();
                Assert.AreEqual(12, historicTaskInstances.Count);
                foreach (var hti in historicTaskInstances)
                {
                    Assert.NotNull(hti.StartTime);
                    Assert.NotNull(hti.EndTime);
                    Assert.NotNull(hti.Assignee);
                    Assert.AreEqual("completed", hti.DeleteReason);
                }
            }
        }


        [Test][Deployment(new []{"resources/bpmn/multiinstance/MultiInstanceTest.TestParallelCallActivityWithTimer.bpmn20.xml", "resources/bpmn/multiinstance/MultiInstanceTest.ExternalSubProcess.bpmn20.xml" })]
        public virtual void testParallelCallActivityWithTimer()
        {
            var procId = runtimeService.StartProcessInstanceByKey("miParallelCallActivity")
                .Id;
            IList<ITask> tasks = taskService.CreateTaskQuery()
                .ToList();
            Assert.AreEqual(6, tasks.Count);
            for (var i = 0; i < 2; i++)
                taskService.Complete(tasks[i].Id);

            // Fire timer
            var timer = managementService.CreateJobQuery()
                .First();
            managementService.ExecuteJob(timer.Id);

            var taskAfterTimer = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("taskAfterTimer", taskAfterTimer.TaskDefinitionKey);
            taskService.Complete(taskAfterTimer.Id);

            AssertProcessEnded(procId);
        }

        [Test][Deployment(new []{"resources/bpmn/multiinstance/MultiInstanceTest.TestNestedSequentialCallActivity.bpmn20.xml", "resources/bpmn/multiinstance/MultiInstanceTest.ExternalSubProcess.bpmn20.xml" })]
        public virtual void testNestedSequentialCallActivity()
        {
            var procId = runtimeService.StartProcessInstanceByKey("miNestedSequentialCallActivity")
                .Id;

            for (var i = 0; i < 4; i++)
            {
                IList<ITask> tasks = taskService.CreateTaskQuery()
                    /*.OrderByTaskName()*/
                    /*.Asc()*/
                    .ToList();
                Assert.AreEqual(2, tasks.Count);
                Assert.AreEqual("task one", tasks[0].Name);
                Assert.AreEqual("task two", tasks[1].Name);
                taskService.Complete(tasks[0].Id);
                taskService.Complete(tasks[1].Id);
            }

            AssertProcessEnded(procId);
        }

        [Test][Deployment(new []{"resources/bpmn/multiinstance/MultiInstanceTest.TestNestedSequentialCallActivityWithTimer.bpmn20.xml", "resources/bpmn/multiinstance/MultiInstanceTest.ExternalSubProcess.bpmn20.xml" }) ]
        public virtual void testNestedSequentialCallActivityWithTimer()
        {
            var procId = runtimeService.StartProcessInstanceByKey("miNestedSequentialCallActivityWithTimer")
                .Id;

            // first instance
            IList<ITask> tasks = taskService.CreateTaskQuery()
                /*.OrderByTaskName()*/
                /*.Asc()*/
                .ToList();
            Assert.AreEqual(2, tasks.Count);
            Assert.AreEqual("task one", tasks[0].Name);
            Assert.AreEqual("task two", tasks[1].Name);
            taskService.Complete(tasks[0].Id);
            taskService.Complete(tasks[1].Id);

            // one task of second instance
            tasks = taskService.CreateTaskQuery()
                .ToList();
            Assert.AreEqual(2, tasks.Count);
            taskService.Complete(tasks[0].Id);

            // Fire timer
            var timer = managementService.CreateJobQuery()
                .First();
            managementService.ExecuteJob(timer.Id);

            var taskAfterTimer = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("taskAfterTimer", taskAfterTimer.TaskDefinitionKey);
            taskService.Complete(taskAfterTimer.Id);

            AssertProcessEnded(procId);
        }

        [Test][Deployment(new []{"resources/bpmn/multiinstance/MultiInstanceTest.TestNestedParallelCallActivity.bpmn20.xml", "resources/bpmn/multiinstance/MultiInstanceTest.ExternalSubProcess.bpmn20.xml" }) ]
        public virtual void testNestedParallelCallActivity()
        {
            var procId = runtimeService.StartProcessInstanceByKey("miNestedParallelCallActivity")
                .Id;

            IList<ITask> tasks = taskService.CreateTaskQuery()
                .ToList();
            Assert.AreEqual(14, tasks.Count);
            for (var i = 0; i < 14; i++)
                taskService.Complete(tasks[i].Id);

            AssertProcessEnded(procId);
        }

        [Test][Deployment(new []{"resources/bpmn/multiinstance/MultiInstanceTest.TestNestedParallelCallActivityWithTimer.bpmn20.xml", "resources/bpmn/multiinstance/MultiInstanceTest.ExternalSubProcess.bpmn20.xml" }) ]
        public virtual void testNestedParallelCallActivityWithTimer()
        {
            var procId = runtimeService.StartProcessInstanceByKey("miNestedParallelCallActivityWithTimer")
                .Id;

            IList<ITask> tasks = taskService.CreateTaskQuery()
                .ToList();
            Assert.AreEqual(4, tasks.Count);
            for (var i = 0; i < 3; i++)
                taskService.Complete(tasks[i].Id);

            // Fire timer
            var timer = managementService.CreateJobQuery()
                .First();
            managementService.ExecuteJob(timer.Id);

            var taskAfterTimer = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("taskAfterTimer", taskAfterTimer.TaskDefinitionKey);
            taskService.Complete(taskAfterTimer.Id);

            AssertProcessEnded(procId);
        }

        [Test][Deployment(new []{"resources/bpmn/multiinstance/MultiInstanceTest.TestNestedParallelCallActivityCompletionCondition.bpmn20.xml", "resources/bpmn/multiinstance/MultiInstanceTest.ExternalSubProcess.bpmn20.xml" }) ]
        public virtual void testNestedParallelCallActivityCompletionCondition()
        {
            var procId = runtimeService.StartProcessInstanceByKey("miNestedParallelCallActivityCompletionCondition")
                .Id;

            Assert.AreEqual(8, taskService.CreateTaskQuery()
                .Count());

            for (var i = 0; i < 2; i++)
            {
                IProcessInstance nextSubProcessInstance =
                        runtimeService.CreateProcessInstanceQuery()
                            //.SetProcessDefinitionKey("externalSubProcess")
                            /*.ListPage(0, 1)*/
                            .ToList()
                    .
                First(); //Get(0);
                IList<ITask> tasks = taskService.CreateTaskQuery(c => c.ProcessInstanceId == nextSubProcessInstance.Id)
                    .ToList();
                Assert.AreEqual(2, tasks.Count);
                foreach (var task in tasks)
                    taskService.Complete(task.Id);
            }

            AssertProcessEnded(procId);
        }

        // ACT-764

        [Test]
        public virtual void testSequentialServiceTaskWithClass()
        {
            var procInst = runtimeService.StartProcessInstanceByKey("multiInstanceServiceTask",
                CollectionUtil.SingletonMap("result", 5));
            var result = (int?) runtimeService.GetVariable(procInst.Id, "result");
            Assert.AreEqual(160, result.Value);

            runtimeService.Signal(procInst.Id);
            AssertProcessEnded(procInst.Id);
        }

        [Test][Deployment(new []{"resources/bpmn/multiinstance/MultiInstanceTest.CallActivityWithBoundaryErrorEvent.bpmn20.xml", "resources/bpmn/multiinstance/MultiInstanceTest.ThrowingErrorEventSubProcess.bpmn20.xml" }) ]
        public virtual void testMultiInstanceCallActivityWithErrorBoundaryEvent()
        {
            IDictionary<string, object> variableMap = new Dictionary<string, object>();
            variableMap["assignees"] = new List<string> {"kermit", "gonzo"};

            var processInstance = runtimeService.StartProcessInstanceByKey("process", variableMap);

            IList<ITask> tasks = taskService.CreateTaskQuery()
                .ToList();
            Assert.AreEqual(2, tasks.Count);

            // finish first call activity with error
            variableMap = new Dictionary<string, object>();
            variableMap["done"] = false;
            taskService.Complete(tasks[0].Id, variableMap);

            tasks = taskService.CreateTaskQuery()
                .ToList();
            Assert.AreEqual(1, tasks.Count);

            taskService.Complete(tasks[0].Id);

            IList<IProcessInstance> processInstances = runtimeService.CreateProcessInstanceQuery()
                //.SetProcessDefinitionKey("process")
                .ToList();
            Assert.AreEqual(0, processInstances.Count);
            AssertProcessEnded(processInstance.Id);
        }

        [Test][Deployment(new []{"resources/bpmn/multiinstance/MultiInstanceTest.CallActivityWithBoundaryErrorEventSequential.bpmn20.xml", "resources/bpmn/multiinstance/MultiInstanceTest.ThrowingErrorEventSubProcess.bpmn20.xml" })]
        public virtual void testSequentialMultiInstanceCallActivityWithErrorBoundaryEvent()
        {
            IDictionary<string, object> variableMap = new Dictionary<string, object>();
            variableMap["assignees"] = new List<string> {"kermit", "gonzo"};

            var processInstance = runtimeService.StartProcessInstanceByKey("process", variableMap);

            IList<ITask> tasks = taskService.CreateTaskQuery()
                .ToList();
            Assert.AreEqual(1, tasks.Count);

            // finish first call activity with error
            variableMap = new Dictionary<string, object>();
            variableMap["done"] = false;
            taskService.Complete(tasks[0].Id, variableMap);

            tasks = taskService.CreateTaskQuery()
                .ToList();
            Assert.AreEqual(1, tasks.Count);

            taskService.Complete(tasks[0].Id);

            AssertProcessEnded(processInstance.Id);
        }

        [Test][Deployment(new []{"resources/bpmn/multiinstance/MultiInstanceTest.TestNestedMultiInstanceTasks.bpmn20.xml"})]
        public virtual void testNestedMultiInstanceTasks()
        {
            IList<string> processes = new List<string> {"process A", "process B"};
            IList<string> assignees = new List<string> {"kermit", "gonzo"};
            IDictionary<string, object> variableMap = new Dictionary<string, object>();
            variableMap["subProcesses"] = processes;
            variableMap["assignees"] = assignees;

            var processInstance = runtimeService.StartProcessInstanceByKey("miNestedMultiInstanceTasks", variableMap);

            IList<ITask> tasks = taskService.CreateTaskQuery()
                .ToList();
            Assert.AreEqual(processes.Count * assignees.Count, tasks.Count);

            foreach (var t in tasks)
                taskService.Complete(t.Id);

            IList<IProcessInstance> processInstances = runtimeService.CreateProcessInstanceQueryProcessDefinitionKey("miNestedMultiInstanceTasks")
                //.SetProcessDefinitionKey("miNestedMultiInstanceTasks")
                .ToList();
            Assert.AreEqual(0, processInstances.Count);
            AssertProcessEnded(processInstance.Id);
        }

        [Test][Deployment(new []{"resources/bpmn/multiinstance/MultiInstanceTest.TestNestedMultiInstanceTasks.bpmn20.xml"}) ]
        public virtual void testNestedMultiInstanceTasksActivityInstance()
        {
            IList<string> processes = new List<string> {"process A", "process B"};
            IList<string> assignees = new List<string> {"kermit", "gonzo"};
            IDictionary<string, object> variableMap = new Dictionary<string, object>();
            variableMap["subProcesses"] = processes;
            variableMap["assignees"] = assignees;

            var processInstance = runtimeService.StartProcessInstanceByKey("miNestedMultiInstanceTasks", variableMap);

            var activityInstance = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(activityInstance)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginMiBody("subprocess1")
                    .BeginScope("subprocess1")
                    .BeginMiBody("miTasks")
                    .Activity("miTasks")
                    .Activity("miTasks")
                    .EndScope()
                    .EndScope()
                    .BeginScope("subprocess1")
                    .BeginMiBody("miTasks")
                    .Activity("miTasks")
                    .Activity("miTasks")
                    .EndScope()
                    .EndScope()
                    .Done());
        }


        [Test]
        public virtual void testActiveExecutionsInParallelTasks()
        {
            runtimeService.StartProcessInstanceByKey("miParallelUserTasks"); //.Id;

            var instance = runtimeService.CreateProcessInstanceQuery()
                .First();

            IList<IExecution> executions = runtimeService.CreateExecutionQuery()
                .ToList();
            Assert.AreEqual(5, executions.Count);

            foreach (var execution in executions)
            {
                var entity = (ExecutionEntity) execution;

                if (!entity.Id.Equals(instance.Id) && !entity.ParentId.Equals(instance.Id))
                    Assert.True(entity.IsActive);
                else
                    Assert.IsFalse(entity.IsActive);
            }
        }

        [Test][Deployment(new []{"resources/bpmn/multiinstance/MultiInstanceTest.TestCatchErrorThrownBySequentialAbstractBpmnActivityBehavior.bpmn20.xml" }) ]
        public virtual void testCatchExceptionThrownByExecuteOfSequentialAbstractBpmnActivityBehavior()
        {
            var pi = runtimeService.StartProcessInstanceByKey("testProcess", ThrowErrorDelegate.throwException())
                .Id;

            Assert.True((bool) runtimeService.GetVariable(pi, "executed"));
            Assert.IsNull(runtimeService.GetVariable(pi, "signaled"));

            var userTask = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi)
                .First();
            Assert.NotNull(userTask);
            Assert.AreEqual("userTaskException", userTask.TaskDefinitionKey);

            taskService.Complete(userTask.Id);
        }

        [Test][Deployment(new []{"resources/bpmn/multiinstance/MultiInstanceTest.TestCatchErrorThrownBySequentialAbstractBpmnActivityBehavior.bpmn20.xml" }) ]
        public virtual void testCatchErrorThrownByExecuteOfSequentialAbstractBpmnActivityBehavior()
        {
            var pi = runtimeService.StartProcessInstanceByKey("testProcess", ThrowErrorDelegate.throwError())
                .Id;

            Assert.True((bool) runtimeService.GetVariable(pi, "executed"));
            Assert.IsNull(runtimeService.GetVariable(pi, "signaled"));

            var userTask = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi)
                .First();
            Assert.NotNull(userTask);
            Assert.AreEqual("userTaskError", userTask.TaskDefinitionKey);

            taskService.Complete(userTask.Id);
        }


        [Test]
        public virtual void testCatchExceptionThrownBySignalOfSequentialAbstractBpmnActivityBehavior()
        {
            var pi = runtimeService.StartProcessInstanceByKey("testProcess")
                .Id;

            Assert.True((bool) runtimeService.GetVariable(pi, "executed"));
            Assert.IsNull(runtimeService.GetVariable(pi, "signaled"));

            // signal 2 times to execute first sequential behaviors
            runtimeService.SetVariables(pi, ThrowErrorDelegate.leaveExecution());
            runtimeService.Signal(
                runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == pi && c.ActivityId == "serviceTask")
                    .First()
                    .Id);
            runtimeService.SetVariables(pi, ThrowErrorDelegate.leaveExecution());
            runtimeService.Signal(
                runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == pi && c.ActivityId == "serviceTask")
                    .First()
                    .Id);

            var serviceTask =
                runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == pi && c.ActivityId == "serviceTask")
                    .First();
            Assert.NotNull(serviceTask);

            runtimeService.SetVariables(pi, ThrowErrorDelegate.throwException());
            runtimeService.Signal(serviceTask.Id);

            Assert.True((bool) runtimeService.GetVariable(pi, "executed"));
            Assert.True((bool) runtimeService.GetVariable(pi, "signaled"));

            var userTask = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi)
                .First();
            Assert.NotNull(userTask);
            Assert.AreEqual("userTaskException", userTask.TaskDefinitionKey);

            taskService.Complete(userTask.Id);
        }

        [Test][Deployment(new []{"resources/bpmn/multiinstance/MultiInstanceTest.TestCatchErrorThrownBySequentialAbstractBpmnActivityBehavior.bpmn20.xml" })]
        public virtual void testCatchErrorThrownBySignalOfSequentialAbstractBpmnActivityBehavior()
        {
            var pi = runtimeService.StartProcessInstanceByKey("testProcess")
                .Id;

            Assert.True((bool) runtimeService.GetVariable(pi, "executed"));
            Assert.IsNull(runtimeService.GetVariable(pi, "signaled"));

            // signal 2 times to execute first sequential behaviors
            runtimeService.SetVariables(pi, ThrowErrorDelegate.leaveExecution());
            runtimeService.Signal(
                runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == pi && c.ActivityId == "serviceTask")
                    .First()
                    .Id);
            runtimeService.SetVariables(pi, ThrowErrorDelegate.leaveExecution());
            runtimeService.Signal(
                runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == pi && c.ActivityId == "serviceTask")
                    .First()
                    .Id);

            var serviceTask =
                runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == pi && c.ActivityId == "serviceTask")
                    .First();
            Assert.NotNull(serviceTask);

            runtimeService.SetVariables(pi, ThrowErrorDelegate.throwError());
            runtimeService.Signal(serviceTask.Id);

            Assert.True((bool) runtimeService.GetVariable(pi, "executed"));
            Assert.True((bool) runtimeService.GetVariable(pi, "signaled"));

            var userTask = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi)
                .First();
            Assert.NotNull(userTask);
            Assert.AreEqual("userTaskError", userTask.TaskDefinitionKey);

            taskService.Complete(userTask.Id);
        }

        [Test][Deployment(new []{"resources/bpmn/multiinstance/MultiInstanceTest.TestCatchErrorThrownByParallelAbstractBpmnActivityBehavior.bpmn20.xml" })]
        public virtual void testCatchExceptionThrownByExecuteOfParallelAbstractBpmnActivityBehavior()
        {
            var pi = runtimeService.StartProcessInstanceByKey("testProcess", ThrowErrorDelegate.throwException())
                .Id;

            Assert.True((bool) runtimeService.GetVariable(pi, "executed"));
            Assert.IsNull(runtimeService.GetVariable(pi, "signaled"));

            var userTask = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi)
                .First();
            Assert.NotNull(userTask);
            Assert.AreEqual("userTaskException", userTask.TaskDefinitionKey);

            taskService.Complete(userTask.Id);
        }

        [Test]
        [Deployment(new []{"resources/bpmn/multiinstance/MultiInstanceTest.TestCatchErrorThrownByParallelAbstractBpmnActivityBehavior.bpmn20.xml" }) ]
        public virtual void testCatchErrorThrownByExecuteOfParallelAbstractBpmnActivityBehavior()
        {
            var pi = runtimeService.StartProcessInstanceByKey("testProcess", ThrowErrorDelegate.throwError())
                .Id;

            Assert.True((bool) runtimeService.GetVariable(pi, "executed"));
            Assert.IsNull(runtimeService.GetVariable(pi, "signaled"));

            var userTask = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi)
                .First();
            Assert.NotNull(userTask);
            Assert.AreEqual("userTaskError", userTask.TaskDefinitionKey);

            taskService.Complete(userTask.Id);
        }


        [Test]
        public virtual void testCatchExceptionThrownBySignalOfParallelAbstractBpmnActivityBehavior()
        {
            var pi = runtimeService.StartProcessInstanceByKey("testProcess")
                .Id;

            Assert.True((bool) runtimeService.GetVariable(pi, "executed"));
            Assert.IsNull(runtimeService.GetVariable(pi, "signaled"));

            IExecution serviceTask =
                runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == pi && c.ActivityId == "serviceTask")
                    .ToList()[3];//.Get(3);
            Assert.NotNull(serviceTask);

            runtimeService.SetVariables(pi, ThrowErrorDelegate.throwException());
            runtimeService.Signal(serviceTask.Id);

            Assert.True((bool) runtimeService.GetVariable(pi, "executed"));
            Assert.True((bool) runtimeService.GetVariable(pi, "signaled"));

            var userTask = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi)
                .First();
            Assert.NotNull(userTask);
            Assert.AreEqual("userTaskException", userTask.TaskDefinitionKey);

            taskService.Complete(userTask.Id);
        }


        [Test]
        public virtual void testCatchErrorThrownBySignalOfParallelAbstractBpmnActivityBehavior()
        {
            var pi = runtimeService.StartProcessInstanceByKey("testProcess")
                .Id;

            Assert.True((bool) runtimeService.GetVariable(pi, "executed"));
            Assert.IsNull(runtimeService.GetVariable(pi, "signaled"));

            IExecution serviceTask =
                runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == pi && c.ActivityId == "serviceTask")
                    .ToList()[3];//.Get(3);
            Assert.NotNull(serviceTask);

            runtimeService.SetVariables(pi, ThrowErrorDelegate.throwError());
            runtimeService.Signal(serviceTask.Id);

            Assert.True((bool) runtimeService.GetVariable(pi, "executed"));
            Assert.True((bool) runtimeService.GetVariable(pi, "signaled"));

            var userTask = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi)
                .First();
            Assert.NotNull(userTask);
            Assert.AreEqual("userTaskError", userTask.TaskDefinitionKey);

            taskService.Complete(userTask.Id);
        }

        [Test][Deployment(new []{"resources/bpmn/multiinstance/MultiInstanceTest.TestCatchErrorThrownBySequentialDelegateExpression.bpmn20.xml" }) ]
        public virtual void testCatchExceptionThrownByExecuteOfSequentialDelegateExpression()
        {
            var variables = Variables.CreateVariables()
                .PutValue("myDelegate", new ThrowErrorDelegate());
            variables.PutAll(ThrowErrorDelegate.throwException() as IDictionary<string, ITypedValue>);
            var pi = runtimeService.StartProcessInstanceByKey("testProcess", variables)
                .Id;

            Assert.True((bool) runtimeService.GetVariable(pi, "executed"));
            Assert.IsNull(runtimeService.GetVariable(pi, "signaled"));

            var userTask = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi)
                .First();
            Assert.NotNull(userTask);
            Assert.AreEqual("userTaskException", userTask.TaskDefinitionKey);

            taskService.Complete(userTask.Id);
        }

        [Test][Deployment(new []{"resources/bpmn/multiinstance/MultiInstanceTest.TestCatchErrorThrownBySequentialDelegateExpression.bpmn20.xml" }) ]
        public virtual void testCatchErrorThrownByExecuteOfSequentialDelegateExpression()
        {
            var variables = Variables.CreateVariables()
                .PutValue("myDelegate", new ThrowErrorDelegate());
            variables.PutAll(ThrowErrorDelegate.throwException() as IDictionary<string, ITypedValue>);
            var pi = runtimeService.StartProcessInstanceByKey("testProcess", variables)
                .Id;

            Assert.True((bool) runtimeService.GetVariable(pi, "executed"));
            Assert.IsNull(runtimeService.GetVariable(pi, "signaled"));

            var userTask = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi)
                .First();
            Assert.NotNull(userTask);
            Assert.AreEqual("userTaskError", userTask.TaskDefinitionKey);

            taskService.Complete(userTask.Id);
        }

        [Test][Deployment(new []{"resources/bpmn/multiinstance/MultiInstanceTest.TestCatchErrorThrownBySequentialDelegateExpression.bpmn20.xml" }) ]
        public virtual void testCatchExceptionThrownBySignalOfSequentialDelegateExpression()
        {
            var variables = Variables.CreateVariables()
                .PutValue("myDelegate", new ThrowErrorDelegate());
            var pi = runtimeService.StartProcessInstanceByKey("testProcess", variables)
                .Id;

            Assert.True((bool) runtimeService.GetVariable(pi, "executed"));
            Assert.IsNull(runtimeService.GetVariable(pi, "signaled"));

            // signal 2 times to execute first sequential behaviors
            runtimeService.SetVariables(pi, ThrowErrorDelegate.leaveExecution());
            runtimeService.Signal(
                runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == pi && c.ActivityId == "serviceTask")
                    .First()
                    .Id);
            runtimeService.SetVariables(pi, ThrowErrorDelegate.leaveExecution());
            runtimeService.Signal(
                runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == pi && c.ActivityId == "serviceTask")
                    .First()
                    .Id);

            var serviceTask =
                runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == pi && c.ActivityId == "serviceTask")
                    .First();
            Assert.NotNull(serviceTask);

            runtimeService.SetVariables(pi, ThrowErrorDelegate.throwException());
            runtimeService.Signal(serviceTask.Id);

            Assert.True((bool) runtimeService.GetVariable(pi, "executed"));
            Assert.True((bool) runtimeService.GetVariable(pi, "signaled"));

            var userTask = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi)
                .First();
            Assert.NotNull(userTask);
            Assert.AreEqual("userTaskException", userTask.TaskDefinitionKey);

            taskService.Complete(userTask.Id);
        }

        [Test][Deployment(new []{"resources/bpmn/multiinstance/MultiInstanceTest.TestCatchErrorThrownBySequentialDelegateExpression.bpmn20.xml" }) ]
        public virtual void testCatchErrorThrownBySignalOfSequentialDelegateExpression()
        {
            var variables = Variables.CreateVariables()
                .PutValue("myDelegate", new ThrowErrorDelegate());
            var pi = runtimeService.StartProcessInstanceByKey("testProcess", variables)
                .Id;

            Assert.True((bool) runtimeService.GetVariable(pi, "executed"));
            Assert.IsNull(runtimeService.GetVariable(pi, "signaled"));

            // signal 2 times to execute first sequential behaviors
            runtimeService.SetVariables(pi, ThrowErrorDelegate.leaveExecution());
            runtimeService.Signal(
                runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == pi && c.ActivityId == "serviceTask")
                    .First()
                    .Id);
            runtimeService.SetVariables(pi, ThrowErrorDelegate.leaveExecution());
            runtimeService.Signal(
                runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == pi && c.ActivityId == "serviceTask")
                    .First()
                    .Id);

            var serviceTask =
                runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == pi && c.ActivityId == "serviceTask")
                    .First();
            Assert.NotNull(serviceTask);

            runtimeService.SetVariables(pi, ThrowErrorDelegate.throwError());
            runtimeService.Signal(serviceTask.Id);

            Assert.True((bool) runtimeService.GetVariable(pi, "executed"));
            Assert.True((bool) runtimeService.GetVariable(pi, "signaled"));

            var userTask = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi)
                .First();
            Assert.NotNull(userTask);
            Assert.AreEqual("userTaskError", userTask.TaskDefinitionKey);

            taskService.Complete(userTask.Id);
        }

        [Test][Deployment(new []{"resources/bpmn/multiinstance/MultiInstanceTest.TestCatchErrorThrownByParallelDelegateExpression.bpmn20.xml" }) ]
        public virtual void testCatchExceptionThrownByExecuteOfParallelDelegateExpression()
        {
            var variables = Variables.CreateVariables()
                .PutValue("myDelegate", new ThrowErrorDelegate());
            variables.PutAll(ThrowErrorDelegate.throwException() as IDictionary<string, ITypedValue>);
            var pi = runtimeService.StartProcessInstanceByKey("testProcess", variables)
                .Id;

            Assert.True((bool) runtimeService.GetVariable(pi, "executed"));
            Assert.IsNull(runtimeService.GetVariable(pi, "signaled"));

            var userTask = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi)
                .First();
            Assert.NotNull(userTask);
            Assert.AreEqual("userTaskException", userTask.TaskDefinitionKey);

            taskService.Complete(userTask.Id);
        }

        [Test][Deployment(new []{"resources/bpmn/multiinstance/MultiInstanceTest.TestCatchErrorThrownByParallelDelegateExpression.bpmn20.xml" })]
        public virtual void testCatchErrorThrownByExecuteOfParallelDelegateExpression()
        {
            var variables = Variables.CreateVariables()
                .PutValue("myDelegate", new ThrowErrorDelegate());
            variables.PutAll(ThrowErrorDelegate.throwException() as IDictionary<string, ITypedValue>);
            var pi = runtimeService.StartProcessInstanceByKey("testProcess", variables)
                .Id;

            Assert.True((bool) runtimeService.GetVariable(pi, "executed"));
            Assert.IsNull(runtimeService.GetVariable(pi, "signaled"));

            var userTask = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi)
                .First();
            Assert.NotNull(userTask);
            Assert.AreEqual("userTaskError", userTask.TaskDefinitionKey);

            taskService.Complete(userTask.Id);
        }

        [Test][Deployment(new []{"resources/bpmn/multiinstance/MultiInstanceTest.TestCatchErrorThrownByParallelDelegateExpression.bpmn20.xml" }) ]
        public virtual void testCatchExceptionThrownBySignalOfParallelDelegateExpression()
        {
            var variables = Variables.CreateVariables()
                .PutValue("myDelegate", new ThrowErrorDelegate());
            var pi = runtimeService.StartProcessInstanceByKey("testProcess", variables)
                .Id;

            Assert.True((bool) runtimeService.GetVariable(pi, "executed"));
            Assert.IsNull(runtimeService.GetVariable(pi, "signaled"));

            var serviceTask =
                runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == pi && c.ActivityId == "serviceTask")
                    .ToList()[3]; //Get(3)
            Assert.NotNull(serviceTask);

            runtimeService.SetVariables(pi, ThrowErrorDelegate.throwException());
            runtimeService.Signal(serviceTask.Id);

            Assert.True((bool) runtimeService.GetVariable(pi, "executed"));
            Assert.True((bool) runtimeService.GetVariable(pi, "signaled"));

            var userTask = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi)
                .First();
            Assert.NotNull(userTask);
            Assert.AreEqual("userTaskException", userTask.TaskDefinitionKey);

            taskService.Complete(userTask.Id);
        }

        [Test][Deployment(new []{"resources/bpmn/multiinstance/MultiInstanceTest.TestCatchErrorThrownByParallelDelegateExpression.bpmn20.xml" })]
        public virtual void testCatchErrorThrownBySignalOfParallelDelegateExpression()
        {
            var variables = Variables.CreateVariables()
                .PutValue("myDelegate", new ThrowErrorDelegate());
            var pi = runtimeService.StartProcessInstanceByKey("testProcess", variables)
                .Id;

            Assert.True((bool) runtimeService.GetVariable(pi, "executed"));
            Assert.IsNull(runtimeService.GetVariable(pi, "signaled"));

            var serviceTask =
                runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == pi && c.ActivityId == "serviceTask")
                    .ToList()[3]; //Get(3);
            Assert.NotNull(serviceTask);

            runtimeService.SetVariables(pi, ThrowErrorDelegate.throwError());
            runtimeService.Signal(serviceTask.Id);

            Assert.True((bool) runtimeService.GetVariable(pi, "executed"));
            Assert.True((bool) runtimeService.GetVariable(pi, "signaled"));

            var userTask = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi)
                .First();
            Assert.NotNull(userTask);
            Assert.AreEqual("userTaskError", userTask.TaskDefinitionKey);

            taskService.Complete(userTask.Id);
        }

        // ACT-901
        [Test]
        [Deployment]
        public virtual void testAct901()
        {
            var startTime = ClockUtil.CurrentTime;

            var pi = runtimeService.StartProcessInstanceByKey("multiInstanceSubProcess");
            IList<ITask> tasks = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id)
                /*.OrderByTaskName()*/
                /*.Asc()*/
                .ToList();

            ClockUtil.CurrentTime = new DateTime(startTime.Ticks + 61000L); // timer is set to one minute
            IList<IJob> timers = managementService.CreateJobQuery()
                .ToList();
            Assert.AreEqual(5, timers.Count);

            // Execute all timers one by one (single thread vs thread pool of job executor, which leads to optimisticlockingexceptions!)
            foreach (var timer in timers)
                managementService.ExecuteJob(timer.Id);

            // All tasks should be canceled
            tasks = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id)
                /*.OrderByTaskName()*/
                /*.Asc()*/
                .ToList();
            Assert.AreEqual(0, tasks.Count);
        }

        [Test]
        [Deployment]
        public virtual void testNestedParallelSubProcess()
        {
            var procId = runtimeService.StartProcessInstanceByKey("miNestedParallelSubProcess")
                .Id;
            IList<ITask> tasks = taskService.CreateTaskQuery()
                .ToList();
            Assert.AreEqual(8, tasks.Count);

            foreach (var task in tasks)
                taskService.Complete(task.Id);
            AssertProcessEnded(procId);
        }

        [Test]
        [Deployment]
        public virtual void testNestedParallelSubProcessWithTimer()
        {
            var procId = runtimeService.StartProcessInstanceByKey("miNestedParallelSubProcess")
                .Id;
            IList<ITask> tasks = taskService.CreateTaskQuery()
                .ToList();
            Assert.AreEqual(12, tasks.Count);

            for (var i = 0; i < 3; i++)
                taskService.Complete(tasks[i].Id);

            // Fire timer
            var timer = managementService.CreateJobQuery()
                .First();
            managementService.ExecuteJob(timer.Id);

            var taskAfterTimer = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("taskAfterTimer", taskAfterTimer.TaskDefinitionKey);
            taskService.Complete(taskAfterTimer.Id);

            AssertProcessEnded(procId);
        }

        [Test]
        [Deployment]
        public virtual void testNestedParallelUserTasks()
        {
            var procId = runtimeService.StartProcessInstanceByKey("miNestedParallelUserTasks")
                .Id;

            IList<ITask> tasks = taskService.CreateTaskQuery(c=>c.Assignee == "kermit")
                .ToList();
            foreach (var task in tasks)
            {
                Assert.AreEqual("My Task", task.Name);
                taskService.Complete(task.Id);
            }

            AssertProcessEnded(procId);
        }

        [Test]
        [Deployment]
        public virtual void testNestedSequentialSubProcess()
        {
            var procId = runtimeService.StartProcessInstanceByKey("miNestedSequentialSubProcess")
                .Id;

            for (var i = 0; i < 3; i++)
            {
                IList<ITask> tasks = taskService.CreateTaskQuery(c=>c.Assignee=="kermit")
                    .ToList();
                taskService.Complete(tasks[0].Id);
                taskService.Complete(tasks[1].Id);
            }

            AssertProcessEnded(procId);
        }

        [Test]
        [Deployment]
        public virtual void testNestedSequentialSubProcessWithTimer()
        {
            var procId = runtimeService.StartProcessInstanceByKey("miNestedSequentialSubProcessWithTimer")
                .Id;

            for (var i = 0; i < 2; i++)
            {
                IList<ITask> tasks = taskService.CreateTaskQuery(c=>c.Assignee == "kermit")
                    .ToList();
                taskService.Complete(tasks[0].Id);
                taskService.Complete(tasks[1].Id);
            }

            // Complete one task, to make it a bit more trickier
            IList<ITask> taskQUERYs = taskService.CreateTaskQuery(c=>c.Assignee == "kermit")
                .ToList();
            taskService.Complete(taskQUERYs[0].Id);

            // Fire timer
            var timer = managementService.CreateJobQuery()
                .First();
            managementService.ExecuteJob(timer.Id);

            var taskAfterTimer = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("taskAfterTimer", taskAfterTimer.TaskDefinitionKey);
            taskService.Complete(taskAfterTimer.Id);

            AssertProcessEnded(procId);
        }

        [Test]
        [Deployment]
        public virtual void testParallelMITasksExecutionListener()
        {
            RecordInvocationListener.reset();

            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["nrOfLoops"] = 5;
            runtimeService.StartProcessInstanceByKey("miSequentialListener", vars);

            Assert.AreEqual(5, (int) RecordInvocationListener.INVOCATIONS[ExecutionListenerFields.EventNameStart]);
            Assert.IsNull(RecordInvocationListener.INVOCATIONS[ExecutionListenerFields.EventNameEnd]);

            IList<ITask> tasks = taskService.CreateTaskQuery()
                .ToList();
            taskService.Complete(tasks[0].Id);

            Assert.AreEqual(5, (int) RecordInvocationListener.INVOCATIONS[ExecutionListenerFields.EventNameStart]);
            Assert.AreEqual(1, (int) RecordInvocationListener.INVOCATIONS[ExecutionListenerFields.EventNameEnd]);

            taskService.Complete(tasks[1].Id);
            taskService.Complete(tasks[2].Id);
            taskService.Complete(tasks[3].Id);
            taskService.Complete(tasks[4].Id);

            Assert.AreEqual(5, (int) RecordInvocationListener.INVOCATIONS[ExecutionListenerFields.EventNameStart]);
            Assert.AreEqual(5, (int) RecordInvocationListener.INVOCATIONS[ExecutionListenerFields.EventNameEnd]);
        }

        [Test]
        [Deployment]
        public virtual void testParallelSubProcess()
        {
            var procId = runtimeService.StartProcessInstanceByKey("miParallelSubprocess")
                .Id;
            IList<ITask> tasks = taskService.CreateTaskQuery(c=>c.ProcessInstanceId == procId)
                /*.OrderByTaskName()*/
                /*.Asc()*/
                .ToList();
            Assert.AreEqual(4, tasks.Count);

            foreach (var task in tasks)
                taskService.Complete(task.Id);
            AssertProcessEnded(procId);
        }

        [Test]
        [Deployment]
        public virtual void testParallelSubProcessAllAutomatic()
        {
            var procId =
                runtimeService.StartProcessInstanceByKey("miParallelSubprocessAllAutomatics",
                        CollectionUtil.SingletonMap("nrOfLoops", 5))
                    .Id;
            var waitState = runtimeService.CreateExecutionQuery()
                .First();
            Assert.AreEqual(10, runtimeService.GetVariable(waitState.Id, "sum"));

            runtimeService.Signal(waitState.Id);
            AssertProcessEnded(procId);
        }

        [Test]
        [Deployment]
        public virtual void testParallelUserTasksBasedOnCollection()
        {
            IList<string> assigneeList = new List<string> {"kermit", "gonzo", "mispiggy", "fozzie", "bubba"};
            var procId = runtimeService.StartProcessInstanceByKey("miParallelUserTasksBasedOnCollection"
                    , CollectionUtil.SingletonMap("assigneeList", assigneeList))
                .Id;

            IList<ITask> tasks = taskService.CreateTaskQuery()
                /*.OrderByTaskAssignee()*/
                /*.Asc()*/
                .ToList();
            Assert.AreEqual(5, tasks.Count);
            Assert.AreEqual("bubba", tasks[0].Assignee);
            Assert.AreEqual("fozzie", tasks[1].Assignee);
            Assert.AreEqual("gonzo", tasks[2].Assignee);
            Assert.AreEqual("kermit", tasks[3].Assignee);
            Assert.AreEqual("mispiggy", tasks[4].Assignee);

            // Completing 3 tasks will trigger completioncondition
            taskService.Complete(tasks[0].Id);
            taskService.Complete(tasks[1].Id);
            taskService.Complete(tasks[2].Id);
            Assert.AreEqual(0, taskService.CreateTaskQuery()
                .Count());
            AssertProcessEnded(procId);
        }

        [Test]
        [Deployment]
        public virtual void testSequentialMITasksExecutionListener()
        {
            RecordInvocationListener.reset();

            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["nrOfLoops"] = 2;
            runtimeService.StartProcessInstanceByKey("miSequentialListener", vars);

            Assert.AreEqual(1, (int) RecordInvocationListener.INVOCATIONS[ExecutionListenerFields.EventNameStart]);
                //.EVENTNAME_START]);
            Assert.IsNull(RecordInvocationListener.INVOCATIONS[ExecutionListenerFields.EventNameEnd]);

            var task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);

            Assert.AreEqual(2, (int) RecordInvocationListener.INVOCATIONS[ExecutionListenerFields.EventNameStart]);
            Assert.AreEqual(1, (int) RecordInvocationListener.INVOCATIONS[ExecutionListenerFields.EventNameEnd]);

            task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);

            Assert.AreEqual(2, (int) RecordInvocationListener.INVOCATIONS[ExecutionListenerFields.EventNameStart]);
            Assert.AreEqual(2, (int) RecordInvocationListener.INVOCATIONS[ExecutionListenerFields.EventNameEnd]);
        }

        [Test]
        [Deployment]
        public virtual void testSequentialScriptTasks()
        {
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["sum"] = 0;
            vars["nrOfLoops"] = 5;
            runtimeService.StartProcessInstanceByKey("miSequentialScriptTask", vars);
            var waitStateExecution = runtimeService.CreateExecutionQuery()
                .First();
            var sum = (int?) runtimeService.GetVariable(waitStateExecution.Id, "sum"); //.Value);
            Assert.AreEqual(10, sum);
        }

        [Test]
        [Deployment]
        public virtual void testSequentialServiceTaskWithClassAndCollection()
        {
            ICollection<int> items = new List<int> {1, 2, 3, 4, 5, 6};
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["result"] = 1;
            vars["items"] = items;

            var procInst = runtimeService.StartProcessInstanceByKey("multiInstanceServiceTask", vars);
            var result = (int?) runtimeService.GetVariable(procInst.Id, "result");
            Assert.AreEqual(720, result.Value);

            runtimeService.Signal(procInst.Id);
            AssertProcessEnded(procInst.Id);
        }

        [Test]
        [Deployment]
        public virtual void testSequentialSubProcessCompletionCondition()
        {
            var procId = runtimeService.StartProcessInstanceByKey("miSequentialSubprocessCompletionCondition")
                .Id;

            IQueryable<ITask> query = taskService.CreateTaskQuery()
                /*.OrderByTaskName()*/
                /*.Asc()*/;
            for (var i = 0; i < 3; i++)
            {
                IList<ITask> tasks = query.ToList();
                Assert.AreEqual(2, tasks.Count);

                Assert.AreEqual("task one", tasks[0].Name);
                Assert.AreEqual("task two", tasks[1].Name);

                taskService.Complete(tasks[0].Id);
                taskService.Complete(tasks[1].Id);
            }

            AssertProcessEnded(procId);
        }

        [Test]
        [Deployment]
        public virtual void testSequentialSubProcessWithTimer()
        {
            var procId = runtimeService.StartProcessInstanceByKey("miSequentialSubprocessWithTimer")
                .Id;

            // Complete one subprocess
            IList<ITask> tasks = taskService.CreateTaskQuery()
                .ToList();
            Assert.AreEqual(2, tasks.Count);
            taskService.Complete(tasks[0].Id);
            taskService.Complete(tasks[1].Id);
            tasks = taskService.CreateTaskQuery()
                .ToList();
            Assert.AreEqual(2, tasks.Count);

            // Fire timer
            var timer = managementService.CreateJobQuery()
                .First();
            managementService.ExecuteJob(timer.Id);

            var taskAfterTimer = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("taskAfterTimer", taskAfterTimer.TaskDefinitionKey);
            taskService.Complete(taskAfterTimer.Id);

            AssertProcessEnded(procId);
        }
    }
}