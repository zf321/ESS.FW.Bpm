using System;

namespace ESS.Shared.Entities.Bpm.Config
{
    [Serializable]
    public class ProcessDefinitionBranch : IDbEntity, IRevision
    {
        public string ProcessDefinitionId { get; set; }

        public string BranchId { get; set; }

        public string BillTypeCode { get; set; }

        public int Category { get; set; }


        public string Id { get; set; }
        public int Revision { get; set; }

        public object GetPersistentState()
        {
            throw new NotImplementedException();
        }
    }
}
