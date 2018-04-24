using System;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Management.Impl;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    public class ActivateJobDefinitionCmd : AbstractSetJobDefinitionStateCmd
    {
        public ActivateJobDefinitionCmd(UpdateJobDefinitionSuspensionStateBuilderImpl builder) : base(builder)
        {
        }

        protected internal override ISuspensionState NewSuspensionState
        {
            get
            {
                return SuspensionStateFields.Active;
            }
        }

        protected internal override string DelayedExecutionJobHandlerType
        {
            get { return TimerActivateJobDefinitionHandler.TYPE; }
        }

        protected internal override string LogEntryOperation
        {
            get { return UserOperationLogEntryFields.OperationTypeActivateJobDefinition; }
        }

        protected internal override AbstractSetJobStateCmd GetNextCommand(
            UpdateJobSuspensionStateBuilderImpl jobCommandBuilder)
        {
            return new ActivateJobCmd(jobCommandBuilder);
        }
        
    }
}