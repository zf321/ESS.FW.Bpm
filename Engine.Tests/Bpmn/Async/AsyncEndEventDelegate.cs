using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.Async
{
    public class AsyncEndEventDelegate : IJavaDelegate
    {
        public void Execute(IBaseDelegateExecution execution)
        {
            execution.SetVariable("message", true);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void execute(delegate.IDelegateExecution execution) throws Exception
        //public void execute(IDelegateExecution execution)
        //{
        //    execution.SetVariable("message", true);
        //}

    }

}