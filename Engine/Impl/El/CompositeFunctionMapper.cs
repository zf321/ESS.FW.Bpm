using System.Collections.Generic;
using System.Reflection;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;

namespace ESS.FW.Bpm.Engine.Impl.EL
{
    /// <summary>
    ///     A <seealso cref="FunctionMapper" /> implemenation which delegates to a list of
    ///     mappers. When a function is resolved, the list of mappers is iterated
    ///     and the first one to return a method is used.
    ///     
    /// </summary>
    public class CompositeFunctionMapper : FunctionMapper
    {
        protected internal IList<FunctionMapper> DelegateMappers;

        public CompositeFunctionMapper(IList<FunctionMapper> delegateMappers)
        {
            this.DelegateMappers = delegateMappers;
        }

        public override MethodInfo ResolveFunction(string prefix, string localName)
        {
            foreach (var mapper in DelegateMappers)
            {
                var resolvedFunction = mapper.ResolveFunction(prefix, localName);
                if (resolvedFunction != null)
                    return resolvedFunction;
            }
            return null;
        }
    }
}