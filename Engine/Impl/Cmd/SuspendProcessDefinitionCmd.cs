using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.runtime;
using ESS.FW.Bpm.Engine.Management.Impl;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository.Impl;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///   暂停
    /// </summary>
    public class SuspendProcessDefinitionCmd : AbstractSetProcessDefinitionStateCmd
    {
        public SuspendProcessDefinitionCmd(UpdateProcessDefinitionSuspensionStateBuilderImpl builder) : base(builder)
        {
        }

        protected internal override ISuspensionState NewSuspensionState
        {
            get { return SuspensionStateFields.Suspended; }
        }

        protected internal override string DelayedExecutionJobHandlerType
        {
            get { return TimerSuspendProcessDefinitionHandler.TYPE; }
        }

        protected internal override string LogEntryOperation
        {
            get { return UserOperationLogEntryFields.OperationTypeSuspendProcessDefinition; }
        }

        protected internal override AbstractSetJobDefinitionStateCmd GetSetJobDefinitionStateCmd(
            UpdateJobDefinitionSuspensionStateBuilderImpl jobDefinitionSuspensionStateBuilder)
        {
            return new SuspendJobDefinitionCmd(jobDefinitionSuspensionStateBuilder);
        }

        protected internal override AbstractSetProcessInstanceStateCmd GetNextCommand(
            UpdateProcessInstanceSuspensionStateBuilderImpl processInstanceCommandBuilder)
        {
            return new SuspendProcessInstanceCmd(processInstanceCommandBuilder);
        }
    }
}