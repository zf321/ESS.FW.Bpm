using System;
using System.Collections.Generic;
using System.Text;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Application.Impl
{
    /// <summary>
    ///     
    /// </summary>
    public class ProcessApplicationLogger : ProcessEngineLogger
    {
        public virtual bool ContextSwitchLoggable
        {
            get { return DebugEnabled; }
        }

        public virtual void TaskNotRelatedToExecution(IDelegateTask delegateTask)
        {
            LogDebug("001", "Task {0} not related to an execution, target process application cannot be determined.",
                delegateTask);
        }

        public virtual ProcessEngineException ExceptionWhileNotifyingPaTaskListener(System.Exception e)
        {
            return
                new ProcessEngineException(
                    ExceptionMessage("002", "Exception while notifying process application task listener."), e);
        }

        public virtual void NoTargetProcessApplicationForExecution(IDelegateExecution execution)
        {
            LogDebug("003", "No target process application found for execution {0}", execution);
        }

        public virtual void PaDoesNotProvideExecutionListener(string paName)
        {
            LogDebug("004", "Target process application '{0}' does not provide an ExecutionListener.", paName);
        }

        public virtual void CannotInvokeListenerPaUnavailable(string paName, ProcessApplicationUnavailableException e)
        {
            LogDebug("005", "Exception while invoking listener: target process application '{0}' unavailable", paName, e);
        }

        public virtual void PaDoesNotProvideTaskListener(string paName)
        {
            LogDebug("006", "Target process application '{0}' does not provide a TaskListener.", paName);
        }

        public virtual void PaElResolversDiscovered(string summary)
        {
            LogDebug("007", summary);
        }

        public virtual void NoElResolverProvided(string paName, string @string)
        {
            LogWarn("008", "Process Application '{0}': No ELResolver provided by ProcessApplicationElResolver {}", paName,
                @string);
        }

        public virtual ProcessApplicationExecutionException ProcessApplicationExecutionException(System.Exception e)
        {
            return new ProcessApplicationExecutionException(e.Message);
        }

        public virtual ProcessEngineException EjbPaCannotLookupSelfReference(System.Exception e)
        {
            return
                new ProcessEngineException(
                    ExceptionMessage("009", "Cannot lookup self reference to EjbProcessApplication"), e);
        }

        public virtual ProcessEngineException EjbPaCannotAutodetectName(System.Exception e)
        {
            return new ProcessEngineException(
                ExceptionMessage("010", "Could not autodetect EjbProcessApplicationName"), e);
        }

        public virtual ProcessApplicationUnavailableException ProcessApplicationUnavailableException(string name,
            System.Exception cause)
        {
            return
                new ProcessApplicationUnavailableException(
                    ExceptionMessage("011", "Process Application '{0}' unavailable", name), cause);
        }

        public virtual ProcessApplicationUnavailableException ProcessApplicationUnavailableException(string name)
        {
            return
                new ProcessApplicationUnavailableException(ExceptionMessage("011",
                    "Process Application '{0}' unavailable", name));
        }

        public virtual void ServletDeployerNoPaFound(string ctxName)
        {
            LogDebug("012", "Listener invoked for context '{0}' but no process application annotation detected.", ctxName);
        }

        //public virtual ServletException multiplePasException(ISet<Type> c, string appId)
        //{
        //    var builder = new StringBuilder();
        //    builder.Append(
        //        "An application must not contain more than one class annotated with @ProcessApplication.\n Application '");
        //    builder.Append(appId);
        //    builder.Append("' contains the following @ProcessApplication classes:\n");
        //    foreach (var clazz in c)
        //    {
        //        builder.Append("  ");
        //        //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
        //        builder.Append(clazz.FullName);
        //        builder.Append("\n");
        //    }
        //    var msg = builder.ToString();

        //    return new ServletException(ExceptionMessage("013", msg));
        //}

        //public virtual ServletException paWrongTypeException(Type paClass)
        //{
        //    //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
        //    return
        //        new ServletException(ExceptionMessage("014",
        //            "Class '{}' is annotated with @{} but is not a subclass of {}", paClass,
        //            typeof(ProcessApplication).FullName, typeof(AbstractProcessApplication).FullName));
        //}

        public virtual void DetectedPa(Type paClass)
        {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
            LogInfo("015", "Detected @ProcessApplication class '{0}'", paClass.FullName);
        }

        public virtual void AlreadyDeployed()
        {
            LogWarn("016", "Ignoring call of deploy() on process application that is already deployed.");
        }

        public virtual void NotDeployed()
        {
            LogWarn("017", "Calling undeploy() on process application that is not deployed.");
        }

        public virtual void CouldNotRemoveDefinitionsFromCache(System.Exception t)
        {
            LogError("018",
                "Unregistering process application for deployment but could not remove process definitions from deployment cache.",
                t);
        }

        public virtual ProcessEngineException ExceptionWhileRegisteringDeploymentsWithJobExecutor(System.Exception e)
        {
            return
                new ProcessEngineException(
                    ExceptionMessage("019", "Exception while registering deployment with job executor"), e);
        }

        public virtual void ExceptionWhileUnregisteringDeploymentsWithJobExecutor(System.Exception e)
        {
            LogError("020", "Exceptions while unregistering deployments with job executor", e);
        }

        public virtual void RegistrationSummary(string @string)
        {
            LogInfo("021", @string);
        }

        public virtual void ExceptionWhileLoggingRegistrationSummary(System.Exception e)
        {
            LogError("022", "Exception while logging registration summary", e);
        }

        public virtual void DebugNoTargetProcessApplicationFound(ExecutionEntity execution,
            ProcessApplicationManager processApplicationManager)
        {
            LogDebug("023",
                "no target process application found for Execution[{0}], ProcessDefinition[{1}], Deployment[{2}] Registrations[{3}]",
                execution.Id, execution.ProcessDefinitionId, execution.ProcessDefinition.DeploymentId,
                processApplicationManager.RegistrationSummary);
        }

        //public virtual void DebugNoTargetProcessApplicationFoundForCaseExecution(CaseExecutionEntity execution,
        //    ProcessApplicationManager processApplicationManager)
        //{
        //    LogDebug("024",
        //        "no target process application found for CaseExecution[{}], CaseDefinition[{}], Deployment[{}] Registrations[{}]",
        //        execution.Id, execution.GetCaseInstanceId(), ((CaseDefinitionEntity)execution.CaseDefinition).GetDeploymentId(),
        //        processApplicationManager.RegistrationSummary);

        //}
    }
}