using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime.Migration;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Model.Bpmn.builder;
using ESS.FW.Bpm.Model.Bpmn.instance;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MessageCorrelationByLocalVariablesTest
    {
        public const string TEST_MESSAGE_NAME = "TEST_MSG";
        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public ProcessEngineRule engineRule = new ProcessEngineRule();
        public ProcessEngineRule engineRule = new ProcessEngineRule();
        private readonly bool InstanceFieldsInitialized;
        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public util.ProcessEngineTestRule testHelper = new util.ProcessEngineTestRule(engineRule);
        public ProcessEngineTestRule testHelper;

        public MessageCorrelationByLocalVariablesTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            testHelper = new ProcessEngineTestRule(engineRule);
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.ExpectedException thrown = org.junit.Rules.ExpectedException.None();
        //public ExpectedException thrown = ExpectedException.None();

        [Test]
        public virtual void testReceiveTaskMessageCorrelation()
        {
            //given
            var model = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("Process_1")
                .StartEvent()
                .SubProcess("SubProcess_1")
                .EmbeddedSubProcess()
                .StartEvent()
                .ReceiveTask("MessageReceiver_1")
                .Message(TEST_MESSAGE_NAME)

                .CamundaInputParameter("localVar", "${loopVar}")
                .CamundaInputParameter("constVar", "someValue")
                .UserTask("UserTask_1")
                .EndEvent()
                .SubProcessDone()
                .MultiInstance()
                .CamundaCollection("${vars}")
                .CamundaElementVariable("loopVar")
                .MultiInstanceDone()
                .EndEvent()
                .Done(); //to test array of parameters

            testHelper.Deploy(model);

            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["vars"] = new[] { 1, 2, 3 };
            var processInstance = engineRule.RuntimeService.StartProcessInstanceByKey("Process_1", variables);

            //when correlated by local variables
            var messageName = TEST_MESSAGE_NAME;
            IDictionary<string, object> correlationKeys = new Dictionary<string, object>();
            var correlationKey = 1;
            correlationKeys["localVar"] = correlationKey;
            correlationKeys["constVar"] = "someValue";

            var messageCorrelationResult =
                engineRule.RuntimeService.CreateMessageCorrelation(messageName)
                    .LocalVariablesEqual(correlationKeys)
                    .SetVariables(ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                        .PutValue("newVar", "newValue"))
                    .CorrelateWithResult();

            //then one message is correlated, two other continue waiting
            checkExecutionMessageCorrelationResult(messageCorrelationResult, processInstance, "MessageReceiver_1");

            //uncorrelated executions
            var uncorrelatedExecutions = engineRule.RuntimeService.CreateExecutionQuery(c => c.ActivityId == "MessageReceiver_1")

                .ToList();
            Assert.AreEqual(2, uncorrelatedExecutions.Count);
        }

        [Test]
        public virtual void testIntermediateCatchEventMessageCorrelation()
        {
            //given
            var model = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("Process_1")
                .StartEvent()
                .SubProcess("SubProcess_1")
                .EmbeddedSubProcess()
                .StartEvent()
                .IntermediateCatchEvent("MessageReceiver_1");
           var r= model
                .Message(TEST_MESSAGE_NAME)
                .CamundaInputParameter("localVar", "${loopVar}")
                .UserTask("UserTask_1")
                .EndEvent()
                .SubProcessDone()
                .MultiInstance()
                .CamundaCollection("${vars}")
                .CamundaElementVariable("loopVar")
                .MultiInstanceDone()
                .EndEvent()
                .Done();

            testHelper.Deploy(r);

            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["vars"] = new[] { 1, 2, 3 };
            var processInstance = engineRule.RuntimeService.StartProcessInstanceByKey("Process_1", variables);

            //when correlated by local variables
            var messageName = TEST_MESSAGE_NAME;
            var correlationKey = 1;

            var messageCorrelationResult =
                engineRule.RuntimeService.CreateMessageCorrelation(messageName)
                    .LocalVariableEquals("localVar", correlationKey)
                    .SetVariables(ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                        .PutValue("newVar", "newValue"))
                    .CorrelateWithResult();

            //then one message is correlated, two others continue waiting
            checkExecutionMessageCorrelationResult(messageCorrelationResult, processInstance, "MessageReceiver_1");

            //uncorrelated executions
            var uncorrelatedExecutions = engineRule.RuntimeService.CreateExecutionQuery(c => c.ActivityId == "MessageReceiver_1")

                .ToList();
            Assert.AreEqual(2, uncorrelatedExecutions.Count);
        }

        [Test]
        public virtual void testMessageBoundaryEventMessageCorrelation()
        {
            //given
            var model = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("Process_1")
                .StartEvent()
                .SubProcess("SubProcess_1")
                .EmbeddedSubProcess()
                .StartEvent()
                .UserTask("UserTask_1")
                .CamundaInputParameter("localVar", "${loopVar}")
                .CamundaInputParameter("constVar", "someValue")
                .BoundaryEvent("MessageReceiver_1")
                .Message(TEST_MESSAGE_NAME)
                .UserTask("UserTask_2")
                .EndEvent()
                .SubProcessDone()
                .MultiInstance()
                .CamundaCollection("${vars}")
                .CamundaElementVariable("loopVar")
                .MultiInstanceDone()
                .EndEvent()
                .Done(); //to test array of parameters

            testHelper.Deploy(model);

            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["vars"] = new[] { 1, 2, 3 };
            var processInstance = engineRule.RuntimeService.StartProcessInstanceByKey("Process_1", variables);

            //when correlated by local variables
            var messageName = TEST_MESSAGE_NAME;
            IDictionary<string, object> correlationKeys = new Dictionary<string, object>();
            var correlationKey = 1;
            correlationKeys["localVar"] = correlationKey;
            correlationKeys["constVar"] = "someValue";
            IDictionary<string, object> messagePayload = new Dictionary<string, object>();
            messagePayload["newVar"] = "newValue";

            var messageCorrelationResult =
                engineRule.RuntimeService.CreateMessageCorrelation(messageName)
                    .LocalVariablesEqual(correlationKeys)
                    .SetVariables(messagePayload)
                    .CorrelateWithResult();

            //then one message is correlated, two others continue waiting
            checkExecutionMessageCorrelationResult(messageCorrelationResult, processInstance, "UserTask_1");

            //uncorrelated executions
            var uncorrelatedExecutions = engineRule.RuntimeService.CreateExecutionQuery(c => c.ActivityId == "UserTask_1")

                .ToList();
            Assert.AreEqual(2, uncorrelatedExecutions.Count);
        }

        [Test]
        public virtual void testBothInstanceAndLocalVariableMessageCorrelation()
        {
            //given
            var model = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("Process_1")
                .StartEvent()
                .SubProcess("SubProcess_1")
               .EmbeddedSubProcess()
                .StartEvent()
                .ReceiveTask("MessageReceiver_1")
                .Message(TEST_MESSAGE_NAME)
                .UserTask("UserTask_1")
                .EndEvent()
                .SubProcessDone()
                .MultiInstance()
                .CamundaCollection("${vars}")
                .CamundaElementVariable("loopVar")
                .MultiInstanceDone()
                .EndEvent()
                .Done();

            model = ModifiableBpmnModelInstance.Modify(model)
                .ActivityBuilder<AbstractActivityBuilder<IActivity>, IActivity>("MessageReceiver_1")
                .CamundaInputParameter("localVar", "${loopVar}")
                .CamundaInputParameter("constVar", "someValue")
                .Done()
                ; //to test array of parameters

            testHelper.Deploy(model);

            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["vars"] = new[] { 1, 2, 3 };
            variables["processInstanceVar"] = "processInstanceVarValue";
            var processInstance = engineRule.RuntimeService.StartProcessInstanceByKey("Process_1", variables);

            //second process instance with another process instance variable value
            variables = new Dictionary<string, object>();
            variables["vars"] = new[] { 1, 2, 3 };
            variables["processInstanceVar"] = "anotherProcessInstanceVarValue";
            engineRule.RuntimeService.StartProcessInstanceByKey("Process_1", variables);

            //when correlated by local variables
            var messageName = TEST_MESSAGE_NAME;
            IDictionary<string, object> correlationKeys = new Dictionary<string, object>();
            var correlationKey = 1;
            correlationKeys["localVar"] = correlationKey;
            correlationKeys["constVar"] = "someValue";
            IDictionary<string, object> processInstanceKeys = new Dictionary<string, object>();
            var processInstanceVarValue = "processInstanceVarValue";
            processInstanceKeys["processInstanceVar"] = processInstanceVarValue;
            IDictionary<string, object> messagePayload = new Dictionary<string, object>();
            messagePayload["newVar"] = "newValue";

            var messageCorrelationResult =
                engineRule.RuntimeService.CreateMessageCorrelation(messageName)
                    .ProcessInstanceVariablesEqual(processInstanceKeys)
                    .LocalVariablesEqual(correlationKeys)
                    .SetVariables(messagePayload)
                    .CorrelateWithResult();

            //then exactly one message is correlated = one receive task is passed by, two + three others continue waiting
            checkExecutionMessageCorrelationResult(messageCorrelationResult, processInstance, "MessageReceiver_1");

            //uncorrelated executions
            var uncorrelatedExecutions = engineRule.RuntimeService.CreateExecutionQuery(c => c.ActivityId == "MessageReceiver_1")

                .ToList();
            Assert.AreEqual(5, uncorrelatedExecutions.Count);
        }

        [Test]
        //[ExpectedException(typeof(MismatchingMessageCorrelationException))]
        public virtual void testReceiveTaskMessageCorrelationFail()
        {
            //given
            var model = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("Process_1")
                .StartEvent()
                .SubProcess("SubProcess_1")
                .EmbeddedSubProcess()
                .StartEvent()
                .ReceiveTask("MessageReceiver_1")
                .Message(TEST_MESSAGE_NAME)
                .CamundaInputParameter("localVar", "${loopVar}")
                .CamundaInputParameter("constVar", "someValue")
                .UserTask("UserTask_1")
                .EndEvent()
                .SubProcessDone()
                .MultiInstance()
                .CamundaCollection("${vars}")
                .CamundaElementVariable("loopVar")
                .MultiInstanceDone()
                .EndEvent()
                .Done(); //to test array of parameters

            testHelper.Deploy(model);

            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["vars"] = new[] { 1, 2, 1 };
            engineRule.RuntimeService.StartProcessInstanceByKey("Process_1", variables);

            //when correlated by local variables
            var messageName = TEST_MESSAGE_NAME;
            IDictionary<string, object> correlationKeys = new Dictionary<string, object>();
            var correlationKey = 1;
            correlationKeys["localVar"] = correlationKey;
            correlationKeys["constVar"] = "someValue";

            // declare expected exception
            //thrown.Expect(typeof(MismatchingMessageCorrelationException));
            //thrown.ExpectMessage(string.Format("Cannot correlate a message with name '{0}' to a single execution",
            //    TEST_MESSAGE_NAME));

            engineRule.RuntimeService.CreateMessageCorrelation(messageName)
                .LocalVariablesEqual(correlationKeys)
                .SetVariables(ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                    .PutValue("newVar", "newValue"))
                .CorrelateWithResult();
        }

        [Test]
        public virtual void testReceiveTaskMessageCorrelationAll()
        {
            //given
            var model = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("Process_1")
                .StartEvent()
                .SubProcess("SubProcess_1")
                .EmbeddedSubProcess()
                .StartEvent()
                .ReceiveTask("MessageReceiver_1")
                .Message(TEST_MESSAGE_NAME)
                .CamundaInputParameter("localVar", "${loopVar}")
                .CamundaInputParameter("constVar", "someValue")
                .UserTask("UserTask_1")
                .EndEvent()
                .SubProcessDone()
                .MultiInstance()
                .CamundaCollection("${vars}")
                .CamundaElementVariable("loopVar")
                .MultiInstanceDone()
                .EndEvent()
                .Done(); //to test array of parameters

            testHelper.Deploy(model);

            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["vars"] = new[] { 1, 2, 1 };
            var processInstance = engineRule.RuntimeService.StartProcessInstanceByKey("Process_1", variables);

            //when correlated Permissions.All by local variables
            var messageName = TEST_MESSAGE_NAME;
            IDictionary<string, object> correlationKeys = new Dictionary<string, object>();
            var correlationKey = 1;
            correlationKeys["localVar"] = correlationKey;
            correlationKeys["constVar"] = "someValue";

            var messageCorrelationResults =
                engineRule.RuntimeService.CreateMessageCorrelation(messageName)
                    .LocalVariablesEqual(correlationKeys)
                    .SetVariables(ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                        .PutValue("newVar", "newValue"))
                    .CorrelateAllWithResult();

            //then two messages correlated, one message task is still waiting
            foreach (var result in messageCorrelationResults)
                checkExecutionMessageCorrelationResult(result, processInstance, "MessageReceiver_1");

            //uncorrelated executions
            var uncorrelatedExecutions = engineRule.RuntimeService.CreateExecutionQuery(c => c.ActivityId == "MessageReceiver_1")

                .ToList();
            Assert.AreEqual(1, uncorrelatedExecutions.Count);
        }

        protected internal virtual void checkExecutionMessageCorrelationResult(IMessageCorrelationResult result,
            IProcessInstance processInstance, string activityId)
        {
            Assert.NotNull(result);
            Assert.AreEqual(MessageCorrelationResultType.Execution, result.ResultType);
            Assert.AreEqual(processInstance.Id, result.Execution.ProcessInstanceId);
            var entity = (ExecutionEntity)result.Execution;
            Assert.AreEqual(activityId, entity.ActivityId);
        }
    }
}