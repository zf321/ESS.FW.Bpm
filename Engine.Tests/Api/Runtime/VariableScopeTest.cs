using System.Collections.Generic;
using Engine.Tests.Api.Runtime.Util;
using ESS.FW.Bpm.Engine.Delegate;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class VariableScopeTest
    {
        [SetUp]
        public virtual void setUp()
        {
            variableScope = new TestVariableScope();
            variableScope.SetVariable(VAR_NAME, VAR_VALUE_STRING);
        }

        private const string VAR_NAME = "foo";

        private const string VAR_VALUE_STRING = "bar";

        private IVariableScope variableScope;

        [Test]
        public virtual void testGetVariables()
        {
            var variables = variableScope.Variables;
            Assert.NotNull(variables);
            Assert.AreEqual(VAR_VALUE_STRING, variables[VAR_NAME]);
        }

        [Test]
        public virtual void testGetVariablesTyped()
        {
            var variables = variableScope.VariablesTyped;
            Assert.NotNull(variables);
            Assert.AreEqual(VAR_VALUE_STRING, variables[VAR_NAME]);
            Assert.AreEqual(variables, variableScope.GetVariablesTyped(true));
        }

        [Test]
        public virtual void testGetVariablesLocal()
        {
            var variables = variableScope.VariablesLocal;
            Assert.NotNull(variables);
            Assert.AreEqual(VAR_VALUE_STRING, variables[VAR_NAME]);
        }

        [Test]
        public virtual void testGetVariablesLocalTyped()
        {
            IDictionary<string, object> variables = variableScope.VariablesLocalTyped;
            Assert.NotNull(variables);
            Assert.AreEqual(VAR_VALUE_STRING, variables[VAR_NAME]);
            Assert.AreEqual(variables, variableScope.GetVariablesLocalTyped(true));
        }
    }
}