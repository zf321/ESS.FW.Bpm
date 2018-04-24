
using ESS.FW.Bpm.Engine.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.task.listener
{
    /// <summary>
    ///     
    /// </summary>
    public class ExpressionTaskListener : ITaskListener
    {
        protected internal IExpression Expression;

        public ExpressionTaskListener(IExpression expression)
        {
            this.Expression = expression;
        }

        /// <summary>
        ///     returns the expression text for this task listener. Comes in handy if you want to
        ///     check which listeners you already have.
        /// </summary>
        public virtual string ExpressionText
        {
            get { return Expression.ExpressionText; }
        }

        public virtual void Notify(IDelegateTask delegateTask)
        {
            Expression.GetValue(delegateTask);
        }
    }
}