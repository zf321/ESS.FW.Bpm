using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;


namespace Engine.Tests.Bpmn.SubProcess
{
    

	/// <summary>
	/// 
	/// </summary>
    [TestFixture]
	public class InterruptingEventSubProcessTest : PluggableProcessEngineTestCase
	{

        [Test]
        [Deployment("resources/bpmn/event/compensate/CompensateEventTest.compensationMiActivity.bpmn20.xml")]
        public virtual void testCancelEventSubscriptionsWhenReceivingAMessage()
	  {
		IProcessInstance pi = runtimeService.StartProcessInstanceByKey("process");

		IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
		IQueryable<IEventSubscription> eventSubscriptionQuery = runtimeService.CreateEventSubscriptionQuery();

		ITask task = taskQuery.FirstOrDefault();
		Assert.NotNull(task);
		Assert.AreEqual("taskBeforeInterruptingEventSuprocess", task.TaskDefinitionKey);

		IList<IEventSubscription> eventSubscriptions = eventSubscriptionQuery.ToList();
		Assert.AreEqual(2, eventSubscriptions.Count);

		runtimeService.MessageEventReceived("newMessage", pi.Id);

		task = taskQuery.First();
		Assert.NotNull(task);
		Assert.AreEqual("taskAfterMessageStartEvent", task.TaskDefinitionKey);

		Assert.AreEqual(0, eventSubscriptionQuery.Count());

		try
		{
		  runtimeService.SignalEventReceived("newSignal", pi.Id);
		  Assert.Fail("A ProcessEngineException was expected.");
		}
		catch (ProcessEngineException)
		{
		  // expected exception;
		}

		taskService.Complete(task.Id);

		AssertProcessEnded(pi.Id);
	  }


        
        [Test]
        [Deployment("resources/bpmn/subprocess/InterruptingEventSubProcessTest.testCancelEventSubscriptions.bpmn")]
        public virtual void testCancelEventSubscriptionsWhenReceivingASignal()
	  {
		IProcessInstance pi = runtimeService.StartProcessInstanceByKey("process");

		IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
		IQueryable<IEventSubscription> eventSubscriptionQuery = runtimeService.CreateEventSubscriptionQuery();

		ITask task = taskQuery.First();
		Assert.NotNull(task);
		Assert.AreEqual("taskBeforeInterruptingEventSuprocess", task.TaskDefinitionKey);

		IList<IEventSubscription> eventSubscriptions = eventSubscriptionQuery.ToList();
		Assert.IsTrue( eventSubscriptions.Count>=2);

		runtimeService.SignalEventReceived("newSignal", pi.Id);

		task = taskQuery.First();
		Assert.NotNull(task);
		Assert.AreEqual("tastAfterSignalStartEvent", task.TaskDefinitionKey);

		Assert.AreEqual(0, eventSubscriptionQuery.Count());

		try
		{
		  runtimeService.MessageEventReceived("newMessage", pi.Id);
		}
		catch (ProcessEngineException ex)
            {
                //Assert.Fail("A ProcessEngineException was expected.");
                Assert.IsTrue(ex.Message.Contains("eventSubscriptions is empty"));
                
                // expected exception;
            }

		taskService.Complete(task.Id);

		AssertProcessEnded(pi.Id);
	  }


        [Test]
        [Deployment]
        public virtual void testCancelTimer()
	  {
		IProcessInstance pi = runtimeService.StartProcessInstanceByKey("process");

		IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
		IQueryable<IJob> jobQuery = managementService.CreateJobQuery()/*.Timers()*/;

		ITask task = taskQuery.First();
		Assert.NotNull(task);
		Assert.AreEqual("taskBeforeInterruptingEventSuprocess", task.TaskDefinitionKey);

		IJob timer = jobQuery.First();
		Assert.NotNull(timer);

		runtimeService.MessageEventReceived("newMessage", pi.Id);

		task = taskQuery.First();
		Assert.NotNull(task);
		Assert.AreEqual("taskAfterMessageStartEvent", task.TaskDefinitionKey);

		Assert.AreEqual(0, jobQuery.Count());

		taskService.Complete(task.Id);

		AssertProcessEnded(pi.Id);
	  }


        [Test]
        [Deployment]
        public virtual void testKeepCompensation()
	  {
		IProcessInstance pi = runtimeService.StartProcessInstanceByKey("process");

		IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
		IQueryable<IEventSubscription> eventSubscriptionQuery = runtimeService.CreateEventSubscriptionQuery();

		ITask task = taskQuery.First();
		Assert.NotNull(task);
		Assert.AreEqual("taskBeforeInterruptingEventSuprocess", task.TaskDefinitionKey);

		IList<IEventSubscription> eventSubscriptions = eventSubscriptionQuery.ToList();
		Assert.AreEqual(true, eventSubscriptions.Count>=2);

		runtimeService.MessageEventReceived("newMessage", pi.Id);

		task = taskQuery.First();
		Assert.NotNull(task);
		Assert.AreEqual("taskAfterMessageStartEvent", task.TaskDefinitionKey);

		Assert.AreEqual(true, eventSubscriptionQuery.Count()>=1);

		taskService.Complete(task.Id);

		AssertProcessEnded(pi.Id);
	  }


        [Test]
        [Deployment]
        public virtual void testTimeCycle()
	  {
		string ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

		IQueryable<IEventSubscription> eventSubscriptionQuery = runtimeService.CreateEventSubscriptionQuery();
		Assert.AreEqual(0, eventSubscriptionQuery.Count());

		IQueryable<ITask> taskQuery = taskService.CreateTaskQuery();
		Assert.AreEqual(true, taskQuery.Count()>=1);
		ITask task = taskQuery.First();
		Assert.AreEqual("task", task.TaskDefinitionKey);

		IQueryable<IJob> jobQuery = managementService.CreateJobQuery()/*.Timers()*/;
		Assert.AreEqual(true, jobQuery.Count()>=1);

		string jobId = jobQuery.First().Id;
		managementService.ExecuteJob(jobId);

		//Assert.AreEqual(0, jobQuery.Count());

		Assert.AreEqual(true, taskQuery.Count()>=1);
		task = taskQuery.First();
		Assert.AreEqual("eventSubProcessTask", task.TaskDefinitionKey);

		taskService.Complete(task.Id);

		AssertProcessEnded(ProcessInstanceId);
	  }

	}

}