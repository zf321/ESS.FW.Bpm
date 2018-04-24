using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.Event.End
{

    public class DummyServiceTask : IJavaDelegate
    {

        public static bool wasExecuted = false;
        public static bool expressionWasExecuted = false;
        public static bool delegateExpressionWasExecuted = false;

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void execute(org.Camunda.bpm.engine.delegate.IDelegateExecution execution) throws Exception
        //public virtual void execute(IDelegateExecution execution)
        //{
        //    bool expressionWasExecuted = (bool?)execution.GetVariable("expressionWasExecuted").Value;
        //    bool delegateExpressionWasExecuted = (bool?)execution.GetVariable("delegateExpressionWasExecuted").Value;
        //    bool wasExecuted = (bool?)execution.GetVariable("wasExecuted").Value;

        //    expressionWasExecuted = expressionWasExecuted;
        //    delegateExpressionWasExecuted = delegateExpressionWasExecuted;
        //    wasExecuted = wasExecuted;
        //}

        public void Execute(IBaseDelegateExecution execution)
        {
            bool varExpressionWasExecuted = (bool)execution.GetVariable("expressionWasExecuted");
            bool varDelegateExpressionWasExecuted = (bool)execution.GetVariable("delegateExpressionWasExecuted");
            bool varWasExecuted = (bool)execution.GetVariable("wasExecuted");

            expressionWasExecuted = varExpressionWasExecuted;
            delegateExpressionWasExecuted = varDelegateExpressionWasExecuted;
            wasExecuted = varWasExecuted;
        }
    }

}