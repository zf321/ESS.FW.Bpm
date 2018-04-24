using System;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Management.Impl;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    public class SuspendJobCmd : AbstractSetJobStateCmd
    {
        public SuspendJobCmd(IUpdateJobSuspensionStateBuilder builder) : base(builder)
        {
        }


        protected internal override string LogEntryOperation => UserOperationLogEntryFields.OperationTypeSuspendJob;

        protected internal override ISuspensionState NewSuspensionState => SuspensionStateFields.Suspended;
    }
}