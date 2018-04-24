using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Bpmn.ServiceTask.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.ServiceTask
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class JavaServiceTaskTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment]
        public virtual void TestGetBusinessKeyFromDelegateExecution()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("businessKeyProcess", "1234567890");
            int result = runtimeService.CreateProcessInstanceQuery()
                //.SetProcessDefinitionKey("businessKeyProcess")
                .Count();
            Assert.AreEqual( true,result>0);

            // Check if business-key was available from the process
            var key = (string) runtimeService.GetVariable(processInstance.Id, "businessKeySetOnExecution");
            Assert.NotNull(key);
            Assert.AreEqual("1234567890", key);
        }

        [Test]
        [Deployment]
        public virtual void TestExceptionHandling()
        {
            // If variable value is != 'throw-exception', process goes
            // through service task and ends immidiately
            IDictionary<string, ITypedValue> vars = new Dictionary<string, ITypedValue>();
            vars["var"] = new StringValueImpl("no-exception");


            var ins = runtimeService.StartProcessInstanceByKey("exceptionHandling", vars);
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery(c=>c.Id == ins.Id)
                .Count());

            // If variable value == 'throw-exception', process executes
            // service task, which generates and catches exception,
            // and takes sequence flow to IUser task
            vars["var"] = new StringValueImpl("throw-exception");

            runtimeService.StartProcessInstanceByKey("exceptionHandling", vars);
            ITask task = taskService.CreateTaskQuery()
                .First();
            // Assert.AreEqual("Fix Exception", task.Name);
            Assert.AreEqual("subtask two", task.Name);
            
        }

        [Test]
        [Deployment]
        public virtual void TestExpressionFieldInjection()
        {
            //IDictionary<string, object> vars = new Dictionary<string, object>();
            //vars["name"] = "kermit";
            //vars["gender"] = "male";
            //vars["genderBean"] = new GenderBean();

            IDictionary<string, ITypedValue> vars = new Dictionary<string, ITypedValue>();
            vars["name"] = new StringValueImpl("kermit");
            vars["gender"] = new StringValueImpl("male");
            vars["genderBean"] = new ObjectValueImpl(new GenderBean());

            // var pi = runtimeService.StartProcessInstanceByKey("simpleSubProcess", vars);

            var pi = runtimeService.StartProcessInstanceByKey("expressionFieldInjection", vars);
            var execution =
                runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == pi.Id && c.ActivityId == "waitState")
                    .First();

            Assert.AreEqual("timrek .rM olleH", runtimeService.GetVariable(execution.Id, "var2"));
            Assert.AreEqual("elam :si redneg ruoY", runtimeService.GetVariable(execution.Id, "var1"));
        }


        [Test]
        [Deployment]
        public virtual void TestFieldInjection()
        {
            var pi = runtimeService.StartProcessInstanceByKey("fieldInjection");
            var execution =
                runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == pi.Id && c.ActivityId == "waitState")
                    .First();

            Assert.AreEqual("HELLO WORLD", runtimeService.GetVariable(execution.Id, "var"));
            Assert.AreEqual("HELLO SETTER", runtimeService.GetVariable(execution.Id, "setterVar"));
        }
        
        public virtual void TestIllegalUseOfResultVariableName()
        {
            try
            {
                repositoryService.CreateDeployment()
                    .AddClasspathResource(
                        "resources/bpmn/servicetask/JavaServiceTaskTest.TestIllegalUseOfResultVariableName.bpmn20.xml")
                    .Deploy();
                Assert.Fail();
            }
            catch (ProcessEngineException e)
            {
                Assert.True(e.Message.Contains("resultVariable"));
            }
        }

        [Test]
        [Deployment]
        public virtual void TestJavaServiceDelegation()
        {
            IDictionary<string, ITypedValue> vars = new Dictionary<string, ITypedValue>();
            vars["input"] = new StringValueImpl("Activiti BPM Engine");
            var pi = runtimeService.StartProcessInstanceByKey("javaServiceDelegation", vars);
            var execution =
                runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == pi.Id && c.ActivityId == "waitState")
                    .FirstOrDefault();
            Assert.AreEqual("ACTIVITI BPM ENGINE", runtimeService.GetVariable(execution?.Id, "input"));
        }

        [Test]
        [Deployment]
        public virtual void TestJavaServiceMethodDelegation()
        {
            IDictionary<string, ITypedValue> vars = new Dictionary<string, ITypedValue>();
            vars["name"] = new StringValueImpl("kermit");
            vars["gender"] = new StringValueImpl("male");
            vars["genderBean"] = new ObjectValueImpl(new GenderBean());
            var pi = runtimeService.StartProcessInstanceByKey("javaServiceMethodDelegation", vars);
            var execution =
                runtimeService.CreateExecutionQuery(c => c.ProcessInstanceId == pi.Id && c.ActivityId == "waitState")
                    .FirstOrDefault();
            Assert.AreEqual("YOUR GENDER IS: MALE", runtimeService.GetVariable(execution?.Id, "result"));
        }
        [Test]
        [Deployment]
        public virtual void TestUnexistingClassDelegation()
        {
            try
            {
                runtimeService.StartProcessInstanceByKey("unexistingClassDelegation");
                Assert.Fail();
            }
            catch (ProcessEngineException e)
            {
                Assert.True(e.Message.Contains("Exception while instantiating class 'test.BogusClass'"));
                Assert.NotNull(e.InnerException);
                //Assert.True(e.InnerException is ClassLoadingException);
            }
        }
    }
}