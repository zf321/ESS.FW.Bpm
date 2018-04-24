using ESS.FW.Bpm.Engine.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Listener
{
    /// <summary>
    ///     An <seealso cref="IExecutionListener" /> that evaluates a <seealso cref="IExpression" /> when notified.
    ///     
    /// </summary>
    public class ExpressionExecutionListener : IDelegateListener<IBaseDelegateExecution>
    {
        protected internal IExpression Expression;

        public ExpressionExecutionListener(IExpression expression)
        {
            this.Expression = expression;
        }

        /// <summary>
        ///     returns the expression text for this execution listener. Comes in handy if you want to
        ///     check which listeners you already have.
        /// </summary>
        public virtual string ExpressionText
        {
            get { return Expression.ExpressionText; }
        }
        
        public virtual void Notify(IBaseDelegateExecution execution)
        {
            // Return value of expression is ignored
            Expression.GetValue(execution);
        }
    }
}