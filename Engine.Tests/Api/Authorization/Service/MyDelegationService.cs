using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Identity;

namespace Engine.Tests.Api.Authorization.Service
{
    
    public abstract class MyDelegationService
    {

        public static Authentication CURRENT_AUTHENTICATION;
        public static long? INSTANCES_COUNT;

        // fetch current authentication //////////////////////////////////////////

        public virtual void logAuthentication(IDelegateExecution execution)
        {
            logAuthentication(execution.ProcessEngineServices);
        }

        public virtual void logAuthentication(IDelegateTask task)
        {
            logAuthentication(task.ProcessEngineServices);
        }

        protected internal virtual void logAuthentication(IProcessEngineServices services)
        {
            IIdentityService identityService = services.IdentityService;
            logAuthentication(identityService);
        }

        protected internal virtual void logAuthentication(IIdentityService identityService)
        {
            CURRENT_AUTHENTICATION = identityService.CurrentAuthentication;
        }

        // execute a query /////////////////////////////////////////////////////////

        public virtual void logInstancesCount(IDelegateExecution execution)
        {
            logInstancesCount(execution.ProcessEngineServices);
        }

        public virtual void logInstancesCount(IDelegateTask task)
        {
            logInstancesCount(task.ProcessEngineServices);
        }

        protected internal virtual void logInstancesCount(IProcessEngineServices services)
        {
            IRuntimeService runtimeService = services.RuntimeService;
            logInstancesCount(runtimeService);
        }

        protected internal virtual void logInstancesCount(IRuntimeService runtimeService)
        {
            INSTANCES_COUNT = runtimeService.CreateProcessInstanceQuery().Count();
        }

        // execute a command ///////////////////////////////////////////////////////

        public virtual void executeCommand(IDelegateExecution execution)
        {
            executeCommand(execution.ProcessEngineServices);
        }

        public virtual void executeCommand(IDelegateTask task)
        {
            executeCommand(task.ProcessEngineServices);
        }

        protected internal virtual void executeCommand(IProcessEngineServices services)
        {
            IRuntimeService runtimeService = services.RuntimeService;
            executeCommand(runtimeService);
        }

        protected internal virtual void executeCommand(IRuntimeService runtimeService)
        {
            runtimeService.StartProcessInstanceByKey("process");
        }

        // helper /////////////////////////////////////////////////////////////////

        public static void clearProperties()
        {
            CURRENT_AUTHENTICATION = null;
            INSTANCES_COUNT = null;
        }

    }

}