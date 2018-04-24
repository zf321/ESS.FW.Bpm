//using System.Linq;
//using ESS.FW.Bpm.Engine.History;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.History
//{
//    /// <summary>
//    /// </summary>
//    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryAudit)]
//    [TestFixture]
//    public class HistoricActivityInstanceSequenceCounterTest : PluggableProcessEngineTestCase
//    {
//        [Test][Deployment("resources/standalone/entity/ExecutionSequenceCounterTest.TestForkAndJoinDifferentSequenceLength.bpmn20.xml") ]
//        public virtual void testForkAndJoin()
//        {
//            // given
//            var query = historyService.CreateHistoricActivityInstanceQuery()
//                //.OrderPartiallyByOccurrence()
//                /*.Asc()*/;

//            // when
//            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process")
//                .Id;

//            // then
//            query.ExecutionId(ProcessInstanceId);
//            verifyOrder(query, "theStart", "theService", "fork", "join", "theService4", "theEnd");

//            var firstExecutionId = historyService.CreateHistoricActivityInstanceQuery()
//                .ActivityId("theService1")
//                .First()
//                .ExecutionId;

//            var secondExecutionId = historyService.CreateHistoricActivityInstanceQuery()
//                .ActivityId("theService2")
//                .First()
//                .ExecutionId;

//            query.ExecutionId(firstExecutionId);
//            verifyOrder(query, "theService1", "join");

//            query.ExecutionId(secondExecutionId);
//            verifyOrder(query, "theService2", "theService3");

//            query = historyService.CreateHistoricActivityInstanceQuery()
//                //.OrderPartiallyByOccurrence()
//                /*.Asc()*/
//                /*.OrderByActivityId()*/
//                /*.Asc()*/;
//            verifyOrder(query, "theStart", "theService", "fork", "theService1", "theService2", "join", "theService3",
//                "join", "theService4", "theEnd");
//        }

//        [Test][Deployment("resources/standalone/entity/ExecutionSequenceCounterTest.TestSequenceInsideSubProcess.bpmn20.xml") ]
//        public virtual void testSequenceInsideSubProcess()
//        {
//            // given
//            var query = historyService.CreateHistoricActivityInstanceQuery()
//                //.OrderPartiallyByOccurrence()
//                /*.Asc()*/;

//            // when
//            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process")
//                .Id;

//            // then
//            query.ExecutionId(ProcessInstanceId);
//            verifyOrder(query, "theStart", "theService1", "theService2", "theEnd");

//            var subProcessExecutionId = historyService.CreateHistoricActivityInstanceQuery()
//                .ActivityId("subProcess")
//                .First()
//                .ExecutionId;

//            query.ExecutionId(subProcessExecutionId);
//            verifyOrder(query, "subProcess", "innerStart", "innerService", "innerEnd");

//            query = historyService.CreateHistoricActivityInstanceQuery()
//                //.OrderPartiallyByOccurrence()
//                /*.Asc()*/;
//            verifyOrder(query, "theStart", "theService1", "subProcess", "innerStart", "innerService", "innerEnd",
//                "theService2", "theEnd");
//        }
//        [Test][Deployment( "resources/standalone/entity/ExecutionSequenceCounterTest.TestSequentialMultiInstance.bpmn20.xml") ]
//        public virtual void testSequentialMultiInstance()
//        {
//            // given
//            var query = historyService.CreateHistoricActivityInstanceQuery()
//                //.OrderPartiallyByOccurrence()
//                /*.Asc()*/;

//            // when
//            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process")
//                .Id;

//            // then
//            query.ExecutionId(ProcessInstanceId);
//            verifyOrder(query, "theStart", "theService1", "theService3", "theEnd");

//            var taskExecutionId = historyService.CreateHistoricActivityInstanceQuery(c=>c.ActivityId == "theService2").First().ExecutionId;

//            query.ExecutionId(taskExecutionId);
//            verifyOrder(query, "theService2#multiInstanceBody", "theService2", "theService2");

//            query = historyService.CreateHistoricActivityInstanceQuery()
//                ////.OrderPartiallyByOccurrence()
//                ///*.Asc()*/
//                ;
//            verifyOrder(query, "theStart", "theService1", "theService2#multiInstanceBody", "theService2", "theService2",
//                "theService3", "theEnd");
//        }
//        [Deployment( "resources/standalone/entity/ExecutionSequenceCounterTest.TestParallelMultiInstance.bpmn20.xml") ]
//        public virtual void testParallelMultiInstance()
//        {
//            // given
//            var query = historyService.CreateHistoricActivityInstanceQuery()
//                ////.OrderPartiallyByOccurrence()
//                ///*.Asc()*/
//                ;

//            // when
//            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process")
//                .Id;

//            // then
//            query.ExecutionId(ProcessInstanceId);
//            verifyOrder(query, "theStart", "theService1", "theService3", "theEnd");

//            var taskActivityInstances = historyService.CreateHistoricActivityInstanceQuery()
//                .ActivityId("theService2")
//                
//                .ToList();
//            foreach (var activityInstance in taskActivityInstances)
//            {
//                query.ExecutionId(activityInstance.ExecutionId);
//                verifyOrder(query, "theService2");
//            }

//            query = historyService.CreateHistoricActivityInstanceQuery()
//                //.OrderPartiallyByOccurrence()
//                /*.Asc()*/;
//            verifyOrder(query, "theStart", "theService1", "theService2#multiInstanceBody", "theService2", "theService2",
//                "theService3", "theEnd");
//        }

//        [Test][Deployment("resources/standalone/entity/ExecutionSequenceCounterTest.TestLoop.bpmn20.xml") ]
//        public virtual void testLoop()
//        {
//            // given
//            var query = historyService.CreateHistoricActivityInstanceQuery()
//                //.OrderPartiallyByOccurrence()
//                /*.Asc()*/;

//            // when
//            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process")
//                .Id;

//            // then
//            query.ExecutionId(ProcessInstanceId);
//            verifyOrder(query, "theStart", "theService1", "join", "theScript", "fork", "join", "theScript", "fork",
//                "theService2", "theEnd");
//        }

//        [Test][Deployment( "resources/standalone/entity/ExecutionSequenceCounterTest.TestInterruptingBoundaryEvent.bpmn20.xml") ]
//        public virtual void testInterruptingBoundaryEvent()
//        {
//            // given
//            var query = historyService.CreateHistoricActivityInstanceQuery()
//                //.OrderPartiallyByOccurrence()
//                /*.Asc()*/;

//            // when
//            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process")
//                .Id;
//            runtimeService.CorrelateMessage("newMessage");

//            // then
//            verifyOrder(query, "theStart", "theService1", "theTask", "messageBoundary", "theServiceAfterMessage",
//                "theEnd2");

//            query.ExecutionId(ProcessInstanceId);
//            verifyOrder(query, "theStart", "theService1", "messageBoundary", "theServiceAfterMessage", "theEnd2");

//            var taskExecutionId = historyService.CreateHistoricActivityInstanceQuery()
//                .ActivityId("theTask")
//                .First()
//                .ExecutionId;

//            query.ExecutionId(taskExecutionId);
//            verifyOrder(query, "theTask");
//        }

//        [Test][Deployment( "resources/standalone/entity/ExecutionSequenceCounterTest.TestNonInterruptingBoundaryEvent.bpmn20.xml")]
//        public virtual void testNonInterruptingBoundaryEvent()
//        {
//            // given
//            var query = historyService.CreateHistoricActivityInstanceQuery()
//                //.OrderPartiallyByOccurrence()
//                /*.Asc()*/;

//            // when
//            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process")
//                .Id;
//            runtimeService.CorrelateMessage("newMessage");
//            runtimeService.CorrelateMessage("newMessage");
//            var taskId = taskService.CreateTaskQuery()
//                .First()
//                .Id;
//            taskService.Complete(taskId);

//            // then
//            query.ExecutionId(ProcessInstanceId);
//            verifyOrder(query, "theStart", "theService1", "theEnd1");

//            var taskExecutionId = historyService.CreateHistoricActivityInstanceQuery()
//                .ActivityId("theTask")
//                .First()
//                .ExecutionId;

//            query.ExecutionId(taskExecutionId);
//            verifyOrder(query, "theTask");

//            var activityInstances = historyService.CreateHistoricActivityInstanceQuery()
//                .ActivityId("messageBoundary")
//                
//                .ToList();
//            foreach (var historicActivityInstance in activityInstances)
//            {
//                query.ExecutionId(historicActivityInstance.ExecutionId);
//                verifyOrder(query, "messageBoundary", "theServiceAfterMessage", "theEnd2");
//            }

//            query = historyService.CreateHistoricActivityInstanceQuery()
//                //.OrderPartiallyByOccurrence()
//                /*.Asc()*/
//                /*.OrderByActivityId()*/
//                /*.Asc()*/;

//            verifyOrder(query, "theStart", "theService1", "messageBoundary", "theTask", "theServiceAfterMessage",
//                "theEnd2", "messageBoundary", "theServiceAfterMessage", "theEnd2", "theEnd1");
//        }

//        protected internal virtual void verifyOrder(IQueryable<IHistoricActivityInstance> query, params string[] expectedOrder)
//        {
//            Assert.AreEqual(expectedOrder.Length, query.Count());

//            var activityInstances = query
//                .ToList();

//            for (var i = 0; i < expectedOrder.Length; i++)
//            {
//                var activityInstance = activityInstances[i];
//                var currentActivityId = activityInstance.ActivityId;
//                var expectedActivityId = expectedOrder[i];
//                Assert.AreEqual(expectedActivityId, currentActivityId);
//            }
//        }

//        [Test]
//        [Deployment(
//            "resources/standalone/entity/ExecutionSequenceCounterTest.TestForkSameSequenceLengthWithoutWaitStates.bpmn20.xml"
//        )]
//        public virtual void testFork()
//        {
//            // given
//            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process")
//                .Id;

//            var query = historyService.CreateHistoricActivityInstanceQuery()
//                //.OrderPartiallyByOccurrence()
//                /*.Asc()*/;

//            // when

//            // then
//            query.ExecutionId(ProcessInstanceId);
//            verifyOrder(query, "theStart", "theService", "fork", "theService2", "theEnd2");

//            var firstExecutionId = historyService.CreateHistoricActivityInstanceQuery()
//                .ActivityId("theService1")
//                .First()
//                .ExecutionId;
//            query.ExecutionId(firstExecutionId);
//            verifyOrder(query, "theService1", "theEnd1");

//            query = historyService.CreateHistoricActivityInstanceQuery()
//                //.OrderPartiallyByOccurrence()
//                /*.Asc()*/;
//            verifyOrder(query, "theStart", "theService", "fork", "theService1", "theEnd1", "theService2", "theEnd2");
//        }

//        [Test]
//        [Deployment("resources/standalone/entity/ExecutionSequenceCounterTest.TestSequence.bpmn20.xml")]
//        public virtual void testSequence()
//        {
//            // given
//            var query = historyService.CreateHistoricActivityInstanceQuery()
//                //.OrderPartiallyByOccurrence()
//                /*.Asc()*/;

//            // when
//            runtimeService.StartProcessInstanceByKey("process");

//            // then
//            verifyOrder(query, "theStart", "theService1", "theService2", "theEnd");
//        }
//    }
//}