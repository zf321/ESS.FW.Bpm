using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.Event.Conditional
{
    public class SetVariableDelegate : IJavaDelegate
    {
        public void Execute(IBaseDelegateExecution execution)
        {
            execution.SetVariable(AbstractConditionalEventTestCase.VariableName, 1);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public void execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
        //public void Execute(IBaseDelegateExecution execution)
        //{
        //    execution.SetVariable(AbstractConditionalEventTestCase.VARIABLE_NAME, 1);
        //}
    }

}