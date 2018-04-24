using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.runtime;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Management.Impl;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    public class SuspendProcessInstanceCmd : AbstractSetProcessInstanceStateCmd
    {
        public SuspendProcessInstanceCmd(IUpdateProcessInstanceSuspensionStateBuilder builder) : base(builder)
        {
        }

        protected internal override ISuspensionState NewSuspensionState => SuspensionStateFields.Suspended;

        protected internal override AbstractSetJobStateCmd GetNextCommand(IUpdateJobSuspensionStateBuilder jobCommandBuilder)
        {
            return new SuspendJobCmd(jobCommandBuilder);
        }
        
        protected internal override string LogEntryOperation => UserOperationLogEntryFields.OperationTypeSuspend;
    }
}