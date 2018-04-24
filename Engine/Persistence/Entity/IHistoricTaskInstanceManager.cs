using System.Collections.Generic;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.DataAccess;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IHistoricTaskInstanceManager: IRepository<HistoricTaskInstanceEventEntity, string>
    {
        void CreateHistoricTask(TaskEntity task);
        void DeleteHistoricTaskInstanceById(string taskId);
        void DeleteHistoricTaskInstancesByCaseDefinitionId(string caseDefinitionId);
        void DeleteHistoricTaskInstancesByCaseInstanceId(string caseInstanceId);
        void DeleteHistoricTaskInstancesByProcessInstanceId(string processInstanceId);
        HistoricTaskInstanceEventEntity FindHistoricTaskInstanceById(string taskId);
        long FindHistoricTaskInstanceCountByNativeQuery(IDictionary<string, object> parameterMap);
        IList<IHistoricTaskInstance> FindHistoricTaskInstancesByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults);
        void MarkTaskInstanceEnded(string taskId, string deleteReason);
        void UpdateHistoricTaskInstance(TaskEntity taskEntity);
    }
}