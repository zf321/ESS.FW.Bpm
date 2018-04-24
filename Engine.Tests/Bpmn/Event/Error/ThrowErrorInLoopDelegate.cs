using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.Event.Error
{
    
    public class ThrowErrorInLoopDelegate : IJavaDelegate
    {
        public void Execute(IBaseDelegateExecution execution)
        {
            throw new BpmnError("E1");
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override public void execute(org.Camunda.bpm.engine.delegate.IDelegateExecution execution) throws Exception
        //public override void execute(IDelegateExecution execution)
        //{
        //    throw new BpmnError("E1");
        //}
    }

}