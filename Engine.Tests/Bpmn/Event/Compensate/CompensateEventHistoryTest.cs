using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Compensate
{
    [TestFixture]
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryAudit)]
    public class CompensateEventHistoryTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment(new string[] { "resources/bpmn/event/compensate/CompensateEventHistoryTest.TestBoundaryCompensationHandlerHistory.bpmn20.xml" })]
        public virtual void testBoundaryCompensationHandlerHistoryActivityInstance()
        {
            // given a process instance
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("boundaryHandlerProcess");

            // when throwing compensation
            ITask beforeCompensationTask = taskService.CreateTaskQuery().First();
            taskService.Complete(beforeCompensationTask.Id);

            string compensationHandlerActivityInstanceId = runtimeService.GetActivityInstance(processInstance.Id).GetActivityInstances("compensationHandler")[0].Id;

            // . and completing compensation
            ITask compensationHandler = taskService.CreateTaskQuery().First();
            taskService.Complete(compensationHandler.Id);

            // then there is a historic activity instance for the compensation handler
            IHistoricActivityInstance historicCompensationHandlerInstance = historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "compensationHandler").First();

            Assert.NotNull(historicCompensationHandlerInstance);
            Assert.AreEqual(compensationHandlerActivityInstanceId, historicCompensationHandlerInstance.Id);
            Assert.AreEqual(processInstance.Id, historicCompensationHandlerInstance.ParentActivityInstanceId);
        }

        //[Test]
        [Deployment(new string[] { "resources/bpmn/event/compensate/CompensateEventHistoryTest.TestBoundaryCompensationHandlerHistory.bpmn20.xml" })]
        public virtual void FAILING_testBoundaryCompensationHandlerHistoryVariableInstance()
        {
            // given a process instance
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("boundaryHandlerProcess");

            // when throwing compensation
            ITask beforeCompensationTask = taskService.CreateTaskQuery().First();
            taskService.Complete(beforeCompensationTask.Id);

            string compensationHandlerActivityInstanceId = runtimeService.GetActivityInstance(processInstance.Id).GetActivityInstances("compensationHandler")[0].Id;

            // . setting a variable via task service API
            ITask compensationHandler = taskService.CreateTaskQuery().First();
            runtimeService.SetVariableLocal(compensationHandler.ExecutionId, "apiVariable", "someValue");

            // . and completing compensation
            taskService.Complete(compensationHandler.Id);

            // then there is a historic variable instance for the variable set by API
            IHistoricVariableInstance historicVariableInstance = historyService.CreateHistoricVariableInstanceQuery().First();

            Assert.NotNull(historicVariableInstance);
            Assert.AreEqual(compensationHandlerActivityInstanceId, historicVariableInstance.ActivityInstanceId);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/compensate/CompensateEventHistoryTest.TestDefaultCompensationHandlerHistory.bpmn20.xml" })]
        public virtual void testDefaultCompensationHandlerHistoryActivityInstance()
        {
            // given a process instance
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("defaultHandlerProcess");

            // when throwing compensation
            ITask beforeCompensationTask = taskService.CreateTaskQuery().First();
            taskService.Complete(beforeCompensationTask.Id);

            IActivityInstance tree = runtimeService.GetActivityInstance(processInstance.Id);
            string compensationHandlerActivityInstanceId = tree.GetActivityInstances("compensationHandler")[0].Id;

            string subProcessActivityInstanceId = tree.GetActivityInstances("subProcess")[0].Id;

            // . and completing compensation
            ITask compensationHandler = taskService.CreateTaskQuery().First();
            taskService.Complete(compensationHandler.Id);

            // then there is a historic activity instance for the compensation handler
            IHistoricActivityInstance historicCompensationHandlerInstance = historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "compensationHandler").First();

            Assert.NotNull(historicCompensationHandlerInstance);
            Assert.AreEqual(compensationHandlerActivityInstanceId, historicCompensationHandlerInstance.Id);
            Assert.AreEqual(subProcessActivityInstanceId, historicCompensationHandlerInstance.ParentActivityInstanceId);
        }

        //[Test]
        [Deployment(new string[] { "resources/bpmn/event/compensate/CompensateEventHistoryTest.TestDefaultCompensationHandlerHistory.bpmn20.xml" })]
        public virtual void FAILING_testDefaultCompensationHandlerHistoryVariableInstance()
        {
            // given a process instance
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("defaultHandlerProcess");

            // when throwing compensation
            ITask beforeCompensationTask = taskService.CreateTaskQuery().First();
            taskService.Complete(beforeCompensationTask.Id);

            IActivityInstance tree = runtimeService.GetActivityInstance(processInstance.Id);
            string compensationHandlerActivityInstanceId = tree.GetActivityInstances("compensationHandler")[0].Id;

            // . setting a variable via task service API
            ITask compensationHandler = taskService.CreateTaskQuery().First();
            runtimeService.SetVariableLocal(compensationHandler.ExecutionId, "apiVariable", "someValue");

            // . and completing compensation
            taskService.Complete(compensationHandler.Id);

            // then there is a historic variable instance for the variable set by API
            IHistoricVariableInstance historicVariableInstance = historyService.CreateHistoricVariableInstanceQuery().First();

            Assert.NotNull(historicVariableInstance);
            Assert.AreEqual(compensationHandlerActivityInstanceId, historicVariableInstance.ActivityInstanceId);
        }


    }

}