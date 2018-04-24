using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Entity
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class ExecutionSequenceCounterTest : PluggableProcessEngineTestCase
    {
        [SetUp]
        public virtual void SetUp()
        {
            ExecutionOrderListener.ClearActivityExecutionOrder();
        }

        [Test]
        [Deployment]
        public virtual void TestSequence()
        {
            // given
            var processInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            // when

            // then
            AssertProcessEnded(processInstanceId);

            var order = ExecutionOrderListener.ActivityExecutionOrder;
            VerifyOrder(order, "theStart", "theService1", "theService2", "theEnd");
        }


        [Test]
        [Deployment]
        public virtual void TestForkSameSequenceLengthWithoutWaitStates()
        {
            // given
            var processInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            // when

            // then
            AssertProcessEnded(processInstanceId);

            var order = ExecutionOrderListener.ActivityExecutionOrder;
            VerifyOrder(order, "theStart", "theService", "fork", "theService1", "theEnd1", "theService2", "theEnd2");
        }


        [Test]
        [Deployment]
        public virtual void TestForkSameSequenceLengthWithAsyncEndEvent()
        {
            // given
            var processInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            var jobQuery = managementService.CreateJobQuery();

            // when (1)

            // then (1)
            var order = ExecutionOrderListener.ActivityExecutionOrder;
            Assert.AreEqual(5, order.Count);

            long lastSequenceCounter = 0;

            var theStartElement = order[0];
            Assert.AreEqual("theStart", theStartElement.ActivityId);
            Assert.True(theStartElement.SequenceCounter > lastSequenceCounter);
            lastSequenceCounter = theStartElement.SequenceCounter;

            var theForkElement = order[1];
            Assert.AreEqual("theService", theForkElement.ActivityId);
            Assert.True(theForkElement.SequenceCounter > lastSequenceCounter);
            lastSequenceCounter = theForkElement.SequenceCounter;

            var theServiceElement = order[2];
            Assert.AreEqual("fork", theServiceElement.ActivityId);
            Assert.True(theServiceElement.SequenceCounter > lastSequenceCounter);
            lastSequenceCounter = theServiceElement.SequenceCounter;

            var theService1Element = order[3];
            Assert.AreEqual("theService1", theService1Element.ActivityId);
            Assert.True(theService1Element.SequenceCounter > lastSequenceCounter);

            var theService2Element = order[4];
            Assert.AreEqual("theService2", theService2Element.ActivityId);
            Assert.True(theService2Element.SequenceCounter > lastSequenceCounter);

            // when (2)
            //var jobId = jobQuery.ActivityId("theEnd1").First().Id;
            //managementService.ExecuteJob(jobId);

            // then (2)
            order = ExecutionOrderListener.ActivityExecutionOrder;
            Assert.AreEqual(6, order.Count);

            var theEnd1Element = order[5];
            Assert.AreEqual("theEnd1", theEnd1Element.ActivityId);
            Assert.True(theEnd1Element.SequenceCounter > theService1Element.SequenceCounter);

            // when (3)
            //jobId = jobQuery.ActivityId("theEnd2").First().Id;
            //managementService.ExecuteJob(jobId);

            // then (3)
            AssertProcessEnded(processInstanceId);

            order = ExecutionOrderListener.ActivityExecutionOrder;
            Assert.AreEqual(7, order.Count);

            var theEnd2Element = order[6];
            Assert.AreEqual("theEnd2", theEnd2Element.ActivityId);
            Assert.True(theEnd2Element.SequenceCounter > theService2Element.SequenceCounter);
        }


        [Test]
        [Deployment]
        public virtual void TestForkDifferentSequenceLengthWithoutWaitStates()
        {
            // given
            var processInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            // when

            // then
            AssertProcessEnded(processInstanceId);

            var order = ExecutionOrderListener.ActivityExecutionOrder;
            VerifyOrder(order, "theStart", "theService", "fork", "theService1", "theEnd1", "theService2", "theService3",
                "theEnd2");
        }

        [Test]
        [Deployment]
        public virtual void TestForkDifferentSequenceLengthWithAsyncEndEvent()
        {
            // given
            var processInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            var jobQuery = managementService.CreateJobQuery();

            // when (1)

            // then (1)
            var order = ExecutionOrderListener.ActivityExecutionOrder;
            Assert.AreEqual(6, order.Count);

            long lastSequenceCounter = 0;

            var theStartElement = order[0];
            Assert.AreEqual("theStart", theStartElement.ActivityId);
            Assert.True(theStartElement.SequenceCounter > lastSequenceCounter);
            lastSequenceCounter = theStartElement.SequenceCounter;

            var theForkElement = order[1];
            Assert.AreEqual("theService", theForkElement.ActivityId);
            Assert.True(theForkElement.SequenceCounter > lastSequenceCounter);
            lastSequenceCounter = theForkElement.SequenceCounter;

            var theServiceElement = order[2];
            Assert.AreEqual("fork", theServiceElement.ActivityId);
            Assert.True(theServiceElement.SequenceCounter > lastSequenceCounter);
            lastSequenceCounter = theServiceElement.SequenceCounter;

            var theService1Element = order[3];
            Assert.AreEqual("theService1", theService1Element.ActivityId);
            Assert.True(theService1Element.SequenceCounter > lastSequenceCounter);

            var theService2Element = order[4];
            Assert.AreEqual("theService2", theService2Element.ActivityId);
            Assert.True(theService2Element.SequenceCounter > lastSequenceCounter);

            var theService3Element = order[5];
            Assert.AreEqual("theService3", theService3Element.ActivityId);
            Assert.True(theService3Element.SequenceCounter > theService2Element.SequenceCounter);

            // when (2)
                        //var jobId = jobQuery.ActivityId("theEnd1").First().Id;
            //managementService.ExecuteJob(jobId);

            // then (2)
            order = ExecutionOrderListener.ActivityExecutionOrder;
            Assert.AreEqual(7, order.Count);

            var theEnd1Element = order[6];
            Assert.AreEqual("theEnd1", theEnd1Element.ActivityId);
            Assert.True(theEnd1Element.SequenceCounter > theService1Element.SequenceCounter);

            // when (3)
            //jobId = jobQuery.ActivityId("theEnd2").First().Id;
            //managementService.ExecuteJob(jobId);

            // then (3)
            AssertProcessEnded(processInstanceId);

            order = ExecutionOrderListener.ActivityExecutionOrder;
            Assert.AreEqual(8, order.Count);

            var theEnd2Element = order[7];
            Assert.AreEqual("theEnd2", theEnd2Element.ActivityId);
            Assert.True(theEnd2Element.SequenceCounter > theService3Element.SequenceCounter);
        }

        [Test]
        [Deployment]
        public virtual void TestForkReplaceBy()
        {
            // given
            var processInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            var jobQuery = managementService.CreateJobQuery();

            // when (1)

            // then (1)
            var order = ExecutionOrderListener.ActivityExecutionOrder;
            Assert.AreEqual(2, order.Count);

            var theService1Element = order[0];
            Assert.AreEqual("theService1", theService1Element.ActivityId);

            var theService3Element = order[1];
            Assert.AreEqual("theService3", theService3Element.ActivityId);

            Assert.True(theService1Element.SequenceCounter == theService3Element.SequenceCounter);

            // when (2)
            //var jobId = jobQuery.ActivityId("theService4").First().Id;
            //managementService.ExecuteJob(jobId);

            // then (2)
            order = ExecutionOrderListener.ActivityExecutionOrder;
            Assert.AreEqual(5, order.Count);

            var theService4Element = order[2];
            Assert.AreEqual("theService4", theService4Element.ActivityId);
            Assert.True(theService4Element.SequenceCounter > theService3Element.SequenceCounter);

            var theService5Element = order[3];
            Assert.AreEqual("theService5", theService5Element.ActivityId);
            Assert.True(theService5Element.SequenceCounter > theService4Element.SequenceCounter);

            var theEnd2Element = order[4];
            Assert.AreEqual("theEnd2", theEnd2Element.ActivityId);
            Assert.True(theEnd2Element.SequenceCounter > theService5Element.SequenceCounter);

            // when (3)
            //jobId = jobQuery.ActivityId("theService2").First().Id;
            //managementService.ExecuteJob(jobId);

            // then (3)
            order = ExecutionOrderListener.ActivityExecutionOrder;
            Assert.AreEqual(7, order.Count);

            var theService2Element = order[5];
            Assert.AreEqual("theService2", theService2Element.ActivityId);
            Assert.True(theService2Element.SequenceCounter > theService1Element.SequenceCounter);
            Assert.True(theService2Element.SequenceCounter > theEnd2Element.SequenceCounter);

            var theEnd1Element = order[6];
            Assert.AreEqual("theEnd1", theEnd1Element.ActivityId);
            Assert.True(theEnd1Element.SequenceCounter > theService2Element.SequenceCounter);

            AssertProcessEnded(processInstanceId);
        }


        [Test]
        [Deployment( new string[]{"resources/standalone/entity/ExecutionSequenceCounterTest.TestForkReplaceBy.bpmn20.xml"})]
        public virtual void TestForkReplaceByAnotherExecutionOrder()
        {
            // given
            var processInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;
            var jobQuery = managementService.CreateJobQuery();

            // when (1)

            // then (1)
            var order = ExecutionOrderListener.ActivityExecutionOrder;
            Assert.AreEqual(2, order.Count);

            var theService1Element = order[0];
            Assert.AreEqual("theService1", theService1Element.ActivityId);

            var theService3Element = order[1];
            Assert.AreEqual("theService3", theService3Element.ActivityId);

            Assert.True(theService1Element.SequenceCounter == theService3Element.SequenceCounter);

            // when (2)
            //var jobId = jobQuery.ActivityId("theService2").First().Id;
            //managementService.ExecuteJob(jobId);

            // then (2)
            order = ExecutionOrderListener.ActivityExecutionOrder;
            Assert.AreEqual(4, order.Count);

            var theService2Element = order[2];
            Assert.AreEqual("theService2", theService2Element.ActivityId);
            Assert.True(theService2Element.SequenceCounter > theService1Element.SequenceCounter);

            var theEnd1Element = order[3];
            Assert.AreEqual("theEnd1", theEnd1Element.ActivityId);
            Assert.True(theEnd1Element.SequenceCounter > theService2Element.SequenceCounter);

            // when (3)
            //jobId = jobQuery.ActivityId("theService4").First().Id;
            //managementService.ExecuteJob(jobId);

            // then (3)
            order = ExecutionOrderListener.ActivityExecutionOrder;
            Assert.AreEqual(7, order.Count);

            var theService4Element = order[4];
            Assert.AreEqual("theService4", theService4Element.ActivityId);
            Assert.True(theService4Element.SequenceCounter > theService3Element.SequenceCounter);
            Assert.True(theService4Element.SequenceCounter > theEnd1Element.SequenceCounter);

            var theService5Element = order[5];
            Assert.AreEqual("theService5", theService5Element.ActivityId);
            Assert.True(theService5Element.SequenceCounter > theService4Element.SequenceCounter);

            var theEnd2Element = order[6];
            Assert.AreEqual("theEnd2", theEnd2Element.ActivityId);
            Assert.True(theEnd2Element.SequenceCounter > theService5Element.SequenceCounter);

            AssertProcessEnded(processInstanceId);
        }

        [Test]
        [Deployment]
        public virtual void TestForkReplaceByThreeBranches()
        {
            // given
            var processInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;
            var jobQuery = managementService.CreateJobQuery();

            // when (1)

            // then (1)
            var order = ExecutionOrderListener.ActivityExecutionOrder;
            Assert.AreEqual(3, order.Count);

            var theService1Element = order[0];
            Assert.AreEqual("theService1", theService1Element.ActivityId);

            var theService3Element = order[1];
            Assert.AreEqual("theService3", theService3Element.ActivityId);

            var theService6Element = order[2];
            Assert.AreEqual("theService6", theService6Element.ActivityId);

            Assert.True(theService1Element.SequenceCounter == theService3Element.SequenceCounter);
            Assert.True(theService3Element.SequenceCounter == theService6Element.SequenceCounter);

            // when (2)
            //var jobId = jobQuery.ActivityId("theService2").First().Id;
            //managementService.ExecuteJob(jobId);

            // then (2)
            order = ExecutionOrderListener.ActivityExecutionOrder;
            Assert.AreEqual(5, order.Count);

            var theService2Element = order[3];
            Assert.AreEqual("theService2", theService2Element.ActivityId);
            Assert.True(theService2Element.SequenceCounter > theService1Element.SequenceCounter);

            var theEnd1Element = order[4];
            Assert.AreEqual("theEnd1", theEnd1Element.ActivityId);
            Assert.True(theEnd1Element.SequenceCounter > theService2Element.SequenceCounter);

            // when (3)
            //jobId = jobQuery.ActivityId("theService4").First().Id;
            //managementService.ExecuteJob(jobId);

            // then (3)
            order = ExecutionOrderListener.ActivityExecutionOrder;
            Assert.AreEqual(8, order.Count);

            var theService4Element = order[5];
            Assert.AreEqual("theService4", theService4Element.ActivityId);
            Assert.True(theService4Element.SequenceCounter > theService3Element.SequenceCounter);

            var theService5Element = order[6];
            Assert.AreEqual("theService5", theService5Element.ActivityId);
            Assert.True(theService5Element.SequenceCounter > theService4Element.SequenceCounter);

            var theEnd2Element = order[7];
            Assert.AreEqual("theEnd2", theEnd2Element.ActivityId);
            Assert.True(theEnd2Element.SequenceCounter > theService5Element.SequenceCounter);

            // when (4)
            //jobId = jobQuery.ActivityId("theService7").First().Id;
            //managementService.ExecuteJob(jobId);

            // then (4)
            order = ExecutionOrderListener.ActivityExecutionOrder;
            Assert.AreEqual(12, order.Count);

            var theService7Element = order[8];
            Assert.AreEqual("theService7", theService7Element.ActivityId);
            Assert.True(theService7Element.SequenceCounter > theService6Element.SequenceCounter);
            Assert.True(theService7Element.SequenceCounter > theEnd2Element.SequenceCounter);

            var theService8Element = order[9];
            Assert.AreEqual("theService8", theService8Element.ActivityId);
            Assert.True(theService8Element.SequenceCounter > theService7Element.SequenceCounter);

            var theService9Element = order[10];
            Assert.AreEqual("theService9", theService9Element.ActivityId);
            Assert.True(theService9Element.SequenceCounter > theService8Element.SequenceCounter);

            var theEnd3Element = order[11];
            Assert.AreEqual("theEnd3", theEnd3Element.ActivityId);
            Assert.True(theEnd3Element.SequenceCounter > theService9Element.SequenceCounter);

            AssertProcessEnded(processInstanceId);
        }

        [Test]
        [Deployment]
        public virtual void TestForkAndJoinSameSequenceLength()
        {
            // given
            var processInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            // when

            // then
            AssertProcessEnded(processInstanceId);

            var order = ExecutionOrderListener.ActivityExecutionOrder;
            Assert.AreEqual(9, order.Count);

            long lastSequenceCounter = 0;

            var theStartElement = order[0];
            Assert.AreEqual("theStart", theStartElement.ActivityId);
            Assert.True(theStartElement.SequenceCounter > lastSequenceCounter);
            lastSequenceCounter = theStartElement.SequenceCounter;

            var theForkElement = order[1];
            Assert.AreEqual("theService", theForkElement.ActivityId);
            Assert.True(theForkElement.SequenceCounter > lastSequenceCounter);
            lastSequenceCounter = theForkElement.SequenceCounter;

            var theServiceElement = order[2];
            Assert.AreEqual("fork", theServiceElement.ActivityId);
            Assert.True(theServiceElement.SequenceCounter > lastSequenceCounter);
            lastSequenceCounter = theServiceElement.SequenceCounter;

            var theService1Element = order[3];
            Assert.AreEqual("theService1", theService1Element.ActivityId);
            Assert.True(theService1Element.SequenceCounter > lastSequenceCounter);
            lastSequenceCounter = theService1Element.SequenceCounter;

            var theJoin1Element = order[4];
            Assert.AreEqual("join", theJoin1Element.ActivityId);
            Assert.True(theJoin1Element.SequenceCounter > lastSequenceCounter);

            lastSequenceCounter = theForkElement.SequenceCounter;

            var theService2Element = order[5];
            Assert.AreEqual("theService2", theService2Element.ActivityId);
            Assert.True(theService2Element.SequenceCounter > lastSequenceCounter);
            lastSequenceCounter = theService2Element.SequenceCounter;

            var theJoin2Element = order[6];
            Assert.AreEqual("join", theJoin2Element.ActivityId);
            Assert.True(theJoin2Element.SequenceCounter > lastSequenceCounter);

            var theService3Element = order[7];
            Assert.AreEqual("theService3", theService3Element.ActivityId);
            Assert.True(theService3Element.SequenceCounter > theJoin1Element.SequenceCounter);
            Assert.True(theService3Element.SequenceCounter > theJoin2Element.SequenceCounter);
            lastSequenceCounter = theService3Element.SequenceCounter;

            var theEndElement = order[8];
            Assert.AreEqual("theEnd", theEndElement.ActivityId);
            Assert.True(theEndElement.SequenceCounter > lastSequenceCounter);
        }

        [Test]
        [Deployment]
        public virtual void TestForkAndJoinDifferentSequenceLength()
        {
            // given
            var processInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            // when

            // then
            AssertProcessEnded(processInstanceId);

            var order = ExecutionOrderListener.ActivityExecutionOrder;
            Assert.AreEqual(10, order.Count);

            long lastSequenceCounter = 0;

            var theStartElement = order[0];
            Assert.AreEqual("theStart", theStartElement.ActivityId);
            Assert.True(theStartElement.SequenceCounter > lastSequenceCounter);
            lastSequenceCounter = theStartElement.SequenceCounter;

            var theForkElement = order[1];
            Assert.AreEqual("theService", theForkElement.ActivityId);
            Assert.True(theForkElement.SequenceCounter > lastSequenceCounter);
            lastSequenceCounter = theForkElement.SequenceCounter;

            var theServiceElement = order[2];
            Assert.AreEqual("fork", theServiceElement.ActivityId);
            Assert.True(theServiceElement.SequenceCounter > lastSequenceCounter);
            lastSequenceCounter = theServiceElement.SequenceCounter;

            var theService1Element = order[3];
            Assert.AreEqual("theService1", theService1Element.ActivityId);
            Assert.True(theService1Element.SequenceCounter > lastSequenceCounter);
            lastSequenceCounter = theService1Element.SequenceCounter;

            var theJoin1Element = order[4];
            Assert.AreEqual("join", theJoin1Element.ActivityId);
            Assert.True(theJoin1Element.SequenceCounter > lastSequenceCounter);

            lastSequenceCounter = theForkElement.SequenceCounter;

            var theService2Element = order[5];
            Assert.AreEqual("theService2", theService2Element.ActivityId);
            Assert.True(theService2Element.SequenceCounter > lastSequenceCounter);
            lastSequenceCounter = theService2Element.SequenceCounter;

            var theService3Element = order[6];
            Assert.AreEqual("theService3", theService3Element.ActivityId);
            Assert.True(theService3Element.SequenceCounter > lastSequenceCounter);
            lastSequenceCounter = theService3Element.SequenceCounter;

            var theJoin2Element = order[7];
            Assert.AreEqual("join", theJoin2Element.ActivityId);
            Assert.True(theJoin2Element.SequenceCounter > lastSequenceCounter);

            Assert.IsFalse(theJoin1Element.SequenceCounter == theJoin2Element.SequenceCounter);

            var theService4Element = order[8];
            Assert.AreEqual("theService4", theService4Element.ActivityId);
            Assert.True(theService4Element.SequenceCounter > theJoin1Element.SequenceCounter);
            Assert.True(theService4Element.SequenceCounter > theJoin2Element.SequenceCounter);
            lastSequenceCounter = theService4Element.SequenceCounter;

            var theEndElement = order[9];
            Assert.AreEqual("theEnd", theEndElement.ActivityId);
            Assert.True(theEndElement.SequenceCounter > lastSequenceCounter);
        }

        [Test]
        [Deployment]
        public virtual void TestForkAndJoinThreeBranchesDifferentSequenceLength()
        {
            // given
            var processInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            // when

            // then
            AssertProcessEnded(processInstanceId);

            var order = ExecutionOrderListener.ActivityExecutionOrder;
            Assert.AreEqual(4, order.Count);

            var theJoin1Element = order[0];
            Assert.AreEqual("join", theJoin1Element.ActivityId);

            var theJoin2Element = order[1];
            Assert.AreEqual("join", theJoin2Element.ActivityId);

            var theJoin3Element = order[2];
            Assert.AreEqual("join", theJoin3Element.ActivityId);

            Assert.IsFalse(theJoin1Element.SequenceCounter == theJoin2Element.SequenceCounter);
            Assert.IsFalse(theJoin2Element.SequenceCounter == theJoin3Element.SequenceCounter);
            Assert.IsFalse(theJoin3Element.SequenceCounter == theJoin1Element.SequenceCounter);

            var theService7Element = order[3];
            Assert.AreEqual("theService7", theService7Element.ActivityId);
            Assert.True(theService7Element.SequenceCounter > theJoin1Element.SequenceCounter);
            Assert.True(theService7Element.SequenceCounter > theJoin2Element.SequenceCounter);
            Assert.True(theService7Element.SequenceCounter > theJoin3Element.SequenceCounter);
        }

        [Test]
        [Deployment]
        public virtual void TestSequenceInsideSubProcess()
        {
            // given
            var processInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            // when

            // then
            AssertProcessEnded(processInstanceId);

            var order = ExecutionOrderListener.ActivityExecutionOrder;
            VerifyOrder(order, "theStart", "theService1", "subProcess", "innerStart", "innerService", "innerEnd",
                "theService2", "theEnd");
        }

        [Test]
        [Deployment]
        public virtual void TestForkSameSequenceLengthInsideSubProcess()
        {
            // given
            var processInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            // when

            // then
            AssertProcessEnded(processInstanceId);

            var order = ExecutionOrderListener.ActivityExecutionOrder;
            Assert.AreEqual(3, order.Count);

            var innerEnd1Element = order[0];
            Assert.AreEqual("innerEnd1", innerEnd1Element.ActivityId);

            var innerEnd2Element = order[1];
            Assert.AreEqual("innerEnd2", innerEnd2Element.ActivityId);

            var theService1Element = order[2];
            Assert.AreEqual("theService1", theService1Element.ActivityId);

            Assert.True(theService1Element.SequenceCounter > innerEnd1Element.SequenceCounter);
            Assert.True(theService1Element.SequenceCounter > innerEnd2Element.SequenceCounter);
        }

        [Test]
        [Deployment]
        public virtual void TestForkDifferentSequenceLengthInsideSubProcess()
        {
            // given
            var processInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            // when

            // then
            AssertProcessEnded(processInstanceId);

            var order = ExecutionOrderListener.ActivityExecutionOrder;
            Assert.AreEqual(3, order.Count);

            var innerEnd1Element = order[0];
            Assert.AreEqual("innerEnd1", innerEnd1Element.ActivityId);

            var innerEnd2Element = order[1];
            Assert.AreEqual("innerEnd2", innerEnd2Element.ActivityId);

            var theService1Element = order[2];
            Assert.AreEqual("theService1", theService1Element.ActivityId);

            Assert.True(theService1Element.SequenceCounter > innerEnd1Element.SequenceCounter);
            Assert.True(theService1Element.SequenceCounter > innerEnd2Element.SequenceCounter);
        }

        [Test]
        [Deployment]
        public virtual void TestSequentialMultiInstance()
        {
            // given
            var processInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            // when

            // then
            AssertProcessEnded(processInstanceId);

            var order = ExecutionOrderListener.ActivityExecutionOrder;
            VerifyOrder(order, "theStart", "theService1", "theService2", "theService2", "theService3", "theEnd");
        }

        [Test]
        [Deployment]
        public virtual void TestParallelMultiInstance()
        {
            // given
            var processInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            // when

            // then
            AssertProcessEnded(processInstanceId);

            var order = ExecutionOrderListener.ActivityExecutionOrder;
            Assert.AreEqual(6, order.Count);

            var theStartElement = order[0];
            Assert.AreEqual("theStart", theStartElement.ActivityId);

            var theService1Element = order[1];
            Assert.AreEqual("theService1", theService1Element.ActivityId);
            Assert.True(theService1Element.SequenceCounter > theStartElement.SequenceCounter);

            var theService21Element = order[2];
            Assert.AreEqual("theService2", theService21Element.ActivityId);
            Assert.True(theService21Element.SequenceCounter > theService1Element.SequenceCounter);

            var theService22Element = order[3];
            Assert.AreEqual("theService2", theService22Element.ActivityId);
            Assert.True(theService22Element.SequenceCounter > theService1Element.SequenceCounter);

            var theService3Element = order[4];
            Assert.AreEqual("theService3", theService3Element.ActivityId);
            Assert.True(theService3Element.SequenceCounter > theService21Element.SequenceCounter);
            Assert.True(theService3Element.SequenceCounter > theService22Element.SequenceCounter);

            var theEndElement = order[5];
            Assert.AreEqual("theEnd", theEndElement.ActivityId);
            Assert.True(theEndElement.SequenceCounter > theService3Element.SequenceCounter);
        }
        [Test]
        [Deployment]
        public virtual void TestLoop()
        {
            // given
            var processInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            // when

            // then
            AssertProcessEnded(processInstanceId);

            var order = ExecutionOrderListener.ActivityExecutionOrder;
            VerifyOrder(order, "theStart", "theService1", "join", "theScript", "fork", "join", "theScript", "fork",
                "theService2", "theEnd");
        }

        [Test]
        [Deployment]
        public virtual void TestInterruptingBoundaryEvent()
        {
            // given
            var processInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            // when (1)

            // then (1)
            var order = ExecutionOrderListener.ActivityExecutionOrder;
            VerifyOrder(order, "theStart", "theService1", "theTask");

            // when (2)
            runtimeService.CorrelateMessage("newMessage");

            // then (2)
            AssertProcessEnded(processInstanceId);

            order = ExecutionOrderListener.ActivityExecutionOrder;
            VerifyOrder(order, "theStart", "theService1", "theTask", "messageBoundary", "theServiceAfterMessage",
                "theEnd2");
        }

        [Test]
        [Deployment]
        public virtual void TestNonInterruptingBoundaryEvent()
        {
            // given
            var processInstanceId = runtimeService.StartProcessInstanceByKey("process").Id;

            // when (1)

            // then (1)
            var order = ExecutionOrderListener.ActivityExecutionOrder;
            VerifyOrder(order, "theStart", "theService1", "theTask");

            // when (2)
            runtimeService.CorrelateMessage("newMessage");

            // then (2)
            order = ExecutionOrderListener.ActivityExecutionOrder;
            Assert.AreEqual(6, order.Count);

            var theService1Element = order[1];
            Assert.AreEqual("theService1", theService1Element.ActivityId);

            var theTaskElement = order[2];
            Assert.AreEqual("theTask", theTaskElement.ActivityId);

            var messageBoundaryElement = order[3];
            Assert.AreEqual("messageBoundary", messageBoundaryElement.ActivityId);
            Assert.True(messageBoundaryElement.SequenceCounter > theService1Element.SequenceCounter);
            Assert.IsFalse(messageBoundaryElement.SequenceCounter > theTaskElement.SequenceCounter);

            var theServiceAfterMessageElement = order[4];
            Assert.AreEqual("theServiceAfterMessage", theServiceAfterMessageElement.ActivityId);
            Assert.True(theServiceAfterMessageElement.SequenceCounter > messageBoundaryElement.SequenceCounter);

            var theEnd2Element = order[5];
            Assert.AreEqual("theEnd2", theEnd2Element.ActivityId);
            Assert.True(theEnd2Element.SequenceCounter > theServiceAfterMessageElement.SequenceCounter);

            // when (3)
            var taskId = taskService.CreateTaskQuery().First().Id;
            taskService.Complete(taskId);

            // then (3)
            AssertProcessEnded(processInstanceId);

            order = ExecutionOrderListener.ActivityExecutionOrder;
            Assert.AreEqual(7, order.Count);

            var theEnd1Element = order[6];
            Assert.AreEqual("theEnd1", theEnd1Element.ActivityId);
            Assert.True(theEnd1Element.SequenceCounter > theEnd2Element.SequenceCounter);
        }

        protected internal virtual void VerifyOrder(IList<ActivitySequenceCounterMap> actualOrder,
            params string[] expectedOrder)
        {
            Assert.AreEqual(expectedOrder.Length, actualOrder.Count);

            long lastActualSequenceCounter = 0;
            for (var i = 0; i < expectedOrder.Length; i++)
            {
                var actual = actualOrder[i];

                var actualActivityId = actual.ActivityId;
                var expectedActivityId = expectedOrder[i];
                Assert.AreEqual(actualActivityId, expectedActivityId);

                var actualSequenceCounter = actual.SequenceCounter;
                Assert.True(actualSequenceCounter > lastActualSequenceCounter);

                lastActualSequenceCounter = actualSequenceCounter;
            }
        }
    }
}