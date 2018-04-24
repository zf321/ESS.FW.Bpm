using ESS.FW.Bpm.Engine.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Variable.listener
{
    /// <summary>
    ///     
    /// </summary>
    public class ExpressionCaseVariableListener : ICaseVariableListener
    {
        protected internal IExpression Expression;

        public ExpressionCaseVariableListener(IExpression expression)
        {
            this.Expression = expression;
        }

        /// <summary>
        ///     Returns the expression text for this execution listener. Comes in handy if you want to
        ///     check which listeners you already have.
        /// </summary>
        public virtual string ExpressionText
        {
            get { return Expression.ExpressionText; }
        }
        
        public virtual void Notify(IDelegateCaseVariableInstance variableInstance)
        {
            var variableInstanceImpl = (DelegateCaseVariableInstanceImpl) variableInstance;

            // Return value of expression is ignored
            Expression.GetValue(variableInstanceImpl.ScopeExecution);
        }
    }
}