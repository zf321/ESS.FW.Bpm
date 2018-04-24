using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime.Migration;
using Engine.Tests.Bpmn.Event.Compensate.Helper;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.builder;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Compensate
{
    [TestFixture]
    public class CompensateEventTest : PluggableProcessEngineTestCase
    {
        [Test]
        public virtual void testCompensateOrder()
        {
            //given two process models, only differ in order of the activities
            const string PROCESS_MODEL_WITH_REF_BEFORE =
                "resources/bpmn/event/compensate/compensation_reference-before.bpmn";
            const string PROCESS_MODEL_WITH_REF_AFTER =
                "resources/bpmn/event/compensate/compensation_reference-after.bpmn";

            //when model with ref before is deployed
            var deployment1 = repositoryService.CreateDeployment()
                .AddClasspathResource(PROCESS_MODEL_WITH_REF_BEFORE)
                .Deploy();
            //then no problem will occure

            //when model with ref after is deployed
            var deployment2 = repositoryService.CreateDeployment()
                .AddClasspathResource(PROCESS_MODEL_WITH_REF_AFTER)
                .Deploy();
            //then also no problem should occure

            //clean up
            repositoryService.DeleteDeployment(deployment1.Id);
            repositoryService.DeleteDeployment(deployment2.Id);
        }

        private void completeTask(string taskName)
        {
            completeTasks(taskName, 1);
        }

        private void completeTasks(string taskName, int times)
        {
            var tasks = taskService.CreateTaskQuery(c => c.NameWithoutCascade == taskName)
                .ToList();

            Assert.True(times <= tasks.Count(),
                "Actual there are " + tasks.Count() + " open tasks with name '" + taskName + "'. Expected at least " +
                times);

            var taskIterator = tasks.GetEnumerator();
            for (var i = 0; i < times; i++)
            {
                //JAVA TO C# CONVERTER TODO Resources.Task: Java iterators are only converted within the context of 'while' and 'for' loops:
                taskIterator.MoveNext();
                var task = taskIterator.Current;
                taskService.Complete(task.Id);
            }
        }

        private void completeTaskWithVariable(string taskName, string variable, object value)
        {
            var task = taskService.CreateTaskQuery(c => c.NameWithoutCascade == taskName)
                .First();
            Assert.NotNull(task, "No open task with name '" + taskName + "'");

            IDictionary<string, object> variables = new Dictionary<string, object>();
            if (!ReferenceEquals(variable, null))
                variables[variable] = value;

            taskService.Complete(task.Id, variables);
        }

        //[Test]
        [Deployment]
        public virtual void FAILING_testActivityInstanceTreeForMiSubProcessDefaultHandler()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("compensateProcess");

            completeTasks("Book Hotel", 5);
            // throw compensation event
            completeTask("throwCompensation");

            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("throwingCompensation")
                    .BeginMiBody("scope")
                    .BeginScope("scope")
                    .Activity("undoBookHotel")
                    .EndScope()
                    .BeginScope("scope")
                    .Activity("undoBookHotel")
                    .EndScope()
                    .BeginScope("scope")
                    .Activity("undoBookHotel")
                    .EndScope()
                    .BeginScope("scope")
                    .Activity("undoBookHotel")
                    .EndScope()
                    .BeginScope("scope")
                    .Activity("undoBookHotel")
                    .Done());
        }

        //[Test]
        [Deployment]
        public virtual void FAILING_testCompensateMiSubprocessVariableSnapshotOfElementVariable()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            // multi instance collection
            IList<string> flights = new List<string> { "STS-14", "STS-28" };
            variables["flights"] = flights;

            // see referenced java delegates in the process definition
            // java delegates read element variable (flight) and add the variable value
            // to a static list
            var processInstance = runtimeService.StartProcessInstanceByKey("compensateProcess", variables);

            if (!processEngineConfiguration.History.Equals(ProcessEngineConfiguration.HistoryNone))
                Assert.AreEqual(flights.Count, historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityId == "undoBookFlight")
                    .Count());

            // java delegates should be invoked for each element in collection
            Assert.AreEqual(flights, BookFlightService.bookedFlights);
            Assert.AreEqual(flights, CancelFlightService.CanceledFlights);

            AssertProcessEnded(processInstance.Id);
        }

        // Todo:EmbeddedSubProcessBuilder & ModifiableBpmnModelInstance
        //[Test]
        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryActivity)]
        public virtual void FAILING_testDeleteInstanceWithEventScopeExecution()
        {
            // given
            IBpmnModelInstance modelInstance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("foo")
                .StartEvent("start")
                .SubProcess("subProcess")
                ////.EmbeddedSubProcess()
                //.StartEvent("subProcessStart")
                .EndEvent("subProcessEnd")
                .SubProcessDone()
                .UserTask("userTask")
                .Done();

            //modelInstance =
            var proces = ModifiableBpmnModelInstance.Modify(modelInstance).AddSubProcessTo("subProcess");
            proces.Id<SubProcessBuilder>("eventSubProcess");
            modelInstance = proces.TriggerByEvent()
                .EmbeddedSubProcess()
                .StartEvent()
                .CompensateEventDefinition()
                .CompensateEventDefinitionDone()
                .EndEvent()
                .Done();

            Deployment(modelInstance);

            long dayInMillis = 1000 * 60 * 60 * 24;
            var date1 = new DateTime(10 * dayInMillis);
            ClockUtil.CurrentTime = date1;
            var processInstance = runtimeService.StartProcessInstanceByKey("foo");

            // when
            var date2 = new DateTime(date1.Ticks + dayInMillis);
            ClockUtil.CurrentTime = date2;
            runtimeService.DeleteProcessInstance(processInstance.Id, null);

            // then
            var historicActivityInstance = historyService.CreateHistoricActivityInstanceQuery()
                ///*.OrderByActivityId()*/
                ///*.Asc()*/
                .ToList();
            Assert.AreEqual(5, historicActivityInstance.Count);

            Assert.AreEqual("start", historicActivityInstance[0].ActivityId);
            Assert.AreEqual(date1, historicActivityInstance[0].EndTime);
            Assert.AreEqual("subProcess", historicActivityInstance[1].ActivityId);
            Assert.AreEqual(date1, historicActivityInstance[1].EndTime);
            Assert.AreEqual("subProcessEnd", historicActivityInstance[2].ActivityId);
            Assert.AreEqual(date1, historicActivityInstance[2].EndTime);
            Assert.AreEqual("subProcessStart", historicActivityInstance[3].ActivityId);
            Assert.AreEqual(date1, historicActivityInstance[3].EndTime);
            Assert.AreEqual("userTask", historicActivityInstance[4].ActivityId);
            Assert.AreEqual(date2, historicActivityInstance[4].EndTime);
        }

        //[Test]
        [Deployment]
        public virtual void FAILING_testReceiveTaskCompensationHandler()
        {
            // given a process instance
            var processInstance = runtimeService.StartProcessInstanceByKey("receiveTaskCompensationHandler");

            // when triggering compensation
            var beforeCompensationTask = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(beforeCompensationTask.Id);

            // then there is a message event subscription for the receive task compensation handler
            var eventSubscription = runtimeService.CreateEventSubscriptionQuery()
                .First();
            Assert.NotNull(eventSubscription);
            Assert.AreEqual(EventType.Message, eventSubscription.EventType);

            // and triggering the message completes compensation
            runtimeService.CorrelateMessage("Message");

            var afterCompensationTask = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(afterCompensationTask);
            Assert.AreEqual("beforeEnd", afterCompensationTask.TaskDefinitionKey);

            taskService.Complete(afterCompensationTask.Id);

            // and the process has successfully ended
            AssertProcessEnded(processInstance.Id);
        }

        //[Test]
        [Deployment]
        public virtual void FAILING_testSubprocessCompensationHandlerWithEventSubprocess()
        {
            // given a process instance in compensation
            runtimeService.StartProcessInstanceByKey("subProcessCompensationHandlerWithEventSubprocess");
            var beforeCompensationTask = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(beforeCompensationTask.Id);

            // when the event subprocess is triggered that is defined as part of the compensation handler
            runtimeService.CorrelateMessage("Message");

            // then activity instance tree is correct
            var task = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(task);
            Assert.AreEqual("eventSubProcessTask", task.TaskDefinitionKey);
        }

        //[Test]
        [Deployment("resources/bpmn/event/compensate/CompensateEventTest.TestSubprocessCompensationHandlerWithEventSubprocess.bpmn20.xml")]
        public virtual void FAILING_testSubprocessCompensationHandlerWithEventSubprocessActivityInstanceTree()
        {
            // given a process instance in compensation
            var processInstance =
                runtimeService.StartProcessInstanceByKey("subProcessCompensationHandlerWithEventSubprocess");
            var beforeCompensationTask = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(beforeCompensationTask.Id);

            // when the event subprocess is triggered that is defined as part of the compensation handler
            runtimeService.CorrelateMessage("Message");

            // then the event subprocess has been triggered
            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("throwCompensate")
                    .BeginScope("compensationHandler")
                    .BeginScope("eventSubProcess")
                    .Activity("eventSubProcessTask")
                    .Done());
        }

        [Test]
        [Deployment(
            new[] { "resources/bpmn/event/compensate/CompensateEventTest.ActivityWithCompensationEndEvent.bpmn20.xml" })]
        public virtual void testActivityInstanceTreeForCompensationEndEvent()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("compensateProcess");
            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            var t = ActivityInstanceAssert.That(tree);
            var instanceTree = ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId);
            instanceTree.Activity("end");
            instanceTree.Activity("undoBookHotel");
            var r = instanceTree.Done();
            t.HasStructure(r);
        }

        [Test]
        [Deployment(
            new[] { "resources/bpmn/event/compensate/CompensateEventTest.TestCompensationEventSubProcess.bpmn20.xml" })]
        public virtual void testActivityInstanceTreeForCompensationEventSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("bookingProcess");

            completeTask("Book Flight");
            completeTask("Book Hotel");

            // throw compensation event
            completeTaskWithVariable("Validate Booking", "valid", false);

            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("throwCompensation")
                    .BeginScope("booking-subprocess")
                    .Activity("cancelFlight")
                    .BeginScope("compensationSubProcess")
                    .Activity("compensateFlight")
                    .Done());
        }

        [Test]
        [Deployment(new[] { "resources/bpmn/event/compensate/CompensateEventTest.compensationMiActivity.bpmn20.xml" })]
        public virtual void testActivityInstanceTreeForMiActivity()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("compensateProcess");

            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("end")
                    .BeginMiBody("bookHotel")
                    .Activity("undoBookHotel")
                    .Activity("undoBookHotel")
                    .Activity("undoBookHotel")
                    .Activity("undoBookHotel")
                    .Activity("undoBookHotel")
                    .Done());
        }

        [Test]
        [Deployment(new[] { "resources/bpmn/event/compensate/CompensateEventTest.compensationMiSubprocess.bpmn20.xml" })]
        public virtual void testActivityInstanceTreeForMiSubprocess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("compensateProcess");

            completeTasks("Book Hotel", 5);
            // throw compensation event
            completeTask("throwCompensation");

            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("throwingCompensation")
                    .BeginMiBody("scope")
                    .Activity("undoBookHotel")
                    .Activity("undoBookHotel")
                    .Activity("undoBookHotel")
                    .Activity("undoBookHotel")
                    .Activity("undoBookHotel")
                    .Done());
        }

        [Test]
        [Deployment("resources/bpmn/event/compensate/CompensateEventTest.TestCompensateParallelSubprocessCompHandlerWaitstate.bpmn20.xml")]
        public virtual void testActivityInstanceTreeForParallelMiActivityInSubprocess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("compensateProcess");

            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("parallelTask")
                    .Activity("throwCompensate")
                    .BeginScope("scope")
                    .BeginMiBody("bookHotel")
                    .Activity("undoBookHotel")
                    .Activity("undoBookHotel")
                    .Activity("undoBookHotel")
                    .Activity("undoBookHotel")
                    .Activity("undoBookHotel")
                    .Done());
        }

        [Test]
        [Deployment]
        public virtual void testActivityInstanceTreeWithoutEventScope()
        {
            // given
            var instance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = instance.Id;

            // when
            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;
            taskService.Complete(taskId);

            // then
            var tree = runtimeService.GetActivityInstance(ProcessInstanceId);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(instance.ProcessDefinitionId)
                    .Activity("task")
                    .Done());
        }

        [Test]
        [Deployment(
            new[]{"resources/bpmn/event/compensate/CompensationHandler.bpmn20.xml","resources/bpmn/event/compensate/CompensateEventTest.TestCallActivityCompensationHandler.bpmn20.xml"
                })]
        public virtual void testCallActivityCompensationHandler()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("compensateProcess");

            if (!processEngineConfiguration.History.Equals(ProcessEngineConfiguration.HistoryNone))
                Assert.AreEqual(5, historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityId == "undoBookHotel")
                    .Count());

            runtimeService.Signal(processInstance.Id);
            AssertProcessEnded(processInstance.Id);

            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
                .Count());

            if (!processEngineConfiguration.History.Equals(ProcessEngineConfiguration.HistoryNone))
                Assert.AreEqual(6, historyService.CreateHistoricProcessInstanceQuery()
                    .Count());
        }

        [Test]
        [Deployment(
            new[] { "resources/bpmn/event/compensate/CompensateEventTest.ActivityWithCompensationEndEvent.bpmn20.xml" })]
        public virtual void testCancelProcessInstanceWithActiveCompensation()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("compensateProcess");

            // when
            runtimeService.DeleteProcessInstance(processInstance.Id, null);

            // then
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment]
        public virtual void testCompensateActivityInConcurrentSubprocess()
        {
            // given
            var instance = runtimeService.StartProcessInstanceByKey("compensateProcess");

            var scopeTask = taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "scopeTask")
                .First();
            taskService.Complete(scopeTask.Id);

            var outerTask = taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "outerTask")
                .First();
            taskService.Complete(outerTask.Id);

            // process has not yet thrown compensation
            // when throw compensation
            runtimeService.Signal(instance.Id);

            // then
            var compensationTask = taskService.CreateTaskQuery().ToList().OrderBy(m=>m.Name)
                .Last();
            Assert.NotNull(compensationTask);
            Assert.AreEqual("undoScopeTask", compensationTask.TaskDefinitionKey);

            taskService.Complete(compensationTask.Id);
            runtimeService.Signal(instance.Id);
            AssertProcessEnded(instance.Id);
        }

        [Test]
        [Deployment]
        public virtual void testCompensateActivityInSubprocess()
        {
            // given
            var instance = runtimeService.StartProcessInstanceByKey("compensateProcess");

            var scopeTask = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(scopeTask.Id);

            // process has not yet thrown compensation
            // when throw compensation
            runtimeService.Signal(instance.Id);
            // then
            var compensationTask = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(compensationTask);
            Assert.AreEqual("undoScopeTask", compensationTask.TaskDefinitionKey);

            taskService.Complete(compensationTask.Id);
            runtimeService.Signal(instance.Id);
            AssertProcessEnded(instance.Id);
        }

        [Test]
        [Deployment]
        public virtual void testCompensateActivityRef()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("compensateProcess");

            Assert.AreEqual(5, runtimeService.GetVariable(processInstance.Id, "undoBookHotel"));
            Assert.IsNull(runtimeService.GetVariable(processInstance.Id, "undoBookFlight"));

            runtimeService.Signal(processInstance.Id);
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(
            new[] {

                "resources/bpmn/event/compensate/CompensateEventTest.TestCompensationInEventSubProcessActivityRef.bpmn20.xml"
            })]
        public virtual void testCompensateActivityRefInEventSubprocess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("compensateProcess");
            AssertProcessEnded(processInstance.Id);
            var historicVariableInstanceQuery = historyService.CreateHistoricVariableInstanceQuery(m=>m.Name== "undoBookSecondHotel")
                //.VariableName("undoBookSecondHotel")
                ;

            if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HistorylevelAudit)
            {
                Assert.AreEqual(1, historicVariableInstanceQuery.Count());
                Assert.AreEqual("undoBookSecondHotel", historicVariableInstanceQuery.First().Name
                    /*.VariableName*/);
                Assert.AreEqual(5, historicVariableInstanceQuery.First()
                    .Value);

                Assert.AreEqual(0, historyService.CreateHistoricVariableInstanceQuery(c => c.ProcessInstanceId == processInstance.Id&&c.Name== "undoBookFlight")
                    ////.VariableName("undoBookFlight")
                    .Count());

                Assert.AreEqual(0, historyService.CreateHistoricVariableInstanceQuery(c => c.ProcessInstanceId == processInstance.Id&&c.Name== "undoBookHotel")
                    ////.VariableName("undoBookHotel")
                    .Count());
            }
        }

        [Test]
        [Deployment]
        public virtual void testCompensateActivityRefMiActivity()
        {
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("compensateProcess")
                .Id;

            completeTasks("Book Hotel", 5);

            // throw compensation event for activity
            completeTaskWithVariable("Request Vacation", "accept", false);

            // execute compensation handlers for each execution of the subprocess
            Assert.AreEqual(5, taskService.CreateTaskQuery()
                .Count());
            completeTasks("Cancel Hotel", 5);

            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment]
        public virtual void testCompensateActivityRefMiSubprocess()
        {
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("compensateProcess")
                .Id;

            completeTasks("Book Hotel", 5);

            // throw compensation event for activity
            completeTaskWithVariable("Request Vacation", "accept", false);

            // execute compensation handlers for each execution of the subprocess
            Assert.AreEqual(5, taskService.CreateTaskQuery()
                .Count());
            completeTasks("Cancel Hotel", 5);

            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(
            new[] {

                "resources/bpmn/event/compensate/CompensateEventTest.TestCompensationTriggeredByEventSubProcessActivityRef.bpmn20.xml"
            })]
        public virtual void testCompensateActivityRefTriggeredByEventSubprocess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("compensateProcess");
            AssertProcessEnded(processInstance.Id);

            var historicVariableInstanceQuery =
                historyService.CreateHistoricVariableInstanceQuery(c => c.ProcessInstanceId == processInstance.Id&&c.Name== "undoBookHotel");
            //.VariableName("undoBookHotel");

            if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HistorylevelAudit)
            {
                Assert.AreEqual(1, historicVariableInstanceQuery.Count());
                Assert.AreEqual("undoBookHotel", historicVariableInstanceQuery.First()
                    .Name);
                Assert.AreEqual(5, historicVariableInstanceQuery.First()
                    .Value);

                Assert.AreEqual(0, historyService.CreateHistoricVariableInstanceQuery(c => c.ProcessInstanceId == processInstance.Id&&c.Name== "undoBookFlight")
                    //.VariableName("undoBookFlight")
                    .Count());
            }
        }

        [Test]
        [Deployment(
            new[] {

                "resources/bpmn/event/compensate/CompensateEventTest.TestCompensationTriggeredByEventSubProcessInSubProcessActivityRef.bpmn20.xml"
            })]
        public virtual void testCompensateActivityRefTriggeredByEventSubprocessInSubProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("compensateProcess");
            AssertProcessEnded(processInstance.Id);

            var historicVariableInstanceQuery =
                historyService.CreateHistoricVariableInstanceQuery(c => c.ProcessInstanceId == processInstance.Id&&c.Name== "undoBookHotel");
            //.VariableName("undoBookHotel");

            if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HistorylevelAudit)
            {
                Assert.AreEqual(1, historicVariableInstanceQuery.Count());
                Assert.AreEqual("undoBookHotel", historicVariableInstanceQuery.First()
                    .Name);
                Assert.AreEqual(5, historicVariableInstanceQuery.First()
                    .Value);

                Assert.AreEqual(0, historyService.CreateHistoricVariableInstanceQuery(c => c.ProcessInstanceId == processInstance.Id&&c.Name== "undoBookFlight")
                    //.VariableName("undoBookFlight")
                    .Count());
            }
        }

        [Test]
        [Deployment]
        public virtual void testCompensateConcurrentMiActivity()
        {
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("compensateProcess")
                .Id;

            // complete 4 of 5 IUser tasks
            completeTasks("Book Hotel", 4);

            // throw compensation event
            completeTaskWithVariable("Request Vacation", "accept", false);

            // should not compensate activity before multi instance activity is completed
            Assert.AreEqual(0, taskService.CreateTaskQuery(c => c.NameWithoutCascade == "Cancel Hotel")
                .Count());

            // complete last open task and end process instance
            completeTask("Book Hotel");
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment]
        public virtual void testCompensateConcurrentMiSubprocess()
        {
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("compensateProcess")
                .Id;

            // complete 4 of 5 IUser tasks
            completeTasks("Book Hotel", 4);

            // throw compensation event
            completeTaskWithVariable("Request Vacation", "accept", false);

            // should not compensate activity before multi instance activity is completed
            Assert.AreEqual(0, taskService.CreateTaskQuery(c => c.NameWithoutCascade== "Cancel Hotel")
                .Count());

            // complete last open task and end process instance
            completeTask("Book Hotel");

            runtimeService.Signal(ProcessInstanceId);
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(
            new[] { "resources/bpmn/event/compensate/CompensateEventTest.TestCompensationInEventSubProcess.bpmn20.xml" })]
        public virtual void testCompensateInEventSubprocess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("compensateProcess");
            AssertProcessEnded(processInstance.Id);
            var historicVariableInstanceQuery = historyService.CreateHistoricVariableInstanceQuery(m=>m.Name== "undoBookSecondHotel");
            //.VariableName("undoBookSecondHotel");

            if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HistorylevelAudit)
            {
                Assert.AreEqual(1, historicVariableInstanceQuery.Count());
                Assert.AreEqual("undoBookSecondHotel", historicVariableInstanceQuery.First()
                    .Name);
                Assert.AreEqual(5, historicVariableInstanceQuery.First()
                    .Value);

                historicVariableInstanceQuery = historyService.CreateHistoricVariableInstanceQuery(m=>m.Name== "undoBookFlight");
                //.VariableName("undoBookFlight");

                Assert.AreEqual(1, historicVariableInstanceQuery.Count());
                Assert.AreEqual(5, historicVariableInstanceQuery.First()
                    .Value);

                historicVariableInstanceQuery = historyService.CreateHistoricVariableInstanceQuery(m => m.Name == "undoBookHotel");
                //.VariableName("undoBookHotel");

                Assert.AreEqual(1, historicVariableInstanceQuery.Count());
                Assert.AreEqual(5, historicVariableInstanceQuery.First()
                    .Value);
            }
        }

        [Test]
        [Deployment]
        public virtual void testCompensateMiSubprocess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("compensateProcess");

            Assert.AreEqual(5, runtimeService.GetVariable(processInstance.Id, "undoBookHotel"));

            runtimeService.Signal(processInstance.Id);
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment]
        public virtual void testCompensateMiSubprocessVariableSnapshots()
        {
            // see referenced java delegates in the process definition.

            IList<string> hotels = new List<string> { "Rupert", "Vogsphere", "Milliways", "Taunton", "Ysolldins" };

            SetVariablesDelegate.Values = hotels;

            // SetVariablesDelegate take the first element of static list and set the value as local variable
            // GetVariablesDelegate read the variable and add the value to static list

            runtimeService.StartProcessInstanceByKey("compensateProcess");

            if (!processEngineConfiguration.History.Equals(ProcessEngineConfiguration.HistoryNone))
                Assert.AreEqual(5, historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityId == "undoBookHotel")
                    .Count());

            //JAVA TO C# CONVERTER TODO Resources.Task: There is no .NET equivalent to the java.util.Collection 'containsAll' method:            
            //Assert.True(GetVariablesDelegate.values.ContainsAll(hotels));
            foreach (var hotel in hotels)
                Assert.True(GetVariablesDelegate.values.Contains(hotel));
        }

        [Test]
        [Deployment]
        public virtual void testCompensateMiSubprocessWithCompensationEventSubProcess()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            // multi instance collection
            variables["flights"] = new List<string> { "STS-14", "STS-28" };

            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("bookingProcess", variables)
                .Id;

            completeTask("Book Flight");
            completeTask("Book Hotel");

            completeTask("Book Flight");
            completeTask("Book Hotel");

            // throw compensation event
            completeTaskWithVariable("Validate Booking", "valid", false);

            // execute compensation handlers for each execution of the subprocess
            completeTasks("Cancel Flight", 2);
            completeTasks("Cancel Hotel", 2);
            completeTasks("Update Customer Record", 2);

            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment]
        public virtual void testCompensateMiSubprocessWithCompensationEventSubprocessVariableSnapshots()
        {
            // see referenced java delegates in the process definition.

            IList<string> hotels = new List<string> { "Rupert", "Vogsphere", "Milliways", "Taunton", "Ysolldins" };

            SetVariablesDelegate.Values = hotels;

            // SetVariablesDelegate take the first element of static list and set the value as local variable
            // GetVariablesDelegate read the variable and add the value to static list

            runtimeService.StartProcessInstanceByKey("compensateProcess");

            if (!processEngineConfiguration.History.Equals(ProcessEngineConfiguration.HistoryNone))
                Assert.AreEqual(5, historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityId == "undoBookHotel")
                    .Count());

            //JAVA TO C# CONVERTER TODO Resources.Task: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
            //Assert.True(GetVariablesDelegate.values.containsAll(hotels));
            foreach (var hotel in hotels)
                Assert.True(GetVariablesDelegate.values.Contains(hotel));
        }

        [Test]
        [Deployment]
        public virtual void testCompensateParallelMiSubprocessWithCompensationEventSubProcess()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            // multi instance collection
            variables["flights"] = new List<string> { "STS-14", "STS-28" };

            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("bookingProcess", variables)
                .Id;

            completeTasks("Book Flight", 2);
            completeTasks("Book Hotel", 2);

            // throw compensation event
            completeTaskWithVariable("Validate Booking", "valid", false);

            // execute compensation handlers for each execution of the subprocess
            completeTasks("Cancel Flight", 2);
            completeTasks("Cancel Hotel", 2);
            completeTasks("Update Customer Record", 2);

            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment]
        public virtual void testCompensateParallelSubprocess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("compensateProcess");

            Assert.AreEqual(5, runtimeService.GetVariable(processInstance.Id, "undoBookHotel"));

            var singleResult = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(singleResult.Id);

            runtimeService.Signal(processInstance.Id);
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment]
        public virtual void testCompensateParallelSubprocessCompHandlerWaitstate()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("compensateProcess");

            var compensationHandlerTasks = taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "undoBookHotel")
                .ToList();
            Assert.AreEqual(5, compensationHandlerTasks.Count());

            var rootActivityInstance = runtimeService.GetActivityInstance(processInstance.Id);
            var compensationHandlerInstances = GetInstancesForActivityId(rootActivityInstance, "undoBookHotel");
            Assert.AreEqual(5, compensationHandlerInstances.Count);

            foreach (var task in compensationHandlerTasks)
                taskService.Complete(task.Id);

            var singleResult = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(singleResult.Id);

            runtimeService.Signal(processInstance.Id);
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment]
        public virtual void testCompensateScope()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("compensateProcess");

            Assert.AreEqual(5, runtimeService.GetVariable(processInstance.Id, "undoBookHotel"));
            Assert.AreEqual(5, runtimeService.GetVariable(processInstance.Id, "undoBookFlight"));

            runtimeService.Signal(processInstance.Id);
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment]
        public virtual void testCompensateSubprocess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("compensateProcess");

            Assert.AreEqual(5, runtimeService.GetVariable(processInstance.Id, "undoBookHotel"));

            runtimeService.Signal(processInstance.Id);
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment]
        public virtual void testCompensateSubprocessInsideSubprocess()
        {
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("compensateProcess")
                .Id;

            completeTask("Book Hotel");
            completeTask("Book Flight");

            // throw compensation event
            completeTask("throw compensation");

            // execute compensation handlers
            completeTask("Cancel Hotel");
            completeTask("Cancel Flight");

            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment]
        public virtual void testCompensateSubprocessWithBoundaryEvent()
        {
            var instance = runtimeService.StartProcessInstanceByKey("compensateProcess");

            var compensationTask = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(compensationTask);
            Assert.AreEqual("undoSubprocess", compensationTask.TaskDefinitionKey);

            taskService.Complete(compensationTask.Id);
            runtimeService.Signal(instance.Id);
            AssertProcessEnded(instance.Id);
        }

        [Test]
        [Deployment]
        public virtual void testCompensationEndEventWithActivityRef()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("compensateProcess");

            if (!processEngineConfiguration.History.Equals(ProcessEngineConfiguration.HistoryNone))
            {
                Assert.AreEqual(5, historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityId == "undoBookHotel")
                    .Count());
                Assert.AreEqual(0, historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityId == "undoBookFlight")
                    .Count());
            }

            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment]
        public virtual void testCompensationEndEventWithScope()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("compensateProcess");

            if (!processEngineConfiguration.History.Equals(ProcessEngineConfiguration.HistoryNone))
            {
                Assert.AreEqual(5, historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityId == "undoBookHotel")
                    .Count());
                Assert.AreEqual(5, historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityId == "undoBookFlight")
                    .Count());
            }

            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment]
        public virtual void testCompensationEventSubprocessConsumeCompensationEvent()
        {
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("compensateProcess")
                .Id;

            completeTask("Book Hotel");
            completeTask("Book Flight");

            // throw compensation event
            completeTask("throw compensation");

            // execute compensation handler and consume compensation event
            completeTask("Cancel Hotel");
            // compensation handler at subprocess (Cancel Flight) should not be executed
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment]
        public virtual void testCompensationEventSubprocessReThrowCompensationEvent()
        {
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("compensateProcess")
                .Id;

            completeTask("Book Hotel");
            completeTask("Book Flight");

            // throw compensation event
            completeTask("throw compensation");

            // execute compensation handler and re-throw compensation event
            completeTask("Cancel Hotel");
            // execute compensation handler at subprocess
            completeTask("Cancel Flight");

            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment]
        public virtual void testCompensationEventSubProcessWithActivityRef()
        {
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("bookingProcess")
                .Id;

            completeTask("Book Hotel");
            completeTask("Book Flight");

            // throw compensation event for specific scope (with activityRef = subprocess)
            completeTaskWithVariable("Validate Booking", "valid", false);

            // compensate the activity within this scope
            Assert.AreEqual(1, taskService.CreateTaskQuery()
                .Count());
            completeTask("Cancel Hotel");

            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment]
        public virtual void testCompensationEventSubprocessWithoutBoundaryEvents()
        {
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("compensateProcess")
                .Id;

            completeTask("Book Hotel");
            completeTask("Book Flight");

            // throw compensation event
            completeTask("throw compensation");

            // execute compensation handlers
            completeTask("Cancel Flight");
            completeTask("Cancel Hotel");

            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment(
            new[] { "resources/bpmn/event/compensate/CompensateEventTest.TestCompensationEventSubProcess.bpmn20.xml" })]
        public virtual void testCompensationEventSubProcessWithScope()
        {
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("bookingProcess")
                .Id;

            completeTask("Book Flight");
            completeTask("Book Hotel");

            // throw compensation event for current scope (without activityRef)
            completeTaskWithVariable("Validate Booking", "valid", false);

            // first - compensate book flight
            Assert.AreEqual(1, taskService.CreateTaskQuery()
                .Count());
            completeTask("Cancel Flight");
            // second - compensate book hotel
            Assert.AreEqual(1, taskService.CreateTaskQuery()
                .Count());
            completeTask("Cancel Hotel");
            // third - additional compensation handler
            completeTask("Update Customer Record");

            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment]
        public virtual void testConcurrentExecutionsAndPendingCompensation()
        {
            // given
            var instance = runtimeService.StartProcessInstanceByKey("process");
            var ProcessInstanceId = instance.Id;
            var taskId = taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "innerTask")
                .First()
                .Id;

            // when (1)
            taskService.Complete(taskId);

            // then (1)
            var executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(
                    ExecutionAssert.DescribeExecutionTree(null)
                        .Scope()
                        .Child("task1")
                        .Concurrent()
                        .NoScope()
                        .Up()
                        .Child("task2")
                        .Concurrent()
                        .NoScope()
                        .Up()
                        .Child("subProcess")
                        .EventScope()
                        .Scope()
                        .Up()
                        .Done());

            var tree = runtimeService.GetActivityInstance(ProcessInstanceId);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(instance.ProcessDefinitionId)
                    .Activity("task1")
                    .Activity("task2")
                    .Done());


            // when (2)
            taskId = taskService.CreateTaskQuery(c => c.TaskDefinitionKeyWithoutCascade == "task1")
                .First()
                .Id;
            taskService.Complete(taskId);

            // then (2)
            executionTree = ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);

            ExecutionAssert.That(executionTree)
                .Matches(
                    ExecutionAssert.DescribeExecutionTree("task2")
                        .Scope()
                        .Child("subProcess")
                        .EventScope()
                        .Scope()
                        .Up()
                        .Done());

            tree = runtimeService.GetActivityInstance(ProcessInstanceId);

            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(instance.ProcessDefinitionId)
                    .Activity("task2")
                    .Done());

            // when (3)
            taskId = taskService.CreateTaskQuery(c => c.TaskDefinitionKeyWithoutCascade == "task2")
                .First()
                .Id;
            taskService.Complete(taskId);

            // then (3)
            AssertProcessEnded(ProcessInstanceId);
        }

        [Test]
        [Deployment]
        public virtual void testConcurrentScopeCompensation()
        {
            // given a process instance with two concurrent tasks, one of which is waiting
            // before throwing compensation
            var processInstance = runtimeService.StartProcessInstanceByKey("concurrentScopeCompensation");
            var beforeCompensationTask = taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "beforeCompensationTask")
                .First();
            var concurrentTask = taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "concurrentTask")
                .First();

            // when throwing compensation such that two subprocesses are compensated
            taskService.Complete(beforeCompensationTask.Id);

            // then both compensation handlers have been executed
            if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HistorylevelAudit)
            {
                var historicVariableInstanceQuery = historyService.CreateHistoricVariableInstanceQuery(m=>m.Name== "compensateScope1Task");
                //.VariableName("compensateScope1Task");

                Assert.AreEqual(1, historicVariableInstanceQuery.Count());
                Assert.AreEqual(1, historicVariableInstanceQuery.First()
                    .Value);

                historicVariableInstanceQuery = historyService.CreateHistoricVariableInstanceQuery(m=>m.Name== "compensateScope2Task");
                //.VariableName("compensateScope2Task");

                Assert.AreEqual(1, historicVariableInstanceQuery.Count());
                Assert.AreEqual(1, historicVariableInstanceQuery.First()
                    .Value);
            }

            // and after completing the concurrent task, the process instance ends successfully
            taskService.Complete(concurrentTask.Id);
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(
            new[] {
                "resources/bpmn/event/compensate/CompensateEventTest.TestCompensateParallelSubprocessCompHandlerWaitstate.bpmn20.xml"
            })]
        public virtual void testDeleteParallelSubprocessCompHandlerWaitstate()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("compensateProcess");

            // five inner tasks
            var compensationHandlerTasks = taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "undoBookHotel")
                .ToList();
            Assert.AreEqual(5, compensationHandlerTasks.Count);

            // when
            runtimeService.DeleteProcessInstance(processInstance.Id, "");

            // then the process has been removed
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment]
        public virtual void testExecutionListeners()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["start"] = 0;
            variables["end"] = 0;

            var processInstance = runtimeService.StartProcessInstanceByKey("testProcess", variables);

            var started = (int?)runtimeService.GetVariable(processInstance.Id, "start");
            Assert.AreEqual(5, started);

            var ended = (int?)runtimeService.GetVariable(processInstance.Id, "end");
            Assert.AreEqual(5, ended);

            var historyLevel = processEngineConfiguration.HistoryLevel.Id;
            if (historyLevel > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                var finishedCount = historyService.CreateHistoricActivityInstanceQuery(c => c.ActivityId == "undoBookHotel")
                    //.Finished()
                    .Count();
                Assert.AreEqual(5, finishedCount);
            }
        }

        [Test]
        [Deployment]
        public virtual void testLocalVariablesInEndExecutionListener()
        {
            // given
            var setListener = new SetLocalVariableListener("foo", "bar");
            var readListener = new ReadLocalVariableListener("foo");

            var processInstance = runtimeService.StartProcessInstanceByKey("process", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("setListener", setListener)
                .PutValue("readListener", readListener));

            var beforeCompensationTask = taskService.CreateTaskQuery()
                .First();

            // when executing the compensation handler
            taskService.Complete(beforeCompensationTask.Id);

            // then the variable listener has been invoked and was able to read the variable on the end event
            readListener = (ReadLocalVariableListener)runtimeService.GetVariable(processInstance.Id, "readListener");

            Assert.AreEqual(1, readListener.VariableEvents.Count);

            var @event = readListener.VariableEvents[0];
            Assert.AreEqual("foo", @event.VariableName);
            Assert.AreEqual("bar", @event.VariableValue);
        }

        [Test]
        [Deployment]
        public virtual void testSubprocessCompensationHandler()
        {
            // given a process instance
            var processInstance = runtimeService.StartProcessInstanceByKey("subProcessCompensationHandler");

            // when throwing compensation
            var beforeCompensationTask = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(beforeCompensationTask.Id);

            // then the compensation handler has been activated
            // and the IUser task in the sub process can be successfully completed
            var subProcessTask = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(subProcessTask);
            Assert.AreEqual("subProcessTask", subProcessTask.TaskDefinitionKey);

            taskService.Complete(subProcessTask.Id);

            // and the task following compensation can be successfully completed
            var afterCompensationTask = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(afterCompensationTask);
            Assert.AreEqual("beforeEnd", afterCompensationTask.TaskDefinitionKey);

            taskService.Complete(afterCompensationTask.Id);

            // and the process has successfully ended
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(
            new[] { "resources/bpmn/event/compensate/CompensateEventTest.TestSubprocessCompensationHandler.bpmn20.xml" })]
        public virtual void testSubprocessCompensationHandlerActivityInstanceTree()
        {
            // given a process instance
            var processInstance = runtimeService.StartProcessInstanceByKey("subProcessCompensationHandler");

            // when throwing compensation
            var beforeCompensationTask = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(beforeCompensationTask.Id);

            // then the activity instance tree is correct
            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(
                ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("throwCompensate")
                    .BeginScope("compensationHandler")
                    .Activity("subProcessTask")
                    .Done());
        }

        [Test]
        [Deployment(
            new[] { "resources/bpmn/event/compensate/CompensateEventTest.TestSubprocessCompensationHandler.bpmn20.xml" })]
        public virtual void testSubprocessCompensationHandlerDeleteProcessInstance()
        {
            // given a process instance in compensation
            var processInstance = runtimeService.StartProcessInstanceByKey("subProcessCompensationHandler");
            var beforeCompensationTask = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(beforeCompensationTask.Id);

            // when deleting the process instance
            runtimeService.DeleteProcessInstance(processInstance.Id, null);

            // then the process instance is ended
            AssertProcessEnded(processInstance.Id);
        }
    }
}