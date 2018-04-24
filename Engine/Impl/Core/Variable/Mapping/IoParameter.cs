using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping.value;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope;

namespace ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping
{
    /// <summary>
    ///     An <seealso cref="IoParameter" /> creates a variable
    ///     in a target variable scope.
    ///     
    /// </summary>
    public abstract class IoParameter
    {
        /// <summary>
        ///     The name of the parameter. The name of the parameter is the
        ///     variable name in the target <seealso cref="IVariableScope" />.
        /// </summary>
        protected internal string name;

        /// <summary>
        ///     The provider of the parameter value.
        /// </summary>
        protected internal IParameterValueProvider valueProvider;

        public IoParameter(string name, IParameterValueProvider valueProvider)
        {
            this.name = name;
            this.valueProvider = valueProvider;
        }

        // getters / setters ///////////////////////////

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }


        public virtual IParameterValueProvider ValueProvider
        {
            get { return valueProvider; }
            set { valueProvider = value; }
        }

        /// <summary>
        ///     Execute the parameter in a given variable scope.
        /// </summary>
        public virtual void Execute(AbstractVariableScope scope)
        {
            Execute(scope, scope.ParentVariableScope);
        }

        /// <param name="innerScope"> </param>
        /// <param name="outerScope"> </param>
        protected internal abstract void Execute(AbstractVariableScope innerScope, AbstractVariableScope outerScope);
    }
}