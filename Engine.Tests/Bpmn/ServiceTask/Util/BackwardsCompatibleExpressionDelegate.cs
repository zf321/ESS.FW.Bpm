using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.ServiceTask.Util
{
    /// <summary>
    /// </summary>
    public class BackwardsCompatibleExpressionDelegate : IJavaDelegate
    {
        private IExpression _expression;

        public virtual IExpression Expression
        {
            set { _expression = value; }
        }

        public void Execute(IBaseDelegateExecution execution)
        {
            var result = _expression.GetValue(execution);
            execution.SetVariable("result", result);
        }
    }
}