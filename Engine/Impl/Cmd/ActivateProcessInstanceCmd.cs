using System;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.runtime;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Management.Impl;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{

    /// <summary>
    ///     
    /// </summary>
    public class ActivateProcessInstanceCmd : AbstractSetProcessInstanceStateCmd
    {
        public ActivateProcessInstanceCmd(UpdateProcessInstanceSuspensionStateBuilderImpl builder) : base(builder)
        {
        }

        protected internal override string LogEntryOperation
        {
            get { return UserOperationLogEntryFields.OperationTypeActivate; }
        }

        protected internal override ISuspensionState NewSuspensionState
        {
            get
            {
                return SuspensionStateFields.Active;
            }
        }

        protected internal override AbstractSetJobStateCmd GetNextCommand(
            IUpdateJobSuspensionStateBuilder jobCommandBuilder)
        {
            return new ActivateJobCmd(jobCommandBuilder);
        }
        
    }
}