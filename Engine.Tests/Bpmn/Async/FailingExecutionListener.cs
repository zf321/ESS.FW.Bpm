using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.Async
{    
    public class FailingExecutionListener : IDelegateListener<IBaseDelegateExecution>
    {
        public void Notify(IBaseDelegateExecution instance)
        {
            throw new System.Exception("I'm supposed to Assert.Fail!");
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void Notify(delegate.IDelegateExecution execution) throws Exception
        //public virtual void Notify(IDelegateExecution execution)
        //{
        //    throw new Exception("I'm supposed to Assert.Fail!");
        //}

    }

}