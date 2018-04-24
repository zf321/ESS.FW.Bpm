using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.Event.Timer
{
    public class DummyServiceTask : IJavaDelegate
    {

        public static bool wasExecuted = false;

        public void Execute(IBaseDelegateExecution execution)
        {
            wasExecuted = true;
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void execute(org.Camunda.bpm.engine.delegate.IDelegateExecution execution) throws Exception
        //public virtual void execute(IDelegateExecution execution)
        //{
        //    wasExecuted = true;
        //}

    }

}