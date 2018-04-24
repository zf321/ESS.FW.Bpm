using System.Threading;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.Event.End
{

    public class EndEventTestJavaDelegate : IJavaDelegate
    {

        public static int timesCalled = 0;

        public void Execute(IBaseDelegateExecution execution)
        {
            timesCalled++;
            Thread.Sleep(3000);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void execute(org.Camunda.bpm.engine.delegate.IDelegateExecution execution) throws Exception
        //public virtual void execute(IDelegateExecution execution)
        //{
        //    timesCalled++;
        //    Thread.Sleep(3000L);
        //}

    }

}