using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping.value
{
    /// <summary>
    ///     
    /// </summary>
    public class MapValueProvider : IParameterValueProvider
    {
        protected internal SortedDictionary<string, IParameterValueProvider> providerMap;

        public MapValueProvider(SortedDictionary<string, IParameterValueProvider> providerMap)
        {
            this.providerMap = providerMap;
        }

        public virtual SortedDictionary<string, IParameterValueProvider> ProviderMap
        {
            get { return providerMap; }
            set { providerMap = value; }
        }

        public virtual object GetValue(IVariableScope variableScope)
        {
            IDictionary<string, object> valueMap = new SortedDictionary<string, object>();
            foreach (var entry in providerMap)
                valueMap[entry.Key] = entry.Value.GetValue(variableScope);
            return valueMap;
        }
    }
}