using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    

    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class GetExternalTaskErrorDetailsCmd : ICommand<string>
    {
        private const long SerialVersionUid = 1L;
        private readonly string _externalTaskId;

        public GetExternalTaskErrorDetailsCmd(string externalTaskId)
        {
            this._externalTaskId = externalTaskId;
        }

        public virtual string Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("externalTaskId", _externalTaskId);

            ExternalTaskEntity externalTask = commandContext.ExternalTaskManager.FindExternalTaskById(_externalTaskId);

            EnsureUtil.EnsureNotNull("No external ITask found with id " + _externalTaskId, "externalTask", externalTask);

            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckReadProcessInstance(externalTask.ProcessInstanceId);
            }

            return externalTask.ErrorDetails;
        }
    }
}