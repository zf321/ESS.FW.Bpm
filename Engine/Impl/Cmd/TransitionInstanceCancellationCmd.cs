using System;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    /// </summary>
    public class TransitionInstanceCancellationCmd : AbstractInstanceCancellationCmd
    {
        protected internal string TransitionInstanceId;

        public TransitionInstanceCancellationCmd(string processInstanceId, string transitionInstanceId)
            : base(processInstanceId)
        {
            this.TransitionInstanceId = transitionInstanceId;
        }
        
        protected internal override ExecutionEntity DetermineSourceInstanceExecution(CommandContext commandContext)
        {
            IActivityInstance instance =
                commandContext.RunWithoutAuthorization(()=> (new GetActivityInstanceCmd(processInstanceId)).Execute(commandContext));
            var instanceToCancel = FindTransitionInstance(instance, TransitionInstanceId);
            EnsureUtil.EnsureNotNull(typeof(NotValidException),
                DescribeFailure("Transition instance '" + TransitionInstanceId + "' does not exist"),
                "transitionInstance", instanceToCancel);

            ExecutionEntity transitionExecution =
                commandContext.ExecutionManager.FindExecutionById(instanceToCancel.ExecutionId);

            return transitionExecution;
        }

        protected internal override string Describe()
        {
            return "Cancel transition instance '" + TransitionInstanceId + "'";
        }
        
    }
}