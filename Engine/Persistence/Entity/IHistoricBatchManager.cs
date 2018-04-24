using ESS.FW.Bpm.Engine.Batch.Impl;
using ESS.FW.Bpm.Engine.Batch.Impl.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.DataAccess;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IHistoricBatchManager:IRepository<HistoricBatchEntity, string>
    {
        void CompleteHistoricBatch(BatchEntity batch);
        void CreateHistoricBatch(BatchEntity batch);
        void DeleteHistoricBatchById(string id);
        HistoricBatchEntity FindHistoricBatchById(string batchId);
    }
}