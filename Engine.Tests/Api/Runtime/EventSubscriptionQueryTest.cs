using System;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class EventSubscriptionQueryTest : PluggableProcessEngineTestCase
    {
        [Test]
        public virtual void TestQueryByEventSubscriptionId()
        {
            CreateExampleEventSubscriptions();

            var list = runtimeService.CreateEventSubscriptionQuery(c=> c.EventName== "messageName2")
                
                .ToList();
            Assert.AreEqual(1, list.Count);

            var eventSubscription = list[0];

            var query = runtimeService.CreateEventSubscriptionQuery(c=> c.Id == eventSubscription.Id);

            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(1, query
                .Count());
            Assert.NotNull(query.First());

            try
            {
                runtimeService.CreateEventSubscriptionQuery(c=> c.Id== null)
                    
                    .ToList();
                Assert.Fail("Expected ProcessEngineException");
            }
            catch (ProcessEngineException)
            {
            }

            CleanDb();
        }

        [Test]
        public virtual void TestQueryByEventName()
        {
            CreateExampleEventSubscriptions();

            var list = runtimeService.CreateEventSubscriptionQuery(c=> c.EventName== "messageName")
                
                .ToList();
            Assert.AreEqual(2, list.Count);

            list = runtimeService.CreateEventSubscriptionQuery(c=> c.EventName== "messageName2")
                
                .ToList();
            Assert.AreEqual(1, list.Count);

            try
            {
                runtimeService.CreateEventSubscriptionQuery(c=> c.EventName== null)
                    
                    .ToList();
                Assert.Fail("Expected ProcessEngineException");
            }
            catch (ProcessEngineException)
            {
            }

            CleanDb();
        }

        [Test]
        public virtual void TestQueryByEventType()
        {
            CreateExampleEventSubscriptions();

            var list = runtimeService.CreateEventSubscriptionQuery(c=> c.EventType== "signal")
                
                .ToList();
            Assert.AreEqual(1, list.Count);

            list = runtimeService.CreateEventSubscriptionQuery(c=> c.EventType== "message")
                
                .ToList();
            Assert.AreEqual(2, list.Count);

            try
            {
                runtimeService.CreateEventSubscriptionQuery(c=> c.EventType== null)
                    
                    .ToList();
                Assert.Fail("Expected ProcessEngineException");
            }
            catch (ProcessEngineException)
            {
            }

            CleanDb();
        }

        [Test]
        public virtual void TestQueryByActivityId()
        {
            CreateExampleEventSubscriptions();

            var list = runtimeService.CreateEventSubscriptionQuery(c=> c.ActivityId =="someOtherActivity")
                
                .ToList();
            Assert.AreEqual(1, list.Count);

            list = runtimeService.CreateEventSubscriptionQuery(c=> c.ActivityId =="someActivity" &&  c.EventType== "message")
                
                .ToList();
            Assert.AreEqual(2, list.Count);

            try
            {
                runtimeService.CreateEventSubscriptionQuery(c=> c.ActivityId ==null)
                    
                    .ToList();
                Assert.Fail("Expected ProcessEngineException");
            }
            catch (ProcessEngineException)
            {
            }

            CleanDb();
        }

        [Test][Deployment ]
        public virtual void TestQueryByExecutionId()
        {
            // starting two instances:
            var processInstance = runtimeService.StartProcessInstanceByKey("catchSignal");
            runtimeService.StartProcessInstanceByKey("catchSignal");

            // test query by process instance id
            var subscription = runtimeService.CreateEventSubscriptionQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();
            Assert.NotNull(subscription);

            var executionWaitingForSignal = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "signalEvent")
                .Where(c=>c.ProcessInstanceId==processInstance.Id)
                .First();

            // test query by execution id
            var signalSubscription = runtimeService.CreateEventSubscriptionQuery(c=>c.ExecutionId ==executionWaitingForSignal.Id)
                .First();
            Assert.NotNull(signalSubscription);

            Assert.AreEqual(signalSubscription, subscription);

            try
            {
                runtimeService.CreateEventSubscriptionQuery(c=>c.ExecutionId== null)
                    
                    .ToList();
                Assert.Fail("Expected ProcessEngineException");
            }
            catch (ProcessEngineException)
            {
            }

            CleanDb();
        }

        [Test]
        public virtual void TestQuerySorting()
        {
            CreateExampleEventSubscriptions();
            var eventSubscriptions = runtimeService.CreateEventSubscriptionQuery()
                //.OrderByCreated()
                /*.Asc()*/
                
                .ToList();
            Assert.AreEqual(3, eventSubscriptions.Count);

            Assert.True(eventSubscriptions[0].Created.CompareTo(eventSubscriptions[1].Created) < 0);
            Assert.True(eventSubscriptions[1].Created.CompareTo(eventSubscriptions[2].Created) < 0);

            CleanDb();
        }

        [Test]
        [Deployment]
        public virtual void TestMultipleEventSubscriptions()
        {
            var message = "cancelation-requested";

            var processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            //Assert.True(TestHelper.AreJobsAvailable());

            var eventSubscriptionCount = runtimeService.CreateEventSubscriptionQuery()
                .Count();
            Assert.AreEqual(2, eventSubscriptionCount);

            var messageEvent = runtimeService.CreateEventSubscriptionQuery(c=> c.EventType== "message")
                .First();
            Assert.AreEqual(message, messageEvent.EventName);

            var compensationEvent = runtimeService.CreateEventSubscriptionQuery(c=> c.EventType== "compensate")
                .First();
            Assert.IsNull(compensationEvent.EventName);

            runtimeService.CreateMessageCorrelation(message)
                .ProcessInstanceId(processInstance.Id)
                .Correlate();

            AssertProcessEnded(processInstance.Id);
        }


        protected internal virtual void CreateExampleEventSubscriptions()
        {
            processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this));
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly EventSubscriptionQueryTest _outerInstance;

            public CommandAnonymousInnerClass(EventSubscriptionQueryTest outerInstance)
            {
                _outerInstance = outerInstance;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                var calendar = new DateTime(); // new GregorianCalendar();


                var messageEventSubscriptionEntity1 = new EventSubscriptionEntity(EventType.Message);
                messageEventSubscriptionEntity1.EventName = "messageName";
                messageEventSubscriptionEntity1.ActivityId = "someActivity";
                calendar = new DateTime(2001, 1, 1);
                messageEventSubscriptionEntity1.Created = calendar;
                messageEventSubscriptionEntity1.Insert();

                var messageEventSubscriptionEntity2 = new EventSubscriptionEntity(EventType.Message);
                messageEventSubscriptionEntity2.EventName = "messageName";
                messageEventSubscriptionEntity2.ActivityId = "someActivity";
                calendar = new DateTime(2000, 1, 1);
                messageEventSubscriptionEntity2.Created = calendar;
                messageEventSubscriptionEntity2.Insert();

                var signalEventSubscriptionEntity3 = new EventSubscriptionEntity(EventType.Signal);
                signalEventSubscriptionEntity3.EventName = "messageName2";
                signalEventSubscriptionEntity3.ActivityId = "someOtherActivity";
                calendar = new DateTime(2002, 1, 1);
                signalEventSubscriptionEntity3.Created = calendar;
                signalEventSubscriptionEntity3.Insert();

                return null;
            }
        }

        protected internal virtual void CleanDb()
        {
            processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass2(this));
        }

        private class CommandAnonymousInnerClass2 : ICommand<object>
        {
            private readonly EventSubscriptionQueryTest _outerInstance;

            public CommandAnonymousInnerClass2(EventSubscriptionQueryTest outerInstance)
            {
                _outerInstance = outerInstance;
            }

            public virtual object Execute(CommandContext commandContext)
            {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.Camunda.bpm.Engine.Runtime.IEventSubscription> subscriptions = new org.Camunda.bpm.Engine.impl.IQueryable<IEventSubscription>().ToList();
                //var subscriptions = new IQueryable<IEventSubscription>()
                //    .ToList();
                //foreach (var eventSubscriptionEntity in subscriptions)
                //    ((EventSubscriptionEntity) eventSubscriptionEntity).Delete();
                return null;
            }
        }
    }
}