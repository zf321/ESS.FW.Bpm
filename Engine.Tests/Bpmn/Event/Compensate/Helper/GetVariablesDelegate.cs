using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.Event.Compensate.Helper
{
    public class GetVariablesDelegate : IJavaDelegate
    {

        private IExpression variable;
        public static IList<string> values = new List<string>();

        public void Execute(IBaseDelegateExecution execution)
        {
            string VariableName = (string)variable.GetValue(execution);
            string value = (string)execution.GetVariable(VariableName);

            values.Add(value);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
        //public virtual void execute(IDelegateExecution execution)
        //{

        //    string VariableName = (string)variable.GetValue(execution);
        //    string value = (string)execution.GetVariable(VariableName);

        //    values.Add(value);
        //}

    }

}