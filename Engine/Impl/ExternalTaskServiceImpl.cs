using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Externaltask;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Externaltask;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl
{
    /// <summary>
    ///     
    ///     
    ///     
    /// </summary>
    public class ExternalTaskServiceImpl : ServiceImpl, IExternalTaskService
    {
        public virtual IQueryable<IExternalTask> FetchAndLock(int maxTasks, string workerId)
        {
            return FetchAndLock(maxTasks, workerId, false);
        }

        public virtual IQueryable<IExternalTask> FetchAndLock(int maxTasks, string workerId, bool usePriority)
        {
            //TODO
            Expression<Func<ExternalTaskEntity, bool>> expression = entity => entity.WorkerId == workerId;
            return CommandExecutor.Execute(new CreateQueryCmd<ExternalTaskEntity>(expression));
        }

        public virtual void Complete(string externalTaskId, string workerId)
        {
            CommandExecutor.Execute(new CompleteExternalTaskCmd(externalTaskId, workerId, null));
        }

        public virtual void Complete(string externalTaskId, string workerId, IDictionary<string, object> variables)
        {
            CommandExecutor.Execute(new CompleteExternalTaskCmd(externalTaskId, workerId, variables));
        }

        public virtual void HandleFailure(string externalTaskId, string workerId, string errorMessage, int retries,
            long retryDuration)
        {
            HandleFailure(externalTaskId, workerId, errorMessage, null, retries, retryDuration);
        }

        public virtual void HandleFailure(string externalTaskId, string workerId, string errorMessage,
            string errorDetails, int retries, long retryDuration)
        {
            CommandExecutor.Execute(new HandleExternalTaskFailureCmd(externalTaskId, workerId, errorMessage,
                errorDetails, retries, retryDuration));
        }

        public virtual void HandleBpmnError(string externalTaskId, string workerId, string errorCode)
        {
            CommandExecutor.Execute(new HandleExternalTaskBpmnErrorCmd(externalTaskId, workerId, errorCode));
        }

        public virtual void Unlock(string externalTaskId)
        {
            CommandExecutor.Execute(new UnlockExternalTaskCmd(externalTaskId));
        }

        public virtual void SetRetries(string externalTaskId, int retries)
        {
            CommandExecutor.Execute(new SetExternalTaskRetriesCmd(externalTaskId, retries));
        }

        public virtual void SetPriority(string externalTaskId, long priority)
        {
            CommandExecutor.Execute(new SetExternalTaskPriorityCmd(externalTaskId, priority));
        }

        public virtual IQueryable<IExternalTask> CreateExternalTaskQuery(Expression<Func<ExternalTaskEntity, bool>> expression)
        {
            return CommandExecutor.Execute(new CreateQueryCmd<ExternalTaskEntity>(expression));
        }


        public virtual string GetExternalTaskErrorDetails(string externalTaskId)
        {
            return CommandExecutor.Execute(new GetExternalTaskErrorDetailsCmd(externalTaskId));
        }

        public void SetRetries(IList<string> externalTaskIds, int retries)
        {
            throw new System.NotImplementedException();
        }

        public IBatch SetRetriesAsync(IList<string> externalTaskIds, IQueryable<IExternalTask> externalTaskQuery, int retries)
        {
            throw new System.NotImplementedException();
        }
    }
}