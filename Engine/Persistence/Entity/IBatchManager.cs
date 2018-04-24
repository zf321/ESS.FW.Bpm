using ESS.FW.Bpm.Engine.Batch.Impl;
using ESS.FW.DataAccess;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IBatchManager:IRepository<BatchEntity,string>
    {
        BatchEntity FindBatchById(string id);
        void InsertBatch(BatchEntity batch);
        void UpdateBatchSuspensionStateById(string batchId, ISuspensionState suspensionState);
    }
}