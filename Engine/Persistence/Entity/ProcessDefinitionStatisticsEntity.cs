using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Management;



namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public class ProcessDefinitionStatisticsEntity : ProcessDefinitionEntity, IProcessDefinitionStatistics
	{


	  public virtual int Instances { get; set; }
	  public virtual int FailedJobs { get; set; }
	  public virtual IList<IIncidentStatistics> IncidentStatistics { get; set; }

	  public override string ToString()
	  {
		return this.GetType().Name + "[instances=" + Instances + ", failedJobs=" + FailedJobs + ", id=" + id + ", deploymentId=" + DeploymentId + ", description=" + description + ", historyLevel=" + historyLevel + ", category=" + Category + ", hasStartFormKey=" + HasStartFormKey + ", diagramResourceName=" + diagramResourceName + ", key=" + Key + ", name=" + name + ", resourceName=" + ResourceName + ", revision=" + Revision + ", version=" + version + ", suspensionState=" + SuspensionState + "]";
	  }
	}

}