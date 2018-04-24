using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Dmn.engine;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;
using NUnit.Framework;

namespace Engine.Tests.Dmn.BusinessRuleTask
{

    /// <summary>
    /// Tests the decision result that is retrieved by an execution listener.
    /// 
    /// 
    /// </summary>
    [TestFixture]
    public class DmnDecisionResultListenerTest : PluggableProcessEngineTestCase
    {

        protected internal const string TEST_PROCESS = "resources/dmn/result/DmnDecisionResultTest.bpmn20.xml";
        protected internal const string TEST_DECISION = "resources/dmn/result/DmnDecisionResultTest.dmn11.xml";
        protected internal const string TEST_DECISION_COLLECT_SUM = "resources/dmn/result/DmnDecisionResultCollectSumHitPolicyTest.Dmn11.xml";
        protected internal const string TEST_DECISION_COLLECT_COUNT = "resources/dmn/result/DmnDecisionResultCollectCountHitPolicyTest.Dmn11.xml";

        protected internal IDmnDecisionResult results;

        [Test]
        [Deployment(new[] { TEST_PROCESS, TEST_DECISION })]
        public virtual void testNoOutput()
        {
            startTestProcess("no output");

            Assert.True(!results.Any(), "The decision result 'ruleResult' should be empty");
        }

        [Test]
        [Deployment(new[] { TEST_PROCESS, TEST_DECISION })]
        public virtual void testEmptyOutput()
        {
            startTestProcess("empty output");

            Assert.IsFalse(!results.Any(), "The decision result 'ruleResult' should not be empty");

            IDmnDecisionResultEntries decisionOutput = results[0];
            Assert.IsNull(decisionOutput.GetFirstEntry());
        }
        [Test]
        [Deployment(new[] { TEST_PROCESS, TEST_DECISION })]
        public virtual void testEmptyMap()
        {
            startTestProcess("empty map");

            Assert.AreEqual(2, results.Count);

            foreach (IDmnDecisionResultEntries output in results)
            {
                Assert.True(!output.Any(), "The decision output should be empty");
            }
        }

        [Test]
        [Deployment(new[] { TEST_PROCESS, TEST_DECISION })]
        public virtual void testSingleEntry()
        {
            startTestProcess("single entry");

            IDmnDecisionResultEntries firstOutput = results[0];
            Assert.AreEqual("foo", firstOutput.GetFirstEntry());
            Assert.AreEqual(Variables.StringValue("foo"), firstOutput.GetFirstEntryTyped<ITypedValue>());
        }

        [Test]
        [Deployment(new[] { TEST_PROCESS, TEST_DECISION })]
        public virtual void testMultipleEntries()
        {
            startTestProcess("multiple entries");

            IDmnDecisionResultEntries firstOutput = results[0];
            Assert.AreEqual(Variables.StringValue("foo"), firstOutput["result1"]);
            Assert.AreEqual(Variables.StringValue("bar"), firstOutput["result2"]);

            Assert.AreEqual(Variables.StringValue("foo"), firstOutput.GetEntryTyped<ITypedValue>("result1"));
            Assert.AreEqual(Variables.StringValue("bar"), firstOutput.GetEntryTyped<ITypedValue>("result2"));
        }

        [Test]
        [Deployment(new[] { TEST_PROCESS, TEST_DECISION })]
        public virtual void testSingleEntryList()
        {
            startTestProcess("single entry list");

            Assert.AreEqual(2, results.Count);

            foreach (IDmnDecisionResultEntries output in results)
            {
                Assert.AreEqual("foo", output.GetFirstEntry());
                Assert.AreEqual(Variables.StringValue("foo"), output.GetFirstEntryTyped<ITypedValue>());
            }
        }

        [Test]
        [Deployment(new[] { TEST_PROCESS, TEST_DECISION })]
        public virtual void testMultipleEntriesList()
        {
            startTestProcess("multiple entries list");

            Assert.AreEqual(2, results.Count);

            foreach (IDmnDecisionResultEntries output in results)
            {
                Assert.AreEqual(2, output.Count);
                Assert.AreEqual(Variables.StringValue("foo"), output["result1"]);
                Assert.AreEqual(Variables.StringValue("bar"), output["result2"]);

                Assert.AreEqual(Variables.StringValue("foo"), output.GetEntryTyped<ITypedValue>("result1"));
                Assert.AreEqual(Variables.StringValue("bar"), output.GetEntryTyped<ITypedValue>("result2"));
            }
        }

        [Test]
        [Deployment(new[] { TEST_PROCESS, TEST_DECISION_COLLECT_COUNT })]
        public virtual void testCollectCountHitPolicyNoOutput()
        {
            startTestProcess("no output");

            Assert.AreEqual(1, results.Count);
            IDmnDecisionResultEntries firstOutput = results[0];

            Assert.AreEqual(0, firstOutput.GetFirstEntry());
            Assert.AreEqual(Variables.IntegerValue(0), firstOutput.GetFirstEntryTyped<ITypedValue>());
        }

        [Test]
        [Deployment(new[] { TEST_PROCESS, TEST_DECISION_COLLECT_SUM })]
        public virtual void testCollectSumHitPolicyNoOutput()
        {
            startTestProcess("no output");

            Assert.True(!results.Any(), "The decision result 'ruleResult' should be empty");
        }

        [Test]
        [Deployment(new[] { TEST_PROCESS, TEST_DECISION_COLLECT_SUM })]
        public virtual void testCollectSumHitPolicySingleEntry()
        {
            startTestProcess("single entry");

            Assert.AreEqual(1, results.Count);
            IDmnDecisionResultEntries firstOutput = results[0];

            Assert.AreEqual(12, firstOutput.GetFirstEntry());
            Assert.AreEqual(Variables.IntegerValue(12), firstOutput.GetFirstEntryTyped<ITypedValue>());
        }

        [Test]
        [Deployment(new[] { TEST_PROCESS, TEST_DECISION_COLLECT_SUM })]
        public virtual void testCollectSumHitPolicySingleEntryList()
        {
            startTestProcess("single entry list");

            Assert.AreEqual(1, results.Count);
            IDmnDecisionResultEntries firstOutput = results[0];

            Assert.AreEqual(33, firstOutput.GetFirstEntry());
            Assert.AreEqual(Variables.IntegerValue(33), firstOutput.GetFirstEntryTyped<ITypedValue>());
        }
        protected internal virtual IProcessInstance startTestProcess(string input)
        {
            var dic = new Dictionary<string, ITypedValue>();
            dic["input"] = new StringValueImpl(input);
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("testProcess", dic);

            // get the result from an execution listener that is invoked at the end of the business rule activity
            results = DecisionResultTestListener.DecisionResult;
            Assert.NotNull(results);

            return processInstance;
        }
        [TearDown]
        public void tearDown()
        {
            // reset the invoked execution listener
            DecisionResultTestListener.Reset();
        }

    }

}