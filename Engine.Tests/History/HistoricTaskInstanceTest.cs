using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.History
{
    /// <summary>
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryAudit)]
    [TestFixture]
    public class HistoricTaskInstanceTest : PluggableProcessEngineTestCase
    {
        [Test]
        public virtual void testDeleteHistoricTaskInstance()
        {
            // deleting unexisting historic task instance should be silently ignored
            historyService.DeleteHistoricTaskInstance("unexistingId");
        }

        [Test]
        public virtual void testHistoricTaskInstanceAssignment()
        {
            var task = taskService.NewTask();
            taskService.SaveTask(task);

            // task exists & has no assignee:
            var hti = historyService.CreateHistoricTaskInstanceQuery()
                .First();
            Assert.IsNull(hti.Assignee);

            // assign task to jonny:
            taskService.SetAssignee(task.Id, "jonny");

            // should be reflected in history
            hti = historyService.CreateHistoricTaskInstanceQuery()
                .First();
            Assert.AreEqual("jonny", hti.Assignee);
            Assert.IsNull(hti.Owner);

            taskService.DeleteTask(task.Id);
            historyService.DeleteHistoricTaskInstance(hti.Id);
        }

        [Test]
        public virtual void testHistoricTaskInstanceOwner()
        {
            var task = taskService.NewTask();
            taskService.SaveTask(task);

            // task exists & has no owner:
            var hti = historyService.CreateHistoricTaskInstanceQuery()
                .First();
            Assert.IsNull(hti.Owner);

            // set owner to jonny:
            taskService.SetOwner(task.Id, "jonny");

            // should be reflected in history
            hti = historyService.CreateHistoricTaskInstanceQuery()
                .First();
            Assert.AreEqual("jonny", hti.Owner);

            taskService.DeleteTask(task.Id);
            historyService.DeleteHistoricTaskInstance(hti.Id);
        }

        [Test]
        public virtual void testHistoricTaskInstancePriority()
        {
            var task = taskService.NewTask();
            taskService.SaveTask(task);

            // task exists & has normal priority:
            var hti = historyService.CreateHistoricTaskInstanceQuery()
                .First();
            Assert.AreEqual(TaskFields.PriorityNormal, hti.Priority);

            // set priority to maximum value:
            taskService.SetPriority(task.Id, TaskFields.PriorityMaximum);

            // should be reflected in history
            hti = historyService.CreateHistoricTaskInstanceQuery()
                .First();
            Assert.AreEqual(TaskFields.PriorityMaximum, hti.Priority);

            taskService.DeleteTask(task.Id);
            historyService.DeleteHistoricTaskInstance(hti.Id);
        }

        [Test]
        public virtual void testInvalidSorting()
        {
            try
            {
                historyService.CreateHistoricTaskInstanceQuery()
                    /*.Asc()*/;
                Assert.Fail();
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                historyService.CreateHistoricTaskInstanceQuery()
                    /*.Desc()*/;
                Assert.Fail();
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                historyService.CreateHistoricTaskInstanceQuery()
                    //.OrderByProcessInstanceId()

                    .ToList();
                Assert.Fail();
            }
            catch (ProcessEngineException)
            {
            }
        }

        [Test]
        [Deployment("resources/history/HistoricTaskInstanceTest.TestHistoricTaskInstance.bpmn20.xml")]
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        public virtual void testHistoricTaskInstanceQueryByFollowUpDate()
        {
            var otherDate = new DateTime();

            runtimeService.StartProcessInstanceByKey("HistoricTaskInstanceTest");

            // do not find any task instances with follow up date
            Assert.AreEqual(0, taskService.CreateTaskQuery()
                //.FollowUpDate(otherDate)
                .Count());

            var task = taskService.CreateTaskQuery()
                .First();

            // set follow-up date on task
            var followUpDate = DateTime.Parse("01/02/2003 01:12:13");
            task.FollowUpDate = followUpDate;
            taskService.SaveTask(task);

            // test that follow-up date was written to historic database
            Assert.AreEqual(followUpDate, historyService.CreateHistoricTaskInstanceQuery(c => c.TaskId == task.Id)
                .First()
                .FollowUpDate);
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery(m=>m.FollowUpDate== followUpDate)
                //.TaskFollowUpDate(followUpDate)
                .Count());

            otherDate = new DateTime(followUpDate.Ticks);

            otherDate.AddYears(1);
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery(m => m.FollowUpDate == otherDate)
                //.TaskFollowUpDate(otherDate)
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.TaskFollowUpBefore(otherDate)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery()
                //.TaskFollowUpAfter(otherDate)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery()
                //.TaskFollowUpAfter(otherDate)
                //.TaskFollowUpDate(followUpDate)
                .Count());

            otherDate.AddYears(-2);
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.TaskFollowUpAfter(otherDate)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery()
                //.TaskFollowUpBefore(otherDate)
                .Count());
            Assert.AreEqual(followUpDate, historyService.CreateHistoricTaskInstanceQuery(c => c.TaskId == task.Id)
                .First()
                .FollowUpDate);
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery()
                //.TaskFollowUpBefore(otherDate)
                //.TaskFollowUpDate(followUpDate)
                .Count());

            taskService.Complete(task.Id);

            Assert.AreEqual(followUpDate, historyService.CreateHistoricTaskInstanceQuery(c => c.TaskId == task.Id)
                .First()
                .FollowUpDate);
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.TaskFollowUpDate(followUpDate)
                .Count());
        }

        [Test]
        [Deployment("resources/history/HistoricTaskInstanceTest.TestHistoricTaskInstance.bpmn20.xml")]
        public virtual void testHistoricTaskInstanceQueryByActivityInstanceId()
        {
            runtimeService.StartProcessInstanceByKey("HistoricTaskInstanceTest");

            var activityInstanceId = historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityId == "task")
                .First()
                .Id;

            var query = historyService.CreateHistoricTaskInstanceQuery()
                //.ActivityInstanceIdIn(activityInstanceId)
                ;

            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(1, query
                .Count());
        }

        [Test]
        [Deployment("resources/history/HistoricTaskInstanceTest.TestHistoricTaskInstance.bpmn20.xml")]
        public virtual void testHistoricTaskInstanceQueryByActivityInstanceIds()
        {
            var pi1 = runtimeService.StartProcessInstanceByKey("HistoricTaskInstanceTest");
            var pi2 = runtimeService.StartProcessInstanceByKey("HistoricTaskInstanceTest");

            var activityInstanceId1 = historyService.CreateHistoricActivityInstanceQuery(c => c.ProcessInstanceId == pi1.Id && c.ActivityId == "task")
                .First()
                .Id;

            var activityInstanceId2 = historyService.CreateHistoricActivityInstanceQuery(c => c.ProcessInstanceId == pi2.Id && c.ActivityId == "task")
                .First()
                .Id;

            var query = historyService.CreateHistoricTaskInstanceQuery()
                //.ActivityInstanceIdIn(activityInstanceId1, activityInstanceId2)
                ;

            Assert.AreEqual(2, query.Count());
            Assert.AreEqual(2, query
                .Count());
        }

        [Test]
        [Deployment("resources/history/HistoricTaskInstanceTest.TestHistoricTaskInstance.bpmn20.xml")]
        public virtual void testHistoricTaskInstanceQueryByInvalidActivityInstanceId()
        {
            var query = historyService.CreateHistoricTaskInstanceQuery();

            //query.ActivityInstanceIdIn("invalid");
            Assert.AreEqual(0, query.Count());

            try
            {
                //query.ActivityInstanceIdIn(null);
                Assert.Fail("A ProcessEngineExcpetion was expected.");
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                //query.ActivityInstanceIdIn((string)null);
                Assert.Fail("A ProcessEngineExcpetion was expected.");
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                string[] values = { "a", null, "b" };
                //query.ActivityInstanceIdIn(values);
                Assert.Fail("A ProcessEngineExcpetion was expected.");
            }
            catch (ProcessEngineException)
            {
            }
        }

        [Test]
        public virtual void testQueryByInvalidCaseDefinitionId()
        {
            var query = historyService.CreateHistoricTaskInstanceQuery();

            //query.CaseDefinitionId("invalid");

            Assert.AreEqual(0, query.Count());
            Assert.AreEqual(0, query
                .Count());
            Assert.IsNull(query.First());

           // query.CaseDefinitionId(null);

            Assert.AreEqual(0, query.Count());
            Assert.AreEqual(0, query
                .Count());
            Assert.IsNull(query.First());
        }

        [Test]
        public virtual void testQueryByInvalidCaseDefinitionKey()
        {
            var query = historyService.CreateHistoricTaskInstanceQuery();

            //query.CaseDefinitionKey("invalid");

            Assert.AreEqual(0, query.Count());
            Assert.AreEqual(0, query
                .Count());
            Assert.IsNull(query.First());

            //query.CaseDefinitionKey(null);

            Assert.AreEqual(0, query.Count());
            Assert.AreEqual(0, query
                .Count());
            Assert.IsNull(query.First());
        }

        [Test]
        public virtual void testQueryByInvalidCaseDefinitionName()
        {
            var query = historyService.CreateHistoricTaskInstanceQuery();

            //query.CaseDefinitionName("invalid");

            Assert.AreEqual(0, query.Count());
            Assert.AreEqual(0, query
                .Count());
            Assert.IsNull(query.First());

            //query.CaseDefinitionName(null);

            Assert.AreEqual(0, query.Count());
            Assert.AreEqual(0, query
                .Count());
            Assert.IsNull(query.First());
        }


        [Test]
        [Deployment(new[] { "resources/history/HistoricTaskInstanceTest.TestQueryByCaseInstanceIdHierarchy.cmmn", "resources/history/HistoricTaskInstanceTest.TestQueryByCaseInstanceIdHierarchy.bpmn20.xml" })]
        public virtual void testQueryByCaseInstanceIdHierarchy()
        {
            // given
            var caseInstanceId = caseService.WithCaseDefinitionByKey("case")
                .Create()
                .Id;

            var id = caseService.CreateCaseExecutionQuery(c => c.ActivityId == "PI_ProcessTask_1")
                .First()
                .Id;

            // then
            var query = historyService.CreateHistoricTaskInstanceQuery();

            //query.CaseInstanceId(caseInstanceId);

            Assert.AreEqual(2, query.Count());
            Assert.AreEqual(2, query
                .Count());

            foreach (var task in query
                .ToList())
            {
                Assert.AreEqual(caseInstanceId, task.CaseInstanceId);

                Assert.IsNull(task.CaseDefinitionId);
                Assert.IsNull(task.CaseExecutionId);

                taskService.Complete(task.Id);
            }

            Assert.AreEqual(3, query.Count());
            Assert.AreEqual(3, query
                .Count());

            foreach (var task in query
                .ToList())
            {
                Assert.AreEqual(caseInstanceId, task.CaseInstanceId);

                Assert.IsNull(task.CaseDefinitionId);
                Assert.IsNull(task.CaseExecutionId);
            }
        }

        [Test]
        public virtual void testQueryByInvalidCaseInstanceId()
        {
            var query = historyService.CreateHistoricTaskInstanceQuery();

            //query.CaseInstanceId("invalid");

            Assert.AreEqual(0, query.Count());
            Assert.AreEqual(0, query
                .Count());
            Assert.IsNull(query.First());

            //query.CaseInstanceId(null);

            Assert.AreEqual(0, query.Count());
            Assert.AreEqual(0, query
                .Count());
            Assert.IsNull(query.First());
        }

        [Test]
        public virtual void testQueryByInvalidCaseExecutionId()
        {
            var query = historyService.CreateHistoricTaskInstanceQuery();

            //query.CaseExecutionId("invalid");

            Assert.AreEqual(0, query.Count());
            Assert.AreEqual(0, query
                .Count());
            Assert.IsNull(query.First());

            //query.CaseExecutionId(null);

            Assert.AreEqual(0, query.Count());
            Assert.AreEqual(0, query
                .Count());
            Assert.IsNull(query.First());
        }

        [Test]
        public virtual void testHistoricTaskInstanceCaseInstanceId()
        {
            var task = taskService.NewTask();
            task.CaseInstanceId = "aCaseInstanceId";
            taskService.SaveTask(task);

            var hti = historyService.CreateHistoricTaskInstanceQuery(c => c.TaskId == task.Id)
                .First();

            Assert.AreEqual("aCaseInstanceId", hti.CaseInstanceId);

            task.CaseInstanceId = "anotherCaseInstanceId";
            taskService.SaveTask(task);

            hti = historyService.CreateHistoricTaskInstanceQuery(c => c.TaskId == task.Id)
                .First();

            Assert.AreEqual("anotherCaseInstanceId", hti.CaseInstanceId);

            // Finally, Delete task
            taskService.DeleteTask(task.Id, true);
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testProcessDefinitionKeyProperty()
        {
            // given
            var key = "oneTaskProcess";
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey(key)
                .Id;

            // when
            var task = historyService.CreateHistoricTaskInstanceQuery(c => c.ProcessInstanceId == ProcessInstanceId)
                //.TaskDefinitionKey("theTask")
                .First();

            // then
            Assert.NotNull(task.ProcessDefinitionKey);
            Assert.AreEqual(key, task.ProcessDefinitionKey);

            Assert.IsNull(task.CaseDefinitionKey);
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testQueryByTaskDefinitionKey()
        {
            // given
            runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            // when
            var query1 = historyService.CreateHistoricTaskInstanceQuery(c => c.TaskDefinitionKey == "theTask");

            var query2 = historyService.CreateHistoricTaskInstanceQuery()
                //.TaskDefinitionKeyIn("theTask")
                ;

            // then
            Assert.AreEqual(1, query1.Count());
            Assert.AreEqual(1, query2.Count());
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testQueryByTaskDefinitionKeys()
        {
            // given
            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            caseService.CreateCaseInstanceByKey("oneTaskCase");

            // when
            var query = historyService.CreateHistoricTaskInstanceQuery()
                //.TaskDefinitionKeyIn("theTask", "PI_HumanTask_1")
                ;

            // then
            Assert.AreEqual(2, query.Count());
        }

        [Test]
        public virtual void testQueryByInvalidTaskDefinitionKeys()
        {
            var query = historyService.CreateHistoricTaskInstanceQuery();

            //query.TaskDefinitionKeyIn("invalid");
            Assert.AreEqual(0, query.Count());

            try
            {
                //query.TaskDefinitionKeyIn(null);
                Assert.Fail("A ProcessEngineExcpetion was expected.");
            }
            catch (NotValidException)
            {
            }

            try
            {
                //query.TaskDefinitionKeyIn((string)null);
                Assert.Fail("A ProcessEngineExcpetion was expected.");
            }
            catch (NotValidException)
            {
            }

            try
            {
                string[] values = { "a", null, "b" };
                //query.TaskDefinitionKeyIn(values);
                Assert.Fail("A ProcessEngineExcpetion was expected.");
            }
            catch (NotValidException)
            {
            }
        }

        [Test]
        [Deployment("resources/api/cmmn/oneTaskCase.cmmn")]
        public virtual void testCaseDefinitionKeyProperty()
        {
            // given
            var key = "oneTaskCase";
            var caseInstanceId = caseService.CreateCaseInstanceByKey(key)
                .Id;

            // when
            var task = historyService.CreateHistoricTaskInstanceQuery()
                //.CaseInstanceId(caseInstanceId)
                //.TaskDefinitionKey("PI_HumanTask_1")
                .First();

            // then
            Assert.NotNull(task.CaseDefinitionKey);
            Assert.AreEqual(key, task.CaseDefinitionKey);

            Assert.IsNull(task.ProcessDefinitionKey);
        }

        [Test]
        [Deployment]
        public virtual void testHistoricTaskInstance()
        {
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("HistoricTaskInstanceTest")
                .Id;

            //SimpleDateFormat sdf = new SimpleDateFormat("dd/MM/yyyy hh:mm:ss");

            // Set priority to non-default value
            var runtimeTask = taskService.CreateTaskQuery(c => c.ProcessInstanceId == ProcessInstanceId)
                .First();
            runtimeTask.Priority = 1234;

            // Set due-date
            var dueDate = DateTime.Parse("01/02/2003 04:05:06");
            runtimeTask.DueDate = dueDate;
            taskService.SaveTask(runtimeTask);

            var taskId = runtimeTask.Id;
            var TaskDefinitionKey = runtimeTask.TaskDefinitionKey;

            var historicTaskInstance = historyService.CreateHistoricTaskInstanceQuery()
                .First();
            Assert.AreEqual(taskId, historicTaskInstance.Id);
            Assert.AreEqual(1234, historicTaskInstance.Priority);
            Assert.AreEqual("Clean up", historicTaskInstance.Name);
            Assert.AreEqual("Schedule an engineering meeting for next week with the new hire.",
                historicTaskInstance.Description);
            Assert.AreEqual(dueDate, historicTaskInstance.DueDate);
            Assert.AreEqual("kermit", historicTaskInstance.Assignee);
            Assert.AreEqual(TaskDefinitionKey, historicTaskInstance.TaskDefinitionKey);
            Assert.IsNull(historicTaskInstance.EndTime);
            Assert.IsNull(historicTaskInstance.DurationInMillis);

            Assert.IsNull(historicTaskInstance.CaseDefinitionId);
            Assert.IsNull(historicTaskInstance.CaseInstanceId);
            Assert.IsNull(historicTaskInstance.CaseExecutionId);

            // the activity instance id is set
            Assert.AreEqual(((TaskEntity)runtimeTask).Execution.ActivityInstanceId,
                historicTaskInstance.ActivityInstanceId);

            runtimeService.SetVariable(ProcessInstanceId, "deadline", "yesterday");

            // move clock by 1 second
            var now = ClockUtil.CurrentTime;
            ClockUtil.CurrentTime = new DateTime(now.Ticks + 1000);

            taskService.Complete(taskId);

            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                .Count());

            historicTaskInstance = historyService.CreateHistoricTaskInstanceQuery()
                .First();
            Assert.AreEqual(taskId, historicTaskInstance.Id);
            Assert.AreEqual(1234, historicTaskInstance.Priority);
            Assert.AreEqual("Clean up", historicTaskInstance.Name);
            Assert.AreEqual("Schedule an engineering meeting for next week with the new hire.",
                historicTaskInstance.Description);
            Assert.AreEqual(dueDate, historicTaskInstance.DueDate);
            Assert.AreEqual("kermit", historicTaskInstance.Assignee);
            //Assert.AreEqual(TaskEntity.DELETE_REASON_COMPLETED, historicTaskInstance.DeleteReason);
            Assert.AreEqual(TaskDefinitionKey, historicTaskInstance.TaskDefinitionKey);
            Assert.NotNull(historicTaskInstance.EndTime);
            Assert.NotNull(historicTaskInstance.DurationInMillis);
            Assert.True(historicTaskInstance.DurationInMillis >= 1000);
            Assert.True(((HistoricTaskInstanceEventEntity)historicTaskInstance).DurationRaw >= 1000);

            Assert.IsNull(historicTaskInstance.CaseDefinitionId);
            Assert.IsNull(historicTaskInstance.CaseInstanceId);
            Assert.IsNull(historicTaskInstance.CaseExecutionId);

            historyService.DeleteHistoricTaskInstance(taskId);

            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery()
                .Count());
        }

        [Test]
        [Deployment]
        public virtual void testHistoricTaskInstanceAssignmentListener()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["assignee"] = "jonny";
            runtimeService.StartProcessInstanceByKey("testProcess", variables);

            var hai = historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityId == "task")
                .First();
            Assert.AreEqual("jonny", hai.Assignee);

            var hti = historyService.CreateHistoricTaskInstanceQuery()
                .First();
            Assert.AreEqual("jonny", hti.Assignee);
            Assert.IsNull(hti.Owner);
        }

        [Test]
        [Deployment]
        public virtual void testHistoricTaskInstanceQuery()
        {
            // First instance is finished
            var finishedInstance = runtimeService.StartProcessInstanceByKey("HistoricTaskQueryTest");

            // Set priority to non-default value
            var task = taskService.CreateTaskQuery(c => c.ProcessInstanceId == finishedInstance.Id)
                .First();
            task.Priority = 1234;
            var dueDate = DateTime.Parse("01/02/2003 04:05:06");
            task.DueDate = dueDate;

            taskService.SaveTask(task);

            // Complete the task
            var taskId = task.Id;
            taskService.Complete(taskId);

            // ITask id
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery(c => c.TaskId == taskId)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery(c => c.TaskId == "unexistingtaskid")
                .Count());

            // Name
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery(c => c.Name == "Clean_up")
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery(c => c.Name == "unexistingname")
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery(c => c.Name.Contains("Clean\\_u%"))
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery(c => c.Name.Contains("%lean\\_up"))
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery(c => c.Name.Contains("%lean\\_u%"))
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery(c => c.Name.Contains("%unexistingname%"))
                .Count());


            // Description
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery(c => c.Description == "Historic task_description")
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery(c => c.Description == "unexistingdescription")
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery(c => c.Description.Contains("%task\\_description"))
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery(c => c.Description.Contains("Historic task%"))
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery(c => c.Description.Contains("%task%"))
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery(c => c.Description.Contains("%unexistingdescripton%"))
                .Count());

            // IExecution id
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery(c => c.ExecutionId == finishedInstance.Id)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery(c => c.ExecutionId == "unexistingexecution")
                .Count());

            // Process instance id
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery(c => c.ProcessInstanceId == finishedInstance.Id)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery(c => c.ProcessInstanceId == "unexistingid")
                .Count());

            // Process definition id
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery(c => c.ProcessDefinitionId == finishedInstance.ProcessDefinitionId)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery(c => c.ProcessDefinitionId == "unexistingdefinitionid")
                .Count());

            // Process definition name
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery(c => c.ProcessDefinitionName == "Historic task query test process")
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery(c => c.ProcessDefinitionName == "unexistingdefinitionname")
                .Count());

            // Process definition key
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery(c => c.ProcessDefinitionKey == "HistoricTaskQueryTest")
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery(c => c.ProcessDefinitionKey == "unexistingdefinitionkey")
                .Count());


            // Assignee
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery(c => c.Assignee == "ker_mit")
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery(c => c.Assignee == "johndoe")
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery(c => c.Assignee.Contains("%er\\_mit"))
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery(c => c.Assignee.Contains("ker\\_mi%"))
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery(c => c.Assignee.Contains("%er\\_mi%"))
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery(c => c.Assignee.Contains("%johndoe%"))
                .Count());

            // Delete reason
            //Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery().TaskDeleteReason(TaskEntity.DELETE_REASON_COMPLETED).Count());
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery(c => c.DeleteReason == "deleted")
                .Count());

            // ITask definition ID
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery(c => c.TaskDefinitionKey == "task")
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery(c => c.TaskDefinitionKey == "unexistingkey")
                .Count());

            // ITask priority
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery(c => c.Priority == 1234)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery(c => c.Priority == 5678)
                .Count());


            // Due date
            var anHourAgo = DateTime.Now;
            anHourAgo = new DateTime(dueDate.Ticks);
            anHourAgo = anHourAgo.AddHours(-1);

            var anHourLater = DateTime.Now;
            anHourLater = new DateTime(dueDate.Ticks);
            anHourLater = anHourLater.AddHours(1);


            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery(c => c.DueDate == dueDate)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery(c => c.DueDate == anHourAgo)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery(c => c.DueDate == anHourLater)
                .Count());

            // Due date before
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery(c => c.DueDate < anHourLater)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery(c => c.DueDate < anHourAgo)
                .Count());

            // Due date after
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery(c => c.DueDate > anHourAgo)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery(c => c.DueDate > anHourLater)
                .Count());

            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery(c => c.DueDate == dueDate && c.DueDate < anHourLater)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery(c => c.DueDate == dueDate && c.DueDate < anHourAgo)
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery(c => c.DueDate == dueDate && c.DueDate > anHourAgo)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery(c => c.DueDate == dueDate && c.DueDate > anHourLater)
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery(c => c.DueDate < anHourAgo && c.DueDate > anHourLater)
                .Count());

            // Finished and Unfinished - Add anther other instance that has a running task (unfinished)
            runtimeService.StartProcessInstanceByKey("HistoricTaskQueryTest");

            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.Finished()
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                /*.Unfinished()*/
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery()
                /*.Unfinished()*/
                //.Finished()
                .Count());
        }

        [Test]
        [Deployment]
        public virtual void testHistoricTaskInstanceQueryByProcessVariableValue()
        {
            var historyLevel = processEngineConfiguration.HistoryLevel.Id;
            if (historyLevel >= ProcessEngineConfigurationImpl.HistorylevelAudit)
            {
                IDictionary<string, object> variables = new Dictionary<string, object>();
                variables["hallo"] = "steffen";

                var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("HistoricTaskInstanceTest", variables)
                    .Id;

                var runtimeTask = taskService.CreateTaskQuery(c => c.ProcessInstanceId == ProcessInstanceId)
                    .First();
                var taskId = runtimeTask.Id;

                var historicTaskInstance = historyService.CreateHistoricTaskInstanceQuery()
                    //.ProcessVariableValueEquals("hallo", "steffen")
                    .First();

                Assert.NotNull(historicTaskInstance);
                Assert.AreEqual(taskId, historicTaskInstance.Id);

                taskService.Complete(taskId);
                Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery(m=>m.TaskId==taskId)
                    //.TaskId(taskId)
                    .Count());

                historyService.DeleteHistoricTaskInstance(taskId);
                Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery()
                    .Count());
            }
        }

        [Test]
        [Deployment]
        public virtual void testHistoricTaskInstanceQueryProcessFinished()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("TwoTaskHistoricTaskQueryTest");
            var task = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id)
                .First();

            // Running task on running process should be available
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.ProcessUnfinished()
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery()
                //.ProcessFinished()
                .Count());

            // Finished and running task on running process should be available
            taskService.Complete(task.Id);
            Assert.AreEqual(2, historyService.CreateHistoricTaskInstanceQuery()
                //.ProcessUnfinished()
                .Count());
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery()
                //.ProcessFinished()
                .Count());

            // 2 finished tasks are found for finished process after completing last task of process
            task = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id)
                .First();
            taskService.Complete(task.Id);
            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery()
                //.ProcessUnfinished()
                .Count());
            Assert.AreEqual(2, historyService.CreateHistoricTaskInstanceQuery()
                //.ProcessFinished()
                .Count());

            Assert.AreEqual(0, historyService.CreateHistoricTaskInstanceQuery()
               // .ProcessUnfinished()
                //.ProcessFinished()
                .Count());
        }

        [Test]
        [Deployment]
        public virtual void testHistoricTaskInstanceQuerySorting()
        {
            var instance = runtimeService.StartProcessInstanceByKey("HistoricTaskQueryTest");

            var taskId = taskService.CreateTaskQuery(c => c.ProcessInstanceId == instance.Id)
                .First()
                .Id;
            taskService.Complete(taskId);

            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByDeleteReason()
                /*.Asc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByExecutionId()
                /*.Asc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByHistoricActivityInstanceId()
                /*.Asc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByHistoricActivityInstanceStartTime()
                /*.Asc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                /*.OrderByProcessDefinitionId()*/
                /*.Asc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByProcessInstanceId()
                /*.Asc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByTaskDescription()
                /*.Asc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                /*.OrderByTaskName()*/
                /*.Asc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByTaskDefinitionKey()
                /*.Asc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByTaskPriority()
                /*.Asc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                /*.OrderByTaskAssignee()*/
                /*.Asc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByTaskId()
                /*.Asc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByTaskDueDate()
                /*.Asc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByTaskFollowUpDate()
                /*.Asc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByCaseDefinitionId()
                /*.Asc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByCaseInstanceId()
                /*.Asc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByCaseExecutionId()
                /*.Asc()*/
                .Count());

            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByDeleteReason()
                /*.Desc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByExecutionId()
                /*.Desc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByHistoricActivityInstanceId()
                /*.Desc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByHistoricActivityInstanceStartTime()
                /*.Desc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                /*.OrderByProcessDefinitionId()*/
                /*.Desc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByProcessInstanceId()
                /*.Desc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByTaskDescription()
                /*.Desc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                /*.OrderByTaskName()*/
                /*.Desc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByTaskDefinitionKey()
                /*.Desc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByTaskPriority()
                /*.Desc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                /*.OrderByTaskAssignee()*/
                /*.Desc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByTaskId()
                /*.Desc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByTaskDueDate()
                /*.Desc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByTaskFollowUpDate()
                /*.Desc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByCaseDefinitionId()
                /*.Desc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByCaseInstanceId()
                /*.Desc()*/
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery()
                //.OrderByCaseExecutionId()
                /*.Desc()*/
                .Count());
        }

        [Test]
        [Deployment("resources/api/cmmn/oneTaskCase.cmmn")]
        public virtual void testQueryByCaseDefinitionId()
        {
            // given
            var caseDefinitionId = repositoryService.CreateCaseDefinitionQuery()
                .First()
                .Id;

            var caseInstanceId = caseService.WithCaseDefinition(caseDefinitionId)
                .Create()
                .Id;

            var humanTaskId = caseService.CreateCaseExecutionQuery(c => c.ActivityId == "PI_HumanTask_1")
                .First()
                .Id;

            // then
            var query = historyService.CreateHistoricTaskInstanceQuery();

            //query.CaseDefinitionId(caseDefinitionId);

            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(1, query
                .Count());
            Assert.NotNull(query.First());

            var task = query.First();
            Assert.NotNull(task);

            Assert.AreEqual(caseDefinitionId, task.CaseDefinitionId);
            Assert.AreEqual(caseInstanceId, task.CaseInstanceId);
            Assert.AreEqual(humanTaskId, task.CaseExecutionId);
        }

        [Test]
        [Deployment("resources/api/cmmn/oneTaskCase.cmmn")]
        public virtual void testQueryByCaseDefinitionKey()
        {
            // given
            var key = "oneTaskCase";

            var caseDefinitionId = repositoryService.CreateCaseDefinitionQuery()
                //.CaseDefinitionKey(key)
                .First()
                .Id;

            var caseInstanceId = caseService.WithCaseDefinitionByKey(key)
                .Create()
                .Id;

            var humanTaskId = caseService.CreateCaseExecutionQuery(c => c.ActivityId == "PI_HumanTask_1")
                .First()
                .Id;

            // then
            var query = historyService.CreateHistoricTaskInstanceQuery();

            //query.CaseDefinitionKey(key);

            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(1, query
                .Count());
            Assert.NotNull(query.First());

            var task = query.First();
            Assert.NotNull(task);

            Assert.AreEqual(caseDefinitionId, task.CaseDefinitionId);
            Assert.AreEqual(caseInstanceId, task.CaseInstanceId);
            Assert.AreEqual(humanTaskId, task.CaseExecutionId);
        }

        [Test]
        [Deployment("resources/api/cmmn/oneTaskCase.cmmn")]
        public virtual void testQueryByCaseDefinitionName()
        {
            // given
            var caseDefinition = repositoryService.CreateCaseDefinitionQuery()
                .First();

            var caseDefinitionName = caseDefinition.Name;
            var caseDefinitionId = caseDefinition.Id;

            var caseInstanceId = caseService.WithCaseDefinitionByKey("oneTaskCase")
                .Create()
                .Id;

            var humanTaskId = caseService.CreateCaseExecutionQuery(c => c.ActivityId == "PI_HumanTask_1")
                .First()
                .Id;

            // then
            var query = historyService.CreateHistoricTaskInstanceQuery();

            //query.CaseDefinitionName(caseDefinitionName);

            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(1, query
                .Count());
            Assert.NotNull(query.First());

            var task = query.First();
            Assert.NotNull(task);

            Assert.AreEqual(caseDefinitionId, task.CaseDefinitionId);
            Assert.AreEqual(caseInstanceId, task.CaseInstanceId);
            Assert.AreEqual(humanTaskId, task.CaseExecutionId);
        }

        [Test]
        [Deployment("resources/api/cmmn/oneTaskCase.cmmn")]
        public virtual void testQueryByCaseExecutionId()
        {
            // given
            var key = "oneTaskCase";

            var caseDefinitionId = repositoryService.CreateCaseDefinitionQuery()
                //.CaseDefinitionKey(key)
                .First()
                .Id;

            var caseInstanceId = caseService.WithCaseDefinitionByKey(key)
                .Create()
                .Id;

            var humanTaskId = caseService.CreateCaseExecutionQuery(c => c.ActivityId == "PI_HumanTask_1")
                .First()
                .Id;

            // then
            var query = historyService.CreateHistoricTaskInstanceQuery();

            //query.CaseExecutionId(humanTaskId);

            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(1, query
                .Count());
            Assert.NotNull(query.First());

            var task = query.First();
            Assert.NotNull(task);

            Assert.AreEqual(caseDefinitionId, task.CaseDefinitionId);
            Assert.AreEqual(caseInstanceId, task.CaseInstanceId);
            Assert.AreEqual(humanTaskId, task.CaseExecutionId);
        }

        [Test]
        [Deployment("resources/api/cmmn/oneTaskCase.cmmn")]
        public virtual void testQueryByCaseInstanceId()
        {
            // given
            var key = "oneTaskCase";

            var caseDefinitionId = repositoryService.CreateCaseDefinitionQuery()
               // .CaseDefinitionKey(key)
                .First()
                .Id;

            var caseInstanceId = caseService.WithCaseDefinitionByKey(key)
                .Create()
                .Id;

            var humanTaskId = caseService.CreateCaseExecutionQuery(c => c.ActivityId == "PI_HumanTask_1")
                .First()
                .Id;

            // then
            var query = historyService.CreateHistoricTaskInstanceQuery();

            //query.CaseInstanceId(caseInstanceId);

            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(1, query
                .Count());
            Assert.NotNull(query.First());

            var task = query.First();
            Assert.NotNull(task);

            Assert.AreEqual(caseDefinitionId, task.CaseDefinitionId);
            Assert.AreEqual(caseInstanceId, task.CaseInstanceId);
            Assert.AreEqual(humanTaskId, task.CaseExecutionId);
        }
    }
}