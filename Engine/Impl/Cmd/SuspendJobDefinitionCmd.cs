using System;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Management.Impl;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    public class SuspendJobDefinitionCmd : AbstractSetJobDefinitionStateCmd
    {
        public SuspendJobDefinitionCmd(IUpdateJobDefinitionSuspensionStateBuilder builder) : base(builder)
        {
        }

        protected internal override ISuspensionState NewSuspensionState => SuspensionStateFields.Suspended;

        protected internal override string DelayedExecutionJobHandlerType => TimerSuspendJobDefinitionHandler.TYPE;

        protected internal override string LogEntryOperation => UserOperationLogEntryFields.OperationTypeSuspendJobDefinition;


        protected internal override AbstractSetJobStateCmd GetNextCommand(
            UpdateJobSuspensionStateBuilderImpl jobCommandBuilder)
        {
            return new SuspendJobCmd(jobCommandBuilder);
        }
        
    }
}