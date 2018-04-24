namespace ESS.FW.Bpm.Engine.Impl.JobExecutor.HistoryCleanup
{

	/// <summary>
	/// 
	/// </summary>
	public class HistoryCleanupContext
	{

	  private bool _immediatelyDue;

	  public HistoryCleanupContext(bool immediatelyDue)
	  {
		this._immediatelyDue = immediatelyDue;
	  }

	  public virtual bool ImmediatelyDue
	  {
		  get
		  {
			return _immediatelyDue;
		  }
		  set
		  {
			this._immediatelyDue = value;
		  }
	  }

	}

}