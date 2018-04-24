using System;
using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.Application.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.context.Impl;

namespace ESS.FW.Bpm.Engine.Impl.Interceptor
{
    /// <summary>
    ///     
    /// </summary>
    public class ProcessApplicationContextInterceptor : CommandInterceptor
    {
        private static readonly CommandLogger Log = ProcessEngineLogger.CmdLogger;

        protected internal ProcessEngineConfigurationImpl ProcessEngineConfiguration;

        public ProcessApplicationContextInterceptor(ProcessEngineConfigurationImpl processEngineConfiguration)
        {
            this.ProcessEngineConfiguration = processEngineConfiguration;
        }
        
        public override T Execute<T>(ICommand<T> command)
        {
            var processApplicationIdentifier = ProcessApplicationContextImpl.Get();

            if (processApplicationIdentifier != null)
            {
                // clear the identifier so this interceptor does not apply to nested commands
                ProcessApplicationContextImpl.Clear();

                try
                {
                    var reference = GetPaReference(processApplicationIdentifier);
                    var cmdObj = command as ICommand<object>;
                    return (T) Context.ExecuteWithinProcessApplication<object>(() => { return Next.Execute(cmdObj); },    reference);
                }
                finally
                {
                    // restore the identifier for subsequent commands
                    ProcessApplicationContextImpl.Set(processApplicationIdentifier);
                }
            }
            return Next.Execute(command);
        }

        protected internal virtual IProcessApplicationReference GetPaReference(
            ProcessApplicationIdentifier processApplicationIdentifier)
        {
            if (processApplicationIdentifier.Reference != null)
                return processApplicationIdentifier.Reference;
            //if (processApplicationIdentifier.ProcessApplication != null)
            //    return processApplicationIdentifier.ProcessApplication.Reference;
            if (processApplicationIdentifier.Name!= null)
            {
                //var runtimeContainerDelegate = RuntimeContainerDelegateFields.INSTANCE.get();
                //var reference = runtimeContainerDelegate.getDeployedProcessApplication(processApplicationIdentifier.Name);

                //if (reference == null)
                //    throw Log.PaWithNameNotRegistered(processApplicationIdentifier.Name);
                //return reference;
                throw new NotImplementedException();
            }
            throw Log.CannotReolvePa(processApplicationIdentifier);
        }
        
    }
}