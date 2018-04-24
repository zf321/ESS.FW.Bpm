using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Engine.Tests.Api.Form
{
    [TestFixture]
    public class FormPropertyDefaultValueTest : PluggableProcessEngineTestCase
    {

        [Test]
        [Deployment]
        public virtual void testDefaultValue()
        {
            var processInstance =
                runtimeService.StartProcessInstanceByKey("FormPropertyDefaultValueTest.TestDefaultValue");
            var task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();

            var formData = formService.GetTaskFormData(task.Id);
            var formProperties = formData.FormProperties;
            Assert.AreEqual(4, formProperties.Count);

            foreach (var prop in formProperties)
                if ("booleanProperty".Equals(prop.Id))
                    Assert.AreEqual("true", prop.Value);
                else if ("stringProperty".Equals(prop.Id))
                    Assert.AreEqual("someString", prop.Value);
                else if ("longProperty".Equals(prop.Id))
                    Assert.AreEqual("42", prop.Value);
                else if ("longExpressionProperty".Equals(prop.Id))
                    Assert.AreEqual("23", prop.Value);
                else
                    Assert.True(false, "Invalid form property: " + prop.Id);

            IDictionary<string, string> formDataUpdate = new Dictionary<string, string>();
            formDataUpdate["longExpressionProperty"] = "1";
            formDataUpdate["booleanProperty"] = "false";
            formService.SubmitTaskFormData(task.Id, formDataUpdate);

            Assert.AreEqual(false, runtimeService.GetVariable(processInstance.Id, "booleanProperty"));
            Assert.AreEqual("someString", runtimeService.GetVariable(processInstance.Id, "stringProperty"));
            Assert.AreEqual(42L, runtimeService.GetVariable(processInstance.Id, "longProperty"));
            Assert.AreEqual(1L, runtimeService.GetVariable(processInstance.Id, "longExpressionProperty"));
        }

        [Test]
        [Deployment]
        public virtual void testStartFormDefaultValue()
        {
            var processDefinitionId = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="FormPropertyDefaultValueTest.TestDefaultValue")
                /*.LatestVersion()*/
                .First()
                .Id;

            var startForm = formService.GetStartFormData(processDefinitionId);


            var formProperties = startForm.FormProperties;
            Assert.AreEqual(4, formProperties.Count);

            foreach (var prop in formProperties)
                if ("booleanProperty".Equals(prop.Id))
                    Assert.AreEqual("true", prop.Value);
                else if ("stringProperty".Equals(prop.Id))
                    Assert.AreEqual("someString", prop.Value);
                else if ("longProperty".Equals(prop.Id))
                    Assert.AreEqual("42", prop.Value);
                else if ("longExpressionProperty".Equals(prop.Id))
                    Assert.AreEqual("23", prop.Value);
                else
                    Assert.True(false, "Invalid form property: " + prop.Id);

            // Override 2 properties. The others should pe posted as the default-value
            IDictionary<string, string> formDataUpdate = new Dictionary<string, string>();
            formDataUpdate["longExpressionProperty"] = "1";
            formDataUpdate["booleanProperty"] = "false";
            var processInstance = formService.SubmitStartFormData(processDefinitionId, formDataUpdate);

            Assert.AreEqual(false, runtimeService.GetVariable(processInstance.Id, "booleanProperty"));
            Assert.AreEqual("someString", runtimeService.GetVariable(processInstance.Id, "stringProperty"));
            Assert.AreEqual(42L, runtimeService.GetVariable(processInstance.Id, "longProperty"));
            Assert.AreEqual(1L, runtimeService.GetVariable(processInstance.Id, "longExpressionProperty"));
        }
    }
}