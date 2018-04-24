using System;
using ESS.FW.Bpm.Engine.Impl.Util;
using NUnit.Framework;

namespace Engine.Tests.Api.Mgmt.Metrics
{
    /// <summary>
    ///     Represents a test suite for the metrics interval query to check if the
    ///     timestamps are read in a correct time zone.
    ///     This was a problem before the column MILLISECONDS_ was added.
    /// </summary>
    [TestFixture]
    public class MetricsIntervalTimezoneTest : AbstractMetricsIntervalTest
    {
        [Test]
        public virtual void testTimestampIsInCorrectTimezone()
        {
            //given generated metric data started at DEFAULT_INTERVAL ends at 3 * DEFAULT_INTERVAL

            //when metric query is executed (hint last interval is returned as first)
            var metrics = managementService.CreateMetricsQuery()
                .Limit(1)
                .Interval();

            //then metric interval time should be less than FIRST_INTERVAL + 3 * DEFAULT_INTERVAL
            var metricIntervalTime = metrics[0].GetTimestamp()
                .TimeOfDay.Ticks;
            Assert.True(metricIntervalTime < firstInterval.AddMinutes(3 * DEFAULT_INTERVAL)
                            .Millisecond);
            //and larger than first interval time, if not than we have a timezone problem
            Assert.True(metricIntervalTime > firstInterval.Millisecond);

            //when current time is used and metric is reported
            var currentTime = DateTime.Now;
            var metricsRegistry = processEngineConfiguration.MetricsRegistry;
            ClockUtil.CurrentTime = currentTime;
            metricsRegistry.MarkOccurrence(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart, 1);
            processEngineConfiguration.DbMetricsReporter.ReportNow();

            //then current time should be larger than metric interval time
            var m2 = managementService.CreateMetricsQuery()
                .Limit(1)
                .Interval();
            Assert.True(m2[0].GetTimestamp()
                            .TimeOfDay.Ticks < currentTime.Ticks);
        }
    }
}