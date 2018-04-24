using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     <para>
    ///         Provide common logic for process instance deletion operations.
    ///         Permissions checking and single process instance removal included.
    ///     </para>
    /// </summary>
    public abstract class AbstractDeleteProcessInstanceCmd
    {
        protected internal string DeleteReason;

        protected internal bool ExternallyTerminated;
        protected internal bool SkipCustomListeners;

        protected internal virtual void CheckDeleteProcessInstance(ExecutionEntity execution,
            CommandContext commandContext)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckDeleteProcessInstance(execution);
        }

        protected internal virtual void DeleteProcessInstance(CommandContext commandContext, string processInstanceId,
            string deleteReason, bool skipCustomListeners, bool externallyTerminated)
        {
            EnsureUtil.EnsureNotNull(typeof(BadUserRequestException), "processInstanceId is null", "processInstanceId",
                processInstanceId);

            // fetch process instance
            var executionManager = commandContext.ExecutionManager;
            var execution = executionManager.FindExecutionById(processInstanceId);

            EnsureUtil.EnsureNotNull(typeof(BadUserRequestException),
                "No process instance found for id '" + processInstanceId + "'", "processInstance", execution);

            CheckDeleteProcessInstance(execution, commandContext);

            // delete process instance
            commandContext.ExecutionManager.DeleteProcessInstance(processInstanceId, deleteReason, false,
                skipCustomListeners, externallyTerminated);

            var superExecution = execution.SuperExecution;
            if (superExecution != null)
                commandContext.RunWithoutAuthorization(() =>
                {
                    var builder =
                        (ProcessInstanceModificationBuilderImpl) new ProcessInstanceModificationBuilderImpl(
                                commandContext, superExecution.ProcessInstanceId)
                            .CancelActivityInstance(superExecution.ActivityInstanceId);
                    builder.Execute(false, skipCustomListeners /*, skipIoMappings*/);
                });
            //// create user operation log
            commandContext.OperationLogManager.LogProcessInstanceOperation(
                UserOperationLogEntryFields.OperationTypeDelete, processInstanceId, null, null,
                new List<PropertyChange> {PropertyChange.EmptyChange});
        }
    }
}