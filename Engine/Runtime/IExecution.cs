namespace ESS.FW.Bpm.Engine.Runtime
{
    /// <summary>
    ///     Represent a 'path of execution' in a process instance.
    ///     Note that a <seealso cref="IProcessInstance" /> also is an execution.
    ///     
    /// </summary>
    public interface IExecution
    {
        /// <summary>
        ///     The unique identifier of the execution.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     Indicates if the execution is suspended.
        /// </summary>
        bool IsSuspended { get; }

        /// <summary>
        ///     Indicates if the execution is ended.
        /// </summary>
        bool IsEnded { get; }

        /// <summary>
        ///     Id of the root of the execution tree representing the process instance.
        ///     It is the same as <seealso cref="#getId()" /> if this execution is the process instance.
        /// </summary>
        string ProcessInstanceId { get; }

        /// <summary>
        ///     The id of the tenant this execution belongs to. Can be <code>null</code>
        ///     if the execution belongs to no single tenant.
        /// </summary>
        string TenantId { get; }
    }
}