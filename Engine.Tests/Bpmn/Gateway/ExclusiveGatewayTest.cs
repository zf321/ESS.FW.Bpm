using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable.Value;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Gateway
{
    [TestFixture]
    public class ExclusiveGatewayTest : PluggableProcessEngineTestCase
    {

        [Test]
        [Deployment("resources/Bpmn/Gateway/ExclusiveGatewayTest.testDivergingExclusiveGateway.bpmn20.xml")]
        public virtual void testDivergingExclusiveGateway()
        {
            for (int i = 1; i <= 3; i++)
            {
                IProcessInstance pi = runtimeService.StartProcessInstanceByKey("exclusiveGwDiverging", CollectionUtil.SingletonMap("input", i) as IDictionary<string, ITypedValue>);
                Assert.AreEqual("ITask " + i, taskService.CreateTaskQuery().First().Name);
                runtimeService.DeleteProcessInstance(pi.Id, "testing deletion");
            }
        }

        [Test]
        [Deployment]
        public virtual void testMergingExclusiveGateway()
        {
            runtimeService.StartProcessInstanceByKey("exclusiveGwMerging");
            Assert.AreEqual(3, taskService.CreateTaskQuery().Count());
        }

        [Test]
        [Deployment]
        public virtual void testMultipleValidConditions()
        {
            runtimeService.StartProcessInstanceByKey("exclusiveGwMultipleValidConditions", CollectionUtil.SingletonMap("input", 5));
            Assert.AreEqual("ITask 2", taskService.CreateTaskQuery().First().Name);
        }

        [Test]
        [Deployment]
        public virtual void testNoSequenceFlowSelected()
        {
            try
            {
                runtimeService.StartProcessInstanceByKey("exclusiveGwNoSeqFlowSelected", CollectionUtil.SingletonMap("input", 4));
                Assert.Fail();
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("ENGINE-02004 No outgoing sequence flow for the element with id 'exclusiveGw' could be selected for continuing the process.", e.Message);
            }
        }

        [Test]
        [Deployment]
        public virtual void testWhitespaceInExpression()
        {
            // Starting a process instance will lead to an exception if whitespace are incorrectly handled
            runtimeService.StartProcessInstanceByKey("whiteSpaceInExpression", CollectionUtil.SingletonMap("input", 1));
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/gateway/ExclusiveGatewayTest.TestDivergingExclusiveGateway.bpmn20.xml" })]
        public virtual void testUnknownVariableInExpression()
        {
            // Instead of 'input' we're starting a process instance with the name 'iinput' (ie. a typo)
            try
            {
                runtimeService.StartProcessInstanceByKey("exclusiveGwDiverging", CollectionUtil.SingletonMap("iinput", 1));
                Assert.Fail();
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Unknown property used in expression", e.Message);
            }
        }

        [Test]
        [Deployment]
        public virtual void testDecideBasedOnBeanProperty()
        {
            runtimeService.StartProcessInstanceByKey("decisionBasedOnBeanProperty", CollectionUtil.SingletonMap("order", new ExclusiveGatewayTestOrder(150)));

            ITask task = taskService.CreateTaskQuery().First();
            Assert.NotNull(task);
            Assert.AreEqual("Standard service", task.Name);
        }

        [Test]
        [Deployment]
        public virtual void testDecideBasedOnListOrArrayOfBeans()
        {
            IList<ExclusiveGatewayTestOrder> orders = new List<ExclusiveGatewayTestOrder>();
            orders.Add(new ExclusiveGatewayTestOrder(50));
            orders.Add(new ExclusiveGatewayTestOrder(300));
            orders.Add(new ExclusiveGatewayTestOrder(175));

            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("decisionBasedOnListOrArrayOfBeans", CollectionUtil.SingletonMap("orders", orders));

            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi.Id).First();
            Assert.NotNull(task);
            Assert.AreEqual("Gold Member service", task.Name);


            // Arrays are usable in exactly the same way
            ExclusiveGatewayTestOrder[] orderArray = orders.ToArray();
            orderArray[1].Price = 10;
            pi = runtimeService.StartProcessInstanceByKey("decisionBasedOnListOrArrayOfBeans", CollectionUtil.SingletonMap("orders", orderArray));

            task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi.Id).First();
            Assert.NotNull(task);
            Assert.AreEqual("Basic service", task.Name);
        }

        [Test]
        [Deployment]
        public virtual void testDecideBasedOnBeanMethod()
        {
            runtimeService.StartProcessInstanceByKey("decisionBasedOnBeanMethod", CollectionUtil.SingletonMap("order", new ExclusiveGatewayTestOrder(300)));

            ITask task = taskService.CreateTaskQuery().First();
            Assert.NotNull(task);
            Assert.AreEqual("Gold Member service", task.Name);
        }

        [Test]
        [Deployment]
        public virtual void testInvalidMethodExpression()
        {
            try
            {
                runtimeService.StartProcessInstanceByKey("invalidMethodExpression", CollectionUtil.SingletonMap("order", new ExclusiveGatewayTestOrder(50)));
                Assert.Fail();
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Unknown method used in expression", e.Message);
            }
        }

        [Test]
        [Deployment]
        public virtual void testDefaultSequenceFlow()
        {

            // Input == 1 -> default is not selected
            string procId = runtimeService.StartProcessInstanceByKey("exclusiveGwDefaultSequenceFlow", CollectionUtil.SingletonMap("input", 1)).Id;
            ITask task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("Input is one", task.Name);
            runtimeService.DeleteProcessInstance(procId, null);

            procId = runtimeService.StartProcessInstanceByKey("exclusiveGwDefaultSequenceFlow", CollectionUtil.SingletonMap("input", 5)).Id;
            task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("Default input", task.Name);
        }

        [Test]
        [Deployment]
        public virtual void testNoIdOnSequenceFlow()
        {
            runtimeService.StartProcessInstanceByKey("noIdOnSequenceFlow", CollectionUtil.SingletonMap("input", 3));
            ITask task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("Input is more than one", task.Name);
        }

        public virtual void testInvalidProcessDefinition()
        {
            string flowWithoutConditionNoDefaultFlow = "<?xml version='1.0' encoding='UTF-8'?>" + "<definitions id='definitions' xmlns='http://www.omg.org/spec/BPMN/20100524/MODEL' xmlns:xsi='http://www.W3.org/2001/XMLSchema-instance' xmlns:activiti='http://activiti.org/bpmn' targetNamespace='Examples'>" + "  <process id='exclusiveGwDefaultSequenceFlow' isExecutable='true'> " + "    <startEvent id='theStart' /> " + "    <sequenceFlow id='flow1' sourceRef='theStart' targetRef='exclusiveGw' /> " + "    <exclusiveGateway id='exclusiveGw' name='Exclusive Gateway' /> " + "    <sequenceFlow id='flow2' sourceRef='exclusiveGw' targetRef='theTask1'> " + "      <conditionExpression xsi:type='tFormalExpression'>${input == 1}</conditionExpression> " + "    </sequenceFlow> " + "    <sequenceFlow id='flow3' sourceRef='exclusiveGw' targetRef='theTask2'/> " + "    <sequenceFlow id='flow4' sourceRef='exclusiveGw' targetRef='theTask2'/> " + "    <userTask id='theTask1' name='Input is one' /> " + "    <userTask id='theTask2' name='Default input' /> " + "  </process>" + "</definitions>";
            try
            {
                repositoryService.CreateDeployment().AddString("myprocess.bpmn20.xml", flowWithoutConditionNoDefaultFlow).Deploy();
                Assert.Fail("Could deploy a process definition with a sequence flow out of a XOR Gateway without condition with is not the default flow.");
            }
            catch (ProcessEngineException ex)
            {
                Assert.True(ex.Message.Contains("Exclusive Gateway 'exclusiveGw' has outgoing sequence flow 'flow3' without condition which is not the default flow."));
            }

            string defaultFlowWithCondition = "<?xml version='1.0' encoding='UTF-8'?>" + "<definitions id='definitions' xmlns='http://www.omg.org/spec/BPMN/20100524/MODEL' xmlns:xsi='http://www.W3.org/2001/XMLSchema-instance' xmlns:activiti='http://activiti.org/bpmn' targetNamespace='Examples'>" + "  <process id='exclusiveGwDefaultSequenceFlow' isExecutable='true'> " + "    <startEvent id='theStart' /> " + "    <sequenceFlow id='flow1' sourceRef='theStart' targetRef='exclusiveGw' /> " + "    <exclusiveGateway id='exclusiveGw' name='Exclusive Gateway' default='flow3' /> " + "    <sequenceFlow id='flow2' sourceRef='exclusiveGw' targetRef='theTask1'> " + "      <conditionExpression xsi:type='tFormalExpression'>${input == 1}</conditionExpression> " + "    </sequenceFlow> " + "    <sequenceFlow id='flow3' sourceRef='exclusiveGw' targetRef='theTask2'> " + "      <conditionExpression xsi:type='tFormalExpression'>${input == 3}</conditionExpression> " + "    </sequenceFlow> " + "    <userTask id='theTask1' name='Input is one' /> " + "    <userTask id='theTask2' name='Default input' /> " + "  </process>" + "</definitions>";
            try
            {
                repositoryService.CreateDeployment().AddString("myprocess.bpmn20.xml", defaultFlowWithCondition).Deploy();
                Assert.Fail("Could deploy a process definition with a sequence flow out of a XOR Gateway without condition with is not the default flow.");
            }
            catch (ProcessEngineException ex)
            {
                Assert.True(ex.Message.Contains("Exclusive Gateway 'exclusiveGw' has outgoing sequence flow 'flow3' which is the default flow but has a condition too."));
            }

            string noOutgoingFlow = "<?xml version='1.0' encoding='UTF-8'?>" + "<definitions id='definitions' xmlns='http://www.omg.org/spec/BPMN/20100524/MODEL' xmlns:xsi='http://www.W3.org/2001/XMLSchema-instance' xmlns:activiti='http://activiti.org/bpmn' targetNamespace='Examples'>" + "  <process id='exclusiveGwDefaultSequenceFlow' isExecutable='true'> " + "    <startEvent id='theStart' /> " + "    <sequenceFlow id='flow1' sourceRef='theStart' targetRef='exclusiveGw' /> " + "    <exclusiveGateway id='exclusiveGw' name='Exclusive Gateway' /> " + "  </process>" + "</definitions>";
            try
            {
                repositoryService.CreateDeployment().AddString("myprocess.bpmn20.xml", noOutgoingFlow).Deploy();
                Assert.Fail("Could deploy a process definition with a sequence flow out of a XOR Gateway without condition with is not the default flow.");
            }
            catch (ProcessEngineException ex)
            {
                Console.WriteLine(ex.ToString());
                Console.Write(ex.StackTrace);
                Assert.True(ex.Message.Contains("Exclusive Gateway 'exclusiveGw' has no outgoing sequence flows."));
            }

        }

        [Test]
        [Deployment]
        public virtual void testLoopWithManyIterations()
        {
            int numOfIterations = 1000;

            // this should not Assert.Fail
            runtimeService.StartProcessInstanceByKey("testProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("numOfIterations", numOfIterations));
        }

        [Test]
        [Deployment]
        public virtual void testDecisionFunctionality()
        {

            IDictionary<string, object> variables = new Dictionary<string, object>();

            // Test with input == 1
            variables["input"] = 1;
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("exclusiveGateway", variables);
            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi.Id).First();
            Assert.AreEqual("Send e-mail for more information", task.Name);

            // Test with input == 2
            variables["input"] = 2;
            pi = runtimeService.StartProcessInstanceByKey("exclusiveGateway", variables);
            task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi.Id).First();
            Assert.AreEqual("Check account balance", task.Name);

            // Test with input == 3
            variables["input"] = 3;
            pi = runtimeService.StartProcessInstanceByKey("exclusiveGateway", variables);
            task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==pi.Id).First();
            Assert.AreEqual("Call customer", task.Name);

            // Test with input == 4
            variables["input"] = 4;
            try
            {
                runtimeService.StartProcessInstanceByKey("exclusiveGateway", variables);
                Assert.Fail();
            }
            catch (ProcessEngineException)
            {
                // Exception is expected since no outgoing sequence flow matches
            }

        }
    }

}