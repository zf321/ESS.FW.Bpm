using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Async
{
    

    [TestFixture]
    public class AsyncStartEventTest : PluggableProcessEngineTestCase
    {

        [Test]
        [Deployment]
        public virtual void testAsyncStartEvent()
        {
            runtimeService.StartProcessInstanceByKey("asyncStartEvent");

            ITask task = taskService.CreateTaskQuery().First();
            Assert.IsNull(task, "The IUser task should not have been reached yet");
            //Assert.IsNull("The IUser task should not have been reached yet", task);

            Assert.AreEqual(1, runtimeService.CreateExecutionQuery(c=>c.ActivityId == "startEvent").Count());

            ExecuteAvailableJobs();
            task = taskService.CreateTaskQuery().First();

            Assert.AreEqual(0, runtimeService.CreateExecutionQuery(c=>c.ActivityId == "startEvent").Count());

            Assert.NotNull(task, "The IUser task should have been reached");
            //Assert.NotNull("The IUser task should have been reached", task);
        }

        [Test]
        [Deployment]
        public virtual void testAsyncStartEventListeners()
        {
            IProcessInstance instance = runtimeService.StartProcessInstanceByKey("asyncStartEvent");

            Assert.IsNull(runtimeService.GetVariable(instance.Id, "listener"));

            ExecuteAvailableJobs();

            Assert.NotNull(runtimeService.GetVariable(instance.Id, "listener"));
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/async/AsyncStartEventTest.TestAsyncStartEvent.bpmn20.xml" })]
        public virtual void testAsyncStartEventActivityInstance()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("asyncStartEvent");

            IActivityInstance tree = runtimeService.GetActivityInstance(processInstance.Id);

            //Assert.That(tree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).Transition("startEvent").Done());
            //Assert.That(tree).HasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).Transition("startEvent").Done());
        }

        [Test]
        [Deployment]
        public virtual void testMultipleAsyncStartEvents()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["foo"] = "bar";
            runtimeService.CorrelateMessage("newInvoiceMessage", new Dictionary<string, object>(), variables);

            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery().Count());

            ExecuteAvailableJobs();

            ITask task = taskService.CreateTaskQuery().First();
            Assert.NotNull(task);
            Assert.AreEqual("taskAfterMessageStartEvent", task.TaskDefinitionKey);

            taskService.Complete(task.Id);

            // Assert process instance is ended
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());

        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/async/AsyncStartEventTest.TestCallActivity-super.bpmn20.xml", "resources/bpmn/async/AsyncStartEventTest.TestCallActivity-sub.bpmn20.xml" })]
        public virtual void testCallActivity()
        {
            runtimeService.StartProcessInstanceByKey("super");

            IProcessInstance pi = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == "sub").First();

            Assert.True(pi is ExecutionEntity);

            Assert.AreEqual("theSubStart", ((ExecutionEntity)pi).ActivityId);

        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @IDeployment public void testAsyncSubProcessStartEvent()
        [Test]
        [Deployment]
        public virtual void testAsyncSubProcessStartEvent()
        {
            runtimeService.StartProcessInstanceByKey("process");

            ITask task = taskService.CreateTaskQuery().First();
            Assert.IsNull(task, "The subprocess IUser task should not have been reached yet");
            //Assert.IsNull("The subprocess IUser task should not have been reached yet", task);

            Assert.AreEqual(1, runtimeService.CreateExecutionQuery(c=>c.ActivityId == "StartEvent_2").Count());

            ExecuteAvailableJobs();
            task = taskService.CreateTaskQuery().First();

            Assert.AreEqual(0, runtimeService.CreateExecutionQuery(c=>c.ActivityId == "StartEvent_2").Count());
            Assert.NotNull(task, "The subprocess IUser task should have been reached");
            //Assert.NotNull("The subprocess IUser task should have been reached", task);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/async/AsyncStartEventTest.TestAsyncSubProcessStartEvent.bpmn" })]
        public virtual void testAsyncSubProcessStartEventActivityInstance()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            IActivityInstance tree = runtimeService.GetActivityInstance(processInstance.Id);
            //Assert.That(tree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).BeginScope("SubProcess_1").Transition("StartEvent_2").Done());
            //Assert.That(tree).HasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("SubProcess_1").Transition("StartEvent_2").Done());
        }
    }

}