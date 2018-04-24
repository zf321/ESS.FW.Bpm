using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;

namespace ESS.FW.Bpm.Engine.Impl.Delegate
{
    /// <summary>
    ///     Class responsible for handling Expression.setValue() invocations.
    ///     
    /// </summary>
    public class ExpressionSetInvocation : DelegateInvocation
    {
        protected internal readonly object Value;

        protected internal readonly ValueExpression ValueExpression;
        protected internal ELContext ElContext;

        public ExpressionSetInvocation(ValueExpression valueExpression, ELContext elContext, object value,
            IBaseDelegateExecution contextExecution) : base(contextExecution, null)
        {
            this.ValueExpression = valueExpression;
            this.Value = value;
            this.ElContext = elContext;
        }
        
        protected internal override void Invoke()
        {
            ValueExpression.SetValue(ElContext, Value);
        }
    }
}