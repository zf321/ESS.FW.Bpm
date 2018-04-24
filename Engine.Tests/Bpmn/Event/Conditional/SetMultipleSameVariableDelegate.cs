using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.Event.Conditional
{

    public class SetMultipleSameVariableDelegate : IJavaDelegate
    {

        private static readonly string VARIABLE_NAME = AbstractConditionalEventTestCase.VariableName + "2";

        public void Execute(IBaseDelegateExecution execution)
        {
            (execution as IDelegateExecution).ProcessInstance.SetVariable(VARIABLE_NAME, 1);
            (execution as IDelegateExecution).ProcessInstance.SetVariable(VARIABLE_NAME, 1);
            (execution as IDelegateExecution).ProcessInstance.SetVariable(VARIABLE_NAME, 1);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
        //public virtual void execute(IDelegateExecution execution)
        //{
        //    execution.IProcessInstance.SetVariable(VARIABLE_NAME, 1);
        //    execution.IProcessInstance.SetVariable(VARIABLE_NAME, 1);
        //    execution.IProcessInstance.SetVariable(VARIABLE_NAME, 1);
        //}
    }

}