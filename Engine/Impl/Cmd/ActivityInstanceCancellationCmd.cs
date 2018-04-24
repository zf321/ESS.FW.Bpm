using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    public class ActivityInstanceCancellationCmd : AbstractInstanceCancellationCmd
    {
        protected internal string ActivityInstanceId;

        public ActivityInstanceCancellationCmd(string processInstanceId, string activityInstanceId)
            : base(processInstanceId)
        {
            this.ActivityInstanceId = activityInstanceId;
        }
        
        protected internal override ExecutionEntity DetermineSourceInstanceExecution(CommandContext commandContext)
        {
            ExecutionEntity processInstance = commandContext.ExecutionManager.FindExecutionById(processInstanceId);

            // rebuild the mapping because the execution tree changes with every iteration
            var mapping = new ActivityExecutionTreeMapping(commandContext, processInstanceId);

            IActivityInstance instance = commandContext.RunWithoutAuthorization(()=> (new GetActivityInstanceCmd(processInstanceId)).Execute(commandContext));

            IActivityInstance instanceToCancel = FindActivityInstance(instance, ActivityInstanceId);
            EnsureUtil.EnsureNotNull(typeof(NotValidException), DescribeFailure("Activity instance '" + ActivityInstanceId + "' does not exist"), "activityInstance", instanceToCancel);
            ExecutionEntity scopeExecution = GetScopeExecutionForActivityInstance(processInstance, mapping, instanceToCancel);

            return scopeExecution;
            //return new ExecutionEntity();
        }
        

        protected internal override string Describe()
        {
            return "Cancel activity instance '" + ActivityInstanceId + "'";
        }
    }
}