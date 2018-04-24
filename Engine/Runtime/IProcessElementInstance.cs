namespace ESS.FW.Bpm.Engine.Runtime
{
    /// <summary>
    ///     <para>
    ///         A ProcessElementInstance is an instance of a process construct
    ///         such as an Activity (see <seealso cref="IActivityInstance" />) or a transition
    ///         (see <seealso cref="ITransitionInstance" />).
    ///         
    ///     </para>
    /// </summary>
    public interface IProcessElementInstance
    {
        /// <summary>
        ///     The id of the process element instance
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     The id of the parent activity instance.
        /// </summary>
        string ParentActivityInstanceId { get; }

        /// <summary>
        ///     the process definition id
        /// </summary>
        string ProcessDefinitionId { get; }

        /// <summary>
        ///     the id of the process instance this process element is part of
        /// </summary>
        string ProcessInstanceId { get; }
    }
}