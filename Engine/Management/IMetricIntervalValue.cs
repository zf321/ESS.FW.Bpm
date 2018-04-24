
 

using System;

namespace ESS.FW.Bpm.Engine.Management
{
    /// <summary>
    ///     Represents a metric which contains a name, reporter like the node,
    ///     timestamp and corresponding value.
    ///     
    /// </summary>
    public interface IMetricIntervalValue
    {
        /// <summary>
        ///     Returns the name of the metric.
        /// </summary>
        /// <seealso cref= constants in
        /// <seealso cref="Metrics" />
        /// for a list of names which can be returned here
        /// </seealso>
        /// <returns> the name of the metric </returns>
        string Name { get; }

        /// <summary>
        ///     Returns the reporter name of the metric. Identifies the node which generates this metric.
        /// </summary>
        /// <returns> the reporter name </returns>
        string Reporter { get; }

        /// <summary>
        ///     Returns the timestamp as date object, on which the metric was created.
        /// </summary>
        /// <returns> the timestamp </returns>
        DateTime GetTimestamp();
        /// <summary>
        ///     Returns the value of the metric.
        /// </summary>
        /// <returns> the value </returns>
        long Value { get; }
    }
}