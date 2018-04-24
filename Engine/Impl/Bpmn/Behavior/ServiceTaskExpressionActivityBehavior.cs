using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     ActivityBehavior that evaluates an expression when executed. Optionally, it
    ///     sets the result of the expression as a variable on the execution.
    /// </summary>
    public class ServiceTaskExpressionActivityBehavior : TaskActivityBehavior
    {
        private readonly IExpression _expression;
        private readonly string _resultVariable;

        public ServiceTaskExpressionActivityBehavior(IExpression expression, string resultVariable)
        {
            _expression = expression;
            _resultVariable = resultVariable;
        }

        protected override void PerformExecution(IActivityExecution execution)
        {
            ExecuteWithErrorPropagation(execution, () =>
            {
                // getValue() can have side - effects, that's why we have to call it independently from the result variable
                var value = _expression.GetValue(execution);
                if (!ReferenceEquals(_resultVariable, null))
                    execution.SetVariable(_resultVariable, value);
                Leave(execution);
            });
        }
    }
}