using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.Event.Compensate.Helper
{
    public class UndoService : IJavaDelegate
    {

        private IExpression counterName;

        public void Execute(IBaseDelegateExecution execution)
        {
            string VariableName = (string)counterName.GetValue(execution);
            object variable = execution.GetVariable(VariableName);
            if (variable == null)
            {
                execution.SetVariable(VariableName, (int?)1);
            }
            else
            {
                execution.SetVariable(VariableName, ((int?)variable) + 1);
            }
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
        //public virtual void execute(IDelegateExecution execution)
        //{
        //    string VariableName = (string)counterName.GetValue(execution);
        //    object variable = execution.GetVariable(VariableName);
        //    if (variable == null)
        //    {
        //        execution.SetVariable(VariableName, (int?)1);
        //    }
        //    else
        //    {
        //        execution.SetVariable(VariableName, ((int?)variable) + 1);
        //    }
        //}

    }
    public class CustomUndoService
    {
        string name = "none";
        public void Todo()
        {
            name = "setted";
        }
    }
}