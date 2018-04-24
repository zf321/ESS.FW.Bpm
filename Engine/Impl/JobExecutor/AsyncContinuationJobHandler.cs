using System.Collections.Generic;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime.Operation;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    
    /// <summary>
    ///     
    ///     
    /// </summary>
    public class AsyncContinuationJobHandler : IJobHandler<AsyncContinuationJobHandler.AsyncContinuationConfiguration>
    {
        public const string TYPE = "async-continuation";

        private readonly IDictionary<string, IPvmAtomicOperation> _supportedOperations;

        public AsyncContinuationJobHandler()
        {
            _supportedOperations = new Dictionary<string, IPvmAtomicOperation>();
            // async before activity
            _supportedOperations[PvmAtomicOperationFields.TransitionCreateScope.CanonicalName] =
                PvmAtomicOperationFields.TransitionCreateScope;
            _supportedOperations[PvmAtomicOperationFields.ActivityStartCreateScope.CanonicalName] =
                PvmAtomicOperationFields.ActivityStartCreateScope;
            // async before start event
            _supportedOperations[PvmAtomicOperationFields.ProcessStart.CanonicalName] =
                PvmAtomicOperationFields.ProcessStart;

            // async after activity depending if an outgoing sequence flow exists
            _supportedOperations[PvmAtomicOperationFields.TransitionNotifyListenerTake.CanonicalName] =
                PvmAtomicOperationFields.TransitionNotifyListenerTake;
            _supportedOperations[PvmAtomicOperationFields.ActivityEnd.CanonicalName] =
                PvmAtomicOperationFields.ActivityEnd;
        }

        public virtual string Type
        {
            get { return TYPE; }
        }

        public virtual IJobHandlerConfiguration NewConfiguration(string canonicalString)
        {
            var configParts = TokenizeJobConfiguration(canonicalString);

            var configuration = new AsyncContinuationConfiguration();

            configuration.AtomicOperation = configParts[0];
            configuration.TransitionId = configParts[1];

            return configuration;
        }

        public virtual void Execute(IJobHandlerConfiguration configuration, ExecutionEntity execution,
            CommandContext commandContext, string tenantId)
        {
            var config = (AsyncContinuationConfiguration) configuration;
            LegacyBehavior.RepairMultiInstanceAsyncJob(execution);

            var atomicOperation = FindMatchingAtomicOperation(config.AtomicOperation);
            EnsureUtil.EnsureNotNull("Cannot process job with configuration " + configuration, "atomicOperation",
                atomicOperation);

            // reset transition id.
            var transitionId = config.TransitionId;
            if (!ReferenceEquals(transitionId, null))
            {
                IPvmActivity activity = execution.GetActivity();
                var transition = (TransitionImpl)activity.FindOutgoingTransition(transitionId);
                execution.Transition = transition;
            }

            Context.CommandInvocationContext.PerformOperation(atomicOperation, execution);
        }

        public virtual void OnDelete(IJobHandlerConfiguration configuration, JobEntity jobEntity)
        {
            // do nothing
        }

        public virtual IPvmAtomicOperation FindMatchingAtomicOperation(string operationName)
        {
            if (ReferenceEquals(operationName, null))
                return PvmAtomicOperationFields.TransitionCreateScope;
            return _supportedOperations[operationName];
        }

        protected internal virtual bool IsSupported(IPvmAtomicOperation atomicOperation)
        {
            return _supportedOperations.ContainsKey(atomicOperation.CanonicalName);
        }

        /// <returns>
        ///     an array of length two with the following contents:
        ///     <ul>
        ///         <li>
        ///             First element: pvm atomic operation name
        ///             <li>Second element: transition id (may be null)
        /// </returns>
        protected internal virtual string[] TokenizeJobConfiguration(string jobConfiguration)
        {
            var configuration = new string[2];

            if (!ReferenceEquals(jobConfiguration, null))
            {
                var configParts = jobConfiguration.Split("\\$", true);
                if (configuration.Length > 2)
                    throw new ProcessEngineException("Illegal async continuation job handler configuration: '" +
                                                     jobConfiguration +
                                                     "': exprecting one part or two parts seperated by '$'.");
                configuration[0] = configParts[0];
                if (configParts.Length == 2)
                    configuration[1] = configParts[1];
            }

            return configuration;
        }

        public class AsyncContinuationConfiguration : IJobHandlerConfiguration
        {
            protected internal string atomicOperation;
            protected internal string transitionId;

            public virtual string AtomicOperation
            {
                get { return atomicOperation; }
                set { atomicOperation = value; }
            }


            public virtual string TransitionId
            {
                get { return transitionId; }
                set { transitionId = value; }
            }


            public virtual string ToCanonicalString()
            {
                var configuration = atomicOperation;

                if (!ReferenceEquals(transitionId, null))
                    configuration += "$" + transitionId;

                return configuration;
            }
        }
    }
}