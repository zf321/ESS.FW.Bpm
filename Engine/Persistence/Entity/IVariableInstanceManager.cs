using System.Collections.Generic;
using ESS.FW.DataAccess;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IVariableInstanceManager:IRepository<VariableInstanceEntity,string>

    {
    void DeleteVariableInstanceByTask(TaskEntity task);
    IList<VariableInstanceEntity> FindVariableInstancesByCaseExecutionId(string caseExecutionId);
    IList<VariableInstanceEntity> FindVariableInstancesByExecutionId(string executionId);

    /// <summary>
    /// 先查缓存
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    VariableInstanceEntity FindVariableInstancesById(string id);

    IList<VariableInstanceEntity> FindVariableInstancesByProcessInstanceId(string processInstanceId);
    IList<VariableInstanceEntity> FindVariableInstancesByTaskId(string taskId);
    }
}