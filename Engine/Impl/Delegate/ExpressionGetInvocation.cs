using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;

namespace ESS.FW.Bpm.Engine.Impl.Delegate
{
    /// <summary>
    ///     Class responsible for handling Expression.getValue invocations
    ///     
    /// </summary>
    public class ExpressionGetInvocation : DelegateInvocation
    {
        private readonly ELContext _elContext;

        private readonly ValueExpression _valueExpression;

        public ExpressionGetInvocation(ValueExpression valueExpression, ELContext elContext,
            IBaseDelegateExecution contextExecution) : base(contextExecution, null)
        {
            this._valueExpression = valueExpression;
            this._elContext = elContext;
        }
        
        protected internal override void Invoke()
        {
            InvocationResult = _valueExpression.GetValue(_elContext);
        }
    }
}