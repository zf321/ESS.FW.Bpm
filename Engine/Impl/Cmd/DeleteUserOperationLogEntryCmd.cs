using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    public class DeleteUserOperationLogEntryCmd : ICommand<object>
    {
        protected internal string EntryId;

        public DeleteUserOperationLogEntryCmd(string entryId)
        {
            this.EntryId = entryId;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull(typeof(NotValidException), "entryId", EntryId);

            IUserOperationLogEntry entry = commandContext.OperationLogManager.FindOperationLogById(EntryId);

            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckDeleteUserOperationLog(entry);
            }

            commandContext.OperationLogManager.DeleteOperationLogEntryById(EntryId);
            return null;
        }
    }
}