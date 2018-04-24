using ESS.FW.Bpm.Engine.Persistence.Entity;
using System.Collections.Generic;
using System.Linq;

namespace ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope
{

    /// <summary>
    ///     
    /// </summary>
    public class VariableCollectionProvider : IVariablesProvider /*where T : ICoreVariableInstance*/
    {
        protected internal ICollection<ICoreVariableInstance> Variables;

        public VariableCollectionProvider(ICollection<ICoreVariableInstance> variables)
        {
            this.Variables = variables;
        }

        public virtual ICollection<ICoreVariableInstance> ProvideVariables()
        {
            if (Variables == null)
                return new List<ICoreVariableInstance>();
            return Variables;
        }

        public static VariableCollectionProvider EmptyVariables()// where T : ICoreVariableInstance
        {
            return new VariableCollectionProvider(Enumerable.Empty<ICoreVariableInstance>().ToList());
        }
    }
}