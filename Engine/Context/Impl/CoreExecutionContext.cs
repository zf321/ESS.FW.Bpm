using ESS.FW.Bpm.Engine.Impl.Core.Instance;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.context.Impl
{

    /// <summary>
    ///     
    ///     
    /// </summary>
    public abstract class CoreExecutionContext/*<T>*/
    {
        //protected internal T execution;
        protected internal CoreExecution execution;

        public CoreExecutionContext(CoreExecution execution)
        {
            this.execution = execution;
        }

        //public virtual T Execution => (T)_execution;

        public virtual T GetExecution<T>() where T:CoreExecution
        {
            return (T)execution;
        }

        protected internal abstract string DeploymentId { get; }

        public virtual DeploymentEntity Deployment => Context.CommandContext.DeploymentManager.FindDeploymentById(DeploymentId);
    }
}