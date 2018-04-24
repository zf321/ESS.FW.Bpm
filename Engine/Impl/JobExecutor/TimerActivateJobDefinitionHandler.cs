using System;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    ///     
    /// </summary>
    public class TimerActivateJobDefinitionHandler : TimerChangeJobDefinitionSuspensionStateJobHandler
    {
        public const string TYPE = "activate-job-definition";

        public override string Type
        {
            get { return TYPE; }
        }

        public override void Execute<T>(T configuration, ExecutionEntity execution, CommandContext commandContext,
            string tenantId)
        {
            throw new NotImplementedException();
        }

        public override void OnDelete<T>(T configuration, JobEntity jobEntity)
        {
            throw new NotImplementedException();
        }

        protected internal override AbstractSetJobDefinitionStateCmd GetCommand(
            JobDefinitionSuspensionStateConfiguration configuration)
        {
            return new ActivateJobDefinitionCmd(configuration.CreateBuilder());
        }
    }
}