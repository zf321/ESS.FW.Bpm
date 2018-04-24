using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Signal
{
    [TestFixture]
    public class SignalEventReceivedBuilderTest : PluggableProcessEngineTestCase
    {

        protected internal virtual IBpmnModelInstance signalStartProcess(string processId)
        {
            return ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(processId).StartEvent().Signal("signal").UserTask().EndEvent().Done();
        }

        protected internal virtual IBpmnModelInstance signalCatchProcess(string processId)
        {
            return ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(processId).StartEvent().IntermediateCatchEvent().Signal("signal").UserTask().EndEvent().Done();
        }

        public virtual void testSendSignalToStartEvent()
        {
            Deployment(signalStartProcess("signalStart"));

            runtimeService.CreateSignalEvent("signal").Send();

            Assert.That(taskService.CreateTaskQuery().Count(), Is.EqualTo(1L));
        }

        public virtual void testSendSignalToIntermediateCatchEvent()
        {
            Deployment(signalCatchProcess("signalCatch"));

            runtimeService.StartProcessInstanceByKey("signalCatch");

            runtimeService.CreateSignalEvent("signal").Send();

            Assert.That(taskService.CreateTaskQuery().Count(), Is.EqualTo(1L));
        }

        public virtual void testSendSignalToStartAndIntermediateCatchEvent()
        {
            Deployment(signalStartProcess("signalStart"), signalCatchProcess("signalCatch"));

            runtimeService.StartProcessInstanceByKey("signalCatch");

            runtimeService.CreateSignalEvent("signal").Send();

            Assert.That(taskService.CreateTaskQuery().Count(), Is.EqualTo(2L));
        }

        public virtual void testSendSignalToMultipleStartEvents()
        {
            Deployment(signalStartProcess("signalStart"), signalStartProcess("signalStart2"));

            runtimeService.CreateSignalEvent("signal").Send();

            Assert.That(taskService.CreateTaskQuery().Count(), Is.EqualTo(2L));
        }

        public virtual void testSendSignalToMultipleIntermediateCatchEvents()
        {
            Deployment(signalCatchProcess("signalCatch"), signalCatchProcess("signalCatch2"));

            runtimeService.StartProcessInstanceByKey("signalCatch");
            runtimeService.StartProcessInstanceByKey("signalCatch2");

            runtimeService.CreateSignalEvent("signal").Send();

            Assert.That(taskService.CreateTaskQuery().Count(), Is.EqualTo(2L));
        }

        public virtual void testSendSignalWithExecutionId()
        {
            Deployment(signalCatchProcess("signalCatch"), signalCatchProcess("signalCatch2"));

            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("signalCatch");
            runtimeService.StartProcessInstanceByKey("signalCatch2");

            IEventSubscription eventSubscription = runtimeService.CreateEventSubscriptionQuery(c=>c.ProcessInstanceId == processInstance.Id).First();
            string ExecutionId = eventSubscription.ExecutionId;

            runtimeService.CreateSignalEvent("signal").SetExecutionId(ExecutionId).Send();

            Assert.That(taskService.CreateTaskQuery().Count(), Is.EqualTo(1L));
        }

        public virtual void testSendSignalToStartEventWithVariables()
        {
            Deployment(signalStartProcess("signalStart"));

            IDictionary<string, object> variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("var1", "a").PutValue("var2", "b");

            runtimeService.CreateSignalEvent("signal").SetVariables(variables).Send();

            IExecution execution = runtimeService.CreateExecutionQuery().First();
            Assert.That(runtimeService.GetVariables(execution.Id), Is.EqualTo(variables));
        }

        public virtual void testSendSignalToIntermediateCatchEventWithVariables()
        {
            Deployment(signalCatchProcess("signalCatch"));

            runtimeService.StartProcessInstanceByKey("signalCatch");

            IDictionary<string, object> variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("var1", "a").PutValue("var2", "b");

            runtimeService.CreateSignalEvent("signal").SetVariables(variables).Send();

            IExecution execution = runtimeService.CreateExecutionQuery().First();
            Assert.That(runtimeService.GetVariables(execution.Id), Is.EqualTo(variables));
        }

        public virtual void testNoSignalEventSubscription()
        {
            // assert that no exception is thrown
            runtimeService.CreateSignalEvent("signal").Send();
        }

        public virtual void testNonExistingExecutionId()
        {

            try
            {
                runtimeService.CreateSignalEvent("signal").SetExecutionId("nonExisting").Send();

            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("Cannot find execution with id 'nonExisting'"));
            }
        }

        public virtual void testNoSignalEventSubscriptionWithExecutionId()
        {
            Deployment(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("noSignal").StartEvent().UserTask().EndEvent().Done());

            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("noSignal");
            string ExecutionId = processInstance.Id;

            try
            {
                runtimeService.CreateSignalEvent("signal").SetExecutionId(ExecutionId).Send();

            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("IExecution '" + ExecutionId + "' has not subscribed to a signal event with name 'signal'"));
            }
        }

    }

}