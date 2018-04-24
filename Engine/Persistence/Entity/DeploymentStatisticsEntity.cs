using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Management;






namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    /// <summary>
    /// Í³¼Æ
    /// </summary>
    public partial class DeploymentStatisticsEntity : DeploymentEntity, IDeploymentStatistics
	{


	  public virtual int Instances { get; set; }
	  public virtual int FailedJobs { get; set; }
	  public virtual IList<IIncidentStatistics> IncidentStatistics { get; set; }

	  public override string ToString()
	  {
		return this.GetType().Name + "[instances=" + Instances + ", failedJobs=" + FailedJobs + ", id=" + Id + ", name=" + Name + ", resources=" + resources + ", deploymentTime=" + DeploymentTime + ", validatingSchema=" + IsValidatingSchema + ", isNew=" + IsNew + "]";
	  }
	}

}