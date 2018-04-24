


using ESS.FW.Bpm.Engine.History;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    /// <summary>
	/// .
	/// </summary>
	public class TaskReportResultEntity : IHistoricTaskInstanceReportResult
	{

	  public virtual long? Count { get; set; }


	  public virtual string ProcessDefinitionKey { get; set; }


	  public virtual string ProcessDefinitionId { get; set; }


	  public virtual string ProcessDefinitionName { get; set; }


	  public virtual string TaskName { get; set; }


	  public virtual string TenantId { get; set; }


	  public override string ToString()
	  {
		return this.GetType().Name + "[" + "count=" + Count + ", processDefinitionKey='" + ProcessDefinitionKey + '\'' + ", processDefinitionId='" + ProcessDefinitionId + '\'' + ", processDefinitionName='" + ProcessDefinitionName + '\'' + ", taskName='" + TaskName + '\'' + ", tenantId='" + TenantId + '\'' + ']';
	  }
	}

}