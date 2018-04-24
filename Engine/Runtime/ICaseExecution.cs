namespace ESS.FW.Bpm.Engine.Runtime
{
    /// <summary>
    ///     <para>Represent a planned item in a case instance.</para>
    ///     <para>Note that a <seealso cref="ICaseInstance" /> also is an case execution.</para>
    ///     
    /// </summary>
    public interface ICaseExecution
    {
        /// <summary>
        ///     <para>The unique identifier of the case execution.</para>
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     <para>Id of the root of the case execution tree representing the case instance.</para>
        ///     <para>It is the same as <seealso cref="#getId()" /> if this case execution is the case instance.</para>
        /// </summary>
        string CaseInstanceId { get; }

        /// <summary>
        ///     <para>The id of the case definition of the case execution.</para>
        /// </summary>
        string CaseDefinitionId { get; }

        /// <summary>
        ///     <para>The id of the activity associated with <code>this</code> case execution.</para>
        /// </summary>
        string ActivityId { get; }

        /// <summary>
        ///     <para>The name of the activity associated with <code>this</code> case execution.</para>
        /// </summary>
        string ActivityName { get; }

        /// <summary>
        ///     <para>The type of the activity associated with <code>this</code> case execution.</para>
        /// </summary>
        string ActivityType { get; }

        /// <summary>
        ///     <para>The description of the activity associated with <code>this</code> case execution.</para>
        /// </summary>
        string ActivityDescription { get; }

        /// <summary>
        ///     <para>The id of the parent of <code>this</code> case execution.</para>
        /// </summary>
        string ParentId { get; }

        /// <summary>
        ///     <para>Returns <code>true</code> if the case execution is required.</para>
        /// </summary>
        bool Required { get; }

        /// <summary>
        ///     <para>Returns <code>true</code> if the case execution is available.</para>
        /// </summary>
        bool Available { get; }

        /// <summary>
        ///     <para>Returns <code>true</code> if the case execution is active.</para>
        /// </summary>
        bool Active { get; }

        /// <summary>
        ///     <para>Returns <code>true</code> if the case execution is enabled.</para>
        ///     <para>
        ///         <strong>Note:</strong> If this case execution is the case execution, it will
        ///         return always <code>false</code>.
        ///     </para>
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        ///     <para>Returns <code>true</code> if the case execution is disabled.</para>
        ///     <para>
        ///         <strong>Note:</strong> If this case execution is the case instance, it will
        ///         return always <code>false</code>.
        ///     </para>
        /// </summary>
        bool Disabled { get; }

        /// <summary>
        ///     <para>Returns <code>true</code> if the case execution is terminated.</para>
        /// </summary>
        bool Terminated { get; }

        /// <summary>
        ///     The id of the tenant this case execution belongs to. Can be <code>null</code>
        ///     if the case execution belongs to no single tenant.
        /// </summary>
        string TenantId { get; }
    }
}