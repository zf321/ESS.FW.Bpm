using ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping.value;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope;

namespace ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping
{
    /// <summary>
    ///     <pre>
    ///         +--------------+
    ///         |              |
    ///         |  inner scope ------> outer scope
    ///         |              |
    ///         +--------------+
    ///     </pre>
    ///     
    /// </summary>
    public class OutputParameter : IoParameter
    {
        private static readonly CoreLogger Log = ProcessEngineLogger.CoreLogger;

        public OutputParameter(string name, IParameterValueProvider valueProvider) : base(name, valueProvider)
        {
        }

        protected internal override void Execute(AbstractVariableScope innerScope, AbstractVariableScope outerScope)
        {
            // get value from inner scope
            var value = valueProvider.GetValue(innerScope);

            Log.DebugMappingValuefromInnerScopeToOuterScope(value, innerScope, name, outerScope);

            // set variable in outer scope
            outerScope.SetVariable(name, value);
        }
    }
}