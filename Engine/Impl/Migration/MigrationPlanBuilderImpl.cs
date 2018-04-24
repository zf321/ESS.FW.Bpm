using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Migration;

namespace ESS.FW.Bpm.Engine.Impl.migration
{
    /// <summary>
    ///     
    /// </summary>
    public class MigrationPlanBuilderImpl : IMigrationInstructionBuilder, IMigrationInstructionsBuilder
    {
        //protected internal CommandExecutor commandExecutor;
        protected internal IList<MigrationInstructionImpl> explicitMigrationInstructions;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal bool MapEqualActivitiesRenamed;

        protected internal string sourceProcessDefinitionId;
        protected internal string targetProcessDefinitionId;
        protected internal bool updateEventTriggersForGeneratedInstructions;

        public MigrationPlanBuilderImpl(ICommandExecutor commandExecutor, string sourceProcessDefinitionId,
            string targetProcessDefinitionId)
        {
            //this.commandExecutor = commandExecutor;
            this.sourceProcessDefinitionId = sourceProcessDefinitionId;
            this.targetProcessDefinitionId = targetProcessDefinitionId;
            explicitMigrationInstructions = new List<MigrationInstructionImpl>();
        }

        public virtual string SourceProcessDefinitionId
        {
            get { return sourceProcessDefinitionId; }
        }

        public virtual string TargetProcessDefinitionId
        {
            get { return targetProcessDefinitionId; }
        }

        public virtual bool UpdateEventTriggersForGeneratedInstructions
        {
            get { return updateEventTriggersForGeneratedInstructions; }
        }

        public virtual IList<MigrationInstructionImpl> ExplicitMigrationInstructions
        {
            get { return explicitMigrationInstructions; }
        }

        public virtual IMigrationInstructionsBuilder MapEqualActivities()
        {
            MapEqualActivitiesRenamed = true;
            return this;
        }

        public virtual IMigrationInstructionBuilder MapActivities(string sourceActivityId, string targetActivityId)
        {
            explicitMigrationInstructions.Add(new MigrationInstructionImpl(sourceActivityId, targetActivityId)
            );
            return this;
        }

        public virtual IMigrationInstructionBuilder UpdateEventTrigger()
        {
            explicitMigrationInstructions[explicitMigrationInstructions.Count - 1].UpdateEventTrigger = true;
            return this;
        }

        public virtual IMigrationPlan Build()
        {
            return null;
            //return commandExecutor.execute(new CreateMigrationPlanCmd(this));
        }

        public virtual IMigrationInstructionsBuilder UpdateEventTriggers()
        {
            updateEventTriggersForGeneratedInstructions = true;
            return this;
        }
    }
}