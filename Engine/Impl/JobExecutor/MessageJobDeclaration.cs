using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    ///     <para>Declaration of a Message Job (Asynchronous continuation job)</para>
    ///     
    /// </summary>
    [Serializable]
    public class MessageJobDeclaration : JobDeclaration<MessageEntity>// JobDeclaration<AtomicOperationInvocation, MessageEntity>
    {
        public const string AsyncBefore = "async-before";
        public const string AsyncAfter = "async-after";

        private const long SerialVersionUid = 1L;

        protected internal string[] OperationIdentifier;

        public MessageJobDeclaration(string[] operationsIdentifier) : base(AsyncContinuationJobHandler.TYPE)
        {
            OperationIdentifier = operationsIdentifier;
        }

        protected internal override MessageEntity NewJobInstance(object context)
        {
            return NewJobInstanceA(context as AtomicOperationInvocation);
        }
        protected  MessageEntity NewJobInstanceA(AtomicOperationInvocation context)
        {
            var message = new MessageEntity();
            message.Execution = context.Execution;

            return message;
        }

        public virtual bool IsApplicableForOperation(IAtomicOperation operation)
        {
            foreach (var identifier in OperationIdentifier)
                if (operation.CanonicalName.Equals(identifier))
                    return true;
            return false;
        }

        protected internal override ExecutionEntity ResolveExecution(object context)
        {
            return (context as AtomicOperationInvocation).Execution;
        }

        protected internal override IJobHandlerConfiguration ResolveJobHandlerConfiguration(
            object context)
        {
            return ResolveJobHandlerConfigurationA(context as AtomicOperationInvocation);
        }
        protected internal  IJobHandlerConfiguration ResolveJobHandlerConfigurationA(
            AtomicOperationInvocation context)
        {
            var configuration = new AsyncContinuationJobHandler.AsyncContinuationConfiguration();

            configuration.AtomicOperation = context.Operation.CanonicalName;

            ExecutionEntity execution = context.Execution;
            IPvmActivity activity = execution.Activity;
            if (activity != null && activity.AsyncAfter)
            {
                if (execution.Transition != null)
                {
                    // store id of selected transition in case this is async after.
                    // id is not serialized with the execution -> we need to remember it as
                    // job handler configuration.
                    configuration.TransitionId = execution.Transition.Id;
                }
            }

            return configuration;
        }


    }
}