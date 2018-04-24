using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Scripting.Engine
{
    public class BeansResolverFactory : IResolverFactory, IResolver
    {
        public IResolver CreateResolver(IVariableScope variableScope)
        {
            return this;
        }

        public bool ContainsKey(Object key)
        {
            return Context.ProcessEngineConfiguration.Beans.ContainsKey(key);
        }

        public Object Get(Object key)
        {
            return Context.ProcessEngineConfiguration.Beans[key];//.Get(key);
        }

        public ICollection<String> KeySet()
        {
            return Context.ProcessEngineConfiguration.Beans.Keys.Cast<string>().ToList();
        }
    }
}
