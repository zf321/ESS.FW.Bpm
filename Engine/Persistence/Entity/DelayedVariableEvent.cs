


using ESS.FW.Bpm.Engine.Impl.Core.Variable.Event;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{


	/// 
	/// <summary>
	/// 
	/// </summary>
	public class DelayedVariableEvent
	{

	  protected internal PvmExecutionImpl targetScope;
	  protected internal VariableEvent @event;

	  public DelayedVariableEvent(PvmExecutionImpl targetScope, VariableEvent @event)
	  {
		this.targetScope = targetScope;
		this.@event = @event;
	  }

	  public virtual PvmExecutionImpl TargetScope
	  {
		  get
		  {
			return targetScope;
		  }
	  }

	  public virtual VariableEvent Event
	  {
		  get
		  {
			return @event;
		  }
	  }

	}

}