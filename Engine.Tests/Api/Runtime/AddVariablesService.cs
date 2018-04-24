using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Api.Runtime
{
    public class AddVariablesService : IJavaDelegate
    {
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
        public void Execute(IBaseDelegateExecution execution)
        {
            var now = DateTime.Now;
            IList<string> serializable = new List<string>();
            serializable.Add("one");
            serializable.Add("two");
            serializable.Add("three");

            // Start process instance with different types of variables
            IDictionary<string, object> variables = new Dictionary<string, object>();

            variables["shortVar"] = (short) 123;
            variables["integerVar"] = 1234;
            variables["longVar"] = 928374L;

            variables["byteVar"] = new byte[] {12, 32, 34};

            variables["stringVar"] = "coca-cola";
            variables["dateVar"] = now;
            variables["nullVar"] = null;
            variables["serializableVar"] = serializable;

            execution.VariablesLocal = variables;
        }
    }
}