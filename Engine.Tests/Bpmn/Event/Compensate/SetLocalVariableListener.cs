using System;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.Event.Compensate
{
    [Serializable]
    public class SetLocalVariableListener : IDelegateListener<IBaseDelegateExecution>
    {

        private const long serialVersionUID = 1L;

        protected internal string VariableName;
        protected internal string VariableValue;

        public SetLocalVariableListener(string VariableName, string variableValue)
        {
            this.VariableName = VariableName;
            this.VariableValue = variableValue;
        }

        public void Notify(IBaseDelegateExecution execution)
        {
            execution.SetVariableLocal(VariableName, VariableValue);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public void Notify(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
        //public override void Notify(IDelegateExecution execution)
        //{
        //    execution.SetVariableLocal(VariableName, variableValue);
        //}

    }

}