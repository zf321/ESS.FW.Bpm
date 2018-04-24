using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.Event.Error
{

    public class ThrowSpecializedExceptionDelegate : IJavaDelegate
    {
        public void Execute(IBaseDelegateExecution execution)
        {
            throw new MySpecializedException("Business Exception");
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void execute(org.Camunda.bpm.engine.delegate.IDelegateExecution execution) throws Exception
        //public virtual void execute(IDelegateExecution execution)
        //{

        //    throw new MySpecializedException("Business Exception");

        //}

    }

}