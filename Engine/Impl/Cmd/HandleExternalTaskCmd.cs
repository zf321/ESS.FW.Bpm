using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     Represents an abstract class for the handle of external ITask commands.
    ///      
    /// </summary>
    public abstract class HandleExternalTaskCmd : ExternalTaskCmd
    {
        /// <summary>
        ///     The reported worker id.
        /// </summary>
        protected internal string WorkerId;

        public HandleExternalTaskCmd(string externalTaskId, string workerId) : base(externalTaskId)
        {
            this.WorkerId = workerId;
        }

        /// <summary>
        ///     Returns the error message. Which is used to create an specific message
        ///     for the BadUserRequestException if an worker has no rights to execute commands of the external ITask.
        /// </summary>
        /// <returns> the specific error message </returns>
        public abstract string ErrorMessageOnWrongWorkerAccess { get; }

        public override object Execute(CommandContext commandContext)
        {
            ValidateInput();

            ExternalTaskEntity externalTask = commandContext.ExternalTaskManager.FindExternalTaskById(ExternalTaskId);
            EnsureUtil.EnsureNotNull(typeof(NotFoundException), "Cannot find external ITask with id " + ExternalTaskId, "externalTask", externalTask);

            if (!WorkerId.Equals(externalTask.WorkerId))
            {
                throw new BadUserRequestException(ErrorMessageOnWrongWorkerAccess + "'. It is locked by worker '" + externalTask.WorkerId + "'.");
            }

            foreach (ICommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckUpdateProcessInstanceById(externalTask.ProcessInstanceId);
            }

            Execute(externalTask);

            return null;
        }

        /// <summary>
        ///     Validates the current input of the command.
        /// </summary>
        protected internal override void ValidateInput()
        {
            EnsureUtil.EnsureNotNull("workerId", WorkerId);
        }
    }
}

