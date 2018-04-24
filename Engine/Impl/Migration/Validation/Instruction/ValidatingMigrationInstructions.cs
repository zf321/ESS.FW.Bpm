using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Migration;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instruction
{
    /// <summary>
    ///     
    /// </summary>
    public class ValidatingMigrationInstructions
    {
        protected internal ICollection<IValidatingMigrationInstruction> instructions;
        protected internal IDictionary<IPvmScope, IList<IValidatingMigrationInstruction>> InstructionsBySourceScope;
        protected internal IDictionary<IPvmScope, IList<IValidatingMigrationInstruction>> InstructionsByTargetScope;

        public ValidatingMigrationInstructions(ICollection<IValidatingMigrationInstruction> instructions)
        {
            this.instructions = instructions;
            InstructionsBySourceScope = new Dictionary<IPvmScope, IList<IValidatingMigrationInstruction>>();
            InstructionsByTargetScope = new Dictionary<IPvmScope, IList<IValidatingMigrationInstruction>>();

            foreach (var instruction in instructions)
                IndexInstruction(instruction);
        }

        public ValidatingMigrationInstructions() : this(new HashSet<IValidatingMigrationInstruction>())
        {
        }

        public virtual IList<IValidatingMigrationInstruction> Instructions
        {
            get { return new List<IValidatingMigrationInstruction>(instructions); }
        }

        public virtual void AddInstruction(IValidatingMigrationInstruction instruction)
        {
            instructions.Add(instruction);
            IndexInstruction(instruction);
        }

        public virtual void AddAll(IList<IValidatingMigrationInstruction> instructions)
        {
            foreach (var instruction in instructions)
                AddInstruction(instruction);
        }

        protected internal virtual void IndexInstruction(IValidatingMigrationInstruction instruction)
        {
            CollectionUtil.AddToMapOfLists(InstructionsBySourceScope, instruction.SourceActivity, instruction);
            CollectionUtil.AddToMapOfLists(InstructionsByTargetScope, instruction.TargetActivity, instruction);
        }

        public virtual IList<IValidatingMigrationInstruction> GetInstructionsBySourceScope(IPvmScope scope)
        {
            var instructions = InstructionsBySourceScope[scope];

            if (instructions == null)
                return null;
            return instructions;
        }

        public virtual IList<IValidatingMigrationInstruction> GetInstructionsByTargetScope(IPvmScope scope)
        {
            var instructions = InstructionsByTargetScope[scope];

            if (instructions == null)
                return null;
            return instructions;
        }

        public virtual void FilterWith(IList<IMigrationInstructionValidator> validators)
        {
            IList<IValidatingMigrationInstruction> validInstructions = new List<IValidatingMigrationInstruction>();

            foreach (var instruction in instructions)
                if (IsValidInstruction(instruction, this, validators))
                    validInstructions.Add(instruction);

            InstructionsBySourceScope.Clear();
            InstructionsByTargetScope.Clear();
            instructions.Clear();

            foreach (var validInstruction in validInstructions)
                AddInstruction(validInstruction);
        }

        public virtual IList<IMigrationInstruction> AsMigrationInstructions()
        {
            IList<IMigrationInstruction> instructions = new List<IMigrationInstruction>();

            foreach (var instruction in this.instructions)
                instructions.Add(instruction.ToMigrationInstruction());

            return instructions;
        }

        public virtual bool Contains(IValidatingMigrationInstruction instruction)
        {
            return instructions.Contains(instruction);
        }

        public virtual bool ContainsInstructionForSourceScope(ScopeImpl sourceScope)
        {
            return InstructionsBySourceScope.ContainsKey(sourceScope);
        }

        protected internal virtual bool IsValidInstruction(IValidatingMigrationInstruction instruction,
            ValidatingMigrationInstructions instructions,
            IList<IMigrationInstructionValidator> migrationInstructionValidators)
        {
            return !ValidateInstruction(instruction, instructions, migrationInstructionValidators).HasFailures();
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
    }
}