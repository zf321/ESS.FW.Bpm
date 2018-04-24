using System.Collections.Generic;
using System.Text;
using ESS.FW.Bpm.Engine.Impl.migration;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Repository;
using NUnit.Framework;

namespace Engine.Tests.Util
{
    public class MigrationPlanAssert
    {
        protected internal IMigrationPlan Actual;

        public MigrationPlanAssert(IMigrationPlan actual)
        {
            Actual = actual;
        }

        public virtual MigrationPlanAssert NotNull
        {
            get
            {
                Assert.NotNull(Actual, "The migration plan is null");

                return this;
            }
        }

        public virtual MigrationPlanAssert HasSourceProcessDefinition(IProcessDefinition sourceProcessDefinition)
        {
            return HasSourceProcessDefinitionId(sourceProcessDefinition.Id);
        }

        public virtual MigrationPlanAssert HasSourceProcessDefinitionId(string sourceProcessDefinitionId)
        {
            var migrationPlanAssert = NotNull;
            Assert.AreEqual("The source process definition id does not match", sourceProcessDefinitionId,
                Actual.SourceProcessDefinitionId);

            return this;
        }

        public virtual MigrationPlanAssert HasTargetProcessDefinition(IProcessDefinition targetProcessDefinition)
        {
            return HasTargetProcessDefinitionId(targetProcessDefinition.Id);
        }

        public virtual MigrationPlanAssert HasTargetProcessDefinitionId(string targetProcessDefinitionId)
        {
            var migrationPlanAssert = NotNull;
            Assert.AreEqual("The target process definition id does not match", targetProcessDefinitionId,
                Actual.TargetProcessDefinitionId);

            return this;
        }

        public virtual MigrationPlanAssert HasInstructions(params MigrationInstructionAssert[] instructionAsserts)
        {
            var migrationPlanAssert = NotNull;

            IList<IMigrationInstruction> notExpected = new List<IMigrationInstruction>(Actual.Instructions);
            IList<MigrationInstructionAssert> notFound = new List<MigrationInstructionAssert>();
            foreach (var migrationInstructionAssert in instructionAsserts)
            {
                notFound.Add(migrationInstructionAssert);
            }
            foreach (var instructionAssert in instructionAsserts)
                foreach (var instruction in Actual.Instructions)
                    if (instructionAssert.SourceActivityId.Equals(instruction.SourceActivityId))
                    {
                        notFound.Remove(instructionAssert);
                        notExpected.Remove(instruction);
                        Assert.AreEqual("Target activity ids do not match for instruction " + instruction,
                            instructionAssert.TargetActivityId, instruction.TargetActivityId);
                        if (instructionAssert.UpdateEventTriggerRenamed != null)
                            Assert.AreEqual(instructionAssert.UpdateEventTriggerRenamed, instruction.UpdateEventTrigger,
                                "Expected instruction to update event trigger: " +
                                instructionAssert.UpdateEventTriggerRenamed + " but is: " +
                                instruction.UpdateEventTrigger);
                    }

            if ((notExpected.Count > 0) || (notFound.Count > 0))
            {
                var builder = new StringBuilder();
                builder.Append("\nActual migration instructions:\n\t").Append(Actual.Instructions).Append("\n");
                if (notExpected.Count > 0)
                    builder.Append("Unexpected migration instructions:\n\t").Append(notExpected).Append("\n");
                if (notFound.Count > 0)
                    builder.Append("Migration instructions missing:\n\t").Append(notFound);
                Assert.Fail(builder.ToString());
            }

            return this;
        }

        public virtual MigrationPlanAssert HasEmptyInstructions()
        {
            var migrationPlanAssert = NotNull;

            var instructions = Actual.Instructions;
            Assert.True(instructions.Count == 0,"Expected migration plan has no instructions but has: " + instructions);

            return this;
        }

        public static MigrationPlanAssert That(IMigrationPlan migrationPlan)
        {
            return new MigrationPlanAssert(migrationPlan);
        }

        public static MigrationInstructionAssert Migrate(string sourceActivityId)
        {
            return new MigrationInstructionAssert().From(sourceActivityId);
        }

        public class MigrationInstructionAssert
        {
            protected internal string SourceActivityId;
            protected internal string TargetActivityId;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
            protected internal bool? UpdateEventTriggerRenamed;

            public virtual MigrationInstructionAssert From(string sourceActivityId)
            {
                SourceActivityId = sourceActivityId;
                return this;
            }

            public virtual MigrationInstructionAssert To(string targetActivityId)
            {
                TargetActivityId = targetActivityId;
                return this;
            }

            public virtual MigrationInstructionAssert UpdateEventTrigger(bool updateEventTrigger)
            {
                UpdateEventTriggerRenamed = updateEventTrigger;
                return this;
            }

            public override string ToString()
            {
                return new MigrationInstructionImpl(SourceActivityId, TargetActivityId).ToString();
            }
        }
    }
}