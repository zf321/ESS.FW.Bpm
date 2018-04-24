using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.migration;
using ESS.FW.Bpm.Engine.Impl.migration.validation.instruction;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    /// </summary>
    public class CreateMigrationPlanCmd : ICommand<IMigrationPlan>
    {
        public static readonly MigrationLogger Log = ProcessEngineLogger.MigrationLogger;

        protected internal MigrationPlanBuilderImpl MigrationBuilder;

        public CreateMigrationPlanCmd(MigrationPlanBuilderImpl migrationPlanBuilderImpl)
        {
            MigrationBuilder = migrationPlanBuilderImpl;
        }

        public virtual IMigrationPlan Execute(CommandContext commandContext)
        {
            var sourceProcessDefinition = GetProcessDefinition(commandContext,
                MigrationBuilder.SourceProcessDefinitionId, "Source");
            var targetProcessDefinition = GetProcessDefinition(commandContext,
                MigrationBuilder.TargetProcessDefinitionId, "Target");

            CheckAuthorization(commandContext, sourceProcessDefinition, targetProcessDefinition);

            var migrationPlan = new MigrationPlanImpl(sourceProcessDefinition.Id, targetProcessDefinition.Id);
            IList<IMigrationInstruction> instructions = new List<IMigrationInstruction>();

            if (MigrationBuilder.MapEqualActivitiesRenamed)
            {
                ((List<IMigrationInstruction>)instructions).AddRange(GenerateInstructions(commandContext, sourceProcessDefinition, targetProcessDefinition, MigrationBuilder.UpdateEventTriggersForGeneratedInstructions));
            }

            ((List<IMigrationInstruction>) instructions).AddRange(MigrationBuilder.ExplicitMigrationInstructions);
            migrationPlan.Instructions = instructions;

            ValidateMigrationPlan(commandContext, migrationPlan, (ProcessDefinitionImpl)sourceProcessDefinition, targetProcessDefinition);

            return migrationPlan;
        }

        protected internal virtual ProcessDefinitionEntity GetProcessDefinition(CommandContext commandContext, string id,
            string type)
        {
            EnsureUtil.EnsureNotNull(typeof(BadUserRequestException), type + " process definition id", id);

            try
            {
                return commandContext.ProcessEngineConfiguration.DeploymentCache.FindDeployedProcessDefinitionById(id);
            }
            catch (NullValueException)
            {
                throw Log.ProcessDefinitionDoesNotExist(id, type);
            }
        }

        protected internal virtual void CheckAuthorization(CommandContext commandContext,
            ProcessDefinitionEntity sourceProcessDefinition, ProcessDefinitionEntity targetProcessDefinition)
        {
            foreach (var checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
                checker.CheckCreateMigrationPlan((IProcessDefinition) sourceProcessDefinition,
                    (IProcessDefinition) targetProcessDefinition);
        }

        protected internal virtual IList<IMigrationInstruction> GenerateInstructions(CommandContext commandContext,
            ProcessDefinitionImpl sourceProcessDefinition, ProcessDefinitionImpl targetProcessDefinition,
            bool updateEventTriggers)
        {
            var processEngineConfiguration = commandContext.ProcessEngineConfiguration;

            // generate instructions
            var migrationInstructionGenerator = processEngineConfiguration.MigrationInstructionGenerator;
            var generatedInstructions = migrationInstructionGenerator.Generate(sourceProcessDefinition,
                targetProcessDefinition, updateEventTriggers);

            // filter only valid instructions
            generatedInstructions.FilterWith(processEngineConfiguration.MigrationInstructionValidators);

            return generatedInstructions.AsMigrationInstructions();
        }

        protected internal virtual void ValidateMigrationPlan(CommandContext commandContext,
            MigrationPlanImpl migrationPlan, ProcessDefinitionImpl sourceProcessDefinition,
            ProcessDefinitionImpl targetProcessDefinition)
        {
            var migrationInstructionValidators =
                commandContext.ProcessEngineConfiguration.MigrationInstructionValidators;

            var planReport = new MigrationPlanValidationReportImpl(migrationPlan);
            var validatingMigrationInstructions = WrapMigrationInstructions(migrationPlan, sourceProcessDefinition,
                targetProcessDefinition, planReport);

            foreach (var validatingMigrationInstruction in validatingMigrationInstructions.Instructions)
            {
                var instructionReport = ValidateInstruction(validatingMigrationInstruction,
                    validatingMigrationInstructions, migrationInstructionValidators);
                if (instructionReport.HasFailures())
                    planReport.AddInstructionReport(instructionReport);
            }

            if (planReport.HasInstructionReports())
                throw Log.FailingMigrationPlanValidation(planReport);
        }

        protected internal virtual MigrationInstructionValidationReportImpl ValidateInstruction(
            IValidatingMigrationInstruction instruction, ValidatingMigrationInstructions instructions,
            IList<IMigrationInstructionValidator> migrationInstructionValidators)
        {
            var validationReport = new MigrationInstructionValidationReportImpl(instruction.ToMigrationInstruction());
            foreach (var migrationInstructionValidator in migrationInstructionValidators)
                migrationInstructionValidator.Validate(instruction, instructions, validationReport);
            return validationReport;
        }

        protected internal virtual ValidatingMigrationInstructions WrapMigrationInstructions(
            IMigrationPlan migrationPlan, ProcessDefinitionImpl sourceProcessDefinition,
            ProcessDefinitionImpl targetProcessDefinition, MigrationPlanValidationReportImpl planReport)
        {
            var validatingMigrationInstructions = new ValidatingMigrationInstructions();
            foreach (var migrationInstruction in migrationPlan.Instructions)
            {
                var instructionReport = new MigrationInstructionValidationReportImpl(migrationInstruction);

                var sourceActivityId = migrationInstruction.SourceActivityId;
                var targetActivityId = migrationInstruction.TargetActivityId;
                if (!ReferenceEquals(sourceActivityId, null) && !ReferenceEquals(targetActivityId, null))
                {
                    IPvmActivity sourceActivity = sourceProcessDefinition.FindActivity(sourceActivityId);
                    IPvmActivity targetActivity = targetProcessDefinition.FindActivity(migrationInstruction.TargetActivityId);

                    if (sourceActivity != null && targetActivity != null)
                    {
                        validatingMigrationInstructions.AddInstruction(new ValidatingMigrationInstructionImpl(sourceActivity, targetActivity, migrationInstruction.UpdateEventTrigger));
                    }
                    else
                    {
                        if (sourceActivity == null)
                        {
                            instructionReport.AddFailure("Source activity '" + sourceActivityId + "' does not exist");
                        }
                        if (targetActivity == null)
                        {
                            instructionReport.AddFailure("Target activity '" + targetActivityId + "' does not exist");
                        }
                    }
                }
                else
                {
                    if (ReferenceEquals(sourceActivityId, null))
                        instructionReport.AddFailure("Source activity id is null");
                    if (ReferenceEquals(targetActivityId, null))
                        instructionReport.AddFailure("Target activity id is null");
                }

                if (instructionReport.HasFailures())
                    planReport.AddInstructionReport(instructionReport);
            }
            return validatingMigrationInstructions;
        }
    }
}