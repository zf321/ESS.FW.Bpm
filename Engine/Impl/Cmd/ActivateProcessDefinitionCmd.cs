using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.runtime;
using ESS.FW.Bpm.Engine.Management.Impl;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository.Impl;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{

    /// <summary>
    ///     
    ///     
    ///     
    /// </summary>
    public class ActivateProcessDefinitionCmd : AbstractSetProcessDefinitionStateCmd
    {
        public ActivateProcessDefinitionCmd(UpdateProcessDefinitionSuspensionStateBuilderImpl builder) : base(builder)
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
            get { return TimerActivateProcessDefinitionHandler.TYPE; }
        }

        protected internal override string LogEntryOperation
        {
            get { return UserOperationLogEntryFields.OperationTypeActivateProcessDefinition; }
        }

        protected internal override AbstractSetJobDefinitionStateCmd GetSetJobDefinitionStateCmd(
            UpdateJobDefinitionSuspensionStateBuilderImpl jobDefinitionSuspensionStateBuilder)
        {
            return new ActivateJobDefinitionCmd(jobDefinitionSuspensionStateBuilder);
        }

        protected internal override AbstractSetProcessInstanceStateCmd GetNextCommand(
            UpdateProcessInstanceSuspensionStateBuilderImpl processInstanceCommandBuilder)
        {
            return new ActivateProcessInstanceCmd(processInstanceCommandBuilder);
        }
    }
}