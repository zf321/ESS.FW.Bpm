using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.ExecutionListener
{

    public class ExampleExecutionListenerTwo : IDelegateListener<IBaseDelegateExecution>
    {
        public void Notify(IBaseDelegateExecution execution)
        {
            execution.SetVariable("variableSetInExecutionListener", "secondValue");
            execution.SetVariable("eventNameReceived", execution.EventName);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void notify(org.Camunda.bpm.engine.delegate.IDelegateExecution execution) throws Exception
        //public virtual void notify(IDelegateExecution execution)
        //{
        //    execution.SetVariable("variableSetInExecutionListener", "secondValue");
        //    execution.SetVariable("eventNameReceived", execution.EventName);
        //}
    }

}