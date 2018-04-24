using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl;
using NUnit.Framework;

namespace Engine.Tests.Api.Task
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class SubTaskTest : PluggableProcessEngineTestCase
    {
        [Test]
        public virtual void testSubTask()
        {
            var gonzoTask = taskService.NewTask();
            gonzoTask.Name = "gonzoTask";
            taskService.SaveTask(gonzoTask);

            var subTaskOne = taskService.NewTask();
            subTaskOne.Name = "subtask one";
            var gonzoTaskId = gonzoTask.Id;
            subTaskOne.ParentTaskId = gonzoTaskId;
            taskService.SaveTask(subTaskOne);

            var subTaskTwo = taskService.NewTask();
            subTaskTwo.Name = "subtask two";
            subTaskTwo.ParentTaskId = gonzoTaskId;
            taskService.SaveTask(subTaskTwo);

            var subTaskId = subTaskOne.Id;
            Assert.True(!taskService.GetSubTasks(subTaskId)
                .Any());
            Assert.True(!historyService.CreateHistoricTaskInstanceQuery(p => p.ParentTaskId == subTaskId)
                //.TaskParentTaskId(subTaskId)
                .ToList()
                .Any());

            var subTasks = taskService.GetSubTasks(gonzoTaskId);
            ISet<string> subTaskNames = new HashSet<string>();
            foreach (var subTask in subTasks)
                subTaskNames.Add(subTask.Name);

            if (processEngineConfiguration.HistoryLevel.Id >= HistoryLevelFields.HistoryLevelAudit.Id)
            {
                ISet<string> expectedSubTaskNames = new HashSet<string>();
                expectedSubTaskNames.Add("subtask one");
                expectedSubTaskNames.Add("subtask two");

                Assert.AreEqual(expectedSubTaskNames, subTaskNames);

                IList<IHistoricTaskInstance> historicSubTasks = historyService
                    .CreateHistoricTaskInstanceQuery(p => p.ParentTaskId == gonzoTaskId)
                    //.TaskParentTaskId(gonzoTaskId)
                    .ToList();

                subTaskNames = new HashSet<string>();
                foreach (var historicSubTask in historicSubTasks)
                    subTaskNames.Add(historicSubTask.Name);

                Assert.AreEqual(expectedSubTaskNames, subTaskNames);
            }

            taskService.DeleteTask(gonzoTaskId, true);
        }
    }
}