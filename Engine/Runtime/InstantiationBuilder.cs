namespace ESS.FW.Bpm.Engine.Runtime
{
    /// <summary>
    ///     
    /// </summary>
    public interface IInstantiationBuilder<T> where T : IInstantiationBuilder<T>
    {
        /// <summary>
        ///     <para>
        ///         <i>Submits the instruction:</i>
        ///     </para>
        ///     <para>Start before the specified activity.</para>
        ///     <para>
        ///         In particular:
        ///         <ul>
        ///             <li>In the parent activity hierarchy, determine the closest existing ancestor activity instance</li>
        ///             <li>Instantiate all parent activities up to the ancestor's activity</li>
        ///             <li>
        ///                 Instantiate and execute the given activity (respects the asyncBefore
        ///                 attribute of the activity)
        ///             </li>
        ///         </ul>
        ///     </para>
        /// </summary>
        /// <param name="activityId"> the activity to instantiate </param>
        /// <exception cref="ProcessEngineException"> if more than one possible ancestor activity instance exists </exception>
        T StartBeforeActivity(string activityId);

        /// <summary>
        ///     Submits an instruction that behaves like <seealso cref="#startTransition(String)" /> and always instantiates
        ///     the single outgoing sequence flow of the given activity. Does not consider asyncAfter.
        /// </summary>
        /// <param name="activityId"> the activity for which the outgoing flow should be executed </param>
        /// <exception cref="ProcessEngineException"> if the activity has 0 or more than 1 outgoing sequence flows </exception>
        T StartAfterActivity(string activityId);

        /// <summary>
        ///     <para>
        ///         <i>Submits the instruction:</i>
        ///     </para>
        ///     <para>Start a sequence flow.</para>
        ///     <para>
        ///         In particular:
        ///         <ul>
        ///             <li>In the parent activity hierarchy, determine the closest existing ancestor activity instance</li>
        ///             <li>Instantiate all parent activities up to the ancestor's activity</li>
        ///             <li>Execute the given transition (does not consider sequence flow conditions)</li>
        ///         </ul>
        ///     </para>
        /// </summary>
        /// <param name="transitionId"> the sequence flow to execute </param>
        /// <exception cref="ProcessEngineException"> if more than one possible ancestor activity instance exists </exception>
        T StartTransition(string transitionId);
    }
}