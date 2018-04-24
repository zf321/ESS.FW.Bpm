using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Timer
{
    [TestFixture]
    public class StartTimerEventTest : PluggableProcessEngineTestCase
    {
        //processDefinitionKey
        private IList<IProcessInstance> CreateProcessInstanceQueryByKey(IRuntimeService runtimeService, string processDefinitionKey)
        {
            DbContext db = runtimeService.GetDbContext();
            return (from a in db.Set<ExecutionEntity>()
                    join b in db.Set<ProcessDefinitionEntity>() on a.ProcessDefinitionId equals b.Id
                    where b.Key == processDefinitionKey && a.ParentId == null
                    select a).ToList().Cast<IProcessInstance>().ToList();
        }
        //processDefinitionId
        private IList<IProcessInstance> CreateProcessInstanceQueryById(IRuntimeService runtimeService, string processDefinitionId)
        {
            DbContext db = runtimeService.GetDbContext();
            return (from a in db.Set<ExecutionEntity>()
                    join b in db.Set<ProcessDefinitionEntity>() on a.ProcessDefinitionId equals b.Id
                    where  a.ParentId == null&&a.ProcessInstanceId==processDefinitionId
                    select a).ToList().Cast<IProcessInstance>().ToList();
        }
        [Test]
        [Deployment]
        public virtual void testDurationStartTimerEvent()
        {
            // Set the clock fixed
            DateTime startTime = DateTime.Now;
            // After process start, there should be timer created
            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(1, jobQuery.Count());
            // After setting the clock to time '50minutes and 5 seconds', the second
            // timer should fire
            ClockUtil.CurrentTime = new DateTime(startTime.Ticks + ((50 * 60 * 1000) + 5000));
            executeAllJobs();
            executeAllJobs();
            //IList<IProcessInstance> pi = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == "startTimerEventExample").ToList();
            DbContext db = runtimeService.GetDbContext();
            IList<IProcessInstance> pi = CreateProcessInstanceQueryByKey(runtimeService, "startTimerEventExample");
            Assert.AreEqual(1, pi.Count);

            Assert.AreEqual(0, jobQuery.Count());

        }

        [Test]
        [Deployment]
        public virtual void testFixedDateStartTimerEvent()
        {
            // After process start, there should be timer created
            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(1, jobQuery.Count());

            //ClockUtil.CurrentTime = (new SimpleDateFormat("dd/MM/yyyy hh:mm:ss")).parse("15/11/2036 11:12:30");
            ClockUtil.CurrentTime = DateTime.Parse("2036-11-15 11:12:30");
            executeAllJobs();

            IList<IProcessInstance> pi = CreateProcessInstanceQueryByKey(runtimeService, "startTimerEventExample");
            Assert.AreEqual(1, pi.Count);

            Assert.AreEqual(0, jobQuery.Count());

        }

        //[Test]
        [Deployment]
        public virtual void FAILING_testCycleDateStartTimerEvent()
        {
            ClockUtil.CurrentTime = DateTime.Now;

            // After process start, there should be timer created
            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(1, jobQuery.Count());

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final org.Camunda.bpm.engine.runtime. IQueryable<IProcessInstance> piq = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionKey=="startTimerEventExample");
            IList<IProcessInstance> piq = CreateProcessInstanceQueryByKey(runtimeService, "startTimerEventExample");

            Assert.AreEqual(0, piq.Count());

            moveByMinutes(5);
            executeAllJobs();
            Assert.AreEqual(1, piq.Count());
            Assert.AreEqual(1, jobQuery.Count());

            moveByMinutes(5);
            executeAllJobs();
            Assert.AreEqual(1, piq.Count());

            Assert.AreEqual(1, jobQuery.Count());
            // have to manually Delete pending timer
            //    cleanDB();

        }


        private void moveByMinutes(int minutes)
        {
            ClockUtil.CurrentTime = ClockUtil.CurrentTime.AddMinutes(minutes)/*.Millisecond + ((minutes * 60 * 1000) + 5000)*/;
        }

        //[Test]
        [Deployment]//TODO 死循环 ExecuteJobsCmd FailedJobListener
        public virtual void testCycleWithLimitStartTimerEvent()
        {
            ClockUtil.CurrentTime = DateTime.Now;

            // After process start, there should be timer created
            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(1, jobQuery.Count());

            // ensure that the deployment Id is set on the new job
            IJob job = jobQuery.First();
            Assert.NotNull(job.DeploymentId);

            IList<IProcessInstance> piq = CreateProcessInstanceQueryByKey(runtimeService, "startTimerEventExampleCycle");

            Assert.AreEqual(0, piq.Count());

            moveByMinutes(5);
            executeAllJobs();
            Assert.AreEqual(1, piq.Count());
            Assert.AreEqual(1, jobQuery.Count());

            // ensure that the deployment Id is set on the new job
            job = jobQuery.First();
            Assert.NotNull(job.DeploymentId);

            moveByMinutes(5);
            executeAllJobs();
            Assert.AreEqual(2, piq.Count());
            Assert.AreEqual(0, jobQuery.Count());

        }

        [Test]
        [Deployment]
        public virtual void testExpressionStartTimerEvent()
        {
            // ACT-1415: fixed start-date is an expression
            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(1, jobQuery.Count());

            //ClockUtil.CurrentTime = (new SimpleDateFormat("dd/MM/yyyy hh:mm:ss")).parse("15/11/2036 11:12:30");
            ClockUtil.CurrentTime = DateTime.Parse("2036-11-15 11:12:30");
            executeAllJobs();

            IList<IProcessInstance> pi = CreateProcessInstanceQueryByKey(runtimeService, "startTimerEventExample");
            Assert.AreEqual(1, pi.Count);

            Assert.AreEqual(0, jobQuery.Count());
        }

        // Todo: GetResourceAsStream
        //[Test]
        //[Deployment]
        //public virtual void testVersionUpgradeShouldCancelJobs()
        //{
        //    ClockUtil.CurrentTime = DateTime.Now;

        //    // After process start, there should be timer created
        //    IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
        //    Assert.AreEqual(1, jobQuery.Count());

        //    // we deploy new process version, with some small change
        //    System.IO.Stream @in = this.GetType().GetResourceAsStream("StartTimerEventTest.testVersionUpgradeShouldCancelJobs.bpmn20.xml");
        //    string process = (new string(IoUtil.ReadInputStream(@in, ""))).Replace("beforeChange", "changed");
        //    IoUtil.CloseSilently(@in);
        //    @in = new System.IO.MemoryStream(process.GetBytes());
        //    string id = repositoryService.CreateDeployment().AddInputStream("StartTimerEventTest.testVersionUpgradeShouldCancelJobs.bpmn20.xml", @in).Deploy().Id;
        //    IoUtil.CloseSilently(@in);

        //    Assert.AreEqual(1, jobQuery.Count());

        //    moveByMinutes(5);
        //    executeAllJobs();
        //    IProcessInstance processInstance = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionKey=="startTimerEventExample").First();
        //    string pi = processInstance.ProcessInstanceId;
        //    Assert.AreEqual("changed", runtimeService.GetActiveActivityIds(pi).ElementAt(0));

        //    Assert.AreEqual(1, jobQuery.Count());

        //    //    cleanDB();
        //    repositoryService.DeleteDeployment(id, true);
        //}

        [Test]
        [Deployment]
        public virtual void testTimerShouldNotBeRecreatedOnDeploymentCacheReboot()
        {

            // Just to be sure, I added this test. Sounds like something that could
            // easily happen
            // when the order of deploy/parsing is altered.

            // After process start, there should be timer created
            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(1, jobQuery.Count());

            // Reset deployment cache
            processEngineConfiguration.DeploymentCache.DiscardProcessDefinitionCache();

            // Start one instance of the process definition, this will trigger a cache
            // reload
            runtimeService.StartProcessInstanceByKey("startTimer");

            // No new jobs should have been created
            Assert.AreEqual(1, jobQuery.Count());
        }

        // Todo: GetResourceAsStream
        //public virtual void testTimerShouldNotBeRemovedWhenUndeployingOldVersion()
        //{
        //    // Deploy test process
        //    System.IO.Stream @in = this.GetType().GetResourceAsStream("StartTimerEventTest.testTimerShouldNotBeRemovedWhenUndeployingOldVersion.bpmn20.xml");
        //    string process = new string(IoUtil.ReadInputStream(@in, ""));
        //    IoUtil.CloseSilently(@in);

        //    @in = new System.IO.MemoryStream(process.GetBytes());
        //    string firstDeploymentId = repositoryService.CreateDeployment().AddInputStream("StartTimerEventTest.testVersionUpgradeShouldCancelJobs.bpmn20.xml", @in).Deploy().Id;
        //    IoUtil.CloseSilently(@in);

        //    // After process start, there should be timer created
        //    IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
        //    Assert.AreEqual(1, jobQuery.Count());

        //    // we deploy new process version, with some small change
        //    string processChanged = process.Replace("beforeChange", "changed");
        //    @in = new System.IO.MemoryStream(processChanged.GetBytes());
        //    string secondDeploymentId = repositoryService.CreateDeployment().AddInputStream("StartTimerEventTest.testVersionUpgradeShouldCancelJobs.bpmn20.xml", @in).Deploy().Id;
        //    IoUtil.CloseSilently(@in);
        //    Assert.AreEqual(1, jobQuery.Count());

        //    // Remove the first deployment
        //    repositoryService.DeleteDeployment(firstDeploymentId, true);

        //    // The removal of an old version should not affect timer deletion
        //    // ACT-1533: this was a bug, and the timer was deleted!
        //    Assert.AreEqual(1, jobQuery.Count());

        //    // Cleanup
        //    cleanDB();
        //    repositoryService.DeleteDeployment(secondDeploymentId, true);
        //}

        [Test]
        [Deployment]
        public virtual void testStartTimerEventInEventSubProcess()
        {
            DummyServiceTask.wasExecuted = false;

            // start process instance
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("startTimerEventInEventSubProcess");

            // check if execution exists
            IQueryable<IExecution> executionQuery = runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == processInstance.Id);
            Assert.AreEqual(1, executionQuery.Count());

            // check if User task exists
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id);
            Assert.AreEqual(1, taskQuery.Count());

            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(1, jobQuery.Count());
            // execute existing timer job
            managementService.ExecuteJob(managementService.CreateJobQuery().First().Id);
            Assert.AreEqual(0, jobQuery.Count());

            Assert.AreEqual(true, DummyServiceTask.wasExecuted);

            // check if User task doesn't exist because timer start event is
            // interrupting
            Assert.AreEqual(0, taskQuery.Count());

            // check if execution doesn't exist because timer start event is
            // interrupting
            Assert.AreEqual(0, executionQuery.Count());

            IQueryable<IProcessInstance> processInstanceQuery = runtimeService.CreateProcessInstanceQuery(c => c.ProcessInstanceId == processInstance.Id);
            Assert.AreEqual(0, processInstanceQuery.Count());
        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingStartTimerEventInEventSubProcess()
        {
            DummyServiceTask.wasExecuted = false;

            // start process instance
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("nonInterruptingStartTimerEventInEventSubProcess");

            // check if execution exists
            IQueryable<IExecution> executionQuery = runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == processInstance.Id);
            Assert.AreEqual(1, executionQuery.Count());

            // check if IUser task exists
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id);
            Assert.AreEqual(1, taskQuery.Count());

            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(1, jobQuery.Count());
            // execute existing job timer
            managementService.ExecuteJob(managementService.CreateJobQuery().First().Id);
            Assert.AreEqual(0, jobQuery.Count());

            Assert.AreEqual(true, DummyServiceTask.wasExecuted);

            // check if IUser task still exists because timer start event is non
            // interrupting
            Assert.AreEqual(1, taskQuery.Count());

            // check if execution still exists because timer start event is non
            // interrupting
            Assert.AreEqual(1, executionQuery.Count());

            IQueryable<IProcessInstance> processInstanceQuery = runtimeService.CreateProcessInstanceQuery(c => c.ProcessInstanceId == processInstance.Id);
            Assert.AreEqual(1, processInstanceQuery.Count());
        }

        [Test]
        [Deployment]
        public virtual void testStartTimerEventSubProcessInSubProcess()
        {
            DummyServiceTask.wasExecuted = false;

            // start process instance
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("startTimerEventSubProcessInSubProcess");

            // check if execution exists
            IQueryable<IExecution> executionQuery = runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == processInstance.Id);
            Assert.AreEqual(2, executionQuery.Count());

            // check if IUser task exists
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id);
            Assert.AreEqual(1, taskQuery.Count());

            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(1, jobQuery.Count());
            // execute existing timer job
            managementService.ExecuteJob(managementService.CreateJobQuery().First().Id);
            Assert.AreEqual(0, jobQuery.Count());

            Assert.AreEqual(true, DummyServiceTask.wasExecuted);

            // check if IUser task doesn't exist because timer start event is
            // interrupting
            Assert.AreEqual(0, taskQuery.Count());

            // check if execution doesn't exist because timer start event is
            // interrupting
            Assert.AreEqual(0, executionQuery.Count());

            IList<IProcessInstance> processInstanceQuery = CreateProcessInstanceQueryById(runtimeService, processInstance.Id);
            Assert.AreEqual(0, processInstanceQuery.Count());

        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingStartTimerEventSubProcessInSubProcess()
        {
            DummyServiceTask.wasExecuted = false;

            // start process instance
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("nonInterruptingStartTimerEventSubProcessInSubProcess");

            // check if execution exists
            IQueryable<IExecution> executionQuery = runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == processInstance.Id);
            Assert.AreEqual(2, executionQuery.Count());

            // check if IUser task exists
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id);
            Assert.AreEqual(1, taskQuery.Count());

            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(1, jobQuery.Count());
            // execute existing timer job
            managementService.ExecuteJob(jobQuery.First().Id);
            Assert.AreEqual(0, jobQuery.Count());

            Assert.AreEqual(true, DummyServiceTask.wasExecuted);

            // check if IUser task still exists because timer start event is non
            // interrupting
            Assert.AreEqual(1, taskQuery.Count());

            // check if execution still exists because timer start event is non
            // interrupting
            Assert.AreEqual(2, executionQuery.Count());

            IList<IProcessInstance> processInstanceQuery = CreateProcessInstanceQueryById(runtimeService, processInstance.Id);
            Assert.AreEqual(1, processInstanceQuery.Count());

        }

        [Test]
        [Deployment]
        public virtual void testStartTimerEventWithTwoEventSubProcesses()
        {
            // start process instance
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("startTimerEventWithTwoEventSubProcesses");

            // check if execution exists
            IQueryable<IExecution> executionQuery = runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == processInstance.Id);
            Assert.AreEqual(1, executionQuery.Count());

            // check if IUser task exists
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id);
            Assert.AreEqual(1, taskQuery.Count());

            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(2, jobQuery.Count());
            // get all timer jobs ordered by dueDate
            IList<IJob> orderedJobList = jobQuery/*.OrderByJobDuedate()*//*.Asc()*/.ToList();
            // execute first timer job
            managementService.ExecuteJob(orderedJobList[0].Id);
            Assert.AreEqual(0, jobQuery.Count());

            // check if IUser task doesn't exist because timer start event is
            // interrupting
            Assert.AreEqual(0, taskQuery.Count());

            // check if execution doesn't exist because timer start event is
            // interrupting
            Assert.AreEqual(0, executionQuery.Count());

            // check if process instance doesn't exist because timer start event is
            // interrupting
            IList<IProcessInstance> processInstanceQuery = CreateProcessInstanceQueryById(runtimeService, processInstance.Id);
            Assert.AreEqual(0, processInstanceQuery.Count());

        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingStartTimerEventWithTwoEventSubProcesses()
        {
            DummyServiceTask.wasExecuted = false;

            // start process instance
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("nonInterruptingStartTimerEventWithTwoEventSubProcesses");

            // check if execution exists
            IQueryable<IExecution> executionQuery = runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == processInstance.Id);
            Assert.AreEqual(1, executionQuery.Count());

            // check if IUser task exists
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id);
            Assert.AreEqual(1, taskQuery.Count());

            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(2, jobQuery.Count());
            // get all timer jobs ordered by dueDate
            IList<IJob> orderedJobList = jobQuery/*.OrderByJobDuedate()*//*.Asc()*/.ToList();
            // execute first timer job
            managementService.ExecuteJob(orderedJobList[0].Id);
            Assert.AreEqual(1, jobQuery.Count());

            Assert.AreEqual(true, DummyServiceTask.wasExecuted);

            DummyServiceTask.wasExecuted = false;

            // check if IUser task still exists because timer start event is non
            // interrupting
            Assert.AreEqual(1, taskQuery.Count());

            // check if execution still exists because timer start event is non
            // interrupting
            Assert.AreEqual(1, executionQuery.Count());

            // execute second timer job
            managementService.ExecuteJob(orderedJobList[1].Id);
            Assert.AreEqual(0, jobQuery.Count());

            Assert.AreEqual(true, DummyServiceTask.wasExecuted);

            // check if IUser task still exists because timer start event is non
            // interrupting
            Assert.AreEqual(1, taskQuery.Count());

            // check if execution still exists because timer event is non interrupting
            Assert.AreEqual(1, executionQuery.Count());

            IList<IProcessInstance> processInstanceQuery = CreateProcessInstanceQueryById(runtimeService, processInstance.Id);
            Assert.AreEqual(1, processInstanceQuery.Count());

        }

        [Test]
        [Deployment]
        public virtual void testStartTimerEventSubProcessWithUserTask()
        {
            // start process instance
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("startTimerEventSubProcessWithUserTask");

            // check if User task exists
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id);
            Assert.AreEqual(1, taskQuery.Count());

            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(2, jobQuery.Count());
            // get all timer jobs ordered by dueDate
            IList<IJob> orderedJobList = jobQuery.OrderBy(m=>m.Duedate)/*.OrderByJobDuedate()*//*.Asc()*/.ToList();
            // execute first timer job
            managementService.ExecuteJob(orderedJobList[0].Id);
            Assert.AreEqual(0, jobQuery.Count());

            // check if IUser task of event subprocess named "subProcess" exists
            Assert.AreEqual(1, taskQuery.Count());
            Assert.AreEqual("subprocessUserTask", taskQuery.First().TaskDefinitionKey);

            // check if process instance exists because subprocess named "subProcess" is
            // already running
            IList<IProcessInstance> processInstanceQuery = CreateProcessInstanceQueryById(runtimeService, processInstance.Id);
            Assert.AreEqual(1, processInstanceQuery.Count());

        }

        [Test]//TODO ExecuteJob
        [Deployment(new string[] { "resources/bpmn/event/timer/simpleProcessWithCallActivity.bpmn20.xml", "resources/bpmn/event/timer/StartTimerEventTest.testStartTimerEventWithTwoEventSubProcesses.bpmn20.xml" })]
        public virtual void testStartTimerEventSubProcessCalledFromCallActivity()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["calledProcess"] = "startTimerEventWithTwoEventSubProcesses";
            // start process instance
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("simpleCallActivityProcess", variables);

            // check if execution exists
            IQueryable<IExecution> executionQuery = runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == processInstance.Id);
            Assert.AreEqual(2, executionQuery.Count());

            // check if IUser task exists
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            Assert.AreEqual(1, taskQuery.Count());

            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(2, jobQuery.Count());
            // get all timer jobs ordered by dueDate
            IList<IJob> orderedJobList = jobQuery/*.OrderByJobDuedate()*//*.Asc()*/.ToList();
            // execute first timer job
            managementService.ExecuteJob(orderedJobList[0].Id);
            Assert.AreEqual(0, jobQuery.Count());

            // check if User task doesn't exist because timer start event is
            // interrupting
            Assert.AreEqual(0, taskQuery.Count());

            // check if execution doesn't exist because timer start event is
            // interrupting
            Assert.AreEqual(0, executionQuery.Count());

            // check if process instance doesn't exist because timer start event is
            // interrupting
            IList<IProcessInstance> processInstanceQuery = CreateProcessInstanceQueryById(runtimeService,processInstance.Id);
            Assert.AreEqual(0, processInstanceQuery.Count());

        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/timer/simpleProcessWithCallActivity.bpmn20.xml", "resources/bpmn/event/timer/StartTimerEventTest.testNonInterruptingStartTimerEventWithTwoEventSubProcesses.bpmn20.xml" })]
        public virtual void testNonInterruptingStartTimerEventSubProcessesCalledFromCallActivity()
        {
            DummyServiceTask.wasExecuted = false;

            // start process instance
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("nonInterruptingStartTimerEventWithTwoEventSubProcesses");

            // check if execution exists
            IQueryable<IExecution> executionQuery = runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == processInstance.Id);
            Assert.AreEqual(1, executionQuery.Count());

            // check if IUser task exists
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id);
            Assert.AreEqual(1, taskQuery.Count());

            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(2, jobQuery.Count());
            // get all timer jobs ordered by dueDate
            IList<IJob> orderedJobList = jobQuery/*.OrderByJobDuedate()*//*.Asc()*/.ToList();
            // execute first timer job
            managementService.ExecuteJob(orderedJobList[0].Id);
            Assert.AreEqual(1, jobQuery.Count());

            Assert.AreEqual(true, DummyServiceTask.wasExecuted);

            DummyServiceTask.wasExecuted = false;

            // check if IUser task still exists because timer start event is non
            // interrupting
            Assert.AreEqual(1, taskQuery.Count());

            // check if execution still exists because timer start event is non
            // interrupting
            Assert.AreEqual(1, executionQuery.Count());

            // execute second timer job
            managementService.ExecuteJob(orderedJobList[1].Id);
            Assert.AreEqual(0, jobQuery.Count());

            Assert.AreEqual(true, DummyServiceTask.wasExecuted);

            // check if IUser task still exists because timer start event is non
            // interrupting
            Assert.AreEqual(1, taskQuery.Count());

            // check if execution still exists because timer event is non interrupting
            Assert.AreEqual(1, executionQuery.Count());

            IQueryable<IProcessInstance> processInstanceQuery = runtimeService.CreateProcessInstanceQuery(c => c.ProcessInstanceId == processInstance.Id);
            Assert.AreEqual(1, processInstanceQuery.Count());

        }

        [Test]
        [Deployment]
        public virtual void testStartTimerEventSubProcessInMultiInstanceSubProcess()
        {
            DummyServiceTask.wasExecuted = false;

            // start process instance
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("startTimerEventSubProcessInMultiInstanceSubProcess");

            // check if IUser task exists
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            Assert.AreEqual(1, taskQuery.Count());

            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(1, jobQuery.Count());
            string jobIdFirstLoop = jobQuery.First().Id;
            // execute timer job
            managementService.ExecuteJob(jobIdFirstLoop);

            Assert.AreEqual(true, DummyServiceTask.wasExecuted);
            DummyServiceTask.wasExecuted = false;

            // execute multiInstance loop number 2
            Assert.AreEqual(1, taskQuery.Count());
            Assert.AreEqual(1, jobQuery.Count());
            string jobIdSecondLoop = jobQuery.First().Id;
            Assert.AreNotSame(jobIdFirstLoop, jobIdSecondLoop);

            // execute timer job
            managementService.ExecuteJob(jobIdSecondLoop);

            Assert.AreEqual(true, DummyServiceTask.wasExecuted);

            // multiInstance loop finished
            Assert.AreEqual(0, jobQuery.Count());

            // check if IUser task doesn't exist because timer start event is
            // interrupting
            Assert.AreEqual(0, taskQuery.Count());

            // check if process instance doesn't exist because timer start event is
            // interrupting
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingStartTimerEventInMultiInstanceEventSubProcess()
        {
            DummyServiceTask.wasExecuted = false;

            // start process instance
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("nonInterruptingStartTimerEventInMultiInstanceEventSubProcess");

            // execute multiInstance loop number 1

            // check if IUser task exists
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            Assert.AreEqual(1, taskQuery.Count());

            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(1, jobQuery.Count());
            string jobIdFirstLoop = jobQuery.First().Id;
            // execute timer job
            managementService.ExecuteJob(jobIdFirstLoop);

            Assert.AreEqual(true, DummyServiceTask.wasExecuted);
            DummyServiceTask.wasExecuted = false;

            Assert.AreEqual(1, taskQuery.Count());
            // complete existing task to start new execution for multi instance loop
            // number 2
            taskService.Complete(taskQuery.First().Id);

            // execute multiInstance loop number 2
            Assert.AreEqual(1, taskQuery.Count());
            Assert.AreEqual(1, jobQuery.Count());
            string jobIdSecondLoop = jobQuery.First().Id;
            Assert.AreNotSame(jobIdFirstLoop, jobIdSecondLoop);
            // execute timer job
            managementService.ExecuteJob(jobIdSecondLoop);

            Assert.AreEqual(true, DummyServiceTask.wasExecuted);

            // multiInstance loop finished
            Assert.AreEqual(0, jobQuery.Count());

            // check if IUser task doesn't exist because timer start event is
            // interrupting
            Assert.AreEqual(1, taskQuery.Count());

            // check if process instance doesn't exist because timer start event is
            // interrupting
            IList<IProcessInstance> processInstanceQuery = CreateProcessInstanceQueryById(runtimeService,processInstance.Id);
            Assert.AreEqual(1, processInstanceQuery.Count());

        }

        [Test]
        [Deployment]
        public virtual void testStartTimerEventSubProcessInParallelMultiInstanceSubProcess()
        {
            DummyServiceTask.wasExecuted = false;

            // start process instance
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("startTimerEventSubProcessInParallelMultiInstanceSubProcess");

            // check if execution exists
            IQueryable<IExecution> executionQuery = runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == processInstance.Id);
            Assert.AreEqual(6, executionQuery.Count());

            // check if IUser task exists
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            Assert.AreEqual(2, taskQuery.Count());

            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(2, jobQuery.Count());
            // execute timer job
            foreach (IJob job in jobQuery.ToList())
            {
                managementService.ExecuteJob(job.Id);

                Assert.AreEqual(true, DummyServiceTask.wasExecuted);
                DummyServiceTask.wasExecuted = false;
            }

            // check if IUser task doesn't exist because timer start event is
            // interrupting
            Assert.AreEqual(0, taskQuery.Count());

            // check if execution doesn't exist because timer start event is
            // interrupting
            Assert.AreEqual(0, executionQuery.Count());

            // check if process instance doesn't exist because timer start event is
            // interrupting
            IList<IProcessInstance> processInstanceQuery = CreateProcessInstanceQueryById(runtimeService,processInstance.Id);
            Assert.AreEqual(0, processInstanceQuery.Count());

        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingStartTimerEventSubProcessWithParallelMultiInstance()
        {
            DummyServiceTask.wasExecuted = false;

            // start process instance
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("nonInterruptingParallelMultiInstance");

            // check if execution exists
            IQueryable<IExecution> executionQuery = runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == processInstance.Id);
            Assert.AreEqual(6, executionQuery.Count());

            // check if IUser task exists
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            Assert.AreEqual(2, taskQuery.Count());

            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(2, jobQuery.Count());
            // execute all timer jobs
            foreach (IJob job in jobQuery.ToList())
            {
                managementService.ExecuteJob(job.Id);

                Assert.AreEqual(true, DummyServiceTask.wasExecuted);
                DummyServiceTask.wasExecuted = false;
            }

            Assert.AreEqual(0, jobQuery.Count());

            // check if IUser task doesn't exist because timer start event is
            // interrupting
            Assert.AreEqual(2, taskQuery.Count());

            // check if execution doesn't exist because timer start event is
            // interrupting
            Assert.AreEqual(6, executionQuery.Count());

            // check if process instance doesn't exist because timer start event is
            // interrupting
            IList<IProcessInstance> processInstanceQuery = CreateProcessInstanceQueryById(runtimeService,processInstance.Id);
            Assert.AreEqual(1, processInstanceQuery.Count());

        }

        [Test]
        [Deployment] //TODO ExecuteJob
        public virtual void testStartTimerEventSubProcessInMultiInstanceSubProcessWithNonInterruptingBoundaryTimerEvent()
        {
            DummyServiceTask.wasExecuted = false;

            // start process instance
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            // check if IUser task exists
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            Assert.AreEqual(1, taskQuery.Count());

            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            // 1 start timer job and 1 boundary timer job
            Assert.AreEqual(2, jobQuery.Count());
            // execute interrupting start timer event subprocess job
            managementService.ExecuteJob(jobQuery.OrderBy(m=>m.Duedate)/*.OrderByJobDuedate()*//*.Asc()*/.ToList().ElementAt(1).Id);

            Assert.AreEqual(true, DummyServiceTask.wasExecuted);

            // after first interrupting start timer event sub process execution
            // multiInstance loop number 2
            Assert.AreEqual(1, taskQuery.Count());
            Assert.AreEqual(2, jobQuery.Count());

            // execute non interrupting boundary timer job
            managementService.ExecuteJob(jobQuery.OrderBy(m=>m.Duedate)/*.OrderByJobDuedate()*//*.Asc()*/.First().Id);

            // after non interrupting boundary timer job execution
            Assert.AreEqual(1, jobQuery.Count());
            Assert.AreEqual(1, taskQuery.Count());
            IList<IProcessInstance> processInstanceQuery = CreateProcessInstanceQueryById(runtimeService,processInstance.Id);
            Assert.AreEqual(1, processInstanceQuery.Count());

        }

        [Test]
        [Deployment]
        public virtual void testStartTimerEventSubProcessInMultiInstanceSubProcessWithInterruptingBoundaryTimerEvent()
        {
            DummyServiceTask.wasExecuted = false;

            // start process instance
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            // execute multiInstance loop number 1
            // check if execution exists

            // check if IUser task exists
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            Assert.AreEqual(1, taskQuery.Count());

            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            // 1 start timer job and 1 boundary timer job
            Assert.AreEqual(2, jobQuery.Count());
            // execute interrupting start timer event subprocess job
            managementService.ExecuteJob(jobQuery.OrderBy(m=>m.Duedate)/*.OrderByJobDuedate()*//*.Asc()*/.ToList().ElementAt(1).Id);

            Assert.AreEqual(true, DummyServiceTask.wasExecuted);

            // after first interrupting start timer event sub process execution
            // multiInstance loop number 2
            Assert.AreEqual(1, taskQuery.Count());
            Assert.AreEqual(2, jobQuery.Count());

            // execute interrupting boundary timer job
            managementService.ExecuteJob(jobQuery.OrderBy(m => m.Duedate)/*.OrderByJobDuedate()*//*.Asc()*/.First().Id);

            // after interrupting boundary timer job execution
            Assert.AreEqual(0, jobQuery.Count());
            Assert.AreEqual(0, taskQuery.Count());

            AssertProcessEnded(processInstance.Id);

        }

        [Test]
        [Deployment("resources/api/bpmn/event/timer/StartTimerEventTest.testNonInterruptingStartTimerEventSubProcessInMultiInstanceSubProcessWithInterruptingBoundaryTE.bpmn20.xml")]
        public virtual void testNonInterruptingStartTimerEventSubProcessInMultiInstanceSubProcessWithInterruptingBoundaryTimerEvent()
        {
            DummyServiceTask.wasExecuted = false;

            // start process instance
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            // execute multiInstance loop number 1
            // check if execution exists
            IQueryable<IExecution> executionQuery = runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == processInstance.Id);
            Assert.AreEqual(3, executionQuery.Count());

            // check if IUser task exists
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            Assert.AreEqual(1, taskQuery.Count());

            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            // 1 start timer job and 1 boundary timer job
            Assert.AreEqual(2, jobQuery.Count());
            // execute non interrupting start timer event subprocess job
            managementService.ExecuteJob(jobQuery.OrderBy(m=>m.Duedate)/*.OrderByJobDuedate()*//*.Asc()*/.ToList().ElementAt(1).Id);

            Assert.AreEqual(true, DummyServiceTask.wasExecuted);

            // complete IUser task to finish execution of first multiInstance loop
            Assert.AreEqual(1, taskQuery.Count());
            taskService.Complete(taskQuery.First().Id);

            // after first non interrupting start timer event sub process execution
            // multiInstance loop number 2
            Assert.AreEqual(1, taskQuery.Count());
            Assert.AreEqual(2, jobQuery.Count());

            // execute interrupting boundary timer job
            managementService.ExecuteJob(jobQuery.OrderBy(m => m.Duedate)/*.OrderByJobDuedate()*//*.Asc()*/.First().Id);

            // after interrupting boundary timer job execution
            Assert.AreEqual(0, jobQuery.Count());
            Assert.AreEqual(0, taskQuery.Count());
            Assert.AreEqual(0, executionQuery.Count());
            IList<IProcessInstance> processInstanceQuery = CreateProcessInstanceQueryById(runtimeService, processInstance.Id);
            Assert.AreEqual(0, processInstanceQuery.Count());

        }

        [Test]
        [Deployment]
        public virtual void testStartTimerEventSubProcessInParallelMultiInstanceSubProcessWithNonInterruptingBoundaryTimerEvent()
        {
            DummyServiceTask.wasExecuted = false;

            // start process instance
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            // execute multiInstance loop number 1
            // check if execution exists
            IQueryable<IExecution> executionQuery = runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == processInstance.Id);
            Assert.AreEqual(6, executionQuery.Count());

            // check if IUser task exists
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            Assert.AreEqual(2, taskQuery.Count());

            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(3, jobQuery.Count());

            // execute interrupting timer job
            managementService.ExecuteJob(jobQuery.OrderBy(m=>m.Duedate)/*.OrderByJobDuedate()*//*.Asc()*/.ToList().ElementAt(1).Id);

            Assert.AreEqual(true, DummyServiceTask.wasExecuted);

            // after interrupting timer job execution
            Assert.AreEqual(2, jobQuery.Count());
            Assert.AreEqual(1, taskQuery.Count());
            Assert.AreEqual(5, executionQuery.Count());

            // execute non interrupting boundary timer job
            managementService.ExecuteJob(jobQuery.OrderBy(m => m.Duedate)/*.OrderByJobDuedate()*//*.Asc()*/.First().Id);

            // after non interrupting boundary timer job execution
            Assert.AreEqual(1, jobQuery.Count());
            Assert.AreEqual(1, taskQuery.Count());
            Assert.AreEqual(5, executionQuery.Count());

            IList<IProcessInstance> processInstanceQuery = CreateProcessInstanceQueryById(runtimeService,processInstance.Id);
            Assert.AreEqual(1, processInstanceQuery.Count());

        }

        [Test]
        [Deployment]
        public virtual void testStartTimerEventSubProcessInParallelMultiInstanceSubProcessWithInterruptingBoundaryTimerEvent()
        {
            // start process instance
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            // execute multiInstance loop number 1
            // check if execution exists
            IQueryable<IExecution> executionQuery = runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == processInstance.Id);
            Assert.AreEqual(6, executionQuery.Count());

            // check if IUser task exists
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            Assert.AreEqual(2, taskQuery.Count());

            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(3, jobQuery.Count());

            // execute interrupting timer job
            managementService.ExecuteJob(jobQuery.OrderBy(m=>m.Duedate)/*.OrderByJobDuedate()*//*.Asc()*/.ToList().ElementAt(1).Id);

            // after interrupting timer job execution
            Assert.AreEqual(2, jobQuery.Count());
            Assert.AreEqual(1, taskQuery.Count());
            Assert.AreEqual(5, executionQuery.Count());

            // execute interrupting boundary timer job
            managementService.ExecuteJob(jobQuery.OrderBy(m => m.Duedate)/*.OrderByJobDuedate()*//*.Asc()*/.First().Id);

            // after interrupting boundary timer job execution
            Assert.AreEqual(0, jobQuery.Count());
            Assert.AreEqual(0, taskQuery.Count());
            Assert.AreEqual(0, executionQuery.Count());

            AssertProcessEnded(processInstance.Id);

        }

        [Test]
        [Deployment("resources/api/bpmn/event/timer/StartTimerEventTest.testNonInterruptingStartTimerEventSubProcessInParallelMiSubProcessWithInterruptingBoundaryTE.bpmn20.xml")]
        public virtual void testNonInterruptingStartTimerEventSubProcessInParallelMiSubProcessWithInterruptingBoundaryTimerEvent()
        {
            DummyServiceTask.wasExecuted = false;

            // start process instance
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            // execute multiInstance loop number 1
            // check if execution exists
            IQueryable<IExecution> executionQuery = runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == processInstance.Id);
            Assert.AreEqual(6, executionQuery.Count());

            // check if IUser task exists
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
            Assert.AreEqual(2, taskQuery.Count());

            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(3, jobQuery.Count());

            // execute non interrupting timer job
            managementService.ExecuteJob(jobQuery.OrderBy(m=>m.Duedate)/*.OrderByJobDuedate()*//*.Asc()*/.ToList().ElementAt(1).Id);

            Assert.AreEqual(true, DummyServiceTask.wasExecuted);

            // after non interrupting timer job execution
            Assert.AreEqual(2, jobQuery.Count());
            Assert.AreEqual(2, taskQuery.Count());
            Assert.AreEqual(6, executionQuery.Count());

            // execute interrupting boundary timer job
            managementService.ExecuteJob(jobQuery.OrderBy(m=>m.Duedate)/*.OrderByJobDuedate()*//*.Asc()*/.First().Id);

            // after interrupting boundary timer job execution
            Assert.AreEqual(0, jobQuery.Count());
            Assert.AreEqual(0, taskQuery.Count());
            Assert.AreEqual(0, executionQuery.Count());

            AssertProcessEnded(processInstance.Id);

            // start process instance again and
            // test if boundary events deleted after all tasks are completed
            processInstance = runtimeService.StartProcessInstanceByKey("process");
            jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(3, jobQuery.Count());

            Assert.AreEqual(2, taskQuery.Count());
            // complete all existing tasks
            foreach (ITask task in taskQuery.ToList())
            {
                taskService.Complete(task.Id);
            }

            Assert.AreEqual(0, jobQuery.Count());
            Assert.AreEqual(0, taskQuery.Count());
            Assert.AreEqual(0, executionQuery.Count());

            AssertProcessEnded(processInstance.Id);

        }

        [Test]
        [Deployment]
        public virtual void testTimeCycle()
        {
            // given
            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(1, jobQuery.Count());

            string JobId = jobQuery.First().Id;

            // when
            managementService.ExecuteJob(JobId);

            // then
            Assert.AreEqual(1, jobQuery.Count());

            string anotherJobId = jobQuery.First().Id;
            Assert.IsFalse(JobId.Equals(anotherJobId));
        }

        [Test]
        [Deployment]//TODO ExecuteJob
        public virtual void testFailingTimeCycle()
        {
            // given
            IQueryable<IJob> query = managementService.CreateJobQuery();
            IQueryable<IJob> failedJobQuery = managementService.CreateJobQuery();

            // a job to start a process instance
            Assert.AreEqual(1, query.Count());

            string JobId = query.First().Id;
            failedJobQuery.Where(c => c.Id == JobId);

            moveByMinutes(5);

            // when (1)
            try
            {
                managementService.ExecuteJob(JobId);
            }
            catch (System.Exception)
            {
                // expected
            }

            // then (1)
            IJob failedJob = failedJobQuery.First();
            Assert.AreEqual(2, failedJob.Retries);

            // a new timer job has been created
            Assert.AreEqual(2, query.Count());

            Assert.AreEqual(1, managementService.CreateJobQuery()/*.SetWithException()*/.Count());
            Assert.AreEqual(0, managementService.CreateJobQuery(c => c.Retries == 0).Count());
            Assert.AreEqual(2, managementService.CreateJobQuery(c => c.Retries > 0).Count());

            // when (2)
            try
            {
                managementService.ExecuteJob(JobId);
            }
            catch (System.Exception)
            {
                // expected
            }

            // then (2)
            failedJob = failedJobQuery.First();
            Assert.AreEqual(1, failedJob.Retries);

            // there are still two jobs
            Assert.AreEqual(2, query.Count());

            Assert.AreEqual(1, managementService.CreateJobQuery()/*.SetWithException()*/.Count());
            Assert.AreEqual(0, managementService.CreateJobQuery(c => c.Retries == 0).Count());
            Assert.AreEqual(2, managementService.CreateJobQuery(c => c.Retries > 0).Count());
        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingTimeCycleInEventSubProcess()
        {
            // given
            runtimeService.StartProcessInstanceByKey("process");

            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(1, jobQuery.Count());

            string JobId = jobQuery.First().Id;

            // when
            managementService.ExecuteJob(JobId);

            // then
            Assert.AreEqual(1, jobQuery.Count());

            string anotherJobId = jobQuery.First().Id;
            Assert.IsFalse(JobId.Equals(anotherJobId));
        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingFailingTimeCycleInEventSubProcess()
        {
            // given
            runtimeService.StartProcessInstanceByKey("process");

            IQueryable<IJob> failedJobQuery = managementService.CreateJobQuery();
            IQueryable<IJob> jobQuery = managementService.CreateJobQuery();

            Assert.AreEqual(1, jobQuery.Count());
            string JobId = jobQuery.First().Id;

            failedJobQuery.Where(c => c.Id == JobId);

            // when (1)
            try
            {
                managementService.ExecuteJob(JobId);
                Assert.Fail();
            }
            catch (System.Exception)
            {
                // expected
            }

            // then (1)
            IJob failedJob = failedJobQuery.First();
            Assert.AreEqual(2, failedJob.Retries);

            // a new timer job has been created
            Assert.AreEqual(2, jobQuery.Count());

            Assert.AreEqual(1, managementService.CreateJobQuery(m=>m.ExceptionMessage!=null||m.ExceptionByteArrayId != null)/*.SetWithException()*/.Count());
            Assert.AreEqual(0, managementService.CreateJobQuery(c => c.RetriesFromPersistence == 0).Count());
            Assert.AreEqual(2, managementService.CreateJobQuery(c => c.RetriesFromPersistence > 0).Count());

            // when (2)
            try
            {
                managementService.ExecuteJob(JobId);
            }
            catch (System.Exception)
            {
                // expected
            }

            // then (2)
            failedJob = failedJobQuery.First();
            Assert.AreEqual(1, failedJob.Retries);

            // there are still two jobs
            Assert.AreEqual(2, jobQuery.Count());

            Assert.AreEqual(1, managementService.CreateJobQuery()/*.SetWithException()*/.Count());
            Assert.AreEqual(0, managementService.CreateJobQuery(c => c.Retries == 0).Count());
            Assert.AreEqual(2, managementService.CreateJobQuery(c => c.Retries > 0).Count());
        }

        // util methods ////////////////////////////////////////

        /// <summary>
        /// executes all jobs in this threads until they are either done or retries are
        /// exhausted.
        /// </summary>
        protected internal virtual void executeAllJobs()
        {
            string nextJobId = NextExecutableJobId;

            while (!string.ReferenceEquals(nextJobId, null))
            {
                try
                {
                    managementService.ExecuteJob(nextJobId);
                }
                catch (System.Exception)
                { // ignore
                }
                nextJobId = NextExecutableJobId;
            }

        }

        protected internal virtual string NextExecutableJobId
        {
            get
            {
                IList<IJob> jobs = managementService.CreateJobQuery().ToList()/*.Executable()*//*.GetListPage(0, 1)*/;
                if (jobs.Count == 1)
                {
                    return jobs[0].Id;
                }
                else
                {
                    return null;
                }
            }
        }

        private void cleanDB()
        {
            string JobId = managementService.CreateJobQuery().First().Id;
            ICommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
            commandExecutor.Execute(new DeleteJobsCmd(JobId, true));
        }

    }

}