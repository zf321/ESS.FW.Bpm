using System.Linq;
using ESS.FW.Bpm.Engine;
using NUnit.Framework;

namespace Engine.Tests.History
{
    /// <summary>
    /// </summary>
   [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryAudit)]
    [TestFixture]
    public class HistoricActivityStatisticsQueryTest : PluggableProcessEngineTestCase
    {
        [Test][Deployment(new [] {"resources/history/HistoricActivityStatisticsQueryTest.TestWithCallActivity.bpmn20.xml", "resources/history/HistoricActivityStatisticsQueryTest.CalledProcess.bpmn20.xml" })]
        public virtual void testMultipleProcessDefinitions()
        {
            var processId = ProcessDefinitionId;
            var calledProcessId = getProcessDefinitionIdByKey("calledProcess");

            startProcesses(5);

            startProcessesByKey(10, "calledProcess");

            var query = historyService.CreateHistoricActivityStatisticsQuery( processId)
                .OrderBy(o => o.Id);
               // /*.OrderByActivityId()*/
                //.Asc();

            var statistics = query
                .ToList();

            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(1, statistics.Count);

            // callActivity
            var calledActivity = statistics[0];

            Assert.AreEqual("callActivity", calledActivity.Id);
            Assert.AreEqual(5, calledActivity.Instances);

            query = historyService.CreateHistoricActivityStatisticsQuery( calledProcessId)
                .OrderBy(o => o.Id);
               // /*.OrderByActivityId()*/
               //.Asc();

            statistics = query
                .ToList();

            Assert.AreEqual(2, query.Count());
            Assert.AreEqual(2, statistics.Count);

            // task1
            var task1 = statistics[0];

            Assert.AreEqual("task1", task1.Id);
            Assert.AreEqual(15, task1.Instances);

            // task2
            var task2 = statistics[1];

            Assert.AreEqual("task2", task2.Id);
            Assert.AreEqual(15, task2.Instances);

            completeProcessInstances();
        }

        protected internal virtual void completeProcessInstances()
        {
            var tasks = taskService.CreateTaskQuery()
                
                .ToList();
            foreach (var task in tasks)
                taskService.Complete(task.Id);
        }

        protected internal virtual void cancelProcessInstances()
        {
            var processInstances = runtimeService.CreateProcessInstanceQuery()
                
                .ToList();
            foreach (var pi in processInstances)
                runtimeService.DeleteProcessInstance(pi.Id, "test");
        }

        protected internal virtual void startProcesses(int numberOfInstances)
        {
            startProcessesByKey(numberOfInstances, "process");
        }

        protected internal virtual void startProcessesByKey(int numberOfInstances, string key)
        {
            for (var i = 0; i < numberOfInstances; i++)
                runtimeService.StartProcessInstanceByKey(key);
        }

        protected internal virtual string getProcessDefinitionIdByKey(string key)
        {
            return repositoryService.CreateProcessDefinitionQuery(c=>c.Key ==key)
                .First()
                .Id;
        }

        protected internal virtual string ProcessDefinitionId
        {
            get { return getProcessDefinitionIdByKey("process"); }
        }

        [Test]
        [Deployment("resources/history/HistoricActivityStatisticsQueryTest.TestSingleTask.bpmn20.xml")]
        public virtual void testDifferentProcessesWithSameActivityId()
        {
            var processDefinitionId = ProcessDefinitionId;
            var anotherProcessDefinitionId = getProcessDefinitionIdByKey("anotherProcess");

            startProcesses(5);

            startProcessesByKey(10, "anotherProcess");

            // first processDefinition
            var query = historyService.CreateHistoricActivityStatisticsQuery(  processDefinitionId);

            var statistics = query
                .ToList();

            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(1, statistics.Count);

            var task = statistics[0];
            Assert.AreEqual(5, task.Instances);

            // second processDefinition
            query = historyService.CreateHistoricActivityStatisticsQuery( anotherProcessDefinitionId);

            statistics = query
                .ToList();

            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(1, statistics.Count);

            task = statistics[0];
            Assert.AreEqual(10, task.Instances);
        }

        [Test]
        [Deployment("resources/history/HistoricActivityStatisticsQueryTest.TestSingleTask.bpmn20.xml")]
        public virtual void testFinishedProcessInstances()
        {
            var processDefinitionId = ProcessDefinitionId;

            startProcesses(5);

            completeProcessInstances();

            var query = historyService.CreateHistoricActivityStatisticsQuery(  processDefinitionId);
            var statistics = query
                .ToList();

            Assert.AreEqual(0, query.Count());
            Assert.AreEqual(0, statistics.Count);
        }

        [Test]
        [Deployment]
        public virtual void testMultipleRunningTasks()
        {
            var processDefinitionId = ProcessDefinitionId;

            startProcesses(5);

            var query = historyService.CreateHistoricActivityStatisticsQuery(processDefinitionId)
                /*.OrderByActivityId()*/
                /*.Asc()*/;

            var statistics = query
                .ToList();

            Assert.AreEqual(4, query.Count());
            Assert.AreEqual(4, statistics.Count);

            // innerTask
            var innerTask = statistics[0];

            Assert.AreEqual("innerTask", innerTask.Id);
            Assert.AreEqual(25, innerTask.Instances);

            // subprocess
            var subProcess = statistics[1];

            Assert.AreEqual("subprocess", subProcess.Id);
            Assert.AreEqual(25, subProcess.Instances);

            // subprocess multi instance body
            var subProcessMiBody = statistics[2];

            Assert.AreEqual("subprocess#multiInstanceBody", subProcessMiBody.Id);
            Assert.AreEqual(5, subProcessMiBody.Instances);

            // task
            var task = statistics[3];

            Assert.AreEqual("task", task.Id);
            Assert.AreEqual(5, task.Instances);

            completeProcessInstances();
        }

        [Test]
        [Deployment("resources/history/HistoricActivityStatisticsQueryTest.TestSingleTask.bpmn20.xml")]
        public virtual void testNoRunningProcessInstances()
        {
            var processDefinitionId = ProcessDefinitionId;

            var query = historyService.CreateHistoricActivityStatisticsQuery(  processDefinitionId);
            var statistics = query
                .ToList();

            Assert.AreEqual(0, query.Count());
            Assert.AreEqual(0, statistics.Count);
        }

        [Test]
        [Deployment("resources/history/HistoricActivityStatisticsQueryTest.TestSingleTask.bpmn20.xml")]
        public virtual void testQueryByCanceled()
        {
            var processDefinitionId = ProcessDefinitionId;

            startProcesses(5);

            cancelProcessInstances();

            var query =
                    historyService.CreateHistoricActivityStatisticsQuery(processDefinitionId)
                ;// /*.IncludeCanceled()*/;

            var statistics = query
                .ToList();

            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(1, statistics.Count);

            // task
            var task = statistics[0];

            Assert.AreEqual("task", task.Id);
            Assert.AreEqual(0, task.Instances);
            Assert.AreEqual(5, task.Canceled);
        }

        [Test]
        [Deployment("resources/history/HistoricActivityStatisticsQueryTest.TestSingleTask.bpmn20.xml")]
        public virtual void testQueryByCanceledAfterCancelingSomeInstances()
        {
            var processDefinitionId = ProcessDefinitionId;

            startProcesses(3);

            // cancel running process instances
            var processInstances = runtimeService.CreateProcessInstanceQuery()
                
                .ToList();
            foreach (var processInstance in processInstances)
                runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            startProcesses(2);

            var query = historyService.CreateHistoricActivityStatisticsQuery(processDefinitionId)
              ;//  /*.IncludeCanceled()*/;

            var statistics = query
                .ToList();

            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(1, statistics.Count);

            // task
            var task = statistics[0];

            Assert.AreEqual("task", task.Id);
            Assert.AreEqual(2, task.Instances);
            Assert.AreEqual(3, task.Canceled);

            completeProcessInstances();
        }

        [Test]
        [Deployment("resources/history/HistoricActivityStatisticsQueryTest.TestSingleTask.bpmn20.xml")]
        public virtual void testQueryByCanceledAndCompleteScope()
        {
            var processDefinitionId = ProcessDefinitionId;

            startProcesses(2);

            // cancel running process instances
            var processInstances = runtimeService.CreateProcessInstanceQuery()
                
                .ToList();
            foreach (var processInstance in processInstances)
                runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            startProcesses(2);

            // complete running tasks
            var tasks = taskService.CreateTaskQuery()
                
                .ToList();
            foreach (var t in tasks)
                taskService.Complete(t.Id);

            startProcesses(2);

            var query = historyService.CreateHistoricActivityStatisticsQuery(processDefinitionId)
                ///*.IncludeCanceled()*/
                ///*.IncludeCompleteScope()*/
                /*.OrderByActivityId()*/
                /*.Asc()*/;
            var statistics = query
                .ToList();

            Assert.AreEqual(2, query.Count());
            Assert.AreEqual(2, statistics.Count);

            // end
            var end = statistics[0];

            Assert.AreEqual("end", end.Id);
            Assert.AreEqual(0, end.Instances);
            Assert.AreEqual(0, end.Canceled);
            Assert.AreEqual(2, end.CompleteScope);

            // task
            var task = statistics[1];

            Assert.AreEqual("task", task.Id);
            Assert.AreEqual(2, task.Instances);
            Assert.AreEqual(2, task.Canceled);
            Assert.AreEqual(0, task.CompleteScope);
        }

        [Test]
        [Deployment("resources/history/HistoricActivityStatisticsQueryTest.TestSingleTask.bpmn20.xml")]
        public virtual void testQueryByCanceledAndFinished()
        {
            var processDefinitionId = ProcessDefinitionId;

            startProcesses(2);

            // cancel running process instances
            var processInstances = runtimeService.CreateProcessInstanceQuery()
                
                .ToList();
            foreach (var processInstance in processInstances)
                runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            startProcesses(2);

            // complete running tasks
            var tasks = taskService.CreateTaskQuery()
                
                .ToList();
            foreach (var t in tasks)
                taskService.Complete(t.Id);

            startProcesses(2);

            var query = historyService.CreateHistoricActivityStatisticsQuery(processDefinitionId)
                /*.IncludeCanceled()*/
                /*.IncludeFinished()*/
                /*.OrderByActivityId()*/
                /*.Asc()*/;
            var statistics = query
                .ToList();

            Assert.AreEqual(3, query.Count());
            Assert.AreEqual(3, statistics.Count);

            // end
            var end = statistics[0];

            Assert.AreEqual("end", end.Id);
            Assert.AreEqual(0, end.Instances);
            Assert.AreEqual(0, end.Canceled);
            Assert.AreEqual(2, end.Finished);

            // start
            var start = statistics[1];

            Assert.AreEqual("start", start.Id);
            Assert.AreEqual(0, start.Instances);
            Assert.AreEqual(0, start.Canceled);
            Assert.AreEqual(6, start.Finished);

            // task
            var task = statistics[2];

            Assert.AreEqual("task", task.Id);
            Assert.AreEqual(2, task.Instances);
            Assert.AreEqual(2, task.Canceled);
            Assert.AreEqual(4, task.Finished);
        }

        [Test]
        [Deployment("resources/history/HistoricActivityStatisticsQueryTest.TestSingleTask.bpmn20.xml")]
        public virtual void testQueryByCompleteScope()
        {
            var processDefinitionId = ProcessDefinitionId;

            startProcesses(5);

            completeProcessInstances();

            var query = historyService.CreateHistoricActivityStatisticsQuery(  processDefinitionId)
               ;// /*.IncludeCompleteScope()*/;
            var statistics = query
                .ToList();

            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(1, statistics.Count);

            // end
            var end = statistics[0];

            Assert.AreEqual("end", end.Id);
            Assert.AreEqual(0, end.Instances);
            Assert.AreEqual(5, end.CompleteScope);
        }

        [Test]
        [Deployment("resources/history/HistoricActivityStatisticsQueryTest.TestSingleTask.bpmn20.xml")]
        public virtual void testQueryByCompleteScopeAfterFinishingSomeInstances()
        {
            var processDefinitionId = ProcessDefinitionId;

            // start five instances
            startProcesses(5);

            // complete two task, so that two process instances are finished
            var tasks = taskService.CreateTaskQuery()
                
                .ToList();
            for (var i = 0; i < 2; i++)
                taskService.Complete(tasks[i].Id);

            var query = historyService.CreateHistoricActivityStatisticsQuery(processDefinitionId)
                ///*.IncludeCompleteScope()*/
                /*.OrderByActivityId()*/
                /*.Asc()*/;

            var statistics = query
                .ToList();

            Assert.AreEqual(2, query.Count());
            Assert.AreEqual(2, statistics.Count);

            // end
            var end = statistics[0];

            Assert.AreEqual("end", end.Id);
            Assert.AreEqual(0, end.Instances);
            Assert.AreEqual(2, end.CompleteScope);

            // task
            var task = statistics[1];

            Assert.AreEqual("task", task.Id);
            Assert.AreEqual(3, task.Instances);
            Assert.AreEqual(0, task.CompleteScope);

            completeProcessInstances();
        }

        [Test]
        [Deployment("resources/history/HistoricActivityStatisticsQueryTest.TestMultipleRunningTasks.bpmn20.xml")]
        public virtual void testQueryByCompleteScopeMultipleRunningTasks()
        {
            var processDefinitionId = ProcessDefinitionId;

            startProcesses(5);

            var tasks = taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "innerTask")
                
                .ToList();
            foreach (var t in tasks)
                taskService.Complete(t.Id);

            var query = historyService.CreateHistoricActivityStatisticsQuery(processDefinitionId)
                /*.IncludeCompleteScope()*/
                /*.OrderByActivityId()*/
                /*.Asc()*/;

            var statistics = query
                .ToList();

            Assert.AreEqual(4, query.Count());
            Assert.AreEqual(4, statistics.Count);

            // end1
            var end1 = statistics[0];

            Assert.AreEqual("end1", end1.Id);
            Assert.AreEqual(0, end1.Instances);
            Assert.AreEqual(5, end1.CompleteScope);

            // innerEnd
            var innerEnd = statistics[1];

            Assert.AreEqual("innerEnd", innerEnd.Id);
            Assert.AreEqual(0, innerEnd.Instances);
            Assert.AreEqual(25, innerEnd.CompleteScope);

            // subprocess (completes the multi-instances body scope, see BPMN spec)
            var subprocess = statistics[2];

            Assert.AreEqual("subprocess", subprocess.Id);
            Assert.AreEqual(0, subprocess.Instances);
            Assert.AreEqual(25, subprocess.CompleteScope);

            // task
            var task = statistics[3];

            Assert.AreEqual("task", task.Id);
            Assert.AreEqual(5, task.Instances);
            Assert.AreEqual(0, task.CompleteScope);

            completeProcessInstances();
        }

        [Test]
        [Deployment("resources/history/HistoricActivityStatisticsQueryTest.TestSingleTask.bpmn20.xml")]
        public virtual void testQueryByFinished()
        {
            var processDefinitionId = ProcessDefinitionId;

            startProcesses(5);

            var query = historyService.CreateHistoricActivityStatisticsQuery(processDefinitionId)
                /*.IncludeFinished()*/
                /*.OrderByActivityId()*/
                /*.Asc()*/;
            var statistics = query
                .ToList();

            Assert.AreEqual(2, query.Count());
            Assert.AreEqual(2, statistics.Count);

            // start
            var start = statistics[0];

            Assert.AreEqual("start", start.Id);
            Assert.AreEqual(0, start.Instances);
            Assert.AreEqual(5, start.Finished);

            // task
            var task = statistics[1];

            Assert.AreEqual("task", task.Id);
            Assert.AreEqual(5, task.Instances);
            Assert.AreEqual(0, task.Finished);
        }

        [Test]
        [Deployment("resources/history/HistoricActivityStatisticsQueryTest.TestSingleTask.bpmn20.xml")]
        public virtual void testQueryByFinishedAfterFinishingSomeInstances()
        {
            var processDefinitionId = ProcessDefinitionId;

            // start five instances
            startProcesses(5);

            // complete two task, so that two process instances are finished
            var tasks = taskService.CreateTaskQuery()
                
                .ToList();
            for (var i = 0; i < 2; i++)
                taskService.Complete(tasks[i].Id);

            var query = historyService.CreateHistoricActivityStatisticsQuery(processDefinitionId)
                /*.IncludeFinished()*/
                /*.OrderByActivityId()*/
                /*.Asc()*/;

            var statistics = query
                .ToList();

            Assert.AreEqual(3, query.Count());
            Assert.AreEqual(3, statistics.Count);

            // end
            var end = statistics[0];

            Assert.AreEqual("end", end.Id);
            Assert.AreEqual(0, end.Instances);
            Assert.AreEqual(2, end.Finished);

            // start
            var start = statistics[1];

            Assert.AreEqual("start", start.Id);
            Assert.AreEqual(0, start.Instances);
            Assert.AreEqual(5, start.Finished);

            // task
            var task = statistics[2];

            Assert.AreEqual("task", task.Id);
            Assert.AreEqual(3, task.Instances);
            Assert.AreEqual(2, task.Finished);

            completeProcessInstances();
        }

        [Test]
        [Deployment("resources/history/HistoricActivityStatisticsQueryTest.TestSingleTask.bpmn20.xml")]
        public virtual void testQueryByFinishedAndCompleteScope()
        {
            var processDefinitionId = ProcessDefinitionId;

            startProcesses(2);

            // cancel running process instances
            var processInstances = runtimeService.CreateProcessInstanceQuery()
                
                .ToList();
            foreach (var processInstance in processInstances)
                runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            startProcesses(2);

            // complete running tasks
            var tasks = taskService.CreateTaskQuery()
                
                .ToList();
            foreach (var t in tasks)
                taskService.Complete(t.Id);

            startProcesses(2);

            var query = historyService.CreateHistoricActivityStatisticsQuery(processDefinitionId)
                /*.IncludeFinished()*/
                /*.IncludeCompleteScope()*/
                /*.OrderByActivityId()*/
                /*.Asc()*/;
            var statistics = query
                .ToList();

            Assert.AreEqual(3, query.Count());
            Assert.AreEqual(3, statistics.Count);

            // end
            var end = statistics[0];

            Assert.AreEqual("end", end.Id);
            Assert.AreEqual(0, end.Instances);
            Assert.AreEqual(2, end.Finished);
            Assert.AreEqual(2, end.CompleteScope);

            // start
            var start = statistics[1];

            Assert.AreEqual("start", start.Id);
            Assert.AreEqual(0, start.Instances);
            Assert.AreEqual(6, start.Finished);
            Assert.AreEqual(0, start.CompleteScope);

            // task
            var task = statistics[2];

            Assert.AreEqual("task", task.Id);
            Assert.AreEqual(2, task.Instances);
            Assert.AreEqual(4, task.Finished);
            Assert.AreEqual(0, task.CompleteScope);
        }

        [Test]
        [Deployment("resources/history/HistoricActivityStatisticsQueryTest.TestSingleTask.bpmn20.xml")]
        public virtual void testQueryByFinishedAndCompleteScopeAndCanceled()
        {
            var processDefinitionId = ProcessDefinitionId;

            startProcesses(2);

            // cancel running process instances
            var processInstances = runtimeService.CreateProcessInstanceQuery()
                
                .ToList();
            foreach (var processInstance in processInstances)
                runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            startProcesses(2);

            // complete running tasks
            var tasks = taskService.CreateTaskQuery()
                .ToList();
            foreach (var t in tasks)
                taskService.Complete(t.Id);

            startProcesses(2);

            var query = historyService.CreateHistoricActivityStatisticsQuery(processDefinitionId)
                /*.IncludeFinished()*/
                /*.IncludeCompleteScope()*/
                /*.IncludeCanceled()*/
                /*.OrderByActivityId()*/
                /*.Asc()*/;
            var statistics = query
                .ToList();

            Assert.AreEqual(3, query.Count());
            Assert.AreEqual(3, statistics.Count);

            // end
            var end = statistics[0];

            Assert.AreEqual("end", end.Id);
            Assert.AreEqual(0, end.Instances);
            Assert.AreEqual(0, end.Canceled);
            Assert.AreEqual(2, end.Finished);
            Assert.AreEqual(2, end.CompleteScope);

            // start
            var start = statistics[1];

            Assert.AreEqual("start", start.Id);
            Assert.AreEqual(0, start.Instances);
            Assert.AreEqual(0, start.Canceled);
            Assert.AreEqual(6, start.Finished);
            Assert.AreEqual(0, start.CompleteScope);

            // task
            var task = statistics[2];

            Assert.AreEqual("task", task.Id);
            Assert.AreEqual(2, task.Instances);
            Assert.AreEqual(2, task.Canceled);
            Assert.AreEqual(4, task.Finished);
            Assert.AreEqual(0, task.CompleteScope);
        }

        [Test]
        [Deployment("resources/history/HistoricActivityStatisticsQueryTest.TestMultipleRunningTasks.bpmn20.xml")]
        public virtual void testQueryByFinishedMultipleRunningTasks()
        {
            var processDefinitionId = ProcessDefinitionId;

            startProcesses(5);

            var tasks = taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "innerTask")
                
                .ToList();
            foreach (var t in tasks)
                taskService.Complete(t.Id);

            var query = historyService.CreateHistoricActivityStatisticsQuery(processDefinitionId)
                /*.IncludeFinished()*/
                /*.OrderByActivityId()*/
                /*.Asc()*/;

            var statistics = query
                .ToList();

            Assert.AreEqual(9, query.Count());
            Assert.AreEqual(9, statistics.Count);

            // end1
            var end1 = statistics[0];

            Assert.AreEqual("end1", end1.Id);
            Assert.AreEqual(0, end1.Instances);
            Assert.AreEqual(5, end1.Finished);

            // gtw
            var gtw = statistics[1];

            Assert.AreEqual("gtw", gtw.Id);
            Assert.AreEqual(0, gtw.Instances);
            Assert.AreEqual(5, gtw.Finished);

            // innerEnd
            var innerEnd = statistics[2];

            Assert.AreEqual("innerEnd", innerEnd.Id);
            Assert.AreEqual(0, innerEnd.Instances);
            Assert.AreEqual(25, innerEnd.Finished);

            // innerStart
            var innerStart = statistics[3];

            Assert.AreEqual("innerStart", innerStart.Id);
            Assert.AreEqual(0, innerStart.Instances);
            Assert.AreEqual(25, innerStart.Finished);

            // innerTask
            var innerTask = statistics[4];

            Assert.AreEqual("innerTask", innerTask.Id);
            Assert.AreEqual(0, innerTask.Instances);
            Assert.AreEqual(25, innerTask.Finished);

            // innerStart
            var start = statistics[5];

            Assert.AreEqual("start", start.Id);
            Assert.AreEqual(0, start.Instances);
            Assert.AreEqual(5, start.Finished);

            // subprocess
            var subProcess = statistics[6];

            Assert.AreEqual("subprocess", subProcess.Id);
            Assert.AreEqual(0, subProcess.Instances);
            Assert.AreEqual(25, subProcess.Finished);

            // subprocess - multi-instance body
            var subProcessMiBody = statistics[7];

            Assert.AreEqual("subprocess#multiInstanceBody", subProcessMiBody.Id);
            Assert.AreEqual(0, subProcessMiBody.Instances);
            Assert.AreEqual(5, subProcessMiBody.Finished);

            // task
            var task = statistics[8];

            Assert.AreEqual("task", task.Id);
            Assert.AreEqual(5, task.Instances);
            Assert.AreEqual(0, task.Finished);

            completeProcessInstances();
        }

        [Test]
        [Deployment]
        public virtual void testSingleTask()
        {
            var processDefinitionId = ProcessDefinitionId;

            startProcesses(5);

            var query = historyService.CreateHistoricActivityStatisticsQuery(  processDefinitionId);
            var statistics = query
                .ToList();

            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(1, statistics.Count);

            var statistic = statistics[0];

            Assert.AreEqual("task", statistic.Id);
            Assert.AreEqual(5, statistic.Instances);

            completeProcessInstances();
        }

        [Test]
        [Deployment("resources/history/HistoricActivityStatisticsQueryTest.TestSingleTask.bpmn20.xml")]
        public virtual void testSorting()
        {
            var processDefinitionId = ProcessDefinitionId;

            startProcesses(5);

            var query = historyService.CreateHistoricActivityStatisticsQuery(processDefinitionId);

            Assert.AreEqual(1, query/*.OrderByActivityId()*/
                /*.Asc()*/
                
                .Count());
            Assert.AreEqual(1, query/*.OrderByActivityId()*/
                /*.Desc()*/
                
                .Count());

            Assert.AreEqual(1, query/*.OrderByActivityId()*/
                /*.Asc()*/
                .Count());
            Assert.AreEqual(1, query/*.OrderByActivityId()*/
                /*.Desc()*/
                .Count());
        }
    }
}