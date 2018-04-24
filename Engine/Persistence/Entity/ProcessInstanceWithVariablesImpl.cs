using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Variable;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
	/// 
	/// <summary>
	///  
	/// </summary>
	public class ProcessInstanceWithVariablesImpl : IProcessInstanceWithVariables
	{

	  protected internal readonly IPvmProcessInstance executionEntity;
	  protected internal readonly IVariableMap variables;

	  public ProcessInstanceWithVariablesImpl(IPvmProcessInstance executionEntity, IVariableMap variables)
	  {
		this.executionEntity = executionEntity;
		this.variables = variables;
	  }

	  public virtual IPvmProcessInstance ExecutionEntity
	  {
		  get
		  {
			return executionEntity;
		  }
	  }

	  public  IVariableMap Variables
	  {
		  get
		  {
			return variables;
		  }
	  }

	  public  string ProcessDefinitionId
	  {
		  get
		  {
			return executionEntity.ProcessDefinitionId;
		  }
	  }

	  public  string BusinessKey
	  {
		  get
		  {
			return executionEntity.BusinessKey;
		  }
	  }

	  public  string CaseInstanceId
	  {
		  get
		  {
			return executionEntity.CaseInstanceId;
		  }
	  }

	  public  bool IsSuspended
	  {
		  get
		  {
			return executionEntity.IsSuspended;
		  }
	  }

	  public  string Id
	  {
		  get
		  {
			return executionEntity.Id;
		  }
	  }

	  public  bool IsEnded
        {
		  get
		  {
			return executionEntity.IsEnded;
		  }
	  }

	  public  string ProcessInstanceId
	  {
		  get
		  {
			return executionEntity.ProcessInstanceId;
		  }
	  }

	  public  string TenantId
	  {
		  get
		  {
			return executionEntity.TenantId;
		  }
	  }

	    public int SuspensionState { get; set; }
	}

}