using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.EL
{
    /// <summary>
    ///     <seealso cref="ICondition" /> that resolves an UEL expression at runtime.
    ///     
    ///     
    /// </summary>
    public class UelExpressionCondition : ICondition
    {
        protected internal IExpression Expression;

        public UelExpressionCondition(IExpression expression)
        {
            this.Expression = expression;
        }

        public virtual bool Evaluate(IDelegateExecution execution)
        {
            return Evaluate(execution, execution);
        }

        public virtual bool Evaluate(IVariableScope scope, IDelegateExecution execution)
        {
            var result = Expression.GetValue(scope, execution);
            EnsureUtil.EnsureNotNull("condition expression returns null", "result", result);
            EnsureUtil.EnsureInstanceOf("condition expression returns non-Boolean", "result", result, typeof(bool));
            return (bool) result;
        }

        public virtual bool TryEvaluate(IVariableScope scope, IDelegateExecution execution)
        {
            var result = false;
            try
            {
                result = Evaluate(scope, execution);
            }
            catch (ProcessEngineException pee)
            {
                if (!(pee.InnerException is PropertyNotFoundException))
                    throw pee;
            }
            return result;
        }
    }
}