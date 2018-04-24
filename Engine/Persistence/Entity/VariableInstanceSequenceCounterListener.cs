
using ESS.FW.Bpm.Engine.Impl.Core.Variable;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{

    /// <summary>
	/// 
	/// </summary>
	public class VariableInstanceSequenceCounterListener : IVariableInstanceLifecycleListener<ICoreVariableInstance>
	{

	  public static readonly IVariableInstanceLifecycleListener<ICoreVariableInstance> Instance = new VariableInstanceSequenceCounterListener();

	  public  void OnCreate(ICoreVariableInstance variableInstance, AbstractVariableScope sourceScope)
	  {
	  }

	  public  void OnDelete(ICoreVariableInstance variableInstance, AbstractVariableScope sourceScope)
	  {
		variableInstance.IncrementSequenceCounter();
	  }

	  public  void OnUpdate(ICoreVariableInstance variableInstance, AbstractVariableScope sourceScope)
	  {
		variableInstance.IncrementSequenceCounter();
	  }

	}

}