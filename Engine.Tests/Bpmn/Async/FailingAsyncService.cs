using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.Async
{
    public class FailingAsyncService : IJavaDelegate
    {
        public void Execute(IBaseDelegateExecution execution)
        {
            throw new System.Exception();
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void execute(delegate.IDelegateExecution execution) throws Exception
        //public virtual void execute(IDelegateExecution execution)
        //{
        //    throw new Exception();
        //}

    }

}