



using ESS.FW.Bpm.Engine.Management;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    /// <summary>
	/// 
	/// </summary>
	public class IncidentStatisticsEntity : Management.IIncidentStatistics
	{

	  protected internal string incidentType;
	  protected internal int incidentCount;

	  public IncidentStatisticsEntity()
	  {
	  }

	  public virtual string IncidentType
	  {
		  get
		  {
			return incidentType;
		  }
	  }

	  public virtual string IncidenType
	  {
		  set
		  {
			this.incidentType = value;
		  }
	  }

	  public virtual int IncidentCount
	  {
		  get
		  {
			return incidentCount;
		  }
	  }

	  public override string ToString()
	  {
		return this.GetType().Name + "[incidentType=" + incidentType + ", incidentCount=" + incidentCount + "]";
	  }

	}

}