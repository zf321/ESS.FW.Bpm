using System;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Management
{
    /// <summary>
    ///     
    ///     
    /// </summary>
    public interface IMetricsQuery
    {
        /// <seealso cref= constants in
        /// <seealso cref="Metrics" />
        /// for a list of names which can be used here.
        /// </seealso>
        /// <param name="name"> The name of the metrics to query for </param>
        IMetricsQuery Name(string name);

        /// <summary>
        ///     Restrict to data collected by the reported with the given identifier
        /// </summary>
        IMetricsQuery Reporter(string reporter);

        /// <summary>
        ///     Restrict to data collected after the given date (inclusive)
        /// </summary>
        IMetricsQuery StartDate(DateTime startTime);

        /// <summary>
        ///     Restrict to data collected before the given date (exclusive)
        /// </summary>
        IMetricsQuery EndDate(DateTime endTime);


        /// <summary>
        ///     Sets the offset of the returned results.
        /// </summary>
        /// <param name="offset"> indicates after which row the result begins </param>
        /// <returns> the adjusted MetricsQuery </returns>
        IMetricsQuery Offset(int offset);

        /// <summary>
        ///     Sets the limit row count of the result.
        ///     Can't be set larger than 200, since it is the maximum row count which should be returned.
        /// </summary>
        /// <param name="maxResults"> the new row limit of the result </param>
        /// <returns> the adjusted MetricsQuery </returns>
        IMetricsQuery Limit(int maxResults);

        /// <summary>
        ///     Returns the metrics summed up and aggregated on a time interval.
        ///     Default interval is 900 (15 minutes). The list size has a maximum of 200
        ///     the maximum can be decreased with the MetricsQuery#limit method. Paging
        ///     is enabled with the help of the offset.
        /// </summary>
        /// <returns> the aggregated metrics </returns>
        IList<IMetricIntervalValue> Interval();


        /// <summary>
        ///     Returns the metrics summed up and aggregated on a time interval.
        ///     The size of the interval is given via parameter.
        ///     The time unit is seconds! The list size has a maximum of 200
        ///     the maximum can be decreased with the MetricsQuery#limit method. Paging
        ///     is enabled with the help of the offset.
        /// </summary>
        /// <param name="interval">
        ///     The time interval on which the metrics should be aggregated.
        ///     The time unit is seconds.
        /// </param>
        /// <returns> the aggregated metrics </returns>
        IList<IMetricIntervalValue> Interval(long interval);

        /// <returns> the aggregated sum </returns>
        long Sum();
    }
}