using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.History
{

    /// <summary>
    /// 
    /// 
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryAudit) ]

    [TestFixture]
    public class HistoricProcessInstanceTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment("resources/history/oneTaskProcess.bpmn20.xml")]
        public virtual void TestHistoricDataCreatedForProcessExecution()
        {
            //DateTime calendar = new GregorianCalendar();
            //calendar.Set(DateTime.YEAR, 2010);
            //calendar.Set(DateTime.MONTH, 8);
            //calendar.Set(DateTime.DAY_OF_MONTH, 30);
            //calendar.Set(DateTime.HOUR_OF_DAY, 12);
            //calendar.Set(DateTime.MINUTE, 0);
            //calendar.Set(DateTime.SECOND, 0);
            //calendar.Set(DateTime.MILLISECOND, 0);
            DateTime calendar = new DateTime(2010, 8, 30, 12, 0, 0);

            DateTime noon = calendar;

            ClockUtil.CurrentTime = noon;
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", "myBusinessKey");

            Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery(m=>m.EndTime==null)/*.Unfinished()*/.Count());
            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery( c=> c.EndTime !=null).Count());
            IHistoricProcessInstance historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId == processInstance.Id).FirstOrDefault();

            Assert.NotNull(historicProcessInstance);
            Assert.AreEqual(processInstance.Id, historicProcessInstance.Id);
            Assert.AreEqual(processInstance.BusinessKey, historicProcessInstance.BusinessKey);
            Assert.AreEqual(processInstance.ProcessDefinitionId, historicProcessInstance.ProcessDefinitionId);
            Assert.AreEqual(noon, historicProcessInstance.StartTime);
            Assert.IsNull(historicProcessInstance.EndTime);
            Assert.IsNull(historicProcessInstance.DurationInMillis);
            Assert.IsNull(historicProcessInstance.CaseInstanceId);

            IList<ITask> tasks = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).ToList();

            Assert.AreEqual(1, tasks.Count);

            // in this test scenario we assume that 25 seconds after the process start, the
            // User completes the task (yes! he must be almost as fast as me)
            DateTime twentyFiveSecsAfterNoon = noon.AddSeconds(25);// new DateTime(noon.Ticks + 25 * 1000);
            ClockUtil.CurrentTime = twentyFiveSecsAfterNoon;
            taskService.Complete(tasks[0].Id);

            var historicProcessInstances = historyService.CreateHistoricProcessInstanceQuery(c => c.ProcessInstanceId == processInstance.Id).ToList();

            historicProcessInstance = historicProcessInstances.First(); //historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId == processInstance.Id).First();

            Assert.NotNull(historicProcessInstance);
            Assert.AreEqual(processInstance.Id, historicProcessInstance.Id);
            Assert.AreEqual(processInstance.ProcessDefinitionId, historicProcessInstance.ProcessDefinitionId);
            Assert.AreEqual(noon, historicProcessInstance.StartTime);
            Assert.AreEqual(twentyFiveSecsAfterNoon, historicProcessInstance.EndTime);
            Assert.AreEqual(new long?(25 * 1000), historicProcessInstance.DurationInMillis);
            Assert.True(((HistoricProcessInstanceEventEntity)historicProcessInstance).DurationRaw >= 25000);
            Assert.IsNull(historicProcessInstance.CaseInstanceId);

            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery(m=>m.EndTime==null).Count());
            Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery(c => c.EndTime != null).Count());

            runtimeService.StartProcessInstanceByKey("oneTaskProcess", "myBusinessKey");
            Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery(c => c.EndTime != null).Count());
            Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery(m => m.EndTime == null).Count());
            //源码：.finished().unfinished().cout 无法理解其查询语句
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery(c => c.EndTime == null).Count());
        }

        [Test]
        [Deployment("resources/history/oneTaskProcess.bpmn20.xml")]
        public virtual void TestLongRunningHistoricDataCreatedForProcessExecution()
        {
           // const long oneYear =   60 * 60 * 24 * 365;//1000 * 60 * 60 * 24 * 365;
            DateTime _now = DateTime.Now;
            DateTime cal = _now.AddSeconds(-_now.Second).AddMilliseconds(-_now.Millisecond);
            //cal..Set(DateTime.SECOND, 0);
            //cal.Set(DateTime.MILLISECOND, 0);
            double oneYear = (_now.AddYears(1) - _now).TotalMilliseconds;
            DateTime now = cal;
            ClockUtil.CurrentTime = now;

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final org.Camunda.bpm.Engine.Runtime.IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", "myBusinessKey");
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", "myBusinessKey");

            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery()/*.Unfinished()*/.Count());
            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery( c=> c.EndTime !=null).Count());
            IHistoricProcessInstance historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId == processInstance.Id).First();

            Assert.AreEqual(now.ToString(), historicProcessInstance.StartTime.ToString());

            IList<ITask> tasks = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).ToList();
            Assert.AreEqual(1, tasks.Count);

            // in this test scenario we assume that one year after the process start, the
            // IUser completes the task (incredible speedy!)
            //cal.AddYears(1);
            DateTime oneYearLater = cal.AddYears(1);
            ClockUtil.CurrentTime = oneYearLater;

            taskService.Complete(tasks[0].Id);

            historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId == processInstance.Id).First();

            Assert.AreEqual(now.ToString(), historicProcessInstance.StartTime.ToString());
            Assert.AreEqual(oneYearLater.ToString(), historicProcessInstance.EndTime.ToString());//结束时间
            Assert.True(historicProcessInstance.DurationInMillis >= oneYear);
            Assert.True(((HistoricProcessInstanceEventEntity)historicProcessInstance).DurationRaw >= oneYear);

            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery(c=>c.EndTime==null)/*.Unfinished()*/.Count());
            Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery( c=> c.EndTime !=null).Count());
        }

        [Test]
        [Deployment("resources/history/oneTaskProcess.bpmn20.xml")]
        public virtual void TestDeleteProcessInstanceHistoryCreated()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            Assert.NotNull(processInstance);

            // Delete process instance should not Delete the history
            runtimeService.DeleteProcessInstance(processInstance.Id, "cancel");
            IHistoricProcessInstance historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId == processInstance.Id).First();
            Assert.NotNull(historicProcessInstance.EndTime);
        }

        [Test]
        [Deployment("resources/history/oneTaskProcess.bpmn20.xml")]
        public virtual void TestHistoricProcessInstanceStartDate()
        {
            runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            DateTime date = DateTime.Now;
            DateTime update = UpDate(date);
            Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery(m =>m.StartTime>= update && m.StartTime <= date)/*.StartDateOn(date)*/.Count());
            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().StartDateBy(date).Count());
            Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery(m=>m.StartTime>=update).Count());
            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().StartDateBy(date.AddDays(-1)).Count());
            DateTime date_2 = date.AddDays(-1);
            DateTime update_2 = UpDate(date_2);
            Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery(m => m.StartTime >= update_2).Count());
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().StartDateBy(date.AddDays(1)).Count());
            DateTime date_3 = date.AddDays(1);
            DateTime update_3 = UpDate(date_3);
            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery(m => m.StartTime >= update_3).Count());
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().StartDateOn(date.AddDays(-1)).Count());
            DateTime date_4 = date.AddDays(-1);
            DateTime update_4 = UpDate(date_4);
            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery(m => m.StartTime >= update_4 && m.StartTime <= date_4).Count());
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().StartDateOn(date.AddDays(1)).Count());
            DateTime date_5 = date.AddDays(1);
            DateTime update_5 = UpDate(date_5);
            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery(m => m.StartTime >= update_5 && m.StartTime <= date_5).Count());
        }
        //源码查询用
        private DateTime UpDate(DateTime date)
        {
            DateTime r =DateTime.Parse(date.ToShortDateString());
            if (date.Hour >= 12)
            {
                r = r.AddHours(12);
            }
            return r;
        }

        [Test]
        [Deployment("resources/history/oneTaskProcess.bpmn20.xml")]
        public virtual void TestHistoricProcessInstanceFinishDateUnfinished()
        {
            runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            DateTime date = DateTime.Now;

            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().FinishDateOn(date).Count());
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().FinishDateBy(date).Count());
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().FinishDateBy(date.AddDays(1)).Count());
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().FinishDateBy(date.AddDays(-1)).Count());
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().FinishDateOn(date.AddDays(-1)).Count());
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().FinishDateOn(date.AddDays(1)).Count());
        }

        [Test]
        [Deployment("resources/history/oneTaskProcess.bpmn20.xml")]
        public virtual void TestHistoricProcessInstanceFinishDateFinished()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            DateTime date = DateTime.Now;

            runtimeService.DeleteProcessInstance(pi.Id, "cancel");

            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().FinishDateOn(date).Count());
            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().FinishDateBy(date).Count());
            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().FinishDateBy(date.AddDays(1)).Count());

            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().FinishDateBy(date.AddDays(-1)).Count());
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().FinishDateOn(date.AddDays(-1)).Count());
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().FinishDateOn(date.AddDays(1)).Count());
        }

        [Test]
        [Deployment("resources/history/oneTaskProcess.bpmn20.xml")]
        public virtual void TestHistoricProcessInstanceDelete()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            runtimeService.DeleteProcessInstance(pi.Id, "cancel");

            IHistoricProcessInstance historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery().First();
            Assert.NotNull(historicProcessInstance.DeleteReason);
            Assert.AreEqual("cancel", historicProcessInstance.DeleteReason);

            Assert.NotNull(historicProcessInstance.EndTime);
        }

        /// <summary>
        /// See: https://app.Camunda.com/jira/browse/CAM-1324 </summary>
        [Test]
        [Deployment]
        public virtual void TestHistoricProcessInstanceDeleteAsync()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("failing");

            runtimeService.DeleteProcessInstance(pi.Id, "cancel");

            IHistoricProcessInstance historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery().First();
            Assert.NotNull(historicProcessInstance.DeleteReason);
            Assert.AreEqual("cancel", historicProcessInstance.DeleteReason);

            Assert.NotNull(historicProcessInstance.EndTime);
        }

        [Test]
        [Deployment][RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
        public virtual void TestHistoricProcessInstanceQueryWithIncidents()
        {
            // start instance with incidents
            runtimeService.StartProcessInstanceByKey("Process_1");
            ExecuteAvailableJobs();

            // start instance without incidents
            runtimeService.StartProcessInstanceByKey("Process_1");

            Assert.AreEqual(2, historyService.CreateHistoricProcessInstanceQuery().Count());
            Assert.AreEqual(2, historyService.CreateHistoricProcessInstanceQuery().Count());

            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().WithIncidents().Count());
            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().WithIncidents().Count());

            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().Where(c=>c.Incidents.Any(d=>d.IncidentMessage.Contains(""Unknown property used%\\_Tr%").Count());
            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().Where(c=>c.Incidents.Any(d=>d.IncidentMessage.Contains(""Unknown property used%\\_Tr%").Count());

            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().Where(c=>c.Incidents.Any(d=>d.IncidentMessage.Contains(""Unknown message%").Count());
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().Where(c=>c.Incidents.Any(d=>d.IncidentMessage.Contains(""Unknown message%").Count());

            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().IncidentMessage("Unknown property used in expression: ${incidentTrigger1}. Cause: Cannot resolve identifier 'incidentTrigger1'").Count());
            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().IncidentMessage("Unknown property used in expression: ${incidentTrigger1}. Cause: Cannot resolve identifier 'incidentTrigger1'").Count());

            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().IncidentMessage("Unknown property used in expression: ${incident_Trigger2}. Cause: Cannot resolve identifier 'incident_Trigger2'").Count());
            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().IncidentMessage("Unknown property used in expression: ${incident_Trigger2}. Cause: Cannot resolve identifier 'incident_Trigger2'").Count());

            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().IncidentMessage("Unknown message").Count());
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().IncidentMessage("Unknown message").Count());
        }

        [Test][Deployment( "resources/api/mgmt/IncidentTest.TestShouldDeleteIncidentAfterJobWasSuccessfully.bpmn") ]
        public virtual void TestHistoricProcessInstanceQueryIncidentStatusOpen()
        {
            //given a processes instance, which will Assert.Fail
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("failingProcessWithUserTask", parameters);

            //when jobs are executed till retry Count is zero
            ExecuteAvailableJobs();

            //then query for historic process instance with open incidents will return one
            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().IncidentStatus("open").Count());
        }

        [Test]
        [Deployment("resources/api/mgmt/IncidentTest.TestShouldDeleteIncidentAfterJobWasSuccessfully.bpmn")]
        public virtual void TestHistoricProcessInstanceQueryIncidentStatusResolved()
        {
            //given a incident processes instance
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["Assert.Fail"] = true;
            IProcessInstance pi1 = runtimeService.StartProcessInstanceByKey("failingProcessWithUserTask", parameters);
            ExecuteAvailableJobs();

            //when `Assert.Fail` variable is set to true and job retry Count is set to one and executed again
            runtimeService.SetVariable(pi1.Id, "Assert.Fail", false);
            IJob jobToResolve = managementService.CreateJobQuery(c=>c.ProcessInstanceId == pi1.Id).First();
            managementService.SetJobRetries(jobToResolve.Id, 1);
            ExecuteAvailableJobs();

            //then query for historic process instance with resolved incidents will return one
            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().IncidentStatus("resolved").Count());
        }

        [Test]
        [Deployment("resources/api/mgmt/IncidentTest.TestShouldDeleteIncidentAfterJobWasSuccessfully.bpmn")]
        public virtual void TestHistoricProcessInstanceQueryIncidentStatusOpenWithTwoProcesses()
        {
            //given two processes, which will Assert.Fail, are started
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["Assert.Fail"] = true;
            IProcessInstance pi1 = runtimeService.StartProcessInstanceByKey("failingProcessWithUserTask", parameters);
            runtimeService.StartProcessInstanceByKey("failingProcessWithUserTask", parameters);
            ExecuteAvailableJobs();
            //Assert.AreEqual(2, historyService.CreateHistoricProcessInstanceQuery().IncidentStatus("open").Count());

            //when 'Assert.Fail' variable is set to false, job retry Count is set to one
            //and available jobs are executed
            runtimeService.SetVariable(pi1.Id, "Assert.Fail", false);
            IJob jobToResolve = managementService.CreateJobQuery(c=>c.ProcessInstanceId == pi1.Id).First();
            managementService.SetJobRetries(jobToResolve.Id, 1);
            ExecuteAvailableJobs();

            ////then query with open and with resolved incidents returns one
            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().IncidentStatus("open").Count());
            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().IncidentStatus("resolved").Count());
        }

        [Test]
        public virtual void TestHistoricProcessInstanceQueryWithIncidentMessageNull()
        {
            try
            {
                //historyService.CreateHistoricProcessInstanceQuery().IncidentMessage(null).Count();
                Assert.Fail("incidentMessage with null value is not allowed");
            }
            catch (NullValueException)
            {
                // expected
            }
        }

        [Test]
        public virtual void TestHistoricProcessInstanceQueryWithIncidentMessageLikeNull()
        {
            try
            {
                //historyService.CreateHistoricProcessInstanceQuery().Where(c=>c.Incidents.Any(d=>d.IncidentMessage.Contains("null).Count();
                Assert.Fail("incidentMessageLike with null value is not allowed");
            }
            catch (NullValueException)
            {
                // expected
            }
        }

        [Test][Deployment( "resources/history/oneAsyncTaskProcess.bpmn20.xml") ]
        public virtual void TestHistoricProcessInstanceQuery()
        {
            DateTime startTime = new DateTime();

            ClockUtil.CurrentTime = startTime;
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", "businessKey_123");
            IJob job = managementService.CreateJobQuery().First();
            managementService.ExecuteJob(job.Id);

            DateTime hourAgo = new DateTime();
            hourAgo.AddHours(-1);
            DateTime hourFromNow = new DateTime();
            hourFromNow.AddHours(1);

            // Start/end dates
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().FinishedBefore(hourAgo).Count());
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().FinishedBefore(hourFromNow).Count());
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().FinishedAfter(hourAgo).Count());
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().FinishedAfter(hourFromNow).Count());
            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().StartedBefore(hourFromNow).Count());
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().StartedBefore(hourAgo).Count());
            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().StartedAfter(hourAgo).Count());
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().StartedAfter(hourFromNow).Count());
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().StartedAfter(hourFromNow).StartedBefore(hourAgo).Count());

            // General fields
            Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery( c=> c.EndTime !=null).Count());
            Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId == processInstance.Id).Count());
            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionId ==processInstance.ProcessDefinitionId).Count());
            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess").Count());

            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().ProcessInstanceBusinessKey("businessKey_123").Count());
            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().ProcessInstanceBusinessKeyLike("business%").Count());
            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().ProcessInstanceBusinessKeyLike("%sinessKey\\_123").Count());
            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().ProcessInstanceBusinessKeyLike("%siness%").Count());

            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().ProcessDefinitionName("The One Task_Process").Count());
            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().ProcessDefinitionNameLike("The One Task%").Count());
            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().ProcessDefinitionNameLike("%One Task\\_Process").Count());
            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().ProcessDefinitionNameLike("%One Task%").Count());

            IList<string> exludeIds = new List<string>();
            exludeIds.Add("unexistingProcessDefinition");

            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().ProcessDefinitionKeyNotIn(exludeIds).Count());

            //exludeIds.Add("oneTaskProcess");
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess").ProcessDefinitionKeyNotIn(exludeIds).Count());
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().ProcessDefinitionKeyNotIn(exludeIds).Count());

            //try
            //{
            //    // oracle handles empty string like null which seems to lead to undefined behavior of the LIKE comparison
            //    historyService.CreateHistoricProcessInstanceQuery().ProcessDefinitionKeyNotIn((new string[]{}));
            //    Assert.Fail("Exception expected");
            //}
            //catch (NotValidException)
            //{
            //    // expected
            //}

            // After finishing process
            taskService.Complete(taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First().Id);
            Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery( c=> c.EndTime !=null).Count());
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().FinishedBefore(hourAgo).Count());
            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().FinishedBefore(hourFromNow).Count());
            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().FinishedAfter(hourAgo).Count());
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().FinishedAfter(hourFromNow).Count());
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().FinishedAfter(hourFromNow).FinishedBefore(hourAgo).Count());

            //// No incidents should are created
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().WithIncidents().Count());
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().Where(c=>c.Incidents.Any(d=>d.IncidentMessage.Contains(""Unknown property used%").Count());
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().IncidentMessage("Unknown property used in expression: #{failing}. Cause: Cannot resolve identifier 'failing'").Count());

            //// execute activities
            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().ExecutedActivityAfter(hourAgo).Count());
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().ExecutedActivityBefore(hourAgo).Count());
            //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().ExecutedActivityBefore(hourFromNow).Count());
            //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery()/*.ExecutedActivityAfter(hourFromNow)*/.Count());

            // execute jobs
            if (processEngineConfiguration.HistoryLevel.Equals(HistoryLevelFields.HistoryLevelFull))
            {
                //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().ExecutedJobAfter(hourAgo).Count());
                //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery().ExecutedActivityBefore(hourAgo).Count());
                //Assert.AreEqual(1, historyService.CreateHistoricProcessInstanceQuery().ExecutedActivityBefore(hourFromNow).Count());
                //Assert.AreEqual(0, historyService.CreateHistoricProcessInstanceQuery()/*.ExecutedActivityAfter(hourFromNow)*/.Count());
            }
        }

        [Test]
        public virtual void TestHistoricProcessInstanceSorting()
        {

            //Deployment("resources/history/oneTaskProcess.bpmn20.xml");
            //Deployment("resources/history/HistoricActivityInstanceTest.TestSorting.bpmn20.xml");

            ////deploy second version of the same process definition
            //Deployment("resources/history/oneTaskProcess.bpmn20.xml");

            //IList<IProcessDefinition> processDefinitions = processEngine.RepositoryService.CreateProcessDefinitionQuery(c=>c.Key == "oneTaskProcess").ToList();
            //foreach (IProcessDefinition processDefinition in processDefinitions)
            //{
            //    runtimeService.StartProcessInstanceById(processDefinition.Id);
            //}
            //runtimeService.StartProcessInstanceByKey("process");

            //IList<IHistoricProcessInstance> processInstances = historyService.CreateHistoricProcessInstanceQuery()//.OrderByProcessInstanceId()/*.Asc()*/.ToList();
            //Assert.AreEqual(3, processInstances.Count);
            //VerifySorting(processInstances, historicProcessInstanceByProcessInstanceId());

            //Assert.AreEqual(3, historyService.CreateHistoricProcessInstanceQuery().OrderByProcessInstanceStartTime()/*.Asc()*/.Count());
            //Assert.AreEqual(3, historyService.CreateHistoricProcessInstanceQuery().OrderByProcessInstanceEndTime()/*.Asc()*/.Count());
            //Assert.AreEqual(3, historyService.CreateHistoricProcessInstanceQuery().OrderByProcessInstanceDuration()/*.Asc()*/.Count());

            //processInstances = historyService.CreateHistoricProcessInstanceQuery()/*.OrderByProcessDefinitionId()*//*.Asc()*/.ToList();
            //Assert.AreEqual(3, processInstances.Count);
            //verifySorting(processInstances, historicProcessInstanceByProcessDefinitionId());

            //processInstances = historyService.CreateHistoricProcessInstanceQuery()//.OrderByProcessDefinitionKey()/*.Asc()*/.ToList();
            //Assert.AreEqual(3, processInstances.Count);
            //verifySorting(processInstances, historicProcessInstanceByProcessDefinitionKey());

            //processInstances = historyService.CreateHistoricProcessInstanceQuery()/*.OrderByProcessDefinitionName()*//*.Asc()*/.ToList();
            //Assert.AreEqual(3, processInstances.Count);
            //verifySorting(processInstances, historicProcessInstanceByProcessDefinitionName());

            //processInstances = historyService.CreateHistoricProcessInstanceQuery().OrderByProcessDefinitionVersion()/*.Asc()*/.ToList();
            //Assert.AreEqual(3, processInstances.Count);
            //verifySorting(processInstances, historicProcessInstanceByProcessDefinitionVersion());

            //Assert.AreEqual(3, historyService.CreateHistoricProcessInstanceQuery().OrderByProcessInstanceBusinessKey()/*.Asc()*/.Count());

            //Assert.AreEqual(3, historyService.CreateHistoricProcessInstanceQuery()//.OrderByProcessInstanceId()/*.Desc()*/.Count());
            //Assert.AreEqual(3, historyService.CreateHistoricProcessInstanceQuery().OrderByProcessInstanceStartTime()/*.Desc()*/.Count());
            //Assert.AreEqual(3, historyService.CreateHistoricProcessInstanceQuery().OrderByProcessInstanceEndTime()/*.Desc()*/.Count());
            //Assert.AreEqual(3, historyService.CreateHistoricProcessInstanceQuery().OrderByProcessInstanceDuration()/*.Desc()*/.Count());
            //Assert.AreEqual(3, historyService.CreateHistoricProcessInstanceQuery()/*.OrderByProcessDefinitionId()*//*.Desc()*/.Count());
            //Assert.AreEqual(3, historyService.CreateHistoricProcessInstanceQuery().OrderByProcessInstanceBusinessKey()/*.Desc()*/.Count());

            //Assert.AreEqual(3, historyService.CreateHistoricProcessInstanceQuery()//.OrderByProcessInstanceId()/*.Asc()*/.Count());
            //Assert.AreEqual(3, historyService.CreateHistoricProcessInstanceQuery().OrderByProcessInstanceStartTime()/*.Asc()*/.Count());
            //Assert.AreEqual(3, historyService.CreateHistoricProcessInstanceQuery().OrderByProcessInstanceEndTime()/*.Asc()*/.Count());
            //Assert.AreEqual(3, historyService.CreateHistoricProcessInstanceQuery().OrderByProcessInstanceDuration()/*.Asc()*/.Count());
            //Assert.AreEqual(3, historyService.CreateHistoricProcessInstanceQuery()/*.OrderByProcessDefinitionId()*//*.Asc()*/.Count());
            //Assert.AreEqual(3, historyService.CreateHistoricProcessInstanceQuery().OrderByProcessInstanceBusinessKey()/*.Asc()*/.Count());

            //Assert.AreEqual(3, historyService.CreateHistoricProcessInstanceQuery()//.OrderByProcessInstanceId()/*.Desc()*/.Count());
            //Assert.AreEqual(3, historyService.CreateHistoricProcessInstanceQuery().OrderByProcessInstanceStartTime()/*.Desc()*/.Count());
            //Assert.AreEqual(3, historyService.CreateHistoricProcessInstanceQuery().OrderByProcessInstanceEndTime()/*.Desc()*/.Count());
            //Assert.AreEqual(3, historyService.CreateHistoricProcessInstanceQuery().OrderByProcessInstanceDuration()/*.Desc()*/.Count());
            //Assert.AreEqual(3, historyService.CreateHistoricProcessInstanceQuery()/*.OrderByProcessDefinitionId()*//*.Desc()*/.Count());
            //Assert.AreEqual(3, historyService.CreateHistoricProcessInstanceQuery().OrderByProcessInstanceBusinessKey()/*.Desc()*/.Count());

        }

        [Test][Deployment( new [] {"resources/api/runtime/superProcess.bpmn20.xml", "resources/api/runtime/subProcess.bpmn20.xml"})]
        public virtual void TestHistoricProcessInstanceSubProcess()
        {
            IProcessInstance superPi = runtimeService.StartProcessInstanceByKey("subProcessQueryTest");
            IProcessInstance subPi = runtimeService.CreateProcessInstanceQuery(m=>m.ProcessInstanceId==superPi.ProcessInstanceId)/*.SetSuperProcessInstanceId(superPi.ProcessInstanceId)*/.First();

            IHistoricProcessInstance historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery(m=>m.ProcessInstanceId==superPi.ProcessInstanceId)/*.SubProcessInstanceId(subPi.ProcessInstanceId)*/.First();
            Assert.NotNull(historicProcessInstance);
            Assert.AreEqual(historicProcessInstance.Id, superPi.Id);
        }

        [Test]
        public virtual void TestInvalidSorting()
        {
            try
            {
                historyService.CreateHistoricProcessInstanceQuery()/*.Asc()*/;
                Assert.Fail();
            }
            catch (ProcessEngineException)
            {

            }

            try
            {
                historyService.CreateHistoricProcessInstanceQuery()/*.Desc()*/;
                Assert.Fail();
            }
            catch (ProcessEngineException)
            {

            }

            try
            {
                historyService.CreateHistoricProcessInstanceQuery()/*//.OrderByProcessInstanceId()*/.ToList();
                Assert.Fail();
            }
            catch (ProcessEngineException)
            {

            }
        }

        [Test]
        [Deployment("resources/history/oneTaskProcess.bpmn20.xml")]
        public virtual void TestDeleteReason()
        {
            // ACT-1098
            if (!ProcessEngineConfiguration.HistoryNone.Equals(processEngineConfiguration.History))
            {
                const string deleteReason = "some Delete reason";
                IProcessInstance pi = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
                runtimeService.DeleteProcessInstance(pi.Id, deleteReason);
                IHistoricProcessInstance hpi = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId == pi.Id).First();
                Assert.AreEqual(deleteReason, hpi.DeleteReason);
            }
        }

        [Test]
        [Deployment]
        public virtual void TestLongProcessDefinitionKey()
        {
            // must be equals to attribute id of element process in process model
            const string processDefinitionKey = "myrealrealrealrealrealrealrealrealrealrealreallongprocessdefinitionkeyawesome";

            IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key ==processDefinitionKey).First();
            IProcessInstance processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);

            // get HPI by process instance id
            IHistoricProcessInstance hpi = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId == processInstance.Id).First();
            Assert.NotNull(hpi);
            AssertProcessEnded(hpi.Id);

            // get HPI by process definition key
            IHistoricProcessInstance hpi2 = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionKey==processDefinitionKey).First();
            Assert.NotNull(hpi2);
            AssertProcessEnded(hpi2.Id);

            // check we got the same HPIs
            Assert.AreEqual(hpi.Id, hpi2.Id);

        }

        //[Test]不启用caseService
        //[Deployment( new []{ "resources/history/HistoricProcessInstanceTest.TestQueryByCaseInstanceId.cmmn", "resources/history/HistoricProcessInstanceTest.TestQueryByCaseInstanceId.bpmn20.xml" }) ]
        public virtual void TestQueryByCaseInstanceId()
        {
            throw new NotImplementedException("不启用caseService");
            // given
            string caseInstanceId = caseService.WithCaseDefinitionByKey("case").Create().Id;

            // then
            IQueryable<IHistoricProcessInstance> query = historyService.CreateHistoricProcessInstanceQuery();

            //query.CaseInstanceId(caseInstanceId);

            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(1, query.Count());

            IHistoricProcessInstance historicProcessInstance = query.First();
            Assert.NotNull(historicProcessInstance);
            Assert.IsNull(historicProcessInstance.EndTime);

            Assert.AreEqual(caseInstanceId, historicProcessInstance.CaseInstanceId);

            // complete existing IUser task -> completes the process instance
            //string taskId = taskService.CreateTaskQuery(c=>c.CaseInstanceId ==caseInstanceId).First().Id;
            //taskService.Complete(taskId);

            // the completed historic process instance is still associated with the
            // case instance id
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(1, query.Count());

            historicProcessInstance = query.First();
            Assert.NotNull(historicProcessInstance);
            Assert.NotNull(historicProcessInstance.EndTime);

            Assert.AreEqual(caseInstanceId, historicProcessInstance.CaseInstanceId);

        }

        //[Test]不启用caseService
        //[Deployment( new [] { "resources/history/HistoricProcessInstanceTest.TestQueryByCaseInstanceId.cmmn", "resources/history/HistoricProcessInstanceTest.TestQueryByCaseInstanceIdHierarchy-super.bpmn20.xml", "resources/history/HistoricProcessInstanceTest.TestQueryByCaseInstanceIdHierarchy-sub.bpmn20.xml" }) ]
        public virtual void TestQueryByCaseInstanceIdHierarchy()
        {
            throw new NotImplementedException("不启用caseService");
            // given
            string caseInstanceId = caseService.WithCaseDefinitionByKey("case").Create().Id;

            // then
            IQueryable<IHistoricProcessInstance> query = historyService.CreateHistoricProcessInstanceQuery();

            //query.CaseInstanceId(caseInstanceId);

            Assert.AreEqual(2, query.Count());
            Assert.AreEqual(2, query.Count());

            foreach (IHistoricProcessInstance hpi in query.ToList())
            {
                Assert.AreEqual(caseInstanceId, hpi.CaseInstanceId);
            }

            // complete existing IUser task -> completes the process instance(s)
            string taskId = taskService.CreateTaskQuery(c=>c.CaseInstanceId ==caseInstanceId).First().Id;
            taskService.Complete(taskId);

            // the completed historic process instance is still associated with the
            // case instance id
            Assert.AreEqual(2, query.Count());
            Assert.AreEqual(2, query.Count());

            foreach (IHistoricProcessInstance hpi in query.ToList())
            {
                Assert.AreEqual(caseInstanceId, hpi.CaseInstanceId);
            }

        }
        [Test]
        public virtual void TestQueryByInvalidCaseInstanceId()
        {
            IQueryable<IHistoricProcessInstance> query = historyService.CreateHistoricProcessInstanceQuery();

            //query.CaseInstanceId("invalid");

            Assert.AreEqual(0, query.Count());
            Assert.AreEqual(0, query.Count());

            //query.CaseInstanceId(null);

            Assert.AreEqual(0, query.Count());
            Assert.AreEqual(0, query.Count());
        }

        //[Test] case
        //[Deployment(new [] { "resources/history/HistoricProcessInstanceTest.TestBusinessKey.cmmn", "resources/history/HistoricProcessInstanceTest.TestBusinessKey.bpmn20.xml" }) ]
        public virtual void TestBusinessKey()
        {
            throw new NotImplementedException("不需要实现case");
            return;
            // given
            string businessKey = "aBusinessKey";
            //Case net未实现
            var id = caseService.WithCaseDefinitionByKey("case").BusinessKey(businessKey).Create().Id;

            // then
            IQueryable<IHistoricProcessInstance> query = historyService.CreateHistoricProcessInstanceQuery();

            //query.ProcessInstanceBusinessKey(businessKey);

            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(1, query.Count());

            IHistoricProcessInstance historicProcessInstance = query.First();
            Assert.NotNull(historicProcessInstance);

            Assert.AreEqual(businessKey, historicProcessInstance.BusinessKey);

        }

        [Test]
        [Deployment( new [] { "resources/history/HistoricProcessInstanceTest.TestStartActivityId-super.bpmn20.xml", "resources/history/HistoricProcessInstanceTest.TestStartActivityId-sub.bpmn20.xml" })]
        public virtual void TestStartActivityId()
        {
            // given

            // when
            runtimeService.StartProcessInstanceByKey("super");

            // then
            IHistoricProcessInstance hpi = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionKey=="sub").First();

            //Assert.AreEqual("theSubStart", hpi.StartActivityId);

        }

        [Test]
        [Deployment( new [] { "resources/history/HistoricProcessInstanceTest.TestStartActivityId-super.bpmn20.xml", "resources/history/HistoricProcessInstanceTest.TestAsyncStartActivityId-sub.bpmn20.xml" }) ]
        //异步
        public virtual void TestAsyncStartActivityId()
        {
            // given
            runtimeService.StartProcessInstanceByKey("super");

            // when
            ExecuteAvailableJobs();

            // then
            IHistoricProcessInstance hpi = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionKey=="sub").First();

            Assert.AreEqual("theSubStart", hpi.StartActivityId);

        }

        [Test][Deployment(  "resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void TestStartByKeyWithCaseInstanceId()
        {
            string caseInstanceId = "aCaseInstanceId";

            string processInstanceId = runtimeService.StartProcessInstanceByKey("oneTaskProcess", null, caseInstanceId).Id;

            IHistoricProcessInstance firstInstance = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId == processInstanceId).First();

            Assert.NotNull(firstInstance);

            Assert.AreEqual(caseInstanceId, firstInstance.CaseInstanceId);

            // the second possibility to start a process instance /////////////////////////////////////////////

            processInstanceId = runtimeService.StartProcessInstanceByKey("oneTaskProcess", null, caseInstanceId, null).Id;

            IHistoricProcessInstance secondInstance = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId == processInstanceId).First();

            Assert.NotNull(secondInstance);

            Assert.AreEqual(caseInstanceId, secondInstance.CaseInstanceId);

        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void TestStartByIdWithCaseInstanceId()
        {
            string processDefinitionId = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "oneTaskProcess").First().Id;

            string caseInstanceId = "aCaseInstanceId";
            string processInstanceId = runtimeService.StartProcessInstanceById(processDefinitionId, null, caseInstanceId).Id;

            IHistoricProcessInstance firstInstance = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId == processInstanceId).First();

            Assert.NotNull(firstInstance);

            Assert.AreEqual(caseInstanceId, firstInstance.CaseInstanceId);

            // the second possibility to start a process instance /////////////////////////////////////////////

            processInstanceId = runtimeService.StartProcessInstanceById(processDefinitionId, null, caseInstanceId, null).Id;

            IHistoricProcessInstance secondInstance = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId == processInstanceId).First();

            Assert.NotNull(secondInstance);

            Assert.AreEqual(caseInstanceId, secondInstance.CaseInstanceId);

        }

        [Test]
        [Deployment]
        public virtual void TestEndTimeAndEndActivity()
        {
            // given
            string processInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            string taskId = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="userTask2").First().Id;

            IQueryable<IHistoricProcessInstance> query = historyService.CreateHistoricProcessInstanceQuery();

            // when (1)
            taskService.Complete(taskId);

            // then (1)
            IHistoricProcessInstance historicProcessInstance = query.First();

            Assert.IsNull(historicProcessInstance.EndActivityId);
            Assert.IsNull(historicProcessInstance.EndTime);

            // when (2)
            runtimeService.DeleteProcessInstance(processInstanceId, null);

            // then (2)
            historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery(m=>m.Id==processInstanceId).First();// query.First();

            Assert.IsNull(historicProcessInstance.EndActivityId);
            Assert.NotNull(historicProcessInstance.EndTime);
        }

        [Test]
        [Deployment(new[] { "resources/api/cmmn/oneProcessTaskCase.cmmn", "resources/api/runtime/oneTaskProcess.bpmn20.xml" })]
        public virtual void TestQueryBySuperCaseInstanceId()
        {
            string superCaseInstanceId = caseService.CreateCaseInstanceByKey("oneProcessTaskCase").Id;

            //IQueryable<IHistoricProcessInstance> query = historyService.CreateHistoricProcessInstanceQuery().SuperCaseInstanceId(superCaseInstanceId);

            //Assert.AreEqual(1, query.Count());
            //Assert.AreEqual(1, query.Count());

            //IHistoricProcessInstance subProcessInstance = query.First();
            //Assert.NotNull(subProcessInstance);
            //Assert.AreEqual(superCaseInstanceId, subProcessInstance.SuperCaseInstanceId);
            //Assert.IsNull(subProcessInstance.SuperProcessInstanceId);
        }

        [Test]
        public virtual void TestQueryByInvalidSuperCaseInstanceId()
        {
            IQueryable<IHistoricProcessInstance> query = historyService.CreateHistoricProcessInstanceQuery();

            //query.SuperCaseInstanceId("invalid");

            Assert.AreEqual(0, query.Count());
            Assert.AreEqual(0, query.Count());

            //query.CaseInstanceId(null);

            Assert.AreEqual(0, query.Count());
            Assert.AreEqual(0, query.Count());
        }

        [Test][Deployment( new [] { "resources/api/runtime/superProcessWithCaseCallActivity.bpmn20.xml", "resources/api/cmmn/oneTaskCase.cmmn" })]
        public virtual void TestQueryBySubCaseInstanceId()
        {
            string superProcessInstanceId = runtimeService.StartProcessInstanceByKey("subProcessQueryTest").Id;

            //string subCaseInstanceId = caseService.CreateCaseInstanceQuery().SuperProcessInstanceId(superProcessInstanceId).First().Id;

            //IQueryable<IHistoricProcessInstance> query = historyService.CreateHistoricProcessInstanceQuery().SubCaseInstanceId(subCaseInstanceId);

            //Assert.AreEqual(1, query.Count());
            //Assert.AreEqual(1, query.Count());

            //IHistoricProcessInstance superProcessInstance = query.First();
            //Assert.NotNull(superProcessInstance);
            //Assert.AreEqual(superProcessInstanceId, superProcessInstance.Id);
            //Assert.IsNull(superProcessInstance.SuperCaseInstanceId);
            //Assert.IsNull(superProcessInstance.SuperProcessInstanceId);
        }

        [Test]
        public virtual void TestQueryByInvalidSubCaseInstanceId()
        {
            IQueryable<IHistoricProcessInstance> query = historyService.CreateHistoricProcessInstanceQuery();

            //query.SubCaseInstanceId("invalid");

            //Assert.AreEqual(0, query.Count());
            //Assert.AreEqual(0, query.Count());

            //query.CaseInstanceId(null);

            Assert.AreEqual(0, query.Count());
            Assert.AreEqual(0, query.Count());
        }

        [Test][Deployment( new []{ "resources/api/cmmn/oneProcessTaskCase.cmmn", "resources/api/runtime/oneTaskProcess.bpmn20.xml" }) ]
        public virtual void TestSuperCaseInstanceIdProperty()
        {
            string superCaseInstanceId = caseService.CreateCaseInstanceByKey("oneProcessTaskCase").Id;

            var id = caseService.CreateCaseExecutionQuery(c=>c.ActivityId == "PI_ProcessTask_1").First().Id;

            IHistoricProcessInstance instance = historyService.CreateHistoricProcessInstanceQuery().First();

            Assert.NotNull(instance);
            Assert.AreEqual(superCaseInstanceId, instance.SuperCaseInstanceId);

            string taskId = taskService.CreateTaskQuery().First().Id;
            taskService.Complete(taskId);

            instance = historyService.CreateHistoricProcessInstanceQuery().First();

            Assert.NotNull(instance);
            Assert.AreEqual(superCaseInstanceId, instance.SuperCaseInstanceId);
        }

        [Test][Deployment(  "resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void TestProcessDefinitionKeyProperty()
        {
            // given
            string key = "oneTaskProcess";
            string processInstanceId = runtimeService.StartProcessInstanceByKey(key).Id;

            // when
            IHistoricProcessInstance instance = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId == processInstanceId).First();

            // then
            Assert.NotNull(instance.ProcessDefinitionKey);
            Assert.AreEqual(key, instance.ProcessDefinitionKey);
        }

        [Test]
        [Deployment]
        public virtual void TestProcessInstanceShouldBeActive()
        {
            // given

            // when
            string processInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            // then
            IHistoricProcessInstance historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId == processInstanceId).First();

            Assert.IsNull(historicProcessInstance.EndTime);
            Assert.IsNull(historicProcessInstance.DurationInMillis);
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void TestRetrieveProcessDefinitionName()
        {

            // given
            string processInstanceId = runtimeService.StartProcessInstanceByKey("oneTaskProcess").Id;

            // when
            IHistoricProcessInstance historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId == processInstanceId).First();

            // then
            Assert.AreEqual("The One ITask Process", historicProcessInstance.ProcessDefinitionName);
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void TestRetrieveProcessDefinitionVersion()
        {

            // given
            string processInstanceId = runtimeService.StartProcessInstanceByKey("oneTaskProcess").Id;

            // when
            IHistoricProcessInstance historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId == processInstanceId).First();

            // then
            Assert.AreEqual(1, historicProcessInstance.ProcessDefinitionVersion);
        }

        [Test]
        public virtual void TestHistoricProcInstExecutedActivityInInterval()
        {
            // given proc instance with wait state
            DateTime now = new DateTime();
            ClockUtil.CurrentTime = now;
            IBpmnModelInstance model = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("proc").StartEvent().UserTask().EndEvent().Done();
            Deployment(model);

            DateTime hourFromNow = (DateTime)now;
            hourFromNow.AddHours(1);

            runtimeService.StartProcessInstanceByKey("proc");

            //when query historic process instance which has executed an activity after the start time
            // and before a hour after start time
            //IHistoricProcessInstance historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery()/*/*.ExecutedActivityAfter(now)*/.ExecutedActivityBefore(hourFromNow)*/.First();


            //then query returns result
            //Assert.NotNull(historicProcessInstance);


            // when proc inst is not in interval
            DateTime sixHoursFromNow = new DateTime(now.Ticks);
            sixHoursFromNow.AddHours(6);


            //historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery()/*/*.ExecutedActivityAfter(hourFromNow)*/.ExecutedActivityBefore(sixHoursFromNow)*/.First();

            //then query should return NO result
            //Assert.IsNull(historicProcessInstance);
        }

        [Test]
        public virtual void TestHistoricProcInstExecutedActivityAfter()
        {
            // given
            DateTime now = new DateTime();
            ClockUtil.CurrentTime = now;
            IBpmnModelInstance model = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("proc").StartEvent().EndEvent().Done();
            Deployment(model);

            DateTime hourFromNow = new DateTime(now.Ticks);
            hourFromNow.AddHours(1);

            runtimeService.StartProcessInstanceByKey("proc");

            //when query historic process instance which has executed an activity after the start time
            IHistoricProcessInstance historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery()/*.ExecutedActivityAfter(now)*/.First();

            //then query returns result
            Assert.NotNull(historicProcessInstance);

            //when query historic proc inst with execute activity after a hour of the starting time
            historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery()/*.ExecutedActivityAfter(hourFromNow)*/.First();

            //then query returns no result
            Assert.IsNull(historicProcessInstance);
        }

        [Test]
        public virtual void TestHistoricProcInstExecutedActivityBefore()
        {
            // given
            DateTime now = new DateTime();
            ClockUtil.CurrentTime = now;
            IBpmnModelInstance model = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("proc").StartEvent().EndEvent().Done();
            Deployment(model);

            DateTime hourBeforeNow = new DateTime(now.Ticks);
            hourBeforeNow = hourBeforeNow.AddHours(-1);

            runtimeService.StartProcessInstanceByKey("proc");

            //when query historic process instance which has executed an activity before the start time
            //IHistoricProcessInstance historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery().ExecutedActivityBefore(now).First();

            ////then query returns result, since the query is less-then-equal
            //Assert.NotNull(historicProcessInstance);

            ////when query historic proc inst which executes an activity an hour before the starting time
            //historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery().ExecutedActivityBefore(hourBeforeNow).First();

            ////then query returns no result
            //Assert.IsNull(historicProcessInstance);
        }

        [Test]
        public virtual void TestHistoricProcInstExecutedActivityWithTwoProcInsts()
        {
            // given
            IBpmnModelInstance model = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("proc").StartEvent().EndEvent().Done();
            Deployment(model);

            DateTime now = new DateTime();
            DateTime hourBeforeNow = new DateTime(now.Ticks);
            hourBeforeNow = hourBeforeNow.AddHours(-1);

            ClockUtil.CurrentTime = hourBeforeNow;
            runtimeService.StartProcessInstanceByKey("proc");

            ClockUtil.CurrentTime = now;
            runtimeService.StartProcessInstanceByKey("proc");

            //when query execute activity between now and an hour ago
            IList<IHistoricProcessInstance> list = historyService.CreateHistoricProcessInstanceQuery()/*.ExecutedActivityAfter(hourBeforeNow)*//*.ExecutedActivityBefore(now)*/.ToList();

            //then two historic process instance have to be returned
            Assert.AreEqual(2, list.Count);

            //when query execute activity after an half hour before now
            DateTime halfHour = new DateTime(now.Ticks);
            halfHour.AddMinutes(-30);
            //IHistoricProcessInstance historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery().ExecutedActivityAfter(halfHour).First();

            //then only the latest historic process instance is returned
            //Assert.NotNull(historicProcessInstance);
        }


        [Test]
        public virtual void TestHistoricProcInstExecutedActivityWithEmptyInterval()
        {
            // given
            DateTime now = new DateTime();
            ClockUtil.CurrentTime = now;
            IBpmnModelInstance model = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("proc").StartEvent().EndEvent().Done();
            Deployment(model);

            DateTime hourBeforeNow = new DateTime(now.Ticks);
            hourBeforeNow = hourBeforeNow.AddHours(-1);

            runtimeService.StartProcessInstanceByKey("proc");

            //when query historic proc inst which executes an activity an hour before and after the starting time
            //IHistoricProcessInstance historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery().ExecutedActivityBefore(hourBeforeNow)/*.ExecutedActivityAfter(hourBeforeNow)*/.First();

            //then query returns no result
            //Assert.IsNull(historicProcessInstance);
        }

        [Test][RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull) ]
        public virtual void TestHistoricProcInstExecutedJobAfter()
        {
            // given
            IBpmnModelInstance asyncModel = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("async").StartEvent().CamundaAsyncBefore().EndEvent().Done();
            Deployment(asyncModel);
            IBpmnModelInstance model = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("proc").StartEvent().EndEvent().Done();
            Deployment(model);

            DateTime now = new DateTime();
            ClockUtil.CurrentTime = now;
            DateTime hourFromNow = new DateTime(now.Ticks);
            hourFromNow.AddHours(1);

            runtimeService.StartProcessInstanceByKey("async");
            IJob job = managementService.CreateJobQuery().First();
            managementService.ExecuteJob(job.Id);
            runtimeService.StartProcessInstanceByKey("proc");

            //when query historic process instance which has executed an job after the start time
            //IHistoricProcessInstance historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery().ExecutedJobAfter(now).First();

            //then query returns only a single process instance
            //Assert.NotNull(historicProcessInstance);

            ////when query historic proc inst with execute job after a hour of the starting time
            //historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery().ExecutedJobAfter(hourFromNow).First();

            ////then query returns no result
            //Assert.IsNull(historicProcessInstance);
        }


        [Test]
        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
        public virtual void TestHistoricProcInstExecutedJobBefore()
        {
            // given
            IBpmnModelInstance asyncModel = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("async").StartEvent().CamundaAsyncBefore().EndEvent().Done();
            Deployment(asyncModel);
            IBpmnModelInstance model = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("proc").StartEvent().EndEvent().Done();
            Deployment(model);

            DateTime now = new DateTime();
            ClockUtil.CurrentTime = now;
            DateTime hourBeforeNow = new DateTime(now.Ticks);
            hourBeforeNow.AddHours(-1);

            runtimeService.StartProcessInstanceByKey("async");
            IJob job = managementService.CreateJobQuery().First();
            managementService.ExecuteJob(job.Id);
            runtimeService.StartProcessInstanceByKey("proc");

            //when query historic process instance which has executed an job before the start time
            IHistoricProcessInstance historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery()/*.ExecutedJobBefore(now)*/.First();

            //then query returns only a single process instance since before is less-then-equal
            Assert.NotNull(historicProcessInstance);

            //when query historic proc inst with executed job before an hour of the starting time
            //historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery().ExecutedJobBefore(hourBeforeNow).First();

            //then query returns no result
            Assert.IsNull(historicProcessInstance);
        }

        [Test]
        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
        public virtual void TestHistoricProcInstExecutedJobWithTwoProcInsts()
        {
            // given
            IBpmnModelInstance asyncModel = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("async").StartEvent().CamundaAsyncBefore().EndEvent().Done();
            Deployment(asyncModel);

            IBpmnModelInstance model = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("proc").StartEvent().EndEvent().Done();
            Deployment(model);

            DateTime now = new DateTime();
            ClockUtil.CurrentTime = now;
            DateTime hourBeforeNow = new DateTime(now.Ticks);
            hourBeforeNow.AddHours(-1);

            ClockUtil.CurrentTime = hourBeforeNow;
            runtimeService.StartProcessInstanceByKey("async");
            IJob job = managementService.CreateJobQuery().First();
            managementService.ExecuteJob(job.Id);

            ClockUtil.CurrentTime = now;
            runtimeService.StartProcessInstanceByKey("async");
            runtimeService.StartProcessInstanceByKey("proc");

            //when query executed job between now and an hour ago
            //IList<IHistoricProcessInstance> list = historyService.CreateHistoricProcessInstanceQuery().ExecutedJobAfter(hourBeforeNow).ExecutedJobBefore(now).ToList();

            //then the two async historic process instance have to be returned
            //Assert.AreEqual(2, list.Count);

            //when query execute activity after an half hour before now
            DateTime halfHour = new DateTime(now.Ticks);
            halfHour.AddMinutes(-30);
            //IHistoricProcessInstance historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery().ExecutedJobAfter(halfHour).First();

            //then only the latest async historic process instance is returned
            //Assert.NotNull(historicProcessInstance);
        }

        [Test]
        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
        public virtual void TestHistoricProcInstExecutedJobWithEmptyInterval()
        {
            // given
            IBpmnModelInstance asyncModel = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("async").StartEvent().CamundaAsyncBefore().EndEvent().Done();
            Deployment(asyncModel);
            IBpmnModelInstance model = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("proc").StartEvent().EndEvent().Done();
            Deployment(model);

            DateTime now = new DateTime();
            ClockUtil.CurrentTime = now;
            DateTime hourBeforeNow = new DateTime(now.Ticks);
            hourBeforeNow.AddHours(-1);

            runtimeService.StartProcessInstanceByKey("async");
            IJob job = managementService.CreateJobQuery().First();
            managementService.ExecuteJob(job.Id);
            runtimeService.StartProcessInstanceByKey("proc");

            //when query historic proc inst with executed job before and after an hour before the starting time
            IHistoricProcessInstance historicProcessInstance = historyService.CreateHistoricProcessInstanceQuery()/*.ExecutedJobBefore(hourBeforeNow).ExecutedJobAfter(hourBeforeNow)*/.First();

            //then query returns no result
            Assert.IsNull(historicProcessInstance);
        }

    }

}