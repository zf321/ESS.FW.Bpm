

using ESS.FW.Bpm.Engine.History;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    /// <summary>
	/// 
	/// 
	/// </summary>
	public partial class DurationReportResultEntity : ReportResultEntity, IDurationReportResult
	{

	  public virtual long Minimum { get; set; }


	  public virtual long Maximum { get; set; }


	  public virtual long Average { get; set; }


	  public override string ToString()
	  {
		return this.GetType().Name + "[period=" + Period + ", periodUnit=" + periodUnit + ", minimum=" + Minimum + ", maximum=" + Maximum + ", average=" + Average + "]";
	  }

	}

}