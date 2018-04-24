using System;
using System.Collections.Generic;
using System.Transactions;
using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Deploy;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    
    /// <summary>
    /// </summary>
    [Serializable]
    public class DeleteDeploymentCmd : ICommand<object>
    {

        private static readonly TransactionLogger TxLog = ProcessEngineLogger.TxLogger;

        private const long SerialVersionUid = 1L;

        protected internal string DeploymentId;
        protected internal bool Cascade;

        protected internal bool SkipCustomListeners;
        protected internal bool SkipIoMappings;

        public DeleteDeploymentCmd(string deploymentId, bool cascade, bool skipCustomListeners, bool skipIoMappings)
        {
            this.DeploymentId = deploymentId;
            this.Cascade = cascade;
            this.SkipCustomListeners = skipCustomListeners;
            this.SkipIoMappings = skipIoMappings;
        }
        
        public virtual object Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("deploymentId", DeploymentId);

            foreach (ICommandChecker checker in commandContext.ProcessEngineConfiguration.CommandCheckers)
            {
                checker.CheckDeleteDeployment(DeploymentId);
            }

            IUserOperationLogManager logManager = commandContext.OperationLogManager;
            IList<PropertyChange> propertyChanges = new List<PropertyChange>(){new PropertyChange("cascade", null, Cascade)};
            logManager.LogDeploymentOperation(UserOperationLogEntryFields.OperationTypeDelete, DeploymentId, propertyChanges);

            commandContext.DeploymentManager.DeleteDeployment(DeploymentId, Cascade, SkipCustomListeners/*, skipIoMappings*/);

            IProcessApplicationReference processApplicationReference = Context.ProcessEngineConfiguration.ProcessApplicationManager.GetProcessApplicationForDeployment(DeploymentId);

            DeleteDeploymentFailListener listener = new DeleteDeploymentFailListener(DeploymentId, processApplicationReference/*, Context.ProcessEngineConfiguration.CommandExecutorTxRequiresNew*/);

            try
            {
                commandContext.RunWithoutAuthorization(() =>
                {
                    (new UnregisterProcessApplicationCmd(DeploymentId, false)).Execute(commandContext);
                    (new UnregisterDeploymentCmd(new List<string>() { DeploymentId })).Execute(commandContext);
                });
            }
            finally
            {
                try
                {
                    commandContext.TransactionContext.AddTransactionListener(TransactionJavaStatus.RolledBack, listener);
                }
                catch (System.Exception)
                {
                    TxLog.DebugTransactionOperation("Could not register transaction synchronization. Probably the TX has already been rolled back by application code.");
                    listener.Execute(commandContext);
                }
            }


            return null;
        }
        
    }
}
