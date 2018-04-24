using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     Represents a base class for the external ITask commands.
    ///     Contains functionality to get the external ITask by id and check
    ///     the authorization for the execution of a command on the requested external ITask.
    ///      
    /// </summary>
    public abstract class ExternalTaskCmd : ICommand<object>
    {
        /// <summary>
        ///     The corresponding external ITask id.
        /// </summary>
        protected internal string ExternalTaskId;

        public ExternalTaskCmd(string externalTaskId)
        {
            this.ExternalTaskId = externalTaskId;
        }


        public virtual object Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("externalTaskId", ExternalTaskId);
            ValidateInput();

            ExternalTaskEntity externalTask = commandContext.ExternalTaskManager.FindExternalTaskById(ExternalTaskId);
            EnsureUtil.EnsureNotNull(typeof(NotFoundException), "Cannot find external ITask with id " + ExternalTaskId, "externalTask", externalTask);

            foreach (ICommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckUpdateProcessInstanceById(externalTask.ProcessInstanceId);
            }

            Execute(externalTask);

            return null;
        }

        /// <summary>
        ///     Executes the specific external ITask commands, which belongs to the current sub class.
        /// </summary>
        /// <param name="externalTask"> the external ITask which is used for the command execution </param>
        protected internal abstract object Execute(ExternalTaskEntity externalTask);

        /// <summary>
        ///     Validates the current input of the command.
        /// </summary>
        protected internal abstract void ValidateInput();
    }
}

