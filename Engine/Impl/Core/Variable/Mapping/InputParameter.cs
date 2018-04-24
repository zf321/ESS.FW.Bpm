using ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping.value;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope;

namespace ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping
{
    /// <summary>
    ///     <pre>
    ///                      +-----------------+
    ///                      |                 |
    ///         outer scope-----> inner scope  |
    ///                      |                 |
    ///                      +-----------------+
    ///     </pre>
    ///     
    /// </summary>
    public class InputParameter : IoParameter
    {
        private static readonly CoreLogger Log = ProcessEngineLogger.CoreLogger;

        public InputParameter(string name, IParameterValueProvider valueProvider) : base(name, valueProvider)
        {
        }

        protected internal override void Execute(AbstractVariableScope innerScope, AbstractVariableScope outerScope)
        {
            // get value from outer scope
            var value = valueProvider.GetValue(outerScope);

            Log.DebugMappingValueFromOuterScopeToInnerScope(value, outerScope, name, innerScope);

            // set variable in inner scope
            innerScope.SetVariableLocal(name, value);
        }
    }
}