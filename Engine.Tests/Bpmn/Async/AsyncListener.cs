using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.Async
{    
    public class AsyncListener : IDelegateListener<IBaseDelegateExecution>
    {
        public void Notify(IBaseDelegateExecution execution)
        {
            execution.SetVariable("listener", "listener invoked");
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void Notify(delegate.IDelegateExecution execution) throws Exception
        //public virtual void Notify(IDelegateExecution execution)
        //{
        //    execution.SetVariable("listener", "listener invoked");

        //}

    }

}