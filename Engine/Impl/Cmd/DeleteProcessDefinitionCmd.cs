using System;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{

    /// <summary>
    ///     Command to delete a process definition form a deployment.
    ///      
    /// </summary>
    [Serializable]
    public class DeleteProcessDefinitionCmd : ICommand<object>
    {
        private readonly bool? _cascade;

        private readonly string _processDefinitionId;
        private readonly bool? _skipCustomListeners;

        public DeleteProcessDefinitionCmd(string processDefinitionId, bool? cascade, bool? skipCustomListeners)
        {
            this._processDefinitionId = processDefinitionId;
            this._cascade = cascade;
            this._skipCustomListeners = skipCustomListeners;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("processDefinitionId", _processDefinitionId);

            IProcessDefinition processDefinition =
                commandContext.ProcessDefinitionManager.FindLatestProcessDefinitionById(_processDefinitionId);
            EnsureUtil.EnsureNotNull(typeof(NotFoundException),
                "No process definition found with id '" + _processDefinitionId + "'", "processDefinition",
                processDefinition);

            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckDeleteProcessDefinitionById(_processDefinitionId);

            IUserOperationLogManager logManager = commandContext.OperationLogManager;
            logManager.LogProcessDefinitionOperation(UserOperationLogEntryFields.OperationTypeDelete,
                _processDefinitionId, processDefinition.Key, new PropertyChange("cascade", null, _cascade));

            commandContext.ProcessDefinitionManager.DeleteProcessDefinition(processDefinition, _processDefinitionId,
                _cascade.Value, _cascade.Value, _skipCustomListeners.Value);
            return null;
        }
    }
}