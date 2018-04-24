using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.ExecutionListener
{

    public class ExampleFieldInjectedExecutionListener : IDelegateListener<IBaseDelegateExecution>
    {

        private IExpression fixedValue;

        private IExpression dynamicValue;

        public void Notify(IBaseDelegateExecution execution)
        {
            execution.SetVariable("var", fixedValue.GetValue(execution).ToString() + dynamicValue.GetValue(execution).ToString());
        }
    }

}