namespace ESS.FW.Bpm.Engine.History
{
    /// <summary>
    ///     <para>Historic form fields</para>
    ///     
    /// </summary>
    public interface IHistoricFormField : IHistoricDetail
    {
        /// <summary>
        ///     the id or key of the property
        /// </summary>
        string FieldId { get; }

        /// <summary>
        ///     the submitted value
        /// </summary>
        object FieldValue { get; }
    }
}