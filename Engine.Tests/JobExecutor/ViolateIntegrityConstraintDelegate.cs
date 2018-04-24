using System;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.DB.EntityManager.Operation;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace Engine.Tests.JobExecutor
{
    /// <summary>
    /// </summary>
    public class ViolateIntegrityConstraintDelegate : IJavaDelegate
    {
        public virtual void Execute(IBaseDelegateExecution execution)
        {
            throw new NotImplementedException("C#与java之间db操作机制不一样，java在这里的逻辑会引发外键约束异常");
            var existingId = execution.Id;

            // insert an execution referencing the current execution

            var newExecution = new ExecutionEntity();
            newExecution.Id = "someId";
            newExecution.SetParentId(existingId);

            var insertOperation = new DbEntityOperation();
            insertOperation.OperationType = DbOperationType.Insert;
            insertOperation.Entity = newExecution;
           
            newExecution.Insert();
            //Context.CommandContext.DbSqlSession.ExecuteDbOperation(insertOperation);
        }
    }
}