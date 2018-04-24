using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;
using NUnit.Framework;

namespace Engine.Tests.Dmn.BusinessRuleTask
{
    /// <summary>
    ///     Tests the mapping of the decision result.
    /// </summary>
    [TestFixture]
    public class DmnBusinessRuleTaskResultMappingTest : PluggableProcessEngineTestCase
    {
        protected internal const string TEST_DECISION =
            "resources/dmn/result/DmnBusinessRuleTaskResultMappingTest.dmn11.xml";

        protected internal const string CUSTOM_MAPPING_BPMN =
            "resources/dmn/result/DmnBusinessRuleTaskResultMappingTest.TestCustomOutputMapping.bpmn20.xml";

        protected internal const string SINGLE_ENTRY_BPMN =
            "resources/dmn/result/DmnBusinessRuleTaskResultMappingTest.TestSingleEntry.bpmn20.xml";

        protected internal const string SINGLE_RESULT_BPMN =
            "resources/dmn/result/DmnBusinessRuleTaskResultMappingTest.TestSingleResult.bpmn20.xml";

        protected internal const string COLLECT_ENTRIES_BPMN =
            "resources/dmn/result/DmnBusinessRuleTaskResultMappingTest.TestCollectEntries.bpmn20.xml";

        protected internal const string RESULT_LIST_BPMN =
            "resources/dmn/result/DmnBusinessRuleTaskResultMappingTest.TestResultList.bpmn20.xml";

        protected internal const string DEFAULT_MAPPING_BPMN =
            "resources/dmn/result/DmnBusinessRuleTaskResultMappingTest.TestDefaultMapping.bpmn20.xml";

        protected internal const string INVALID_MAPPING_BPMN =
            "resources/dmn/result/DmnBusinessRuleTaskResultMappingTest.TestInvalidMapping.bpmn20.xml";

        protected internal const string OVERRIDE_DECISION_RESULT_BPMN =
            "resources/dmn/result/DmnBusinessRuleTaskResultMappingTest.TestOverrideVariable.bpmn20.xml";

        [Test]
        [Deployment(new[] { SINGLE_RESULT_BPMN, TEST_DECISION })]
        public virtual void testSingleResultMapping()
        {
            //<rule id="ruleMultipleEntries">
            //  <inputEntry>
            //    <text>"multiple entries"</text>
            //  </inputEntry>
            //  <outputEntry>
            //    <text>"foo1"</text>
            //  </outputEntry>
            //  <outputEntry>
            //    <text>"bar1"</text>
            //  </outputEntry>
            //</rule>
            var processInstance = startTestProcess("multiple entries");

            var output = (IDictionary<string, object>)runtimeService.GetVariable(processInstance.Id, "result");

            Assert.AreEqual(2, output.Count);
            Assert.AreEqual(Variables.StringValue("foo"), output["result1"]);
            Assert.AreEqual(Variables.StringValue("bar"), output["result2"]);
        }

        [Test]
        [Deployment(new[] { COLLECT_ENTRIES_BPMN, TEST_DECISION })]
        public virtual void testCollectEntriesMapping()
        {
            var processInstance = startTestProcess("single entry list");

            var output = (IList<ITypedValue>)runtimeService.GetVariable(processInstance.Id, "result");

            Assert.AreEqual(2, output.Count);
            Assert.AreEqual(Variables.StringValue("foo"), output[0]);
            Assert.AreEqual(Variables.StringValue("foo"), output[1]);
        }
        [Test]
        [Deployment(new[] { RESULT_LIST_BPMN, TEST_DECISION })]
        public virtual void testResultListMapping()
        {
            var processInstance = startTestProcess("multiple entries list");

            var resultList =
                (IList<IDictionary<string, object>>)runtimeService.GetVariable(processInstance.Id, "result");
            Assert.AreEqual(2, resultList.Count);

            foreach (var valueMap in resultList)
            {
                Assert.AreEqual(2, valueMap.Count);
                Assert.AreEqual(Variables.StringValue("foo"), valueMap["result1"]);
                Assert.AreEqual(Variables.StringValue("bar"), valueMap["result2"]);
            }
        }

        [Test]
        [Deployment(new[] { DEFAULT_MAPPING_BPMN, TEST_DECISION })]
        public virtual void testDefaultResultMapping()
        {
            var processInstance = startTestProcess("multiple entries list");

            // default mapping is 'resultList'
            var resultList =
                (IList<IDictionary<string, object>>)runtimeService.GetVariable(processInstance.Id, "result");
            Assert.AreEqual(2, resultList.Count);

            foreach (var valueMap in resultList)
            {
                Assert.AreEqual(2, valueMap.Count);
                Assert.AreEqual(Variables.StringValue("foo"), valueMap["result1"]);
                Assert.AreEqual(Variables.StringValue("bar"), valueMap["result2"]);
            }
        }

        [Test]
        [Deployment(new[] { SINGLE_ENTRY_BPMN, TEST_DECISION })]
        public virtual void testSingleEntryMappingFailureMultipleOutputs()
        {
            try
            {
                //camunda:mapDecisionResult="singleEntry" 限定了single
                startTestProcess("single entry list");

                Assert.Fail("expect exception");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("ENGINE-22001", e.Message);
            }
        }

        [Test]
        [Deployment(new[] { SINGLE_ENTRY_BPMN, TEST_DECISION })]
        public virtual void testSingleEntryMappingFailureMultipleValues()
        {
            try
            {
                //camunda:mapDecisionResult="singleEntry" 限定了single
                startTestProcess("multiple entries");

                Assert.Fail("expect exception");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("ENGINE-22001", e.Message);
            }
        }

        [Test]
        [Deployment(new[] { SINGLE_RESULT_BPMN, TEST_DECISION })]
        public virtual void testSingleResultMappingFailure()
        {
            try
            {
                //camunda:mapDecisionResult="singleEntry" 限定了single
                startTestProcess("single entry list");

                Assert.Fail("expect exception");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("ENGINE-22001", e.Message);
            }
        }

        [Test]
        [Deployment(new[] { COLLECT_ENTRIES_BPMN, TEST_DECISION })]
        public virtual void testCollectEntriesMappingFailure()
        {
            try
            {
                //camunda:mapDecisionResult="collectEntries" 限定了List，而不能List再包含字典
                startTestProcess("multiple entries");

                Assert.Fail("expect exception");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("ENGINE-22002", e.Message);
            }
        }

        [Test]
        public virtual void testInvalidMapping()
        {
            try
            {
                DeploymentId = repositoryService.CreateDeployment()
                    .AddClasspathResource(INVALID_MAPPING_BPMN)
                    .Deploy()
                    .Id;

                Assert.Fail("expect parse exception");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("No decision result mapper found for name 'invalid'", e.Message);
            }
        }

        [Test]
        [Deployment(new[] { DEFAULT_MAPPING_BPMN, TEST_DECISION })]
        public virtual void testTransientDecisionResult()
        {
            // when a decision is evaluated and the result is stored in a transient variable "decisionResult"
            var processInstance = startTestProcess("single entry");

            // then the variable should not be available outside the business rule task
            Assert.IsNull(runtimeService.GetVariable(processInstance.Id, "decisionResult"));
            // and should not create an entry in history since it is not persistent
            Assert.IsNull(historyService.CreateHistoricVariableInstanceQuery(m => m.Name == "decisionResult")
                //.VariableName("decisionResult")
                .FirstOrDefault());
        }
        [Test]
        [Deployment(new[] { OVERRIDE_DECISION_RESULT_BPMN, TEST_DECISION })]
        public virtual void testFailedToOverrideDecisionResultVariable()
        {
            try
            {
                // the transient variable "decisionResult" should not be overridden by the task result variable
                startTestProcess("single entry");
                Assert.Fail("expect exception");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("variable with name 'decisionResult' can not be updated", e.Message);
            }
        }

        [Test]
        [Deployment(new[] { SINGLE_ENTRY_BPMN, TEST_DECISION })]
        public virtual void testSingleEntryEmptyResult()
        {
            var processInstance = startTestProcess("empty result");

            var result = runtimeService.GetVariable(processInstance.Id, "result");
            Assert.IsNull(result);
            ITypedValue resultTyped = runtimeService.GetVariableTyped<ITypedValue>(processInstance.Id, "result");
            Assert.AreEqual(Variables.UntypedNullValue(), resultTyped);
        }

        [Test]
        [Deployment(new[] { SINGLE_RESULT_BPMN, TEST_DECISION })]
        public virtual void testSingleResultEmptyResult()
        {
            var processInstance = startTestProcess("empty result");

            var result = runtimeService.GetVariable(processInstance.Id, "result");
            Assert.IsNull(result);
            ITypedValue resultTyped = runtimeService.GetVariableTyped<ITypedValue>(processInstance.Id, "result");
            Assert.AreEqual(Variables.UntypedNullValue(), resultTyped);
        }
        [Test]
        [Deployment(new[] { COLLECT_ENTRIES_BPMN, TEST_DECISION })]
        public virtual void testCollectEntriesEmptyResult()
        {
            var processInstance = startTestProcess("empty result");

            var result = /*(IList<object>) */runtimeService.GetVariable(processInstance.Id, "result");
            //Assert.True(result.Count == 0);
            Assert.IsNull(result);
            var result2 = runtimeService.GetVariableTyped<ITypedValue>(processInstance.Id, "result");
            Assert.AreEqual(Variables.UntypedNullValue(), result2);
        }

        [Test]
        [Deployment(new[] { RESULT_LIST_BPMN, TEST_DECISION })]
        public virtual void testResultListEmptyResult()
        {
            var processInstance = startTestProcess("empty result");

            var result = (IList<IDictionary<string, object>>)runtimeService.GetVariable(processInstance.Id, "result");
            Assert.True(result.Count == 0);
            //Assert.IsNull(result);
        }

        [Test]
        [Deployment(new[] { DEFAULT_MAPPING_BPMN, TEST_DECISION })]
        public virtual void testDefaultMappingEmptyResult()
        {
            var processInstance = startTestProcess("empty result");

            var result = (IList<IDictionary<string, object>>)runtimeService.GetVariable(processInstance.Id, "result");
            Assert.True(result.Count == 0);
        }
        protected internal virtual IProcessInstance startTestProcess(string input)
        {
            var dic = new Dictionary<string, ITypedValue>();
            dic["input"] = new StringValueImpl(input);
            return runtimeService.StartProcessInstanceByKey("testProcess"
                , dic);
        }

        [Test]
        [Deployment(new[] { CUSTOM_MAPPING_BPMN, TEST_DECISION })]
        public virtual void testCustomOutputMapping()
        {
            var processInstance = startTestProcess("multiple entries");

            Assert.AreEqual("foo", runtimeService.GetVariable(processInstance.Id, "result1"));
            Assert.AreEqual(Variables.StringValue("foo"),
                runtimeService.GetVariableTyped<ITypedValue>(processInstance.Id, "result1"));

            Assert.AreEqual("bar", runtimeService.GetVariable(processInstance.Id, "result2"));
            Assert.AreEqual(Variables.StringValue("bar"),
                runtimeService.GetVariableTyped<ITypedValue>(processInstance.Id, "result2"));
        }

        [Test]
        [Deployment(new[] { SINGLE_ENTRY_BPMN, TEST_DECISION })]
        public virtual void testSingleEntryMapping()
        {
            var processInstance = startTestProcess("single entry");

            Assert.AreEqual("foo", runtimeService.GetVariable(processInstance.Id, "result"));
            Assert.AreEqual(Variables.StringValue("foo"),
                runtimeService.GetVariableTyped<StringValueImpl>(processInstance.Id, "result"));
        }
        //[Test]
        //[Deployment(new string[] { SINGLE_ENTRY_BPMN , "resources/dmn/result/DmnDecisionResultTestExt.dmn11.xml" })]
        public virtual void testExpressMapping()
        {
            var processInstance = startTestProcess("input");
            var r = runtimeService.GetVariable(processInstance.Id, "result");
        }
    }
}