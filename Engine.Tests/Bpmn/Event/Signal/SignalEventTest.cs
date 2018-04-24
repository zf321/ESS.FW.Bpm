using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Engine.Tests.Bpmn.ExecutionListener;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Signal
{
    [TestFixture]
    public class SignalEventTest : PluggableProcessEngineTestCase
    {
        public SignalEventTest()
        {
            ClearDeploymentAll = true;
        }
        protected IQueryable<ExecutionEntity> CreateProcessInstanceQuery(IRuntimeService runtimeService)
        {
            DbContext db = runtimeService.GetDbContext();
            var query = from a in db.Set<ExecutionEntity>()
                        join b in db.Set<ProcessDefinitionEntity>() on a.ProcessDefinitionId equals b.Id
                        where a.ParentId == null
                        select a;
            return query;
        }
        [Test]
        [Deployment(new string[] { "resources/bpmn/event/signal/SignalEventTests.catchAlertSignal.bpmn20.xml", "resources/bpmn/event/signal/SignalEventTests.throwAlertSignal.bpmn20.xml" })]
        public virtual void testSignalCatchIntermediate()
        {
            runtimeService.StartProcessInstanceByKey("catchSignal");

            Assert.AreEqual(1, CreateEventSubscriptionQuery().Count);
            //select count(distinct RES.ID_) from ACT_RU_EXECUTION RES inner join ACT_RE_PROCDEF P on RES.PROC_DEF_ID_ = P.ID_ WHERE RES.PARENT_ID_ is null
            //Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery().Count());
            DbContext db = runtimeService.GetDbContext();
            var query = from a in db.Set<ExecutionEntity>()
                        join b in db.Set<ProcessDefinitionEntity>() on a.ProcessDefinitionId equals b.Id
                        where a.ParentId == null
                        select a;
            Assert.AreEqual(1, query.Count());
            runtimeService.StartProcessInstanceByKey("throwSignal");
            Assert.AreEqual(0, CreateEventSubscriptionQuery().Count());
            Assert.AreEqual(0, query.Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/signal/SignalEventTests.catchAlertSignalBoundary.bpmn20.xml", "resources/bpmn/event/signal/SignalEventTests.throwAlertSignal.bpmn20.xml" })]
        public virtual void testSignalCatchBoundary()
        {
            DbContext db = runtimeService.GetDbContext();
            var query = from a in db.Set<ExecutionEntity>()
                        join b in db.Set<ProcessDefinitionEntity>() on a.ProcessDefinitionId equals b.Id
                        where a.ParentId == null
                        select a;
            runtimeService.StartProcessInstanceByKey("catchSignal");

            Assert.AreEqual(1, CreateEventSubscriptionQuery().Count());
            Assert.AreEqual(1, query.Count());

            runtimeService.StartProcessInstanceByKey("throwSignal");

            Assert.AreEqual(0, CreateEventSubscriptionQuery().Count());
            Assert.AreEqual(0, CreateProcessInstanceQuery(runtimeService).Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/signal/SignalEventTests.catchAlertSignalBoundaryWithReceiveTask.bpmn20.xml", "resources/bpmn/event/signal/SignalEventTests.throwAlertSignal.bpmn20.xml" })]
        public virtual void testSignalCatchBoundaryWithVariables()
        {
            Dictionary<string, ITypedValue> variables1 = new Dictionary<string, ITypedValue>();
            variables1["processName"] =new StringValueImpl("catchSignal");
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("catchSignal", variables1);

            Dictionary<string, ITypedValue> variables2 = new Dictionary<string, ITypedValue>();
            variables2["processName"] = new StringValueImpl("throwSignal");
            runtimeService.StartProcessInstanceByKey("throwSignal", variables2);

            Assert.AreEqual("catchSignal", runtimeService.GetVariable(pi.Id, "processName"));
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/signal/SignalEventTests.catchAlertSignal.bpmn20.xml", "resources/bpmn/event/signal/SignalEventTests.throwAlertSignalAsynch.bpmn20.xml" })]
        public virtual void testSignalCatchIntermediateAsynch()
        {

            runtimeService.StartProcessInstanceByKey("catchSignal");

            Assert.AreEqual(1, CreateEventSubscriptionQuery().Count());
            DbContext db = runtimeService.GetDbContext();
            var query = from a in db.Set<ExecutionEntity>()
                        join b in db.Set<ProcessDefinitionEntity>() on a.ProcessDefinitionId equals b.Id
                        where a.ParentId == null
                        select a;
            Assert.AreEqual(1, query.Count());

            runtimeService.StartProcessInstanceByKey("throwSignal");

            Assert.AreEqual(1, CreateEventSubscriptionQuery().Count());
            Assert.AreEqual(1, query.Count());

            // there is a job:
            Assert.AreEqual(1, managementService.CreateJobQuery().Count());

            try
            {
                ClockUtil.CurrentTime = DateTime.Now.AddMilliseconds(1000); // new DateTime(DateTimeHelperClass.CurrentUnixTimeMillis() + 1000);
                WaitForJobExecutorToProcessAllJobs(10000);

                Assert.AreEqual(0, CreateEventSubscriptionQuery().Count());
                Assert.AreEqual(0, query.Count());
                Assert.AreEqual(0, managementService.CreateJobQuery().Count());
            }
            finally
            {
                ClockUtil.CurrentTime = DateTime.Now;
            }

        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/signal/SignalEventTests.catchMultipleSignals.bpmn20.xml", "resources/bpmn/event/signal/SignalEventTests.throwAlertSignal.bpmn20.xml", "resources/bpmn/event/signal/SignalEventTests.throwAbortSignal.bpmn20.xml" })]
        public virtual void testSignalCatchDifferentSignals()
        {

            runtimeService.StartProcessInstanceByKey("catchSignal");

            Assert.AreEqual(2, CreateEventSubscriptionQuery().Count());
            Assert.AreEqual(1, CreateProcessInstanceQuery(runtimeService).Count());

            runtimeService.StartProcessInstanceByKey("throwAbort");

            Assert.AreEqual(1, CreateEventSubscriptionQuery().Count());
            Assert.AreEqual(1, CreateProcessInstanceQuery(runtimeService).Count());

            ITask taskAfterAbort = taskService.CreateTaskQuery(c=>c.AssigneeWithoutCascade/*.Assignee */== "gonzo").First();
            Assert.NotNull(taskAfterAbort);
            taskService.Complete(taskAfterAbort.Id);

            runtimeService.StartProcessInstanceByKey("throwSignal");

            Assert.AreEqual(0, CreateEventSubscriptionQuery().Count());
            Assert.AreEqual(0, CreateProcessInstanceQuery(runtimeService).Count());
        }

        [Test]
        [Deployment]
        public virtual void testSignalBoundaryOnSubProcess()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("signalEventOnSubprocess");
            runtimeService.SignalEventReceived("stopSignal");
            AssertProcessEnded(pi.ProcessInstanceId);
        }

        private IList<EventSubscriptionEntity> CreateEventSubscriptionQuery()
        {
            //return processEngineConfiguration.CommandExecutorTxRequired.Execute(new CreateQueryCmd<EventSubscriptionEntity>(null));
            return runtimeService.GetDbContext().Set<EventSubscriptionEntity>().ToList();
        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingSignal()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("nonInterruptingSignalEvent");

            IList<ITask> tasks = taskService.CreateTaskQuery(c=>c.ProcessInstanceId == pi.ProcessInstanceId).ToList();
            Assert.AreEqual(1, tasks.Count);
            ITask currentTask = tasks[0];
            Assert.AreEqual("My User Task", currentTask.Name);

            runtimeService.SignalEventReceived("alert");

            tasks = taskService.CreateTaskQuery(c=>c.ProcessInstanceId == pi.ProcessInstanceId).ToList();
            Assert.AreEqual(2, tasks.Count);

            foreach (ITask task in tasks)
            {
                if (!task.Name.Equals("My User Task") && !task.Name.Equals("My Second User Task"))
                {
                    Assert.Fail("Expected: <My User Task> or <My Second User Task> but was <" + task.Name + ">.");
                }
            }

            taskService.Complete(taskService.CreateTaskQuery(c=>c.NameWithoutCascade/*.Name */=="My User Task").First().Id);

            tasks = taskService.CreateTaskQuery(c=>c.ProcessInstanceId == pi.ProcessInstanceId).ToList();
            Assert.AreEqual(1, tasks.Count);
            currentTask = tasks[0];
            Assert.AreEqual("My Second User Task", currentTask.Name);
        }


        [Test]
        [Deployment]
        public virtual void testNonInterruptingSignalWithSubProcess()
        {
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("nonInterruptingSignalWithSubProcess");
            IList<ITask> tasks = taskService.CreateTaskQuery(c=>c.ProcessInstanceId == pi.ProcessInstanceId).ToList();
            Assert.AreEqual(1, tasks.Count);

            ITask currentTask = tasks[0];
            Assert.AreEqual("Approve", currentTask.Name);

            runtimeService.SignalEventReceived("alert");

            tasks = taskService.CreateTaskQuery(c=>c.ProcessInstanceId == pi.ProcessInstanceId).ToList();
            Assert.AreEqual(2, tasks.Count);

            foreach (ITask task in tasks)
            {
                if (!task.Name.Equals("Approve") && !task.Name.Equals("Review"))
                {
                    Assert.Fail("Expected: <Approve> or <Review> but was <" + task.Name + ">.");
                }
            }

            taskService.Complete(taskService.CreateTaskQuery(c=>c.NameWithoutCascade/*.Name */=="Approve").First().Id);

            tasks = taskService.CreateTaskQuery(c=>c.ProcessInstanceId == pi.ProcessInstanceId).ToList();
            Assert.AreEqual(1, tasks.Count);

            currentTask = tasks[0];
            Assert.AreEqual("Review", currentTask.Name);

            taskService.Complete(taskService.CreateTaskQuery(c=>c.NameWithoutCascade/*.Name */=="Review").First().Id);

            tasks = taskService.CreateTaskQuery(c=>c.ProcessInstanceId == pi.ProcessInstanceId).ToList();
            Assert.AreEqual(1, tasks.Count);

        }

        [Test]
        [Deployment]
        public virtual void testSignalStartEventInEventSubProcess()
        {
            // start process instance
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("signalStartEventInEventSubProcess");

            // check if execution exists
            IQueryable<IExecution> executionQuery = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==processInstance.Id);
            Assert.AreEqual(1, executionQuery.Count());

            // check if User task exists
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id);
            Assert.AreEqual(1, taskQuery.Count());

            // send interrupting signal to event sub process
            runtimeService.SignalEventReceived("alert");

            Assert.AreEqual(true, DummyServiceTask.wasExecuted);

            // check if IUser task doesn't exist because signal start event is interrupting
            taskQuery = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id);
            Assert.AreEqual(0, taskQuery.Count());

            // check if execution doesn't exist because signal start event is interrupting
            executionQuery = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==processInstance.Id);
            Assert.AreEqual(0, executionQuery.Count());
        }

        [Test]
        [Deployment]
        public virtual void testNonInterruptingSignalStartEventInEventSubProcess()
        {
            // start process instance
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("nonInterruptingSignalStartEventInEventSubProcess");

            // check if execution exists
            IQueryable<IExecution> executionQuery = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==processInstance.Id);
            Assert.AreEqual(1, executionQuery.Count());

            // check if IUser task exists
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id);
            Assert.AreEqual(1, taskQuery.Count());

            // send non interrupting signal to event sub process
            runtimeService.SignalEventReceived("alert");

            Assert.AreEqual(true, DummyServiceTask.wasExecuted);

            // check if IUser task still exists because signal start event is non interrupting
            taskQuery = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id);
            Assert.AreEqual(1, taskQuery.Count());

            // check if execution still exists because signal start event is non interrupting
            executionQuery = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==processInstance.Id);
            Assert.AreEqual(1, executionQuery.Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml" })]
        public virtual void testSignalStartEvent()
        {
            // event subscription for signal start event
            Assert.AreEqual(1, runtimeService.CreateEventSubscriptionQuery(c=>c.EventType == "signal" && c.EventName == "alert").Count());

            runtimeService.SignalEventReceived("alert");
            // the signal should start a new process instance
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml" })]
        public virtual void testSuspendedProcessWithSignalStartEvent()
        {
            // event subscription for signal start event
            Assert.AreEqual(1, runtimeService.CreateEventSubscriptionQuery(c=>c.EventType == "signal" && c.EventName == "alert").Count());

            IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
            repositoryService.SuspendProcessDefinitionById(processDefinition.Id);

            runtimeService.SignalEventReceived("alert");
            // the signal should not start a process instance for the suspended process definition
            Assert.AreEqual(0, taskService.CreateTaskQuery().Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml", "resources/bpmn/event/signal/SignalEventTest.testOtherSignalStartEvent.bpmn20.xml" })]
        public virtual void testMultipleProcessesWithSameSignalStartEvent()
        {
            // event subscriptions for signal start event
            Assert.AreEqual(2, runtimeService.CreateEventSubscriptionQuery(c=>c.EventType == "signal" && c.EventName == "alert").Count());

            runtimeService.SignalEventReceived("alert");
            // the signal should start new process instances for both process definitions
            Assert.AreEqual(2, taskService.CreateTaskQuery().Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml", "resources/bpmn/event/signal/SignalEventTests.throwAlertSignal.bpmn20.xml" })]
        public virtual void testStartProcessInstanceBySignalFromIntermediateThrowingSignalEvent()
        {
            // start a process instance to throw a signal
            runtimeService.StartProcessInstanceByKey("throwSignal");
            // the signal should start a new process instance
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml", "resources/bpmn/event/signal/SignalEventTests.throwAlertSignal.bpmn20.xml" })]
        public virtual void testIntermediateThrowingSignalEventWithSuspendedSignalStartEvent()
        {
            // event subscription for signal start event
            Assert.AreEqual(1, runtimeService.CreateEventSubscriptionQuery(c=>c.EventType == "signal" && c.EventName == "alert").Count());

            IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "startBySignal").First();
            //暂停流程定义
            repositoryService.SuspendProcessDefinitionById(processDefinition.Id);

            // start a process instance to throw a signal
            runtimeService.StartProcessInstanceByKey("throwSignal");
            // the signal should not start a new process instance of the suspended process definition
            Assert.AreEqual(0, taskService.CreateTaskQuery().Count());
        }

        [Test]
        [Deployment]
        public virtual void testProcessesWithMultipleSignalStartEvents()
        {
            // event subscriptions for signal start event
            Assert.AreEqual(2, runtimeService.CreateEventSubscriptionQuery(c=>c.EventType == "signal").Count());

            runtimeService.SignalEventReceived("alert");
            // the signal should start new process instances for both process definitions
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/signal/SignalEventTests.catchAlertTwiceAndTerminate.bpmn20.xml" })]
        public virtual void testThrowSignalMultipleCancellingReceivers()
        {
            RecorderExecutionListener.Clear();

            runtimeService.StartProcessInstanceByKey("catchAlertTwiceAndTerminate");

            // event subscription for intermediate signal events
            Assert.AreEqual(2, runtimeService.CreateEventSubscriptionQuery(c=>c.EventType == "signal" && c.EventName == "alert").Count());

            // try to send 'alert' signal to both executions
            runtimeService.SignalEventReceived("alert");

            // then only one terminate end event was executed
            Assert.AreEqual(1, RecorderExecutionListener.RecordedEvents.Count);

            // and instances ended successfully
            Assert.AreEqual(0, CreateProcessInstanceQuery(runtimeService).Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/signal/SignalEventTests.catchAlertTwiceAndTerminate.bpmn20.xml", "resources/bpmn/event/signal/SignalEventTests.throwAlertSignal.bpmn20.xml" })]
        public virtual void testIntermediateThrowSignalMultipleCancellingReceivers()
        {
            RecorderExecutionListener.Clear();

            runtimeService.StartProcessInstanceByKey("catchAlertTwiceAndTerminate");

            // event subscriptions for intermediate events
            Assert.AreEqual(2, runtimeService.CreateEventSubscriptionQuery(c=>c.EventType == "signal" && c.EventName == "alert").Count());

            // started process instance try to send 'alert' signal to both executions
            runtimeService.StartProcessInstanceByKey("throwSignal");

            // then only one terminate end event was executed
            Assert.AreEqual(1, RecorderExecutionListener.RecordedEvents.Count);

            // and both instances ended successfully
            Assert.AreEqual(0, CreateProcessInstanceQuery(runtimeService).Count());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml", "resources/bpmn/event/signal/SignalEventTests.throwAlertSignalAsync.bpmn20.xml" })]
        public virtual void testAsyncSignalStartEventJobProperties()
        {
            IProcessDefinition catchingProcessDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "startBySignal").First();

            // given a process instance that throws a signal asynchronously
            runtimeService.StartProcessInstanceByKey("throwSignalAsync");
            // where the throwing instance ends immediately

            // then there is not yet a catching process instance
            Assert.AreEqual(0, CreateProcessInstanceQuery(runtimeService).Count());

            // but there is a job for the asynchronous continuation
            IJob asyncJob = managementService.CreateJobQuery().First();
            Assert.AreEqual(catchingProcessDefinition.Id, asyncJob.ProcessDefinitionId);
            Assert.AreEqual(catchingProcessDefinition.Key, asyncJob.ProcessDefinitionKey);
            Assert.IsNull(asyncJob.ExceptionMessage);
            Assert.IsNull(asyncJob.ExecutionId);
            Assert.IsNull(asyncJob.JobDefinitionId);
            Assert.AreEqual(0, asyncJob.Priority);
            Assert.IsNull(asyncJob.ProcessInstanceId);
            Assert.AreEqual(3, asyncJob.Retries);
            Assert.IsNull(asyncJob.Duedate);
            Assert.IsNull(asyncJob.DeploymentId);
        }

        [Test]//TODO 异步
        [Deployment(new string[] { "resources/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml", "resources/bpmn/event/signal/SignalEventTests.throwAlertSignalAsync.bpmn20.xml" })]
        public virtual void testAsyncSignalStartEvent()
        {
            IProcessDefinition catchingProcessDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "startBySignal").FirstOrDefault();

            // given a process instance that throws a signal asynchronously
            runtimeService.StartProcessInstanceByKey("throwSignalAsync");

            // with an async job to trigger the signal event
            IJob job = managementService.CreateJobQuery().FirstOrDefault();

            // when the job is executed
            managementService.ExecuteJob(job.Id);

            // then there is a process instance
            IProcessInstance processInstance = CreateProcessInstanceQuery(runtimeService).FirstOrDefault();
            Assert.NotNull(processInstance);
            Assert.AreEqual(catchingProcessDefinition.Id, processInstance.ProcessDefinitionId);

            // and a task
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());
        }

        //[Test]
        [Deployment]
        public virtual void FAILING_testNoContinuationWhenSignalInterruptsThrowingActivity()
        {
            // given a process instance
            runtimeService.StartProcessInstanceByKey("signalEventSubProcess");

            // when throwing a signal in the sub process that interrupts the subprocess
            ITask subProcessTask = taskService.CreateTaskQuery().First();
            taskService.Complete(subProcessTask.Id);

            // then execution should not have been continued after the subprocess
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());
            Assert.AreEqual(0, taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="afterSubProcessTask").Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="eventSubProcessTask").Count());
        }

        // Todo: ObjectInputStream
        //[Test]
        //[Deployment(new string[] { "resources/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml" })]
        //public virtual void testSetSerializedVariableValues()
        //{

        //    // when
        //    FailingJavaSerializable javaSerializable = new FailingJavaSerializable("foo");

        //    System.IO.MemoryStream baos = new System.IO.MemoryStream();
        //    (new ObjectOutputStream(baos)).writeObject(javaSerializable);
        //    string serializedObject = StringUtil.FromBytes(Base64.EncodeBase64(baos.ToArray()), ProcessEngine);

        //    // then it is not possible to deserialize the object
        //    try
        //    {
        //        (new ObjectInputStream(new System.IO.MemoryStream(baos.ToArray()))).readObject();
        //    }
        //    catch (Exception e)
        //    {
        //        AssertTextPresent("Exception while deserializing object.", e.Message);
        //    }

        //    // but it can be set as a variable when delivering a message:
        //    //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
        //    runtimeService.SignalEventReceived("alert", Variable.Variables.CreateVariables().PutValueTyped("var", Variables.SerializedObjectValue(serializedObject)
        //        .ObjectTypeName(typeof(FailingJavaSerializable).FullName).SerializationDataFormat(Variables.SerializationDataFormats.Java.ToString()).Create()));

        //    // then
        //    IProcessInstance startedInstance = runtimeService.CreateProcessInstanceQuery().First();
        //    Assert.NotNull(startedInstance);

        //    IObjectValue variableTyped = runtimeService.GetVariableTyped<IObjectValue>(startedInstance.Id, "var", false);
        //    Assert.NotNull(variableTyped);
        //    Assert.IsFalse(variableTyped.Deserialized);
        //    Assert.AreEqual(serializedObject, variableTyped.ValueSerialized);
        //    //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
        //    Assert.AreEqual(typeof(FailingJavaSerializable).FullName, variableTyped.ObjectTypeName);
        //    Assert.AreEqual(Variables.SerializationDataFormats.Java.ToString(), variableTyped.SerializationDataFormat);
        //}


        //[Test]
        [Deployment(new string[] { "resources/bpmn/event/signal/SignalEventTests.catchAlertSignalBoundary.bpmn20.xml", "resources/bpmn/event/signal/SignalEventTests.throwAlertSignalAsync.bpmn20.xml" })]
        public virtual void FAILING_testAsyncSignalBoundary()
        {
            runtimeService.StartProcessInstanceByKey("catchSignal");

            // given a process instance that throws a signal asynchronously
            runtimeService.StartProcessInstanceByKey("throwSignalAsync");

            IJob job = managementService.CreateJobQuery().FirstOrDefault();
            Assert.NotNull(job); // Throws Exception!

            // when the job is executed
            managementService.ExecuteJob(job.Id);

            // then there is a process instance
            IProcessInstance processInstance = CreateProcessInstanceQuery(runtimeService).First();
            Assert.NotNull(processInstance);
            //    Assert.AreEqual(catchingProcessDefinition.getId(), processInstance.getProcessDefinitionId());

            // and a task
            Assert.AreEqual(1, taskService.CreateTaskQuery().Count());
        }

    }

}