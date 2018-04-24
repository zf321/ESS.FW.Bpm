using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.migration.validation.activity;
using ESS.FW.Bpm.Engine.Impl.migration.validation.instruction;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.migration
{
    /// <summary>
    ///     
    /// </summary>
    public class DefaultMigrationInstructionGenerator : IMigrationInstructionGenerator
    {
        protected internal IMigrationActivityMatcher MigrationActivityMatcher;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal IList<IMigrationActivityValidator> MigrationActivityValidatorsRenamed =
            new List<IMigrationActivityValidator>();

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal IList<IMigrationInstructionValidator> MigrationInstructionValidatorsRenamed =
            new List<IMigrationInstructionValidator>();

        public DefaultMigrationInstructionGenerator(IMigrationActivityMatcher migrationActivityMatcher)
        {
            this.MigrationActivityMatcher = migrationActivityMatcher;
        }

        public virtual IMigrationInstructionGenerator MigrationActivityValidators(
            IList<IMigrationActivityValidator> migrationActivityValidators)
        {
            MigrationActivityValidatorsRenamed = migrationActivityValidators;
            return this;
        }

        public virtual IMigrationInstructionGenerator MigrationInstructionValidators(
            IList<IMigrationInstructionValidator> migrationInstructionValidators)
        {
            MigrationInstructionValidatorsRenamed = new List<IMigrationInstructionValidator>();
            foreach (var validator in migrationInstructionValidators)
                if (
                    !(validator is CannotAddMultiInstanceInnerActivityValidator ||
                      validator is CannotRemoveMultiInstanceInnerActivityValidator))
                    MigrationInstructionValidatorsRenamed.Add(validator);

            return this;
        }

        public virtual ValidatingMigrationInstructions Generate(ProcessDefinitionImpl sourceProcessDefinition,
            ProcessDefinitionImpl targetProcessDefinition, bool updateEventTriggers)
        {
            var migrationInstructions = new ValidatingMigrationInstructions();
            Generate(sourceProcessDefinition, targetProcessDefinition, sourceProcessDefinition, targetProcessDefinition,
                migrationInstructions, updateEventTriggers);
            return migrationInstructions;
        }

        protected internal virtual IList<IValidatingMigrationInstruction> GenerateInstructionsForActivities(
            ICollection<ActivityImpl> sourceActivities, ICollection<ActivityImpl> targetActivities,
            bool updateEventTriggers, ValidatingMigrationInstructions existingInstructions)
        {
            IList<IValidatingMigrationInstruction> generatedInstructions = new List<IValidatingMigrationInstruction>();

            foreach (var sourceActivity in sourceActivities)
                if (!existingInstructions.ContainsInstructionForSourceScope(sourceActivity))
                    foreach (var targetActivity in targetActivities)
                        if (IsValidActivity(sourceActivity) && IsValidActivity(targetActivity) &&
                            MigrationActivityMatcher.MatchActivities(sourceActivity, targetActivity))
                        {
                            //for conditional events the update event trigger must be set
                            var updateEventTriggersForInstruction =
                                sourceActivity.ActivityBehavior is IConditionalEventBehavior ||
                                (updateEventTriggers &&
                                 UpdateEventTriggersValidator.DefinesPersistentEventTrigger(sourceActivity));

                            IValidatingMigrationInstruction generatedInstruction =
                                new ValidatingMigrationInstructionImpl(sourceActivity, targetActivity,
                                    updateEventTriggersForInstruction);
                            generatedInstructions.Add(generatedInstruction);
                        }

            return generatedInstructions;
        }

        public virtual void Generate(ScopeImpl sourceScope, ScopeImpl targetScope,
            ProcessDefinitionImpl sourceProcessDefinition, ProcessDefinitionImpl targetProcessDefinition,
            ValidatingMigrationInstructions existingInstructions, bool updateEventTriggers)
        {
            var flowScopeInstructions =
                GenerateInstructionsForActivities((ICollection<ActivityImpl>) sourceScope.Activities,
                    (ICollection<ActivityImpl>) targetScope.Activities,
                    updateEventTriggers, existingInstructions);

            existingInstructions.AddAll(flowScopeInstructions);

            var eventScopeInstructions = GenerateInstructionsForActivities(sourceScope.EventActivities,
                targetScope.EventActivities, updateEventTriggers, existingInstructions);

            existingInstructions.AddAll(eventScopeInstructions);

            existingInstructions.FilterWith(MigrationInstructionValidatorsRenamed);

            foreach (var generatedInstruction in flowScopeInstructions)
                if (existingInstructions.Contains(generatedInstruction))
                    Generate((ScopeImpl) generatedInstruction.SourceActivity, (ScopeImpl) generatedInstruction.TargetActivity,
                        sourceProcessDefinition, targetProcessDefinition, existingInstructions, updateEventTriggers);
        }

        protected internal virtual bool IsValidActivity(ActivityImpl activity)
        {
            foreach (var migrationActivityValidator in MigrationActivityValidatorsRenamed)
                if (!migrationActivityValidator.Valid(activity))
                    return false;
            return true;
        }
    }
}