
using ESS.FW.Bpm.Engine.Impl.Core.Variable;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{


    /// <summary>
	/// 
	/// 
	/// </summary>
	public class VariableInstanceEntityPersistenceListener : IVariableInstanceLifecycleListener<ICoreVariableInstance>
    {

        public static readonly IVariableInstanceLifecycleListener<ICoreVariableInstance> Instance = new VariableInstanceEntityPersistenceListener();

        public void OnCreate(ICoreVariableInstance variable, AbstractVariableScope sourceScope)
        {
            if (!variable.GetIsTransient())
            {
                var variableEntity = (VariableInstanceEntity)variable;

                VariableInstanceEntity.Insert(variableEntity);
            }
        }

        public void OnDelete(ICoreVariableInstance variable, AbstractVariableScope sourceScope)
        {
            variable.Delete();
        }

        public void OnUpdate(ICoreVariableInstance variable, AbstractVariableScope sourceScope)
        {
        }

    }

}