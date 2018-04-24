using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.Event.Conditional
{
    public class LoopDelegate : IJavaDelegate
    {

        private static int instanceCount = 0;

        public LoopDelegate()
        {
            instanceCount++;
        }

        public void Execute(IBaseDelegateExecution execution)
        {
            if (instanceCount < 3)
            {
                execution.SetVariable(AbstractConditionalEventTestCase.VariableName, 1);
            }
            else
            {
                execution.SetVariable(AbstractConditionalEventTestCase.VariableName, instanceCount);
            }
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public void execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
        //public void Execute(IBaseDelegateExecution execution)
        //{
        //    if (instanceCount < 3)
        //    {
        //        execution.SetVariable(AbstractConditionalEventTestCase.VARIABLE_NAME, 1);
        //    }
        //    else
        //    {
        //        execution.SetVariable(AbstractConditionalEventTestCase.VARIABLE_NAME, instanceCount);
        //    }
        //}
    }

}