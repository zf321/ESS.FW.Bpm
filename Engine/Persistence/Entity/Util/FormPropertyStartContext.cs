//using ActivityImpl = org.camunda.bpm.engine.impl.Pvm.Process.ActivityImpl;
//using ProcessInstanceStartContext = org.camunda.bpm.engine.impl.Pvm.Runtime.ProcessInstanceStartContext;
//using PvmExecutionImpl = org.camunda.bpm.engine.impl.Pvm.Runtime.PvmExecutionImpl;
//using IVariableMap = org.camunda.bpm.engine.Variable.IVariableMap;

using ESS.FW.Bpm.Engine.Form.Impl;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Variable;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Util
{
    

	/// <summary>
	/// 
	/// 
	/// </summary>
	public class FormPropertyStartContext : ProcessInstanceStartContext
	{

	  protected internal IVariableMap formProperties;

	  public FormPropertyStartContext(ActivityImpl selectedInitial) : base(selectedInitial)
	  {
	  }

	  /// <param name="properties"> </param>
	  public virtual IVariableMap FormProperties
	  {
		  set
		  {
			this.formProperties = value;
		  }
	  }

	  public override void ExecutionStarted(IActivityExecution execution)
	  {
		FormPropertyHelper.InitFormPropertiesOnScope(formProperties, execution);

		// make sure create events are fired after form is submitted
		base.ExecutionStarted(execution);
	  }

	}

}