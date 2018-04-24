using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Management.Impl
{
    /// <summary>
    ///     Represents an interface for the purge reporting.
    ///     Contains all information of the data which is deleted during the purge.
    /// </summary>
    public interface IPurgeReporting<T>
    {
        /// <summary>
        ///     Returns the current purge report.
        /// </summary>
        /// <returns> the purge report </returns>
        IDictionary<string, T> PurgeReport { get; }

        /// <summary>
        ///     Transforms and returns the purge report to a string.
        /// </summary>
        /// <returns> the purge report as string </returns>
        string PurgeReportAsString { get; }

        /// <summary>
        ///     Returns true if the report is empty.
        /// </summary>
        /// <returns> true if the report is empty, false otherwise </returns>
        bool Empty { get; }

        /// <summary>
        ///     Adds the key value pair as report information to the current purge report.
        /// </summary>
        /// <param name="key"> the report key </param>
        /// <param name="value"> the report value </param>
        void AddPurgeInformation(string key, T value);

        /// <summary>
        ///     Returns the value for the given key.
        /// </summary>
        /// <param name="key"> the key which exist in the current report </param>
        /// <returns> the corresponding value </returns>
        T GetReportValue(string key);

        /// <summary>
        ///     Returns true if the key is present in the current report.
        /// </summary>
        /// <param name="key"> the key </param>
        /// <returns> true if the key is present </returns>
        bool ContainsReport(string key);
    }
}