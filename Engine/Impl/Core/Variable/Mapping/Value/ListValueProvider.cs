using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping.value
{
    /// <summary>
    ///     
    /// </summary>
    public class ListValueProvider : IParameterValueProvider
    {
        protected internal IList<IParameterValueProvider> providerList;

        public ListValueProvider(IList<IParameterValueProvider> providerList)
        {
            this.providerList = providerList;
        }

        public virtual IList<IParameterValueProvider> ProviderList
        {
            get { return providerList; }
            set { providerList = value; }
        }

        public virtual object GetValue(IVariableScope variableScope)
        {
            IList<object> valueList = new List<object>();
            foreach (var provider in providerList)
                valueList.Add(provider.GetValue(variableScope));
            return valueList;
        }
    }
}