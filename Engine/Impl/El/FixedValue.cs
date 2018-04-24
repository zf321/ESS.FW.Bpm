using System;
using ESS.FW.Bpm.Engine.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.EL
{
    /// <summary>
    ///     Expression that always returns the same value when <code>getValue</code> is
    ///     called. Setting of the value is not supported.
    ///     
    /// </summary>
    public class FixedValue : Engine.Delegate.IExpression
    {
        private readonly object _value;

        public FixedValue(object value)
        {
            this._value = value;
        }

        public virtual object GetValue(IVariableScope variableScope)
        {
            return _value;
        }

        public virtual void SetValue(object value, IVariableScope variableScope)
        {
            throw new ProcessEngineException("Cannot change fixed value");
        }

        public virtual string ExpressionText
        {
            get { return _value.ToString(); }
        }

        public virtual bool LiteralText
        {
            get { return true; }
        }

        public virtual object GetValue(IVariableScope variableScope, IBaseDelegateExecution contextExecution)
        {
            return GetValue(variableScope);
        }
        
    }
}