


using ESS.FW.Bpm.Engine.Impl.Core.Variable;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    

    /// <summary>
	/// 
	/// </summary>
	public class VariableInstanceConcurrentLocalInitializer : IVariableInstanceLifecycleListener<ICoreVariableInstance>
	{

	  protected internal ExecutionEntity Execution;

	  public VariableInstanceConcurrentLocalInitializer(ExecutionEntity execution)
	  {
		this.Execution = execution;
	  }

	  public  void OnCreate(ICoreVariableInstance variableInstance, AbstractVariableScope sourceScope)
	  {
		variableInstance.IsConcurrentLocal= !Execution.IsScope || Execution.ExecutingScopeLeafActivity;
	  }

	  public  void OnDelete(ICoreVariableInstance variableInstance, AbstractVariableScope sourceScope)
	  {

	  }

	  public  void OnUpdate(ICoreVariableInstance variableInstance, AbstractVariableScope sourceScope)
	  {

	  }

	}

}