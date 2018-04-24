using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.Event.Compensate.Helper
{

    

    /// <summary>
    /// 
    /// </summary>
    public class SetVariablesDelegate : IJavaDelegate
    {

        private IExpression variable;
        public static IList<string> values = new List<string>();

        public void Execute(IBaseDelegateExecution execution)
        {
            string VariableName = (string)variable.GetValue(execution);
            var tmp = values.GetEnumerator();
            tmp.MoveNext();
            string value = tmp.Current;

            execution.SetVariableLocal(VariableName, value);

            values.Remove(value);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
        //public virtual void execute(IDelegateExecution execution)
        //{

        //    string VariableName = (string)variable.GetValue(execution);
        //    string value = values.GetEnumerator().Next();

        //    execution.SetVariableLocal(VariableName, value);

        //    values.Remove(value);
        //}

        public static IList<string> Values
        {
            set
            {
                SetVariablesDelegate.values.Clear();
                ((List<string>)SetVariablesDelegate.values).AddRange(value);
            }
        }

    }

}