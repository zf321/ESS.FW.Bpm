using System;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IMeterLogManager
    {
        void DeleteAll();
        void DeleteByTimestampAndReporter(DateTime? timestamp, string reporter);
        void Insert(MeterLogEntity meterLogEntity);
    }
}