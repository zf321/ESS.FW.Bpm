using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Management;


namespace ESS.FW.Bpm.Engine.Persistence.Entity
{


	public class ActivityStatisticsImpl : IActivityStatistics
	{
	    public virtual string Id { get; set; }

	    public virtual int Instances { get; set; }

	    public virtual int FailedJobs { get; set; }

	    public virtual IList<IIncidentStatistics> IncidentStatistics { get; set; }

	    public override string ToString()
	  {
		return this.GetType().Name + "[id=" + Id + ", instances=" + Instances + ", failedJobs=" + FailedJobs + "]";
	  }

	}

}