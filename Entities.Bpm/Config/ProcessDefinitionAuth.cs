using System;

namespace ESS.Shared.Entities.Bpm.Config
{
    [Serializable]
    public class ProcessDefinitionAuth : IDbEntity, IRevision
    {
        public string ProcessDefinitionId { get; set; }

        public string ActivityId { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public string Id { get; set; }

        public int Revision { get; set; }

        public object GetPersistentState()
        {
            throw new NotImplementedException();
        }
    }
}
