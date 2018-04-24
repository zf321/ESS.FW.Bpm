using System;
using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.Impl.Core.Instance;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository.Impl;
using ESS.FW.Bpm.Engine.Application.Impl;
using ESS.FW.Bpm.Engine.Impl;

namespace ESS.FW.Bpm.Engine.context.Impl
{
    public class ProcessApplicationContextUtil
    {
        private static readonly ProcessApplicationLogger Log = ProcessEngineLogger.ProcessApplicationLogger;

        public static IProcessApplicationReference GetTargetProcessApplication(CoreExecution execution)
        {
            if (execution is ExecutionEntity)
            {
                return GetTargetProcessApplication((ExecutionEntity)execution);
            }
            throw new NotImplementedException();
            //return GetTargetProcessApplication((CaseExecutionEntity)execution);
        }

        public static IProcessApplicationReference GetTargetProcessApplication(ExecutionEntity execution)
        {
            if (execution == null)
                return null;

            IProcessApplicationReference processApplicationForDeployment =
                GetTargetProcessApplication(execution.GetProcessDefinition());

            //logg application context switch details
            if (Log.ContextSwitchLoggable && processApplicationForDeployment == null)
            {
                //打印 ENGINE-07023 no target process application found for...
                LoggContextSwitchDetails(execution);
            }

            return processApplicationForDeployment;

        }

        //public static IProcessApplicationReference GetTargetProcessApplication(CaseExecutionEntity execution)
        //{
        //    if (execution == null)
        //        return null;

        //    ProcessApplicationReference processApplicationForDeployment =
        //        getTargetProcessApplication((CaseDefinitionEntity)execution.CaseDefinition);

        //    logg application context switch details
        //    if (LOG.ContextSwitchLoggable && processApplicationForDeployment == null)
        //    {
        //        loggContextSwitchDetails(execution);
        //    }

        //    return processApplicationForDeployment;
        //    return null;
        //}

        public static IProcessApplicationReference GetTargetProcessApplication(TaskEntity task)
        {
            if (task.ProcessDefinition != null)
            {
                return GetTargetProcessApplication(task.ProcessDefinition);
            }
            //if (task.CaseDefinition != null)
            //{
            //    return getTargetProcessApplication(task.CaseDefinition);
            //}
            return null;
        }

        public static IProcessApplicationReference GetTargetProcessApplication(IResourceDefinitionEntity definition)
        {
            var reference = GetTargetProcessApplication(definition.DeploymentId);

            if ((reference == null) && AreProcessApplicationsRegistered())
            {
                var previous = definition.PreviousDefinition;

                // do it in a iterative way instead of recursive to avoid
                // a possible StackOverflowException in cases with a lot
                // of versions of a definition
                while (previous != null)
                {
                    reference = GetTargetProcessApplication(previous.DeploymentId);

                    if (reference == null)
                        previous = previous.PreviousDefinition;
                    else
                        return reference;
                }
            }

            return reference;
        }

        public static IProcessApplicationReference GetTargetProcessApplication(string deploymentId)
        {
            var processEngineConfiguration = Context.ProcessEngineConfiguration;
            var processApplicationManager = processEngineConfiguration.ProcessApplicationManager;

            var processApplicationForDeployment =
                processApplicationManager.GetProcessApplicationForDeployment(deploymentId);

            return processApplicationForDeployment;
        }

        public static bool AreProcessApplicationsRegistered()
        {
            var processEngineConfiguration = Context.ProcessEngineConfiguration;
            var processApplicationManager = processEngineConfiguration.ProcessApplicationManager;

            return processApplicationManager.HasRegistrations();
        }

        private static void LoggContextSwitchDetails(ExecutionEntity execution)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final CoreExecutionContext<? extends org.camunda.bpm.engine.impl.core.instance.CoreExecution> executionContext = Context.getCoreExecutionContext();
            //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
            var executionContext = Context.CoreExecutionContext;
            // only log for first atomic op:
            //if (executionContext == null || (executionContext.Execution != execution))
            if (executionContext == null || executionContext.GetExecution<ExecutionEntity>() != execution)
            {
                var processApplicationManager = Context.ProcessEngineConfiguration.ProcessApplicationManager;
                //_logger.DebugNoTargetProcessApplicationFound(execution, processApplicationManager);
                Log.DebugNoTargetProcessApplicationFound(execution, processApplicationManager);
            }
        }

        //private static void loggContextSwitchDetails(CaseExecutionEntity execution)
        //{
        //    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //    //ORIGINAL LINE: final CoreExecutionContext<? extends org.camunda.bpm.engine.impl.core.instance.CoreExecution> executionContext = Context.getCoreExecutionContext();
        //    //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
        //    var executionContext = Context.CoreExecutionContext;
        //    // only log for first atomic op:
        //    //if (executionContext == null || (executionContext.Execution != (CoreExecution)execution))
        //    //{
        //    //    var processApplicationManager = Context.ProcessEngineConfiguration.ProcessApplicationManager;
        //    //    _logger.DebugNoTargetProcessApplicationFoundForCaseExecution(execution, processApplicationManager);
        //    //}
        //}

        public static bool RequiresContextSwitch(IProcessApplicationReference processApplicationReference)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final org.camunda.bpm.application.ProcessApplicationReference currentProcessApplication = Context.getCurrentProcessApplication();
            var currentProcessApplication = Context.CurrentProcessApplication;

            if (processApplicationReference == null)
                return false;

            if (currentProcessApplication == null)
                return true;
            if (!processApplicationReference.Name.Equals(currentProcessApplication.Name))
                return true;
            // check whether the thread context has been manipulated since last context switch. This can happen as a result of
            // an operation causing the container to switch to a different application.
            // Example: JavaDelegate implementation (inside PA) invokes an EJB from different application which in turn interacts with the Process engine.
            //ClassLoader processApplicationClassLoader =
            //    ProcessApplicationClassloaderInterceptor.ProcessApplicationClassLoader;
            //ClassLoader currentClassloader = ClassLoaderUtil.ContextClassloader;
            //return currentClassloader != processApplicationClassLoader;
            return true;
        }
    }
}