using System.Collections.Generic;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Task;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface ITaskReportManager
    {
        IList<IDurationReportResult> CreateHistoricTaskDurationReport(HistoricTaskInstanceReportImpl query);
        IList<ITaskCountByCandidateGroupResult> CreateTaskCountByCandidateGroupReport(TaskReportImpl query);
        IList<IHistoricTaskInstanceReportResult> SelectHistoricTaskInstanceCountByProcDefKeyReport(HistoricTaskInstanceReportImpl query);
        IList<IHistoricTaskInstanceReportResult> SelectHistoricTaskInstanceCountByTaskNameReport(HistoricTaskInstanceReportImpl query);
    }
}