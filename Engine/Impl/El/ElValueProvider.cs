using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping.value;
using ESS.FW.Bpm.Engine.Impl.Util;
using IExpression= ESS.FW.Bpm.Engine.Delegate.IExpression;

namespace ESS.FW.Bpm.Engine.Impl.EL
{
    /// <summary>
    ///     Makes it possible to use expression in <seealso cref="IoParameter" /> mappings.
    ///     
    /// </summary>
    public class ElValueProvider : IParameterValueProvider
    {
        public ElValueProvider(Engine.Delegate.IExpression expression)
        {
            this.Expression = expression;
        }

        public virtual Engine.Delegate.IExpression Expression { get; set; }

        public virtual object GetValue(IVariableScope variableScope)
        {
            EnsureUtil.EnsureNotNull("variableScope", variableScope);
            return Expression.GetValue(variableScope);
        }
    }
}