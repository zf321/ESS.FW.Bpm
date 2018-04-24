using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Batch.Impl;
using ESS.FW.Bpm.Engine.Migration;

namespace ESS.FW.Bpm.Engine.Impl.migration.batch
{
    public  class MigrationBatchConfiguration : BatchConfiguration
    {
        public MigrationBatchConfiguration(IList<string> ids) : base(ids)
        {
        }

        public MigrationBatchConfiguration(IList<string> ids, IMigrationPlan migrationPlan, bool isSkipCustomListeners,
            bool isSkipIoMappings) : base(ids)
        {
            MigrationPlan = migrationPlan;
            SkipCustomListeners = isSkipCustomListeners;
            SkipIoMappings = isSkipIoMappings;
        }

        public IMigrationPlan MigrationPlan { get; set; }


        public bool SkipCustomListeners { get; set; }


        public bool SkipIoMappings { get; set; }
    }
}