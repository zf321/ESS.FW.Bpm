using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Javax.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Scripting.Engine
{
    public class ScriptBindingsFactory
    {
        protected IList<IResolverFactory> resolverFactories;

        public ScriptBindingsFactory(IList<IResolverFactory> resolverFactories)
        {
            this.resolverFactories = resolverFactories;
        }

        public IBindings CreateBindings(IVariableScope variableScope, IBindings engineBindings)
        {
            List<IResolver> scriptResolvers = new List<IResolver>();
            foreach (IResolverFactory scriptResolverFactory in resolverFactories)
            {
                IResolver resolver = scriptResolverFactory.CreateResolver(variableScope);
                if (resolver != null)
                {
                    scriptResolvers.Add(resolver);
                }
            }
            return new ScriptBindings(scriptResolvers, variableScope, engineBindings);
        }

        public IList<IResolverFactory> GetResolverFactories()
        {
            return resolverFactories;
        }

        public void SetResolverFactories(IList<IResolverFactory> resolverFactories)
        {
            this.resolverFactories = resolverFactories;
        }
    }
}
