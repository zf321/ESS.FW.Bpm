using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Task;

namespace ESS.FW.Bpm.Engine.Impl
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class TaskReportImpl : ITaskReport
    {
        private const long SerialVersionUid = 1L;

        [NonSerialized] 
        protected internal ICommandExecutor commandExecutor;

        public TaskReportImpl(ICommandExecutor commandExecutor)
        {
            this.commandExecutor = commandExecutor;
        }

        public virtual TenantCheck TenantCheck { get; } = new TenantCheck();


        public virtual IList<ITaskCountByCandidateGroupResult> TaskCountByCandidateGroup()
        {
            return commandExecutor.Execute(new CommandAnonymousInnerClass(this));
        }

        protected internal virtual IList<ITaskCountByCandidateGroupResult> CreateTaskCountByCandidateGroupReport(
            CommandContext commandContext)
        {
            return commandContext.TaskReportManager.CreateTaskCountByCandidateGroupReport(this);
        }

        private class CommandAnonymousInnerClass : ICommand<IList<ITaskCountByCandidateGroupResult>>
        {
            private readonly TaskReportImpl _outerInstance;

            public CommandAnonymousInnerClass(TaskReportImpl outerInstance)
            {
                this._outerInstance = outerInstance;
            }

            public virtual IList<ITaskCountByCandidateGroupResult> Execute(CommandContext commandContext)
            {
                return _outerInstance.CreateTaskCountByCandidateGroupReport(commandContext);
                //return null;
            }
        }
    }
}