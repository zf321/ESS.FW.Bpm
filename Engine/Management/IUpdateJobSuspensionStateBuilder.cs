using ESS.FW.Bpm.Engine.Authorization;

namespace ESS.FW.Bpm.Engine.Management
{
    /// <summary>
    ///     Fluent builder to update the suspension state of jobs.
    /// </summary>
    public interface IUpdateJobSuspensionStateBuilder
    {
        /// <summary>
        ///     Activates the provided jobs.
        /// </summary>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#UPDATE" /> permission on
        ///     <seealso cref="Resources#PROCESS_INSTANCE" /> or no
        ///     <seealso cref="Permissions#UPDATE_INSTANCE" /> permission on
        ///     <seealso cref="Resources#PROCESS_DEFINITION" />.
        /// </exception>
        void Activate();

        /// <summary>
        ///     Suspends the provided jobs. If a job is in state suspended, it will not be
        ///     executed by the job executor.
        /// </summary>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#UPDATE" /> permission on
        ///     <seealso cref="Resources#PROCESS_INSTANCE" /> or no
        ///     <seealso cref="Permissions#UPDATE_INSTANCE" /> permission on
        ///     <seealso cref="Resources#PROCESS_DEFINITION" />.
        /// </exception>
        void Suspend();

         string ProcessDefinitionKey { get;  }

         string ProcessDefinitionId { get;  }

         string processDefinitionTenantId { get;  }

         bool ProcessDefinitionTenantIdSet { get;  }

         string JobId { get;  }

         string JobDefinitionId { get;  }

         string ProcessInstanceId { get;  }
    }
}