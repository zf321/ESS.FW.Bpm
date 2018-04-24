using ESS.FW.Bpm.Engine.Authorization;

namespace ESS.FW.Bpm.Engine.Runtime
{
    /// <summary>
    ///     Fluent builder to update the suspension state of process instances.
    /// </summary>
    public interface IUpdateProcessInstanceSuspensionStateBuilder
    {
        /// <summary>
        ///     <para>
        ///         Activates the provided process instances.
        ///     </para>
        ///     <para>
        ///         If you have a process instance hierarchy, activating one process instance
        ///         from the hierarchy will not activate other process instances from that
        ///         hierarchy.
        ///     </para>
        /// </summary>
        /// <exception cref="ProcessEngineException">
        ///     If no such processDefinition can be found.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#UPDATE_INSTANCE" /> permission
        ///     on <seealso cref="Resources#PROCESS_DEFINITION" />.
        /// </exception>
        void Activate();

        /// <summary>
        ///     <para>
        ///         Suspends the provided process instances. This means that the execution is
        ///         stopped, so the <i>token state</i> will not change. However, actions that
        ///         do not change token state, like setting/removing variables, etc. will
        ///         succeed.
        ///     </para>
        ///     <para>
        ///         Tasks belonging to the suspended process instance will also be suspended.
        ///         This means that any actions influencing the tasks' lifecycles will fail,
        ///         such as
        ///         <ul>
        ///             <li>claiming</li>
        ///             <li>completing</li>
        ///             <li>delegation</li>
        ///             <li>changes in task assignees, owners, etc.</li>
        ///         </ul>
        ///         Actions that only change task properties will succeed, such as changing
        ///         variables or adding comments.
        ///     </para>
        ///     <para>
        ///         If a process instance is in state suspended, the engine will also not
        ///         execute jobs (timers, messages) associated with this instance.
        ///     </para>
        ///     <para>
        ///         If you have a process instance hierarchy, suspending one process instance
        ///         from the hierarchy will not suspend other process instances from that
        ///         hierarchy.
        ///     </para>
        /// </summary>
        /// <exception cref="ProcessEngineException">
        ///     If no such processDefinition can be found.
        /// </exception>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#UPDATE_INSTANCE" /> permission
        ///     on <seealso cref="Resources#PROCESS_DEFINITION" />.
        /// </exception>
        void Suspend();

        string ProcessDefinitionKey { get;  }

        string ProcessDefinitionId { get;  }

        string processDefinitionTenantId { get;  }

        bool ProcessDefinitionTenantIdSet { get;  }

        string ProcessInstanceId { get;  }
    }
}