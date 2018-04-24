using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.Impl.Event;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Signal
{
    [TestFixture]
    public class SignalEventDeploymentTest : PluggableProcessEngineTestCase
    {

        private const string SIGNAL_START_EVENT_PROCESS = "resources/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml";
        private const string SIGNAL_START_EVENT_PROCESS_NEW_VERSION = "resources/bpmn/event/signal/SignalEventTest.signalStartEvent_v2.bpmn20.xml";
        [Test]
        public virtual void testCreateEventSubscriptionOnDeployment()
        {
            DeploymentId = repositoryService.CreateDeployment().AddClasspathResource(SIGNAL_START_EVENT_PROCESS).Deploy().Id;

            IEventSubscription eventSubscription = runtimeService.CreateEventSubscriptionQuery().First();
            Assert.NotNull(eventSubscription);

            Assert.AreEqual(EventType.Signal.ToString(), eventSubscription.EventType);
            Assert.AreEqual("alert", eventSubscription.EventName);
            Assert.AreEqual("start", eventSubscription.ActivityId);
        }
        [Test]
        public virtual void testUpdateEventSubscriptionOnDeployment()
        {
            DeploymentId = repositoryService.CreateDeployment().AddClasspathResource(SIGNAL_START_EVENT_PROCESS).Deploy().Id;

            IEventSubscription eventSubscription = runtimeService.CreateEventSubscriptionQuery(c=>c.EventType == "signal").First();
            Assert.NotNull(eventSubscription);
            Assert.AreEqual("alert", eventSubscription.EventName);

            // deploy a new version of the process with different signal name
            string newDeploymentId = repositoryService.CreateDeployment().AddClasspathResource(SIGNAL_START_EVENT_PROCESS_NEW_VERSION).Deploy().Id;

            IProcessDefinition newProcessDefinition = repositoryService.CreateProcessDefinitionQuery()/*.LatestVersion()*/.First();
            Assert.AreEqual(2, newProcessDefinition.Version);

            IList<IEventSubscription> newEventSubscriptions = runtimeService.CreateEventSubscriptionQuery(c=>c.EventType == "signal").ToList();
            // only one event subscription for the new version of the process definition
            Assert.AreEqual(1, newEventSubscriptions.Count);

            var tmp = newEventSubscriptions.GetEnumerator();
            tmp.MoveNext();
            EventSubscriptionEntity newEventSubscription = (EventSubscriptionEntity)tmp.Current;
            Assert.AreEqual(newProcessDefinition.Id, newEventSubscription.Configuration);
            Assert.AreEqual("abort", newEventSubscription.EventName);

            // clean db
            repositoryService.DeleteDeployment(newDeploymentId);
        }
        [Test]
        public virtual void testAsyncSignalStartEventDeleteDeploymentWhileAsync()
        {
            // given a deployment
            IDeployment deployment = repositoryService.CreateDeployment()
                .AddClasspathResource("resources/bpmn/event/signal/SignalEventTest.signalStartEvent.bpmn20.xml")
                .AddClasspathResource("resources/bpmn/event/signal/SignalEventTests.throwAlertSignalAsync.bpmn20.xml").Deploy();

            // and an active job for asynchronously triggering a signal start event
            runtimeService.StartProcessInstanceByKey("throwSignalAsync");

            // then deleting the deployment succeeds
            repositoryService.DeleteDeployment(deployment.Id, true);

            Assert.AreEqual(0, repositoryService.CreateDeploymentQuery().Count());

            int historyLevel = processEngineConfiguration.HistoryLevel.Id;
            if (historyLevel >= HistoryLevelFields.HistoryLevelFull.Id)
            {
                // and there are no job logs left
                Assert.AreEqual(0, historyService.CreateHistoricJobLogQuery().Count());
            }

        }

    }

}