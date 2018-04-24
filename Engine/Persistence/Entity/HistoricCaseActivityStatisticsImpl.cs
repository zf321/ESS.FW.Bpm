using System;
using ESS.FW.Bpm.Engine.History;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    /// <summary>
	/// 
	/// 
	/// </summary>
	public class HistoricCaseActivityStatisticsImpl : IHistoricCaseActivityStatistics, IDbEntity
    {
        protected internal long available;
	  protected internal long enabled;
	  protected internal long disabled;
	  protected internal long active;
	  protected internal long completed;
	  protected internal long terminated;

	  public virtual string Id { get; set; }

        public virtual long Available
	  {
		  get
		  {
			return available;
		  }
	  }

	  public virtual long Enabled
	  {
		  get
		  {
			return enabled;
		  }
	  }

	  public virtual long Disabled
	  {
		  get
		  {
			return disabled;
		  }
	  }

	  public virtual long Active
	  {
		  get
		  {
			return active;
		  }
	  }

	  public virtual long Completed
	  {
		  get
		  {
			return completed;
		  }
	  }

	  public virtual long Terminated
	  {
		  get
		  {
			return terminated;
		  }
	  }
        

        public object GetPersistentState()
        {
            throw new NotImplementedException();
        }
    }

}