using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Migration;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instruction
{
    public interface IValidatingMigrationInstruction
    {
        IPvmActivity SourceActivity { get; }

        IPvmActivity TargetActivity { get; }

        bool UpdateEventTrigger { get; }

        IMigrationInstruction ToMigrationInstruction();
    }
}