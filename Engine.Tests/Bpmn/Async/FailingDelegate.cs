using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.Async
{
    public class FailingDelegate : IJavaDelegate
    {
        public void Execute(IBaseDelegateExecution execution)
        {
            throw new System.Exception("I'm supposed to Assert.Fail!");
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void execute(delegate.IDelegateExecution execution) throws Exception
        //public virtual void execute(IDelegateExecution execution)
        //{
        //    throw new Exception("I'm supposed to Assert.Fail!");
        //}

    }

}