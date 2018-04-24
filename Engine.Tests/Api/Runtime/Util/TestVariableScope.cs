using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Core.Variable;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace Engine.Tests.Api.Runtime.Util
{
    /// <summary>
    /// </summary>
    public class TestVariableScope : AbstractVariableScope
    {
        protected internal VariableStore variableStore =
            new VariableStore();

        protected override VariableStore VariableStore
        {
            get { return variableStore; }
        }

        public override AbstractVariableScope ParentVariableScope
        {
            get { return null; }
        }

        protected override IVariableInstanceFactory<ICoreVariableInstance> VariableInstanceFactory
        {
            get { return VariableInstanceEntityFactory.Instance; }
        }

        protected override IList<IVariableInstanceLifecycleListener<ICoreVariableInstance>>
            VariableInstanceLifecycleListeners { get; } =
            new List<IVariableInstanceLifecycleListener<ICoreVariableInstance>>();
    }
}