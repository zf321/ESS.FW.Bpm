namespace ESS.FW.Bpm.Engine.History
{
    /// <summary>
    ///     .
    /// </summary>
    public interface IHistoricTaskInstanceReportResult
    {
        /// <summary>
        ///     <para>Returns the count of the grouped items.</para>
        /// </summary>
        long? Count { get; }

        /// <summary>
        ///     <para>Returns the process definition key for the selected definition key.</para>
        /// </summary>
        string ProcessDefinitionKey { get; }

        /// <summary>
        ///     <para>Returns the process definition id for the selected definition key</para>
        /// </summary>
        string ProcessDefinitionId { get; }

        /// <summary>
        ///     <para></para>
        ///     Returns the process definition name for the selected definition key</p>
        /// </summary>
        string ProcessDefinitionName { get; }

        /// <summary>
        ///     <para>Returns the name of the ITask</para>
        /// </summary>
        /// <returns>
        ///     A ITask name when the query is triggered with a 'countByTaskName'. Else the return
        ///     value is null.
        /// </returns>
        string TaskName { get; }

        /// <summary>
        ///     <para>Returns the id of the tenant ITask</para>
        /// </summary>
        string TenantId { get; }
    }
}