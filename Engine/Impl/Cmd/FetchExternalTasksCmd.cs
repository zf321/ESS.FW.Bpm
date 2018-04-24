using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Externaltask;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.DB.EntityManager;
using ESS.FW.Bpm.Engine.Impl.DB.EntityManager.Operation;
using ESS.FW.Bpm.Engine.Impl.Externaltask;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    ///     
    /// </summary>
    public class FetchExternalTasksCmd : ICommand<IList<ILockedExternalTask>>
    {
        protected internal static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;

        protected internal IDictionary<string, TopicFetchInstruction> FetchInstructions =
            new Dictionary<string, TopicFetchInstruction>();

        protected internal int MaxResults;
        protected internal bool UsePriority;

        protected internal string WorkerId;

        public FetchExternalTasksCmd(string workerId, int maxResults,
            IDictionary<string, TopicFetchInstruction> instructions) : this(workerId, maxResults, instructions, false)
        {
        }

        public FetchExternalTasksCmd(string workerId, int maxResults,
            IDictionary<string, TopicFetchInstruction> instructions, bool usePriority)
        {
            this.WorkerId = workerId;
            this.MaxResults = maxResults;
            FetchInstructions = instructions;
            this.UsePriority = usePriority;
        }

        public virtual IList<ILockedExternalTask> Execute(CommandContext commandContext)
        {
            ValidateInput();

            IList<ExternalTaskEntity> externalTasks = commandContext.ExternalTaskManager.SelectExternalTasksForTopics(FetchInstructions.Keys, MaxResults, UsePriority);
            
            IList<ILockedExternalTask> result = new List<ILockedExternalTask>();

            foreach (ExternalTaskEntity entity in externalTasks)
            {

                TopicFetchInstruction fetchInstruction = FetchInstructions[entity.TopicName];
                entity.Lock(WorkerId, fetchInstruction.LockDuration.Value);

                LockedExternalTaskImpl resultTask = LockedExternalTaskImpl.FromEntity(entity, fetchInstruction.VariablesToFetch, fetchInstruction.DeserializeVariables);

                result.Add(resultTask);
            }

            FilterOnOptimisticLockingFailure(commandContext, result);

            return result;
        }
        
        protected internal virtual void FilterOnOptimisticLockingFailure(CommandContext commandContext,
            IList<ILockedExternalTask> tasks)
        {
            //commandContext.DbEntityManager.RegisterOptimisticLockingListener(
            //    new OptimisticLockingListenerAnonymousInnerClass(this, tasks));
        }

        protected internal virtual void ValidateInput()
        {
            EnsureUtil.EnsureNotNull("workerId", WorkerId);
            EnsureUtil.EnsureGreaterThanOrEqual("maxResults", MaxResults, 0);

            foreach (var instruction in FetchInstructions.Values)
            {
                EnsureUtil.EnsureNotNull("topicName", instruction.TopicName);
                EnsureUtil.EnsurePositive("lockTime", instruction.LockDuration);
            }
        }

        private class OptimisticLockingListenerAnonymousInnerClass : IOptimisticLockingListener
        {
            private readonly FetchExternalTasksCmd _outerInstance;

            private readonly IList<ILockedExternalTask> _tasks;

            public OptimisticLockingListenerAnonymousInnerClass(FetchExternalTasksCmd outerInstance,
                IList<ILockedExternalTask> tasks)
            {
                this._outerInstance = outerInstance;
                this._tasks = tasks;
            }


            public virtual Type EntityType
            {
                get { return typeof(ExternalTaskEntity); }
            }

            public virtual void FailedOperation(DbOperation operation)
            {
                if (operation is DbEntityOperation)
                {
                    var dbEntityOperation = (DbEntityOperation) operation;
                    var dbEntity = dbEntityOperation.Entity;

                    var failedOperationEntityInList = false;

                    var it = _tasks.GetEnumerator();
                    while (it.MoveNext())
                    {
                        var resultTask = it.Current;
                        if (resultTask.Id.Equals(dbEntity.Id))
                        {
//JAVA TO C# CONVERTER TODO ITask: .NET enumerators are read-only:
                            //it.remove();
                            failedOperationEntityInList = true;
                            break;
                        }
                    }

                    if (!failedOperationEntityInList)
                        throw Log.ConcurrentUpdateDbEntityException(operation);
                }
            }
        }
    }
}