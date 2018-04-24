using System;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
	/// <summary>
	/// 
	/// 
	/// </summary>
	[Serializable]
	public class MeterLogEntity : IDbEntity
	{
	  public MeterLogEntity(string name, long value, DateTime timestamp) : this(name, null, value, timestamp)
	  {
	  }

	  public MeterLogEntity(string name, string reporter, long value, DateTime timestamp)
	  {
		this.Name = name;
		this.Reporter = reporter;
		this.Value = value;
		this.Timestamp = timestamp;
		this.Milliseconds = timestamp.Ticks;
	  }

	  public MeterLogEntity()
	  {
	  }

	  public virtual string Id { get; set; }


	  public virtual DateTime Timestamp { get; set; }


	  public virtual long? Milliseconds { get; set; }


	  public virtual string Name { get; set; }


	  public virtual long Value { get; set; }


	  public virtual string Reporter { get; set; }


	  public virtual object GetPersistentState()
	  {
			// immutable
			return typeof(MeterLogEntity);
	  }

	}

}