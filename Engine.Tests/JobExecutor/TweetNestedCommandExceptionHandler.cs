using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace Engine.Tests.JobExecutor
{
    /// <summary>
    ///     Throws an exception from a nested command; Unlike <seealso cref="TweetExceptionHandler" />, this handler always
    ///     throws exceptions.
    /// </summary>
    public class TweetNestedCommandExceptionHandler : IJobHandler<IJobHandlerConfiguration>
    {
        public const string TYPE = "tweet-exception-nested";

        public virtual string Type
        {
            get { return TYPE; }
        }

        public virtual void Execute(IJobHandlerConfiguration configuration, ExecutionEntity execution,
            CommandContext commandContext, string tenantId)
        {
            Context.ProcessEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this,
                commandContext));
        }

        public IJobHandlerConfiguration NewConfiguration(string canonicalString)
        {
            return new JobHandlerConfigurationAnonymousInnerClass(this);
        }

        public virtual void OnDelete(IJobHandlerConfiguration configuration, JobEntity jobEntity)
        {
            // do nothing
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly TweetNestedCommandExceptionHandler _outerInstance;

            private CommandContext _commandContext;

            public CommandAnonymousInnerClass(TweetNestedCommandExceptionHandler outerInstance,
                CommandContext commandContext)
            {
                _outerInstance = outerInstance;
                _commandContext = commandContext;
            }


            public virtual object Execute(CommandContext commandContext)
            {
                throw new System.Exception("nested command exception");
            }
        }

        private class JobHandlerConfigurationAnonymousInnerClass : IJobHandlerConfiguration
        {
            private readonly TweetNestedCommandExceptionHandler _outerInstance;

            public JobHandlerConfigurationAnonymousInnerClass(TweetNestedCommandExceptionHandler outerInstance)
            {
                _outerInstance = outerInstance;
            }

            public string ToCanonicalString()
            {
                return null;
            }
        }
    }
}