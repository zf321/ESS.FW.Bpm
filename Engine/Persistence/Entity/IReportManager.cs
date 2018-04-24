using System.Collections.Generic;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IReportManager
    {
        IList<IDurationReportResult> SelectHistoricProcessInstanceDurationReport(HistoricProcessInstanceReportImpl query);
    }
}