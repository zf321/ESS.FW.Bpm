using System.Collections.Generic;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.ScriptTask
{

    /// <summary>
    /// 
    /// </summary>
    public class ExternalScriptTaskTest : PluggableProcessEngineTestCase
    {

        [Deployment]
        public virtual void testDefaultExternalScript()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            string greeting = (string)runtimeService.GetVariable(processInstance.Id, "greeting");
            Assert.NotNull(greeting);
            Assert.AreEqual("Greetings camunda BPM speaking", greeting);
        }

        [Deployment]
        public virtual void testDefaultExternalScriptAsVariable()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["scriptPath"] = "resources/bpmn/scripttask/greeting.py";
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process", variables);

            string greeting = (string)runtimeService.GetVariable(processInstance.Id, "greeting");
            Assert.NotNull(greeting);
            Assert.AreEqual("Greetings camunda BPM speaking", greeting);
        }

        [Deployment("resources/bpmn/scripttask/ExternalScriptTaskTest.TestDefaultExternalScriptAsVariable.bpmn20.xml")]
        public virtual void testDefaultExternalScriptAsNonExistingVariable()
        {
            try
            {
                runtimeService.StartProcessInstanceByKey("process");
                Assert.Fail("Process variable 'scriptPath' not defined");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresentIgnoreCase("Cannot resolve identifier 'scriptPath'", e.Message);
            }
        }

        [Deployment]
        public virtual void testDefaultExternalScriptAsBean()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["scriptResourceBean"] = new ScriptResourceBean();
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process", variables);

            string greeting = (string)runtimeService.GetVariable(processInstance.Id, "greeting");
            Assert.NotNull(greeting);
            Assert.AreEqual("Greetings camunda BPM speaking", greeting);
        }

        [Deployment]
        public virtual void testScriptInClasspath()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            string greeting = (string)runtimeService.GetVariable(processInstance.Id, "greeting");
            Assert.NotNull(greeting);
            Assert.AreEqual("Greetings camunda BPM speaking", greeting);
        }

        [Deployment]
        public virtual void testScriptInClasspathAsVariable()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["scriptPath"] = "classpath://resources/bpmn/scripttask/greeting.py";
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process", variables);

            string greeting = (string)runtimeService.GetVariable(processInstance.Id, "greeting");
            Assert.NotNull(greeting);
            Assert.AreEqual("Greetings camunda BPM speaking", greeting);
        }

        [Deployment]
        public virtual void testScriptInClasspathAsBean()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["scriptResourceBean"] = new ScriptResourceBean();
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process", variables);

            string greeting = (string)runtimeService.GetVariable(processInstance.Id, "greeting");
            Assert.NotNull(greeting);
            Assert.AreEqual("Greetings camunda BPM speaking", greeting);
        }

        [Deployment]
        public virtual void testScriptNotFoundInClasspath()
        {
            try
            {
                runtimeService.StartProcessInstanceByKey("process");
                Assert.Fail("Resource does not exist in classpath");
            }
            catch (NotFoundException e)
            {
                AssertTextPresentIgnoreCase("unable to find resource at path classpath://resources/bpmn/scripttask/notexisting.py", e.Message);
            }
        }


        //TODO 暂不支持python
        [Deployment(new[]
         {
             "resources/bpmn/scripttask/ExternalScriptTaskTest.TestScriptInDeployment.bpmn20.xml",
             "resources/bpmn/scripttask/greeting.py"
         })]
        public virtual void testScriptInDeployment()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

            string greeting = (string)runtimeService.GetVariable(processInstance.Id, "greeting");
            Assert.NotNull(greeting);
            Assert.AreEqual("Greetings camunda BPM speaking", greeting);
        }


        //TODO 暂不支持python
        [Test]
        [Deployment(new[]
         {
             "resources/bpmn/scripttask/ExternalScriptTaskTest.TestScriptInDeploymentAsVariable.bpmn20.xml",
             "resources/bpmn/scripttask/greeting.py"
         })]
        public virtual void testScriptInDeploymentAsVariable()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["scriptPath"] = "deployment://resources/bpmn/scripttask/greeting.py";
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process", variables);

            string greeting = (string)runtimeService.GetVariable(processInstance.Id, "greeting");
            Assert.NotNull(greeting);
            Assert.AreEqual("Greetings camunda BPM speaking", greeting);
        }

        [Deployment(new[]
         {
             "resources/bpmn/scripttask/ExternalScriptTaskTest.TestScriptInDeploymentAsBean.bpmn20.xml",
             "resources/bpmn/scripttask/greeting.py"
         })]
        public virtual void testScriptInDeploymentAsBean()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["scriptResourceBean"] = new ScriptResourceBean();
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process", variables);

            string greeting = (string)runtimeService.GetVariable(processInstance.Id, "greeting");
            Assert.NotNull(greeting);
            Assert.AreEqual("Greetings camunda BPM speaking", greeting);
        }

        //TODO 暂不支持python
        [Deployment]
        public virtual void testScriptNotFoundInDeployment()
        {
            try
            {
                runtimeService.StartProcessInstanceByKey("process");
                //Assert.Fail("Resource does not exist in classpath");
            }
            catch (NotFoundException e)
            {
                //AssertTextPresentIgnoreCase("unable to find resource at path deployment://resources/bpmn/scripttask/notexisting.py", e.Message);
            }
        }


        //TODO 暂不支持groovy
        [Deployment]
        public virtual void testNotExistingImport()
        {
            try
            {
                runtimeService.StartProcessInstanceByKey("process");
                Assert.Fail("Should Assert.Fail during script compilation");
            }
            catch (ScriptCompilationException e)
            {
                AssertTextPresentIgnoreCase("import unknown", e.Message);
            }
        }

    }

}