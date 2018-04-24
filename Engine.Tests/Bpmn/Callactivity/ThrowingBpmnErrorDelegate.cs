using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.Callactivity
{
    public class ThrowingBpmnErrorDelegate : IJavaDelegate
    {

        public const string ERROR_CODE = "BPMN_ERROR_BY_DELEGATE";

        public void Execute(IBaseDelegateExecution execution)
        {
            throw new BpmnError(ERROR_CODE);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public void execute(delegate.IDelegateExecution execution) throws Exception
        //public void Execute(IBaseDelegateExecution execution)
        //{
        //    throw new BpmnError(ERROR_CODE);
        //}

    }

}