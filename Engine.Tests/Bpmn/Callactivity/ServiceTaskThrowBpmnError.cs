using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.Callactivity
{
    public class ServiceTaskThrowBpmnError : IJavaDelegate
    {
        public void Execute(IBaseDelegateExecution execution)
        {
            throw new BpmnError("errorCode", "ouch!");
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public void execute(delegate.IDelegateExecution execution) throws Exception
        //public void Execute(IBaseDelegateExecution execution)
        //{
        //    throw new BpmnError("errorCode", "ouch!");
        //}

    }

}