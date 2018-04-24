using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Form.Impl.Type;
using ESS.FW.Bpm.Engine.Form.Impl.Validator;
using NUnit.Framework;

namespace Engine.Tests.Api.Form
{
    /// <summary>
    ///     <para>
    ///         Testcase verifying support for form matadata provided using
    ///         custom extension elements in BPMN Xml
    ///     </para>
    /// </summary>
    [TestFixture]
    public class FormDataTest : PluggableProcessEngineTestCase
    {
        [Test][Deployment ]
        public virtual void testGetFormFieldBasicProperties()
        {
            runtimeService.StartProcessInstanceByKey("FormDataTest.TestGetFormFieldBasicProperties");

            var task = taskService.CreateTaskQuery()
                .First();
            var taskFormData = formService.GetTaskFormData(task.Id);

            // validate properties:
            var formFields = taskFormData.FormFields;

            // validate field 1
            var formField1 = formFields[0];
            Assert.NotNull(formField1);
            Assert.AreEqual(formField1.Id, "formField1");
            Assert.AreEqual(formField1.Label, "Form Field 1");
            Assert.AreEqual("string", formField1.TypeName);
            Assert.NotNull(formField1.Type);

            // validate field 2
            var formField2 = formFields[1];
            Assert.NotNull(formField2);
            Assert.AreEqual(formField2.Id, "formField2");
            Assert.AreEqual(formField2.Label, "Form Field 2");
            Assert.AreEqual("boolean", formField2.TypeName);
            Assert.NotNull(formField1.Type);
        }

        [Test]
        [Deployment]
        public virtual void testGetFormFieldBuiltInTypes()
        {
            runtimeService.StartProcessInstanceByKey("FormDataTest.TestGetFormFieldBuiltInTypes");

            var task = taskService.CreateTaskQuery()
                .First();

            var taskFormData = formService.GetTaskFormData(task.Id);

            // validate properties:
            var formFields = taskFormData.FormFields;

            // validate string field
            var stringField = formFields[0];
            Assert.NotNull(stringField);
            Assert.AreEqual("string", stringField.TypeName);
            Assert.NotNull(stringField.Type);
            Assert.AreEqual("someString", stringField.DefaultValue);

            // validate long field
            var longField = formFields[1];
            Assert.NotNull(longField);
            Assert.AreEqual("long", longField.TypeName);
            Assert.NotNull(longField.Type);
            Assert.AreEqual(Convert.ToInt64(1l), longField.DefaultValue);

            // validate boolean field
            var booleanField = formFields[2];
            Assert.NotNull(booleanField);
            Assert.AreEqual("boolean", booleanField.TypeName);
            Assert.NotNull(booleanField.Type);
            Assert.AreEqual(Convert.ToBoolean(true), booleanField.DefaultValue);

            // validate date field
            var dateField = formFields[3];
            Assert.NotNull(dateField);
            Assert.AreEqual("date", dateField.TypeName);
            Assert.NotNull(dateField.Type);
            var dateValue = (DateTime) dateField.DefaultValue;
            var calendar = new DateTime();
            calendar = new DateTime(dateValue.Ticks);
            Assert.AreEqual(10, calendar.Day);
            Assert.AreEqual(1, calendar.Month);
            Assert.AreEqual(2013, calendar.Year);

            // validate enum field
            var enumField = formFields[4];
            Assert.NotNull(enumField);
            Assert.AreEqual("enum", enumField.TypeName);
            Assert.NotNull(enumField.Type);
            var enumFormType = (EnumFormType) enumField.Type;
            var values = enumFormType.Values;
            Assert.AreEqual("A", values["a"]);
            Assert.AreEqual("B", values["b"]);
            Assert.AreEqual("C", values["c"]);
        }

        [Test]
        [Deployment]
        public virtual void testGetFormFieldProperties()
        {
            runtimeService.StartProcessInstanceByKey("FormDataTest.TestGetFormFieldProperties");

            var task = taskService.CreateTaskQuery()
                .First();

            var taskFormData = formService.GetTaskFormData(task.Id);

            var formFields = taskFormData.FormFields;

            var stringField = formFields[0];
            var properties = stringField.Properties;
            Assert.AreEqual("property1", properties["p1"]);
            Assert.AreEqual("property2", properties["p2"]);
        }

        [Test]
        [Deployment]
        public virtual void testGetFormFieldValidationConstraints()
        {
            runtimeService.StartProcessInstanceByKey("FormDataTest.TestGetFormFieldValidationConstraints");

            var task = taskService.CreateTaskQuery()
                .First();

            var taskFormData = formService.GetTaskFormData(task.Id);

            var formFields = taskFormData.FormFields;

            var field1 = formFields[0];
            var validationConstraints = field1.ValidationConstraints;
            var constraint1 = validationConstraints[0];
            Assert.AreEqual("maxlength", constraint1.Name);
            Assert.AreEqual("10", constraint1.Configuration);
            var constraint2 = validationConstraints[1];
            Assert.AreEqual("minlength", constraint2.Name);
            Assert.AreEqual("5", constraint2.Configuration);
        }

        [Test]
        [Deployment]
        public virtual void testFormFieldSubmit()
        {
            // valid submit
            var processInstance = runtimeService.StartProcessInstanceByKey("FormDataTest.TestFormFieldSubmit");
            var task = taskService.CreateTaskQuery()
                .First();
            IDictionary<string, object> formValues = new Dictionary<string, object>();
            formValues["stringField"] = "12345";
            formValues["longField"] = 9L;
            formValues["customField"] = "validValue";
            //formService.SubmitTaskForm(task.Id, formValues);

            Assert.AreEqual(formValues, runtimeService.GetVariables(processInstance.Id));
            runtimeService.DeleteProcessInstance(processInstance.Id, "test complete");

            runtimeService.StartProcessInstanceByKey("FormDataTest.TestFormFieldSubmit");
            task = taskService.CreateTaskQuery()
                .First();
            // invalid submit 1

            formValues = new Dictionary<string, object>();
            formValues["stringField"] = "1234";
            formValues["longField"] = 9L;
            formValues["customField"] = "validValue";
            try
            {
                //formService.SubmitTaskForm(task.Id, formValues);
                Assert.Fail();
            }
            catch (FormFieldValidatorException e)
            {
                Assert.AreEqual(e.Name, "minlength");
            }

            // invalid submit 2
            formValues = new Dictionary<string, object>();

            formValues["customFieldWithValidationDetails"] = "C";
            try
            {
                //formService.SubmitTaskForm(task.Id, formValues);
                Assert.Fail();
            }
            catch (FormFieldValidatorException e)
            {
                Assert.AreEqual(e.Name, "validator");
                Assert.AreEqual(e.Id, "customFieldWithValidationDetails");

                Assert.True(e.InnerException is FormFieldValidationException);

                var exception = (FormFieldValidationException) e.InnerException;
                //Assert.AreEqual(exception.Detail, "EXPIRED");
            }
        }

        [Test]
        [Deployment]
        public virtual void testMissingFormVariables()
        {
            // given process definition with defined form varaibles
            // when start process instance with no variables
            var processInstance = runtimeService.StartProcessInstanceByKey("date-form-property-test");
            var task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();

            // then taskFormData contains form variables with null as values
            var taskFormData = formService.GetTaskFormData(task.Id);
            Assert.NotNull(taskFormData);
            Assert.AreEqual(5, taskFormData.FormFields.Count);
            foreach (var field in taskFormData.FormFields)
            {
                Assert.NotNull(field);
                Assert.IsNull(field.Value.Value);
            }
        }

        [Test][Deployment( "resources/api/form/FormDataTest.TestDoubleQuotesAreEscapedInGeneratedTaskForms.bpmn20.xml") ]
        public virtual void testDoubleQuotesAreEscapedInGeneratedTaskForms()
        {
            // given
            var variables = new Dictionary<string, object>();
            variables["foo"] = "This is a \"Test\" message!";
            var pi = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);
            var taskWithForm = taskService.CreateTaskQuery()
                .First();

            // when
            var renderedStartForm = formService.GetRenderedTaskForm(taskWithForm.Id);
            Assert.True(renderedStartForm is string);

            // then
            var renderedForm = (string) renderedStartForm;
            var expectedFormValueWithEscapedQuotes = "This is a &quot;Test&quot; message!";
            Assert.True(renderedForm.Contains(expectedFormValueWithEscapedQuotes));
        }
    }
}