using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Form;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;
using NUnit.Framework;

namespace Engine.Tests.Api.Form
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class FormServiceTest : PluggableProcessEngineTestCase
    {
        [SetUp]
        public virtual void setUp()
        {
            identityService.SaveUser(identityService.NewUser("fozzie"));
            identityService.SaveGroup(identityService.NewGroup("management"));
            identityService.CreateMembership("fozzie", "management");
        }
        [TearDown]
        public virtual void tearDown()
        {
            identityService.DeleteGroup("management");
            identityService.DeleteUser("fozzie");
        }

        [Test][Deployment(new [] { "resources/api/form/util/VacationRequest_deprecated_forms.bpmn20.xml", "resources/api/form/util/approve.Form", "resources/api/form/util/request.Form", "resources/api/form/util/adjustRequest.Form" }) ]
        public virtual void testGetStartFormByProcessDefinitionId()
        {
            var processDefinitions = repositoryService.CreateProcessDefinitionQuery()
                
                .ToList();
            Assert.AreEqual(1, processDefinitions.Count);
            var processDefinition = processDefinitions[0];

            var startForm = formService.GetRenderedStartForm(processDefinition.Id, "juel");
            Assert.NotNull(startForm);
        }

        [Test][Deployment(  "resources/api/oneTaskProcess.bpmn20.xml" ) ]
        public virtual void testGetStartFormByProcessDefinitionIdWithoutStartform()
        {
            var processDefinitions = repositoryService.CreateProcessDefinitionQuery()
                
                .ToList();
            Assert.AreEqual(1, processDefinitions.Count);
            var processDefinition = processDefinitions[0];

            var startForm = formService.GetRenderedStartForm(processDefinition.Id);
            Assert.IsNull(startForm);
        }

        [Test]
        public virtual void testGetStartFormByKeyNullKey()
        {
            try
            {
                formService.GetRenderedStartForm(null);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException)
            {
                // Exception expected
            }
        }

        [Test]
        public virtual void testGetStartFormByIdNullId()
        {
            try
            {
                formService.GetRenderedStartForm(null);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException)
            {
                // Exception expected
            }
        }

        [Test]
        public virtual void testGetStartFormByIdUnexistingProcessDefinitionId()
        {
            try
            {
                formService.GetRenderedStartForm("unexistingId");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("no deployed process definition found with id", ae.Message);
            }
        }

        [Test]
        public virtual void testGetTaskFormNullTaskId()
        {
            try
            {
                formService.GetRenderedTaskForm(null);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException)
            {
                // Expected Exception
            }
        }

        [Test]
        public virtual void testGetTaskFormUnexistingTaskId()
        {
            try
            {
                formService.GetRenderedTaskForm("unexistingtask");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("ITask 'unexistingtask' not found", ae.Message);
            }
        }

        [Test][Deployment(new [] { "resources/api/form/FormsProcess.bpmn20.xml", "resources/api/form/start.Form", "resources/api/form/task.Form" })]
        public virtual void testTaskFormPropertyDefaultsAndFormRendering()
        {
            var procDefId = repositoryService.CreateProcessDefinitionQuery()
                .First()
                .Id;
            var startForm = formService.GetStartFormData(procDefId);
            Assert.NotNull(startForm);
            Assert.AreEqual(DeploymentId, startForm.DeploymentId);
            Assert.AreEqual("resources/api/form/start.Form", startForm.FormKey);
            Assert.AreEqual(new List<IFormProperty>(), startForm.FormProperties);
            Assert.AreEqual(procDefId, startForm.ProcessDefinition.Id);

            var renderedStartForm = formService.GetRenderedStartForm(procDefId, "juel");
            Assert.AreEqual("start form content", renderedStartForm);

            IDictionary<string, string> properties = new Dictionary<string, string>();
            properties["room"] = "5b";
            properties["speaker"] = "Mike";
            var ProcessInstanceId = formService.SubmitStartFormData(procDefId, properties)
                .Id;

            IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
            expectedVariables["room"] = "5b";
            expectedVariables["speaker"] = "Mike";

            var variables = runtimeService.GetVariables(ProcessInstanceId);
            Assert.AreEqual(expectedVariables, variables);

            var task = taskService.CreateTaskQuery()
                .First();
            var taskId = task.Id;
            var taskForm = formService.GetTaskFormData(taskId);
            Assert.AreEqual(DeploymentId, taskForm.DeploymentId);
            Assert.AreEqual("resources/api/form/task.Form", taskForm.FormKey);
            Assert.AreEqual(new List<IFormProperty>(), taskForm.FormProperties);
            Assert.AreEqual(taskId, taskForm.Task.Id);

            Assert.AreEqual("Mike is speaking in room 5b", formService.GetRenderedTaskForm(taskId, "juel"));

            properties = new Dictionary<string, string>();
            properties["room"] = "3f";
            formService.SubmitTaskFormData(taskId, properties);

            expectedVariables = new Dictionary<string, object>();
            expectedVariables["room"] = "3f";
            expectedVariables["speaker"] = "Mike";

            variables = runtimeService.GetVariables(ProcessInstanceId);
            Assert.AreEqual(expectedVariables, variables);
        }

        [Test][Deployment ]
        public virtual void testFormPropertyHandlingDeprecated()
        {
            IDictionary<string, string> properties = new Dictionary<string, string>();
            properties["room"] = "5b"; // default
            properties["speaker"] = "Mike"; // variable name mapping
            properties["duration"] = "45"; // type conversion
            properties["free"] = "true"; // type conversion

            var procDefId = repositoryService.CreateProcessDefinitionQuery()
                .First()
                .Id;
            var ProcessInstanceId = formService.SubmitStartFormData(procDefId, properties)
                .Id;

            IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
            expectedVariables["room"] = "5b";
            expectedVariables["SpeakerName"] = "Mike";
            expectedVariables["duration"] = 45;
            expectedVariables["free"] = true;

            var variables = runtimeService.GetVariables(ProcessInstanceId);
            Assert.AreEqual(expectedVariables, variables);

            var address = new Address();
            address.Street = "broadway";
            runtimeService.SetVariable(ProcessInstanceId, "address", address);

            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;
            var taskFormData = formService.GetTaskFormData(taskId);

            var formProperties = taskFormData.FormProperties;
            var propertyRoom = formProperties[0];
            Assert.AreEqual("room", propertyRoom.Id);
            Assert.AreEqual("5b", propertyRoom.Value);

            var propertyDuration = formProperties[1];
            Assert.AreEqual("duration", propertyDuration.Id);
            Assert.AreEqual("45", propertyDuration.Value);

            var propertySpeaker = formProperties[2];
            Assert.AreEqual("speaker", propertySpeaker.Id);
            Assert.AreEqual("Mike", propertySpeaker.Value);

            var propertyStreet = formProperties[3];
            Assert.AreEqual("street", propertyStreet.Id);
            Assert.AreEqual("broadway", propertyStreet.Value);

            var propertyFree = formProperties[4];
            Assert.AreEqual("free", propertyFree.Id);
            Assert.AreEqual("true", propertyFree.Value);

            Assert.AreEqual(5, formProperties.Count);

            try
            {
                formService.SubmitTaskFormData(taskId, new Dictionary<string, string>());
                Assert.Fail("expected exception about required form property 'street'");
            }
            catch (ProcessEngineException)
            {
                // OK
            }

            try
            {
                properties = new Dictionary<string, string>();
                properties["speaker"] = "its not allowed to update speaker!";
                formService.SubmitTaskFormData(taskId, properties);
                Assert.Fail("expected exception about a non writable form property 'speaker'");
            }
            catch (ProcessEngineException)
            {
                // OK
            }

            properties = new Dictionary<string, string>();
            properties["street"] = "rubensstraat";
            formService.SubmitTaskFormData(taskId, properties);

            expectedVariables = new Dictionary<string, object>();
            expectedVariables["room"] = "5b";
            expectedVariables["SpeakerName"] = "Mike";
            expectedVariables["duration"] = 45;
            expectedVariables["free"] = true;

            variables = runtimeService.GetVariables(ProcessInstanceId);
            //address = (Address) variables.Remove("address");
            Assert.AreEqual("rubensstraat", address.Street);
            Assert.AreEqual(expectedVariables, variables);
        }

        [Test]
        [Deployment]
        public virtual void testFormPropertyHandling()
        {
            IDictionary<string, object> properties = new Dictionary<string, object>();
            properties["room"] = "5b"; // default
            properties["speaker"] = "Mike"; // variable name mapping
            properties["duration"] = 45L; // type conversion
            properties["free"] = "true"; // type conversion

            var procDefId = repositoryService.CreateProcessDefinitionQuery()
                .First()
                .Id;
            var ProcessInstanceId = formService.SubmitStartForm(procDefId, properties)
                .Id;

            IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
            expectedVariables["room"] = "5b";
            expectedVariables["SpeakerName"] = "Mike";
            expectedVariables["duration"] = 45;
            expectedVariables["free"] = true;

            var variables = runtimeService.GetVariables(ProcessInstanceId);
            Assert.AreEqual(expectedVariables, variables);

            var address = new Address();
            address.Street = "broadway";
            runtimeService.SetVariable(ProcessInstanceId, "address", address);

            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;
            var taskFormData = formService.GetTaskFormData(taskId);

            var formProperties = taskFormData.FormProperties;
            var propertyRoom = formProperties[0];
            Assert.AreEqual("room", propertyRoom.Id);
            Assert.AreEqual("5b", propertyRoom.Value);

            var propertyDuration = formProperties[1];
            Assert.AreEqual("duration", propertyDuration.Id);
            Assert.AreEqual("45", propertyDuration.Value);

            var propertySpeaker = formProperties[2];
            Assert.AreEqual("speaker", propertySpeaker.Id);
            Assert.AreEqual("Mike", propertySpeaker.Value);

            var propertyStreet = formProperties[3];
            Assert.AreEqual("street", propertyStreet.Id);
            Assert.AreEqual("broadway", propertyStreet.Value);

            var propertyFree = formProperties[4];
            Assert.AreEqual("free", propertyFree.Id);
            Assert.AreEqual("true", propertyFree.Value);

            Assert.AreEqual(5, formProperties.Count);

            try
            {
                //formService.SubmitTaskForm(taskId, new Dictionary<string, object>());
                Assert.Fail("expected exception about required form property 'street'");
            }
            catch (ProcessEngineException)
            {
                // OK
            }

            try
            {
                properties = new Dictionary<string, object>();
                properties["speaker"] = "its not allowed to update speaker!";
                //formService.SubmitTaskForm(taskId, properties);
                Assert.Fail("expected exception about a non writable form property 'speaker'");
            }
            catch (ProcessEngineException)
            {
                // OK
            }

            properties = new Dictionary<string, object>();
            properties["street"] = "rubensstraat";
            //formService.SubmitTaskForm(taskId, properties);

            expectedVariables = new Dictionary<string, object>();
            expectedVariables["room"] = "5b";
            expectedVariables["SpeakerName"] = "Mike";
            expectedVariables["duration"] = 45;
            expectedVariables["free"] = true;

            variables = runtimeService.GetVariables(ProcessInstanceId);
            //address = (Address) variables.Remove("address");
            Assert.AreEqual("rubensstraat", address.Street);
            Assert.AreEqual(expectedVariables, variables);
        }

        [Test]
        [Deployment]
        public virtual void testFormPropertyDetails()
        {
            var procDefId = repositoryService.CreateProcessDefinitionQuery()
                .First()
                .Id;
            var startFormData = formService.GetStartFormData(procDefId);
            var property = startFormData.FormProperties[0];
            Assert.AreEqual("speaker", property.Id);
            Assert.IsNull(property.Value);
            Assert.True(property.Readable);
            Assert.True(property.Writable);
            Assert.IsFalse(property.Required);
            Assert.AreEqual("string", property.Type.Name);

            property = startFormData.FormProperties[1];
            Assert.AreEqual("start", property.Id);
            Assert.IsNull(property.Value);
            Assert.True(property.Readable);
            Assert.True(property.Writable);
            Assert.IsFalse(property.Required);
            Assert.AreEqual("date", property.Type.Name);
            Assert.AreEqual("dd-MMM-yyyy", property.Type.GetInformation("datePattern"));

            property = startFormData.FormProperties[2];
            Assert.AreEqual("direction", property.Id);
            Assert.IsNull(property.Value);
            Assert.True(property.Readable);
            Assert.True(property.Writable);
            Assert.IsFalse(property.Required);
            Assert.AreEqual("enum", property.Type.Name);
            var values = (IDictionary<string, string>) property.Type.GetInformation("values");

            IDictionary<string, string> expectedValues = new Dictionary<string, string>();
            expectedValues["left"] = "Go Left";
            expectedValues["right"] = "Go Right";
            expectedValues["up"] = "Go Up";
            expectedValues["down"] = "Go Down";

            // ACT-1023: check if ordering is retained
            var expectedValuesIterator = expectedValues.GetEnumerator();
            foreach (var entry in values)
            {
                //JAVA TO C# CONVERTER TODO Resources.Task: Java iterators are only converted within the context of 'while' and 'for' loops:
                expectedValuesIterator.MoveNext();
                var expectedEntryAtLocation = expectedValuesIterator.Current;
                Assert.AreEqual(expectedEntryAtLocation.Key, entry.Key);
                Assert.AreEqual(expectedEntryAtLocation.Value, entry.Value);
            }
            Assert.AreEqual(expectedValues, values);
        }

        [Test]
        [Deployment]
        public virtual void testInvalidFormKeyReference()
        {
            try
            {
                formService.GetRenderedStartForm(repositoryService.CreateProcessDefinitionQuery()
                    .First()
                    .Id, "juel");
                Assert.Fail();
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Form with formKey 'IDoNotExist' does not exist", e.Message);
            }
        }

        [Test]
        [Deployment]
        public virtual void testSubmitStartFormDataWithBusinessKey()
        {
            IDictionary<string, string> properties = new Dictionary<string, string>();
            properties["duration"] = "45";
            properties["speaker"] = "Mike";
            var procDefId = repositoryService.CreateProcessDefinitionQuery()
                .First()
                .Id;

            var processInstance = formService.SubmitStartFormData(procDefId, "123", properties);
            Assert.AreEqual("123", processInstance.BusinessKey);

            Assert.AreEqual(processInstance.Id, runtimeService.CreateProcessInstanceQuery()
               // .SetProcessInstanceBusinessKey("123")
                .First()
                .Id);
        }

        [Test][Deployment( "resources/api/form/FormsProcess.bpmn20.xml")]
        public virtual void testSubmitStartFormDataTypedVariables()
        {
            var procDefId = repositoryService.CreateProcessDefinitionQuery()
                .First()
                .Id;

            var stringValue = "some string";
            var serializedValue = "some value";

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var processInstance = formService.SubmitStartForm(procDefId, ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValueTyped("boolean", ESS.FW.Bpm.Engine.Variable.Variables.BooleanValue(null))
                .PutValueTyped("string", ESS.FW.Bpm.Engine.Variable.Variables.StringValue(stringValue))
                .PutValueTyped("serializedObject", ESS.FW.Bpm.Engine.Variable.Variables.SerializedObjectValue(serializedValue)
                    .ObjectTypeName(typeof(string).FullName)
                    //.SerializationDataFormat(Variable.Variables.SerializationDataFormats.Java)
                    .Create())
                .PutValueTyped("object", ESS.FW.Bpm.Engine.Variable.Variables.ObjectValue(serializedValue)
                    .Create()));

            var variables = runtimeService.GetVariablesTyped(processInstance.Id, false);
            Assert.AreEqual(ESS.FW.Bpm.Engine.Variable.Variables.BooleanValue(null), variables.GetValueTyped<ITypedValue>("boolean"));
            Assert.AreEqual(ESS.FW.Bpm.Engine.Variable.Variables.StringValue(stringValue), variables.GetValueTyped<ITypedValue>("string"));
            Assert.NotNull(variables.GetValueTyped<IObjectValue>("serializedObject")
                .ValueSerialized);
            Assert.NotNull(variables.GetValueTyped<IObjectValue>("object")
                .ValueSerialized);
        }

        [Test]
        [Deployment("resources/api/form/FormsProcess.bpmn20.xml")]
        public virtual void testSubmitTaskFormDataTypedVariables()
        {
            var procDefId = repositoryService.CreateProcessDefinitionQuery()
                .First()
                .Id;

            var processInstance = formService.SubmitStartForm(procDefId, ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables());

            var task = taskService.CreateTaskQuery()
                .First();

            var stringValue = "some string";
            var serializedValue = "some value";

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            //formService.SubmitTaskForm(task.Id, Variable.Variables.CreateVariables()
            //    .PutValueTyped("boolean", Variable.Variables.BooleanValue(null))
            //    .PutValueTyped("string", Variable.Variables.StringValue(stringValue))
            //    .PutValueTyped("serializedObject", Variable.Variables.SerializedObjectValue(serializedValue)
            //        .ObjectTypeName(typeof(string).FullName)
            //        .SerializationDataFormat(Variable.Variables.SerializationDataFormats.Java)
            //        .Create())
            //    .PutValueTyped("object", Variable.Variables.ObjectValue(serializedValue)
            //        .Create()));

            var variables = runtimeService.GetVariablesTyped(processInstance.Id, false);
            Assert.AreEqual(ESS.FW.Bpm.Engine.Variable.Variables.BooleanValue(null), variables.GetValueTyped<ITypedValue>("boolean"));
            Assert.AreEqual(ESS.FW.Bpm.Engine.Variable.Variables.StringValue(stringValue), variables.GetValueTyped<ITypedValue>("string"));
            Assert.NotNull(variables.GetValueTyped<IObjectValue>("serializedObject")
                .ValueSerialized);
            Assert.NotNull(variables.GetValueTyped<IObjectValue>("object")
                .ValueSerialized);
        }

        [Test]
        [Deployment("resources/api/form/FormsProcess.bpmn20.xml")]
        public virtual void testSubmitFormVariablesNull()
        {
            var procDefId = repositoryService.CreateProcessDefinitionQuery()
                .First()
                .Id;

            // Assert that I can submit the start form with variables null
            formService.SubmitStartForm(procDefId, null);

            var task = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(task);

            // Assert that I can submit the task form with variables null
            formService.SubmitTaskForm(task.Id, null);
        }

        [Test]
        public virtual void testSubmitTaskFormForStandaloneTask()
        {
            // given
            var id = "standaloneTask";
            var task = taskService.NewTask(id);
            taskService.SaveTask(task);

            // when
            //formService.SubmitTaskForm(task.Id, Variable.Variables.CreateVariables()
            //    .PutValue("foo", "bar"));


            if (processEngineConfiguration.HistoryLevel.Id >= HistoryLevelFields.HistoryLevelAudit.Id)
            {
                var variableInstance = historyService.CreateHistoricVariableInstanceQuery()
                    //.TaskIdIn(id)
                    .First();

                Assert.NotNull(variableInstance);
                Assert.AreEqual("foo", variableInstance.Name);
                Assert.AreEqual("bar", variableInstance.Value);
            }

            taskService.DeleteTask(id, true);
        }

        [Test][Deployment("resources/api/cmmn/oneTaskCase.cmmn") ]
        public virtual void testSubmitTaskFormForCmmnHumanTask()
        {
            caseService.CreateCaseInstanceByKey("oneTaskCase");

            var task = taskService.CreateTaskQuery()
                .First();

            var stringValue = "some string";
            var serializedValue = "some value";

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            //formService.SubmitTaskForm(task.Id, Variable.Variables.CreateVariables()
            //    .PutValueTyped("boolean", Variable.Variables.BooleanValue(null))
            //    .PutValueTyped("string", Variable.Variables.StringValue(stringValue))
            //    .PutValueTyped("serializedObject", Variable.Variables.SerializedObjectValue(serializedValue)
            //        .ObjectTypeName(typeof(string).FullName)
            //        .SerializationDataFormat(Variable.Variables.SerializationDataFormats.Java)
            //        .Create())
            //    .PutValueTyped("object", Variable.Variables.ObjectValue(serializedValue)
            //        .Create()));
        }


        [Test][Deployment]
        public virtual void testSubmitStartFormWithBusinessKey()
        {
            IDictionary<string, object> properties = new Dictionary<string, object>();
            properties["duration"] = 45L;
            properties["speaker"] = "Mike";
            var procDefId = repositoryService.CreateProcessDefinitionQuery()
                .First()
                .Id;

            var processInstance = formService.SubmitStartForm(procDefId, "123", properties);
            Assert.AreEqual("123", processInstance.BusinessKey);

            Assert.AreEqual(processInstance.Id, runtimeService.CreateProcessInstanceQuery()
               // .SetProcessInstanceBusinessKey("123")
                .First()
                .Id);
            var variables = runtimeService.GetVariables(processInstance.Id);
            Assert.AreEqual("Mike", variables["SpeakerName"]);
            Assert.AreEqual(45L, variables["duration"]);
        }

        [Test]
        [Deployment]
        public virtual void testSubmitStartFormWithoutProperties()
        {
            IDictionary<string, object> properties = new Dictionary<string, object>();
            properties["duration"] = 45L;
            properties["speaker"] = "Mike";
            var procDefId = repositoryService.CreateProcessDefinitionQuery()
                .First()
                .Id;

            var processInstance = formService.SubmitStartForm(procDefId, "123", properties);
            Assert.AreEqual("123", processInstance.BusinessKey);

            Assert.AreEqual(processInstance.Id, runtimeService.CreateProcessInstanceQuery()
               // .SetProcessInstanceBusinessKey("123")
                .First()
                .Id);
            var variables = runtimeService.GetVariables(processInstance.Id);
            Assert.AreEqual("Mike", variables["speaker"]);
            Assert.AreEqual(45L, variables["duration"]);
        }

        [Test]
        public virtual void testGetStartFormKeyEmptyArgument()
        {
            try
            {
                formService.GetStartFormKey(null);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("The process definition id is mandatory, but 'null' has been provided.", ae.Message);
            }

            try
            {
                formService.GetStartFormKey("");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("The process definition id is mandatory, but '' has been provided.", ae.Message);
            }
        }

        [Test][Deployment(  "resources/api/form/FormsProcess.bpmn20.xml") ]
        public virtual void testGetStartFormKey()
        {
            var processDefinitionId = repositoryService.CreateProcessDefinitionQuery()
                .First()
                .Id;
            var expectedFormKey = formService.GetStartFormData(processDefinitionId)
                .FormKey;
            var actualFormKey = formService.GetStartFormKey(processDefinitionId);
            Assert.AreEqual(expectedFormKey, actualFormKey);
        }

        [Test]
        public virtual void testGetTaskFormKeyEmptyArguments()
        {
            try
            {
                formService.GetTaskFormKey(null, "23");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("The process definition id is mandatory, but 'null' has been provided.", ae.Message);
            }

            try
            {
                formService.GetTaskFormKey("", "23");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("The process definition id is mandatory, but '' has been provided.", ae.Message);
            }

            try
            {
                formService.GetTaskFormKey("42", null);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("The task definition key is mandatory, but 'null' has been provided.", ae.Message);
            }

            try
            {
                formService.GetTaskFormKey("42", "");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("The task definition key is mandatory, but '' has been provided.", ae.Message);
            }
        }

        [Test][Deployment( "resources/api/form/FormsProcess.bpmn20.xml") ]
        public virtual void testGetTaskFormKey()
        {
            var processDefinitionId = repositoryService.CreateProcessDefinitionQuery()
                .First()
                .Id;
            runtimeService.StartProcessInstanceById(processDefinitionId);
            var task = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(task);
            var expectedFormKey = formService.GetTaskFormData(task.Id)
                .FormKey;
            var actualFormKey = formService.GetTaskFormKey(task.ProcessDefinitionId, task.TaskDefinitionKey);
            Assert.AreEqual(expectedFormKey, actualFormKey);
        }

        [Test][Deployment ]
        public virtual void testGetTaskFormKeyWithExpression()
        {
            runtimeService.StartProcessInstanceByKey("FormsProcess",
                new Dictionary<string, ITypedValue> {{"dynamicKey", new StringValueImpl("test")}});
            var task = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(task);
            Assert.AreEqual("test", formService.GetTaskFormData(task.Id)
                .FormKey);
        }

        [Test][Deployment("resources/api/form/FormServiceTest.StartFormFields.bpmn20.xml") ]
        public virtual void testGetStartFormVariables()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            var variables = formService.GetStartFormVariables(processDefinition.Id);
            Assert.AreEqual(4, variables.Count);

            Assert.AreEqual("someString", variables["stringField"]);
            Assert.AreEqual("someString", variables.GetValueTyped<ITypedValue>("stringField")
                .Value);
            Assert.AreEqual(ValueTypeFields.String, variables.GetValueTyped<ITypedValue>("stringField")
                .Type);

            Assert.AreEqual(5l, variables["longField"]);
            Assert.AreEqual(5l, variables.GetValueTyped<ITypedValue>("longField")
                .Value);
            Assert.AreEqual(ValueTypeFields.Long, variables.GetValueTyped<ITypedValue>("longField")
                .Type);

            Assert.IsNull(variables["customField"]);
            Assert.IsNull(variables.GetValueTyped<ITypedValue>("customField")
                .Value);
            Assert.AreEqual(ValueTypeFields.String, variables.GetValueTyped<ITypedValue>("customField")
                .Type);

            Assert.NotNull(variables["dateField"]);
            Assert.AreEqual(variables["dateField"], variables.GetValueTyped<ITypedValue>("dateField")
                .Value);
            Assert.AreEqual(ValueTypeFields.String, variables.GetValueTyped<ITypedValue>("dateField")
                .Type);

            var dateFormType = processEngineConfiguration.FormTypes.GetFormType("date");
            var dateValue =
                (DateTime) dateFormType.ConvertToModelValue(variables.GetValueTyped<ITypedValue>("dateField"))
                    .Value;
            var calendar = new DateTime();
            calendar = new DateTime(dateValue.Ticks);
            Assert.AreEqual(10, calendar.Day);
            Assert.AreEqual(1, calendar.Month);
            Assert.AreEqual(2013, calendar.Year);

            // get restricted set of variables:
            variables = formService.GetStartFormVariables(processDefinition.Id, new[] {"stringField"}, true);
            Assert.AreEqual(1, variables.Count);
            Assert.AreEqual("someString", variables["stringField"]);
            Assert.AreEqual("someString", variables.GetValueTyped<ITypedValue>("stringField")
                .Value);
            Assert.AreEqual(ValueTypeFields.String, variables.GetValueTyped<ITypedValue>("stringField")
                .Type);

            // request non-existing variable
            variables = formService.GetStartFormVariables(processDefinition.Id, new[] {"non-existing!"}, true);
            Assert.AreEqual(0, variables.Count);

            // null => all
            variables = formService.GetStartFormVariables(processDefinition.Id, null, true);
            Assert.AreEqual(4, variables.Count);
        }

        [Test][Deployment("resources/api/form/FormServiceTest.StartFormFieldsUnknownType.bpmn20.xml") ]
        public virtual void testGetStartFormVariablesEnumType()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            var startFormVariables = formService.GetStartFormVariables(processDefinition.Id);
            Assert.AreEqual("a", startFormVariables["enumField"]);
            Assert.AreEqual(ValueTypeFields.String, startFormVariables.GetValueTyped<ITypedValue>("enumField")
                .Type);
        }

        [Test][Deployment("resources/api/form/FormServiceTest.TaskFormFields.bpmn20.xml") ]
        public virtual void testGetTaskFormVariables()
        {
            IDictionary<string, object> processVars = new Dictionary<string, object>();
            processVars["someString"] = "initialValue";
            processVars["initialBooleanVariable"] = true;
            processVars["initialLongVariable"] = 1l;
            processVars["serializable"] = new[] {"a", "b", "c"};

            runtimeService.StartProcessInstanceByKey("testProcess", processVars);

            var task = taskService.CreateTaskQuery()
                .First();
            var variables = formService.GetTaskFormVariables(task.Id);
            Assert.AreEqual(7, variables.Count);

            Assert.AreEqual("someString", variables["stringField"]);
            Assert.AreEqual("someString", variables.GetValueTyped<ITypedValue>("stringField")
                .Value);
            Assert.AreEqual(ValueTypeFields.String, variables.GetValueTyped<ITypedValue>("stringField")
                .Type);

            Assert.AreEqual(5l, variables["longField"]);
            Assert.AreEqual(5l, variables.GetValueTyped<ITypedValue>("longField")
                .Value);
            Assert.AreEqual(ValueTypeFields.Long, variables.GetValueTyped<ITypedValue>("longField")
                .Type);

            Assert.IsNull(variables["customField"]);
            Assert.IsNull(variables.GetValueTyped<ITypedValue>("customField")
                .Value);
            Assert.AreEqual(ValueTypeFields.String, variables.GetValueTyped<ITypedValue>("customField")
                .Type);

            Assert.AreEqual("initialValue", variables["someString"]);
            Assert.AreEqual("initialValue", variables.GetValueTyped<ITypedValue>("someString")
                .Value);
            Assert.AreEqual(ValueTypeFields.String, variables.GetValueTyped<ITypedValue>("someString")
                .Type);

            Assert.AreEqual(true, variables["initialBooleanVariable"]);
            Assert.AreEqual(true, variables.GetValueTyped<ITypedValue>("initialBooleanVariable")
                .Value);
            Assert.AreEqual(ValueTypeFields.Boolean, variables.GetValueTyped<ITypedValue>("initialBooleanVariable")
                .Type);

            Assert.AreEqual(1l, variables["initialLongVariable"]);
            Assert.AreEqual(1l, variables.GetValueTyped<ITypedValue>("initialLongVariable")
                .Value);
            Assert.AreEqual(ValueTypeFields.Long, variables.GetValueTyped<ITypedValue>("initialLongVariable")
                .Type);

            Assert.NotNull(variables["serializable"]);

            // override the long variable
            taskService.SetVariableLocal(task.Id, "initialLongVariable", 2l);

            variables = formService.GetTaskFormVariables(task.Id);
            Assert.AreEqual(7, variables.Count);

            Assert.AreEqual(2l, variables["initialLongVariable"]);
            Assert.AreEqual(2l, variables.GetValueTyped<ITypedValue>("initialLongVariable")
                .Value);
            Assert.AreEqual(ValueTypeFields.Long, variables.GetValueTyped<ITypedValue>("initialLongVariable")
                .Type);

            // get restricted set of variables (form field):
            variables = formService.GetTaskFormVariables(task.Id, new List<string> {"someString"}, true);
            Assert.AreEqual(1, variables.Count);
            Assert.AreEqual("initialValue", variables["someString"]);
            Assert.AreEqual("initialValue", variables.GetValueTyped<ITypedValue>("someString")
                .Value);
            Assert.AreEqual(ValueTypeFields.String, variables.GetValueTyped<ITypedValue>("someString")
                .Type);

            // get restricted set of variables (process variable):
            variables = formService.GetTaskFormVariables(task.Id, new List<string> {"initialBooleanVariable"}, true);
            Assert.AreEqual(1, variables.Count);
            Assert.AreEqual(true, variables["initialBooleanVariable"]);
            Assert.AreEqual(true, variables.GetValueTyped<ITypedValue>("initialBooleanVariable")
                .Value);
            Assert.AreEqual(ValueTypeFields.Boolean, variables.GetValueTyped<ITypedValue>("initialBooleanVariable")
                .Type);

            // request non-existing variable
            variables = formService.GetTaskFormVariables(task.Id, new List<string> {"non-existing!"}, true);
            Assert.AreEqual(0, variables.Count);

            // null => all
            variables = formService.GetTaskFormVariables(task.Id, null, true);
            Assert.AreEqual(7, variables.Count);
        }

        [Test]
        public virtual void testGetTaskFormVariables_StandaloneTask()
        {
            IDictionary<string, object> processVars = new Dictionary<string, object>();
            processVars["someString"] = "initialValue";
            processVars["initialBooleanVariable"] = true;
            processVars["initialLongVariable"] = 1l;
            processVars["serializable"] = new List<string> {"a", "b", "c"};

            // create new standalone task
            var standaloneTask = taskService.NewTask();
            standaloneTask.Name = "A Standalone Task";
            taskService.SaveTask(standaloneTask);

            var task = taskService.CreateTaskQuery()
                .First();

            // set variables
            taskService.SetVariables(task.Id, processVars);

            var variables = formService.GetTaskFormVariables(task.Id);
            Assert.AreEqual(4, variables.Count);

            Assert.AreEqual("initialValue", variables["someString"]);
            Assert.AreEqual("initialValue", variables.GetValueTyped<ITypedValue>("someString")
                .Value);
            Assert.AreEqual(ValueTypeFields.String, variables.GetValueTyped<ITypedValue>("someString")
                .Type);

            Assert.AreEqual(true, variables["initialBooleanVariable"]);
            Assert.AreEqual(true, variables.GetValueTyped<ITypedValue>("initialBooleanVariable")
                .Value);
            Assert.AreEqual(ValueTypeFields.Boolean, variables.GetValueTyped<ITypedValue>("initialBooleanVariable")
                .Type);

            Assert.AreEqual(1l, variables["initialLongVariable"]);
            Assert.AreEqual(1l, variables.GetValueTyped<ITypedValue>("initialLongVariable")
                .Value);
            Assert.AreEqual(ValueTypeFields.Long, variables.GetValueTyped<ITypedValue>("initialLongVariable")
                .Type);

            Assert.NotNull(variables["serializable"]);

            // override the long variable
            taskService.SetVariable(task.Id, "initialLongVariable", 2l);

            variables = formService.GetTaskFormVariables(task.Id);
            Assert.AreEqual(4, variables.Count);

            Assert.AreEqual(2l, variables["initialLongVariable"]);
            Assert.AreEqual(2l, variables.GetValueTyped<ITypedValue>("initialLongVariable")
                .Value);
            Assert.AreEqual(ValueTypeFields.Long, variables.GetValueTyped<ITypedValue>("initialLongVariable")
                .Type);

            // get restricted set of variables
            variables = formService.GetTaskFormVariables(task.Id, new[] {"someString"}, true);
            Assert.AreEqual(1, variables.Count);
            Assert.AreEqual("initialValue", variables["someString"]);
            Assert.AreEqual("initialValue", variables.GetValueTyped<ITypedValue>("someString")
                .Value);
            Assert.AreEqual(ValueTypeFields.String, variables.GetValueTyped<ITypedValue>("someString")
                .Type);

            // request non-existing variable
            variables = formService.GetTaskFormVariables(task.Id, new[] {"non-existing!"}, true);
            Assert.AreEqual(0, variables.Count);

            // null => all
            variables = formService.GetTaskFormVariables(task.Id, null, true);
            Assert.AreEqual(4, variables.Count);

            // Finally, Delete task
            taskService.DeleteTask(task.Id, true);
        }

        [Test][Deployment(  "resources/api/oneTaskProcess.bpmn20.xml" ) ]
        public virtual void testSubmitStartFormWithObjectVariables()
        {
            // given
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            // when a start form is submitted with an object variable
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["var"] = new List<string>();
            var processInstance = formService.SubmitStartForm(processDefinition.Id, variables);

            // then the variable is available as a process variable
            var var = (List<string>) runtimeService.GetVariable(processInstance.Id, "var");
            Assert.NotNull(var);
            Assert.True(var.Count == 0);

            // then no historic form property event has been written since this is not supported for custom objects
            if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HistorylevelFull)
                Assert.AreEqual(0, historyService.CreateHistoricDetailQuery()
                   // .FormFields()
                    .Count());
        }

        [Test][Deployment(  "resources/api/twoTasksProcess.bpmn20.xml" ) ]
        public virtual void testSubmitTaskFormWithObjectVariables()
        {
            // given
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            var processInstance = runtimeService.StartProcessInstanceByKey("twoTasksProcess");

            // when a task form is submitted with an object variable
            var task = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(task);

            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["var"] = new List<string>();
            //formService.SubmitTaskForm(task.Id, variables);

            // then the variable is available as a process variable
            var var = (List<string>) runtimeService.GetVariable(processInstance.Id, "var");
            Assert.NotNull(var);
            Assert.True(var.Count == 0);

            // then no historic form property event has been written since this is not supported for custom objects
            if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HistorylevelFull)
                Assert.AreEqual(0, historyService.CreateHistoricDetailQuery()
                   // .FormFields()
                    .Count());
        }

        [Test][Deployment ]
        public virtual void testSubmitTaskFormContainingReadonlyVariable()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);

            var task = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(task);

            formService.SubmitTaskForm(task.Id, new Dictionary<string, ITypedValue>());

            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment]
        public virtual void testGetTaskFormWithoutLabels()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            var task = taskService.CreateTaskQuery()
                .First();

            // form data can be retrieved
            var formData = formService.GetTaskFormData(task.Id);

            var formFields = formData.FormFields;
            Assert.AreEqual(3, formFields.Count);

            IList<string> formFieldIds = new List<string>();
            foreach (var field in formFields)
            {
                Assert.IsNull(field.Label);
                formFieldIds.Add(field.Id);
            }

//JAVA TO C# CONVERTER TODO Resources.Task: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
            //Assert.True(formFieldIds.Contains("stringField", "customField", "longField"));

            // the form can be rendered
            var startForm = formService.GetRenderedTaskForm(task.Id);
            Assert.NotNull(startForm);
        }

        [Test]
        public virtual void testDeployTaskFormWithoutFieldTypes()
        {
            try
            {
                repositoryService.CreateDeployment()
                    .AddClasspathResource(
                        "resources/api/form/FormServiceTest.TestDeployTaskFormWithoutFieldTypes.bpmn20.xml")
                    .Deploy();
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("form field must have a 'type' attribute", e.Message);
            }
        }

        [Test]
        [Deployment]
        public virtual void testGetStartFormWithoutLabels()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            runtimeService.StartProcessInstanceById(processDefinition.Id);

            // form data can be retrieved
            var formData = formService.GetStartFormData(processDefinition.Id);

            var formFields = formData.FormFields;
            Assert.AreEqual(3, formFields.Count);

            IList<string> formFieldIds = new List<string>();
            foreach (var field in formFields)
            {
                Assert.IsNull(field.Label);
                formFieldIds.Add(field.Id);
            }

//JAVA TO C# CONVERTER TODO Resources.Task: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
            //Assert.True(formFieldIds.containsAll(("stringField", "customField", "longField")));

            // the form can be rendered
            var startForm = formService.GetRenderedStartForm(processDefinition.Id);
            Assert.NotNull(startForm);
        }

        [Test]
        public virtual void testDeployStartFormWithoutFieldTypes()
        {
            try
            {
                repositoryService.CreateDeployment()
                    .AddClasspathResource(
                        "resources/api/form/FormServiceTest.TestDeployStartFormWithoutFieldTypes.bpmn20.xml")
                    .Deploy();
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("form field must have a 'type' attribute", e.Message);
            }
        }

        [Test][Deployment( new []{ "resources/api/form/util/VacationRequest_deprecated_forms.bpmn20.xml", "resources/api/form/util/approve.Form", "resources/api/form/util/request.Form", "resources/api/form/util/adjustRequest.Form" }) ]
        public virtual void testTaskFormsWithVacationRequestProcess()
        {
            // Get start form
            var procDefId = repositoryService.CreateProcessDefinitionQuery()
                .First()
                .Id;
            var startForm = formService.GetRenderedStartForm(procDefId, "juel");
            Assert.NotNull(startForm);

            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();
            var processDefinitionId = processDefinition.Id;
            Assert.AreEqual("resources/api/form/util/request.Form", formService.GetStartFormData(processDefinitionId)
                .FormKey);

            // Define variables that would be filled in through the form
            IDictionary<string, string> formProperties = new Dictionary<string, string>();
            formProperties["employeeName"] = "kermit";
            formProperties["numberOfDays"] = "4";
            formProperties["vacationMotivation"] = "I'm tired";
            formService.SubmitStartFormData(procDefId, formProperties);

            // Management should now have a task assigned to them
            var task = taskService.CreateTaskQuery()
                //.TaskCandidateGroup("management")
                .First();
            Assert.AreEqual("Vacation request by kermit", task.Description);
            var taskForm = formService.GetRenderedTaskForm(task.Id, "juel");
            Assert.NotNull(taskForm);

            // Rejecting the task should put the process back to first task
            taskService.Complete(task.Id, new Dictionary<string, object> {{"vacationApproved", "false"}});
            task = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("Adjust vacation request", task.Name);
        }

        [Test]
        [Deployment]
        public virtual void testTaskFormUnavailable()
        {
            var procDefId = repositoryService.CreateProcessDefinitionQuery()
                .First()
                .Id;
            Assert.IsNull(formService.GetRenderedStartForm(procDefId));

            runtimeService.StartProcessInstanceByKey("noStartOrTaskForm");
            var task = taskService.CreateTaskQuery()
                .First();
            Assert.IsNull(formService.GetRenderedTaskForm(task.Id));
        }

        [Test]
        [Deployment]
        public virtual void testBusinessKey()
        {
            // given
            var procDefId = repositoryService.CreateProcessDefinitionQuery()
                .First()
                .Id;

            // when
            var startFormData = formService.GetStartFormData(procDefId);

            // then
            var formField = startFormData.FormFields[0];
            Assert.True(formField.BusinessKey);
        }

        [Test]
        [Deployment]
        public virtual void testSubmitStartFormWithFormFieldMarkedAsBusinessKey()
        {
            var procDefId = repositoryService.CreateProcessDefinitionQuery()
                .First()
                .Id;
            var pi = formService.SubmitStartForm(procDefId, "foo", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("secondParam", "bar"));

            Assert.AreEqual("foo", pi.BusinessKey);

            var result = runtimeService.CreateVariableInstanceQuery()
                
                .ToList();
            Assert.AreEqual(1, result.Count);
            Assert.True(result[0].Name.Equals("secondParam"));
        }
    }
}