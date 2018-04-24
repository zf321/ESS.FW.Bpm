using ESS.FW.Bpm.Engine.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping.value
{
    /// <summary>
    ///     A constant parameter value.
    ///     
    /// </summary>
    public class ConstantValueProvider : IParameterValueProvider
    {
        protected internal object value;

        public ConstantValueProvider(object value)
        {
            this.value = value;
        }

        public virtual object Value
        {
            set { this.value = value; }
        }

        public virtual object GetValue(IVariableScope scope)
        {
            return value;
        }
    }
}