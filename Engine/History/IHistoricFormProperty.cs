using System;

namespace ESS.FW.Bpm.Engine.History
{
    /// <summary>
    ///     A single field that was submitted in either a start form or a ITask form.
    ///     This is the audit information that can be used to trace who supplied which
    ///     input for which tasks at what time.
    ///      
    /// </summary>
    [Obsolete]
    public interface IHistoricFormProperty : IHistoricDetail
    {
        /// <summary>
        ///     the id or key of the property
        /// </summary>
        string PropertyId { get; set; }

        /// <summary>
        ///     the submitted value
        /// </summary>
        string PropertyValue { get; set; }
    }
}