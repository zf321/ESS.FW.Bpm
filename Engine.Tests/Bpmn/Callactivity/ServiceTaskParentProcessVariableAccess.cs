using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.Callactivity
{
    public class ServiceTaskParentProcessVariableAccess : IJavaDelegate
    {
        public void Execute(IBaseDelegateExecution execution)
        {
            (execution as IDelegateExecution).ProcessInstance.SuperExecution.SetVariable("greeting", "hello");
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void execute(delegate.IDelegateExecution execution) throws Exception
        //public virtual void execute(IDelegateExecution execution)
        //{
        //    execution.IProcessInstance.SuperExecution.SetVariable("greeting", "hello");
        //}

    }

}