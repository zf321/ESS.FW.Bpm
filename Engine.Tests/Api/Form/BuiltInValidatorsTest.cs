using System.Collections.Generic;
using Engine.Tests.Api.Runtime.Util;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Form.Impl;
using ESS.FW.Bpm.Engine.Form.Impl.Handler;
using ESS.FW.Bpm.Engine.Form.Impl.Validator;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using NUnit.Framework;

namespace Engine.Tests.Api.Form
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class BuiltInValidatorsTest : PluggableProcessEngineTestCase
    {
        [Test]
        public virtual void testDefaultFormFieldValidators()
        {
            // Assert default validators are registered
            var formValidators =
                ((ProcessEngineConfigurationImpl) ((ProcessEngineImpl) ProcessEngine).ProcessEngineConfiguration)
                .FormValidators;

            var validators = formValidators.Validators;
            Assert.AreEqual(typeof(RequiredValidator), validators["required"]);
            Assert.AreEqual(typeof(ReadOnlyValidator), validators["readonly"]);
            Assert.AreEqual(typeof(MinValidator), validators["min"]);
            Assert.AreEqual(typeof(MaxValidator), validators["max"]);
            Assert.AreEqual(typeof(MaxLengthValidator), validators["maxlength"]);
            Assert.AreEqual(typeof(MinLengthValidator), validators["minlength"]);
        }

        [Test]
        public virtual void testRequiredValidator()
        {
            var validator = new RequiredValidator();
            var validatorContext = new TestValidatorContext(null);

            Assert.True(validator.Validate("test", validatorContext));
            Assert.True(validator.Validate(1, validatorContext));
            Assert.True(validator.Validate(true, validatorContext));

            // empty string and 'null' are invalid
            Assert.IsFalse(validator.Validate("", validatorContext));
            Assert.IsFalse(validator.Validate(null, validatorContext));

            // can submit null if the value already exists
            validatorContext = new TestValidatorContext(null, "fieldName");
            validatorContext.VariableScope.SetVariable("fieldName", "existingValue");
            Assert.True(validator.Validate(null, validatorContext));
        }

        [Test]
        public virtual void testReadOnlyValidator()
        {
            var validator = new ReadOnlyValidator();

            Assert.IsFalse(validator.Validate("", null));
            Assert.IsFalse(validator.Validate("aaa", null));
            Assert.IsFalse(validator.Validate(11, null));
            Assert.IsFalse(validator.Validate(2d, null));
            Assert.True(validator.Validate(null, null));
        }

        [Test]
        public virtual void testMinValidator()
        {
            var validator = new MinValidator();

            Assert.True(validator.Validate(null, null));

            Assert.True(validator.Validate(4, new TestValidatorContext("4")));
            Assert.IsFalse(validator.Validate(4, new TestValidatorContext("5")));

            try
            {
                validator.Validate(4, new TestValidatorContext("4.4"));
                Assert.Fail("exception expected");
            }
            catch (FormException e)
            {
                Assert.True(
                    e.Message.Contains("Cannot validate Integer value 4: configuration 4.4 cannot be parsed as Integer."));
            }

            Assert.IsFalse(validator.Validate(4d, new TestValidatorContext("4.1")));
            Assert.True(validator.Validate(4.1d, new TestValidatorContext("4.1")));

            Assert.IsFalse(validator.Validate(4f, new TestValidatorContext("4.1")));
            Assert.True(validator.Validate(4.1f, new TestValidatorContext("4.1")));
        }

        [Test]
        public virtual void testMaxValidator()
        {
            var validator = new MaxValidator();

            Assert.True(validator.Validate(null, null));

            Assert.True(validator.Validate(3, new TestValidatorContext("4")));
            Assert.IsFalse(validator.Validate(4, new TestValidatorContext("3")));

            try
            {
                validator.Validate(4, new TestValidatorContext("4.4"));
                Assert.Fail("exception expected");
            }
            catch (FormException e)
            {
                Assert.True(
                    e.Message.Contains("Cannot validate Integer value 4: configuration 4.4 cannot be parsed as Integer."));
            }

            Assert.IsFalse(validator.Validate(4.1d, new TestValidatorContext("4")));
            Assert.True(validator.Validate(4.1d, new TestValidatorContext("4.2")));

            Assert.IsFalse(validator.Validate(4.1f, new TestValidatorContext("4")));
            Assert.True(validator.Validate(4.1f, new TestValidatorContext("4.2")));
        }

        [Test]
        public virtual void testMaxLengthValidator()
        {
            var validator = new MaxLengthValidator();

            Assert.True(validator.Validate(null, null));

            Assert.True(validator.Validate("test", new TestValidatorContext("5")));
            Assert.IsFalse(validator.Validate("test", new TestValidatorContext("4")));

            try
            {
                validator.Validate("test", new TestValidatorContext("4.4"));
                Assert.Fail("exception expected");
            }
            catch (FormException e)
            {
                Assert.True(
                    e.Message.Contains(
                        "Cannot validate \"maxlength\": configuration 4.4 cannot be interpreted as Integer"));
            }
        }

        [Test]
        public virtual void testMinLengthValidator()
        {
            var validator = new MinLengthValidator();

            Assert.True(validator.Validate(null, null));

            Assert.True(validator.Validate("test", new TestValidatorContext("4")));
            Assert.IsFalse(validator.Validate("test", new TestValidatorContext("5")));

            try
            {
                validator.Validate("test", new TestValidatorContext("4.4"));
                Assert.Fail("exception expected");
            }
            catch (FormException e)
            {
                Assert.True(
                    e.Message.Contains(
                        "Cannot validate \"minlength\": configuration 4.4 cannot be interpreted as Integer"));
            }
        }

        protected internal class TestValidatorContext : IFormFieldValidatorContext
        {
            internal string configuration;
            internal FormFieldHandler formFieldHandler = new FormFieldHandler();

            internal TestVariableScope variableScope = new TestVariableScope();

            public TestValidatorContext(string configuration)
            {
                this.configuration = configuration;
            }

            public TestValidatorContext(string configuration, string formFieldId)
            {
                this.configuration = configuration;
                FormFieldHandler.Id = formFieldId;
            }

            public virtual FormFieldHandler FormFieldHandler
            {
                get { return formFieldHandler; }
            }

            public virtual IDelegateExecution Execution
            {
                get { return null; }
            }

            public virtual string Configuration
            {
                get { return configuration; }
            }

            public virtual IDictionary<string, object> SubmittedValues
            {
                get { return null; }
            }

            public virtual IVariableScope VariableScope
            {
                get { return variableScope; }
            }
        }
    }
}