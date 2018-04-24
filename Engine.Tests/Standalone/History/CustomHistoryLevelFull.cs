using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.History.Impl.Event;

namespace Engine.Tests.Standalone.History
{
    
	/// <summary>
	/// 
	/// </summary>
	public class CustomHistoryLevelFull : IHistoryLevel
	{

	  public virtual int Id
	  {
		  get
		  {
			return 42;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return "aCustomHistoryLevel";
		  }
	  }

	  public virtual bool IsHistoryEventProduced(HistoryEventTypes eventType, object entity)
	  {
		return true;
	  }
	}

}