using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.migration
{
    public class MigrationPlanExecutionBuilderImpl : IMigrationPlanExecutionBuilder
    {
        //protected internal CommandExecutor commandExecutor;
        protected internal IMigrationPlan migrationPlan;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal IList<string> ProcessInstanceIdsRenamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal IQueryable<IProcessInstance> ProcessInstanceQueryRenamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal bool SkipCustomListenersRenamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal bool SkipIoMappingsRenamed;

        public MigrationPlanExecutionBuilderImpl(ICommandExecutor commandExecutor, IMigrationPlan migrationPlan)
        {
            //this.commandExecutor = commandExecutor;
            this.migrationPlan = migrationPlan;
        }

        public virtual IMigrationPlan MigrationPlan
        {
            get { return migrationPlan; }
        }

        public virtual IList<string> _ProcessInstanceIds
        {
            get { return ProcessInstanceIdsRenamed; }
        }

        public virtual IQueryable<IProcessInstance> _ProcessInstanceQuery
        {
            get { return ProcessInstanceQueryRenamed; }
        }

        public virtual bool _SkipCustomListeners
        {
            get { return SkipCustomListenersRenamed; }
        }

        public virtual bool _SkipIoMappings
        {
            get { return SkipIoMappingsRenamed; }
        }

        public virtual IMigrationPlanExecutionBuilder ProcessInstanceIds(IList<string> processInstanceIds)
        {
            ProcessInstanceIdsRenamed = processInstanceIds;
            return this;
        }

        public virtual IMigrationPlanExecutionBuilder ProcessInstanceIds(params string[] processInstanceIds)
        {
            if (processInstanceIds == null)
                ProcessInstanceIdsRenamed = null;
            else
                ProcessInstanceIdsRenamed = processInstanceIds;
            return this;
        }

        public virtual IMigrationPlanExecutionBuilder ProcessInstanceQuery(IQueryable<IProcessInstance> processInstanceQuery)
        {
            ProcessInstanceQueryRenamed = processInstanceQuery;
            return this;
        }

        public virtual IMigrationPlanExecutionBuilder SkipCustomListeners()
        {
            SkipCustomListenersRenamed = true;
            return this;
        }

        public virtual IMigrationPlanExecutionBuilder SkipIoMappings()
        {
            SkipIoMappingsRenamed = true;
            return this;
        }

        public virtual void Execute()
        {
            throw new NotImplementedException();
            //Execute(true);
        }

        public IBatch ExecuteAsync()
        {
            throw new NotImplementedException();
        }

        //public virtual Batch executeAsync()
        //{
        //    return commandExecutor.execute(new MigrateProcessInstanceBatchCmd(this));
        //}

        //public virtual void execute(bool writeOperationLog)
        //{
        //    commandExecutor.execute(new MigrateProcessInstanceCmd(this, writeOperationLog));

        //}
    }
}