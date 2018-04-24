using ESS.FW.Bpm.Engine.Authorization;

namespace ESS.FW.Bpm.Engine.Runtime
{
    /// <summary>
    ///     <para>
    ///         A fluent builder to specify a modification of process instance state in terms
    ///         of cancellation of activity instances and instantiations of activities and sequence flows.
    ///         Allows to specify an ordered set of instructions that are all executed within one
    ///         transaction. Individual instructions are executed in the order of their specification.
    ///     </para>
    ///     
    /// </summary>
    public interface IProcessInstanceModificationBuilder :
        IInstantiationBuilder<IProcessInstanceModificationInstantiationBuilder>
    {
        /// <summary>
        ///     <para>
        ///         <i>Submits the instruction:</i>
        ///     </para>
        ///     <para>
        ///         Start before the specified activity. Instantiate the given activity
        ///         as a descendant of the given ancestor activity instance.
        ///     </para>
        ///     <para>
        ///         In particular:
        ///         <ul>
        ///             <li>Instantiate all activities between the ancestor activity and the activity to execute</li>
        ///             <li>
        ///                 Instantiate and execute the given activity (respects the asyncBefore
        ///                 attribute of the activity)
        ///             </li>
        ///         </ul>
        ///     </para>
        /// </summary>
        /// <param name="activityId"> the activity to instantiate </param>
        /// <param name="ancestorActivityInstanceId">
        ///     the ID of an existing activity instance under which the new
        ///     activity instance should be created
        /// </param>
        IProcessInstanceModificationInstantiationBuilder StartBeforeActivity(string activityId,
            string ancestorActivityInstanceId);

        /// <summary>
        ///     Submits an instruction that behaves like <seealso cref="#startTransition(String,String)" /> and always instantiates
        ///     the single outgoing sequence flow of the given activity. Does not consider asyncAfter.
        /// </summary>
        /// <param name="activityId"> the activity for which the outgoing flow should be executed </param>
        /// <param name="ancestorActivityInstanceId"></param>
        /// <exception cref="ProcessEngineException"> if the activity has 0 or more than 1 outgoing sequence flows </exception>
        IProcessInstanceModificationInstantiationBuilder StartAfterActivity(string activityId,
            string ancestorActivityInstanceId);

        /// <summary>
        ///     <para>
        ///         <i>Submits the instruction:</i>
        ///     </para>
        ///     <para>
        ///         Start the specified sequence flow. Instantiate the given sequence flow
        ///         as a descendant of the given ancestor activity instance.
        ///     </para>
        ///     <para>
        ///         In particular:
        ///         <ul>
        ///             <li>Instantiate all activities between the ancestor activity and the activity to execute</li>
        ///             <li>Execute the given transition (does not consider sequence flow conditions)</li>
        ///         </ul>
        ///     </para>
        /// </summary>
        /// <param name="transitionId"> the sequence flow to execute </param>
        /// <param name="ancestorActivityInstanceId">
        ///     the ID of an existing activity instance under which the new
        ///     transition should be executed
        /// </param>
        IProcessInstanceModificationInstantiationBuilder StartTransition(string transitionId,
            string ancestorActivityInstanceId);

        /// <summary>
        ///     <para>
        ///         <i>Submits the instruction:</i>
        ///     </para>
        ///     <para>
        ///         Cancel an activity instance in a process. If this instance has child activity instances
        ///         (e.g. in a subprocess instance), these children, their grandchildren, etc. are cancelled as well.
        ///     </para>
        /// </summary>
        /// <param name="activityInstanceId"> the id of the activity instance to cancel </param>
        IProcessInstanceModificationBuilder CancelActivityInstance(string activityInstanceId);

        /// <summary>
        ///     <para>
        ///         <i>Submits the instruction:</i>
        ///     </para>
        ///     <para>Cancel a transition instance (i.e. an async continuation) in a process.</para>
        /// </summary>
        /// <param name="transitionInstanceId"> the id of the transition instance to cancel </param>
        IProcessInstanceModificationBuilder CancelTransitionInstance(string transitionInstanceId);

        /// <summary>
        ///     <para>
        ///         <i>Submits the instruction:</i>
        ///     </para>
        ///     <para>
        ///         Cancel all instances of the given activity in an arbitrary order, which are:
        ///         <ul>
        ///             <li>
        ///                 activity instances of that activity
        ///                 <li>transition instances entering or leaving that activity
        ///         </ul>
        ///     </para>
        ///     <para>
        ///         Therefore behaves like <seealso cref="#cancelActivityInstance(String)" /> for each individual
        ///         activity instance and like <seealso cref="#cancelTransitionInstance(String)" /> for each
        ///         individual transition instance.
        ///     </para>
        ///     <para>The cancellation order of the instances is arbitrary</para>
        /// </summary>
        /// <param name="activityId"> the activity for which all instances should be cancelled </param>
        IProcessInstanceModificationBuilder CancelAllForActivity(string activityId);

        /// <summary>
        ///     Execute all instructions. Custom execution and task listeners, as well as task input output mappings
        ///     are executed.
        /// </summary>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#UPDATE" /> permission on
        ///     <seealso cref="Resources#PROCESS_INSTANCE" />
        ///     or no <seealso cref="Permissions#UPDATE_INSTANCE" /> permission on <seealso cref="Resources#PROCESS_DEFINITION" />.
        ///     if the process instance will be delete and the user has no <seealso cref="Permissions#DELETE" /> permission
        ///     on <seealso cref="Resources#PROCESS_INSTANCE" /> or no <seealso cref="Permissions#DELETE_INSTANCE" /> permission on
        ///     <seealso cref="Resources#PROCESS_DEFINITION" />.
        /// </exception>
        void Execute();

        /// <param name="skipCustomListeners">
        ///     specifies whether custom listeners (task and execution)
        ///     should be invoked when executing the instructions
        /// </param>
        /// <param name="skipIoMappings">
        ///     specifies whether input/output mappings for tasks should be invoked
        ///     throughout the transaction when executing the instructions
        /// </param>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#UPDATE" /> permission on
        ///     <seealso cref="Resources#PROCESS_INSTANCE" />
        ///     or no <seealso cref="Permissions#UPDATE_INSTANCE" /> permission on <seealso cref="Resources#PROCESS_DEFINITION" />.
        ///     if the process instance will be delete and the user has no <seealso cref="Permissions#DELETE" /> permission
        ///     on <seealso cref="Resources#PROCESS_INSTANCE" /> or no <seealso cref="Permissions#DELETE_INSTANCE" /> permission on
        ///     <seealso cref="Resources#PROCESS_DEFINITION" />.
        /// </exception>
        void Execute(bool skipCustomListeners, bool skipIoMappings);
    }
}