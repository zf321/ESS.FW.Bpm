using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    public class TweetHandler : IJobHandler<TweetHandler.TweetJobConfiguration>
    {
        internal IList<string> messages = new List<string>();

        public virtual IList<string> Messages
        {
            get { return messages; }
        }

        public virtual string Type
        {
            get { return "tweet"; }
        }

        public virtual void Execute(IJobHandlerConfiguration configuration, ExecutionEntity execution,
            CommandContext commandContext, string tenantId)
        {
            messages.Add(((TweetJobConfiguration)configuration).Message);
            Assert.NotNull(commandContext);
        }

        public IJobHandlerConfiguration NewConfiguration(string canonicalString)
        {
            var config = new TweetJobConfiguration();
            config.message = canonicalString;

            return config;
        }

        public virtual void OnDelete(IJobHandlerConfiguration configuration, JobEntity jobEntity)
        {
            // do nothing
        }

        public class TweetJobConfiguration : IJobHandlerConfiguration
        {
            protected internal string message;

            public virtual string Message
            {
                get { return message; }
            }

            public virtual string ToCanonicalString()
            {
                return message;
            }
        }
    }
}