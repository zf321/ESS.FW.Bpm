using System;
using ESS.FW.Bpm.Engine.History;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    /// 
	/// <summary>
	/// 
	/// 
	/// </summary>
	public class HistoricActivityStatisticsImpl : IHistoricActivityStatistics, IDbEntity
    {
        public virtual string Id { get; set; }


        public virtual long Instances { get; set; }


        public virtual long Finished { get; set; }


        public virtual long Canceled { get; set; }


        public virtual long CompleteScope { get; set; }

        public object GetPersistentState()
        {
            throw new NotImplementedException();
        }
    }

}