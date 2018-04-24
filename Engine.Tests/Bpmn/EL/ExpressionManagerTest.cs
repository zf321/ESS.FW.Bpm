using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.EL
{
    [TestFixture]
    public class ExpressionManagerTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment]
        public virtual void testMethodExpressions()
        {
            // Process contains 2 service tasks. one containing a method with no params, the other
            // contains a method with 2 params. When the process completes without exception,
            // test passed.
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["aString"] = "abcdefgh";
            runtimeService.StartProcessInstanceByKey("methodExpressionProcess", vars);

            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == "methodExpressionProcess").Count());
        }

        [Test]
        [Deployment]
        public virtual void testExecutionAvailable()
        {
            IDictionary<string, object> vars = new Dictionary<string, object>();

            vars["myVar"] = new ExecutionTestVariable();
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("testExecutionAvailableProcess", vars);

            // Check of the testMethod has been called with the current execution
            string value = (string)runtimeService.GetVariable(processInstance.Id, "testVar");
            Assert.NotNull(value);
            Assert.AreEqual("myValue", value);
        }

        [Test]
        [Deployment]
        public virtual void testAuthenticatedUserIdAvailable()
        {
            try
            {
                // Setup authentication
                identityService.AuthenticatedUserId = "frederik";
                IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("testAuthenticatedUserIdAvailableProcess");

                // Check if the variable that has been set in service-task is the authenticated user
                string value = (string)runtimeService.GetVariable(processInstance.Id, "theUser");
                Assert.NotNull(value);
                Assert.AreEqual("frederik", value);
            }
            finally
            {
                // Cleanup
                identityService.ClearAuthentication();
            }
        }

        [Test]
        [Deployment]
        public virtual void testResolvesVariablesFromDifferentScopes()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["assignee"] = "michael";

            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);
            ITask task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("michael", task.Assignee);

            variables["assignee"] = "johnny";
            IProcessInstance secondInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);
            task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==secondInstance.Id).First();
            Assert.AreEqual("johnny", task.Assignee);
        }
    }

}