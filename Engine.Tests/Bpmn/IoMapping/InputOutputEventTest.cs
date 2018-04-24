using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Calendar;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.IoMapping
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class InputOutputEventTest : PluggableProcessEngineTestCase
    {
        public InputOutputEventTest()
        {
            SetUpAfterEvent += SetUp;
            TearDownAfterEvent += TearDown;
        }
        protected internal virtual void SetUp()
        {
            //base.SetUp();
            VariableLogDelegate.Reset();
        }


        [Deployment]
        [Test]
        public virtual void TestMessageThrowEvent()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            // input mapping
            IDictionary<string, object> mappedVariables = VariableLogDelegate.LOCAL_VARIABLES;
            Assert.AreEqual(1, mappedVariables.Count);
            Assert.AreEqual("mappedValue", ((PrimitiveTypeValueImpl<string>)mappedVariables["mappedVariable"]).Value);

            // output mapping
            string variable = (string)runtimeService.GetVariableLocal(processInstance.Id, "outVariable");
            Assert.AreEqual("mappedValue", variable);
        }

        [Test]
        [Deployment]
        public virtual void TestMessageCatchEvent()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            IExecution messageExecution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "messageCatch").First();
            IDictionary<string, object> localVariables = runtimeService.GetVariablesLocal(messageExecution.Id);
           
            Assert.AreEqual(1, localVariables.Count);
            Assert.AreEqual("mappedValue", ((StringValueImpl)localVariables["mappedVariable"]).Value);

            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["messageVariable"] = "outValue";
            runtimeService.MessageEventReceived("IncomingMessage", messageExecution.Id, variables);

            // output mapping
            string variable = (string)runtimeService.GetVariableLocal(processInstance.Id, "outVariable");
            Assert.AreEqual("outValue", variable);
        }

        [Test]
        [Deployment]
        public virtual void TestTimerCatchEvent()
        {
            IDictionary<string, ITypedValue> variables = new Dictionary<string, ITypedValue>();
            DateTime dueDate = DateTimeUtil.Now().AddMinutes(5);/*.PlusMinutes(5);*/
            variables["outerVariable"] = new DateValueImpl(dueDate);// (new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss")).Format(dueDate);
            runtimeService.StartProcessInstanceByKey("testProcess", variables);

            IJob job = managementService.CreateJobQuery().First();
            TimerEntity timer = (TimerEntity)job;
            AssertDateEquals(dueDate,(DateTime) timer.Duedate);
        }

        protected internal virtual void AssertDateEquals(DateTime expected, DateTime actual)
        {
            string format = "yyyy-MM-dd'T'HH:mm:ss";
            //IDateFormat format = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
            //Assert.AreEqual(format.Format(expected), format.Format(actual));
            Assert.AreEqual(expected.ToString(format), actual.ToString(format));
        }

        [Test]
        [Deployment]
        public virtual void TestNoneThrowEvent()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            IDictionary<string, object> mappedVariables = VariableLogDelegate.LOCAL_VARIABLES;
            Assert.AreEqual(1, mappedVariables.Count);
            Assert.AreEqual("mappedValue", ((PrimitiveTypeValueImpl<string>)mappedVariables["mappedVariable"]).Value);

            // output mapping
            string variable = (string)runtimeService.GetVariableLocal(processInstance.Id, "outVariable");
            Assert.AreEqual("mappedValue", variable);
        }
        [Test]
        public virtual void TestMessageStartEvent()
        {
            try
            {
                repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/iomapping/InputOutputEventTest.TestMessageStartEvent.bpmn20.xml").Deploy();
                Assert.Fail("expected exception");
            }
            catch (System.Exception e)
            {
                AssertTextPresent("camunda:inputOutput mapping unsupported for element type 'startEvent'", e.Message);
            }
        }
        [Test]
        public virtual void TestNoneEndEvent()
        {
            try
            {
                repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/iomapping/InputOutputEventTest.TestNoneEndEvent.bpmn20.xml").Deploy();
                Assert.Fail("expected exception");
            }
            catch (System.Exception e)
            {
                AssertTextPresent("camunda:outputParameter not allowed for element type 'endEvent'", e.Message);
            }
        }

        [Test]
        [Deployment]
        public virtual void TestMessageEndEvent()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");
            //09:23:41.678 [main] DEBUG o.c.b.e.i.p.e.E.selectProcessInstanceCountByQueryCriteria - ==>  Preparing: select count(distinct RES.ID_) from ACT_RU_EXECUTION RES inner join ACT_RE_PROCDEF P on RES.PROC_DEF_ID_ = P.ID_ WHERE RES.PARENT_ID_ is null 
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());

            // input mapping
            IDictionary<string, object> mappedVariables = VariableLogDelegate.LOCAL_VARIABLES;
            Assert.AreEqual(1, mappedVariables.Count);
            Assert.AreEqual("mappedValue", mappedVariables["mappedVariable"]);
        }
        [Test]
        public virtual void TestMessageBoundaryEvent()
        {
            try
            {
                repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/iomapping/InputOutputEventTest.TestMessageBoundaryEvent.bpmn20.xml").Deploy();
                Assert.Fail("expected exception");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("camunda:inputOutput mapping unsupported for element type 'boundaryEvent'", e.Message);
            }
        }


        protected internal virtual void TearDown()
        {
            //base.TearDown();

            VariableLogDelegate.Reset();
        }

    }

}