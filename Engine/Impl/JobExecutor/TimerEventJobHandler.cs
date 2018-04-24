using System;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    ///     
    /// </summary>
    public abstract class TimerEventJobHandler : IJobHandler<TimerEventJobHandler.TimerJobConfiguration>
    {
        public const string JobHandlerConfigPropertyDelimiter = "$";
        public const string JobHandlerConfigPropertyFollowUpJobCreated = "followUpJobCreated";
        public abstract string Type { get; }

        public virtual IJobHandlerConfiguration NewConfiguration(string canonicalString)
        {
            var configParts = canonicalString.Split("\\" + JobHandlerConfigPropertyDelimiter, true);

            if (configParts.Length > 2)
                throw new ProcessEngineException("Illegal timer job handler configuration: '" + canonicalString +
                                                 "': exprecting a one or two part configuration seperated by '" +
                                                 JobHandlerConfigPropertyDelimiter + "'.");

            var configuration = new TimerJobConfiguration();
            configuration.timerElementKey = configParts[0];

            if (configParts.Length == 2)
                configuration.followUpJobCreated =
                    JobHandlerConfigPropertyFollowUpJobCreated.Equals(configParts[1]);

            return configuration;
        }

        public virtual void OnDelete(IJobHandlerConfiguration configuration, JobEntity jobEntity)
        {
            // do nothing
        }

        public abstract void Execute(IJobHandlerConfiguration configuration, ExecutionEntity execution,
            CommandContext commandContext, string tenantId);

        public abstract void OnDelete<T>(T configuration, JobEntity jobEntity);

        public abstract void Execute<T>(T configuration, ExecutionEntity execution, CommandContext commandContext,
            string tenantId);

        public class TimerJobConfiguration : IJobHandlerConfiguration
        {
            protected internal bool followUpJobCreated;

            protected internal string timerElementKey;

            public virtual string TimerElementKey
            {
                get { return timerElementKey; }
                set { timerElementKey = value; }
            }


            public virtual bool FollowUpJobCreated
            {
                get { return followUpJobCreated; }
                set { followUpJobCreated = value; }
            }


            public virtual string ToCanonicalString()
            {
                var canonicalString = timerElementKey;

                if (followUpJobCreated)
                    canonicalString += JobHandlerConfigPropertyDelimiter +
                                       JobHandlerConfigPropertyFollowUpJobCreated;

                return canonicalString;
            }
        }
    }
}