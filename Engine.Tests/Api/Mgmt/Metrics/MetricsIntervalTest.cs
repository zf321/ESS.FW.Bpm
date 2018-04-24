using System;
using ESS.FW.Bpm.Engine.Impl.Metrics;
using ESS.FW.Bpm.Engine.Impl.Util;
using NUnit.Framework;

namespace Engine.Tests.Api.Mgmt.Metrics
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MetricsIntervalTest : AbstractMetricsIntervalTest
    {
        // OFFSET //////////////////////////////////////////////////////////////////////

        [Test]
        public virtual void testMeterQueryOffset()
        {
            //given metric data

            //when query metric interval data with offset of metrics Count
            var metrics = managementService.CreateMetricsQuery()
                .Offset(metricsCount)
                .Interval();

            //then 2 * metricsCount values are returned and highest interval is second last interval, since first 9 was skipped
            Assert.AreEqual(2 * metricsCount, metrics.Count);
            Assert.AreEqual(firstInterval.AddMinutes(DEFAULT_INTERVAL)
                .Minute, metrics[0].GetTimestamp()
                .TimeOfDay.Ticks);
        }

        [Test]
        public virtual void testMeterQueryMaxOffset()
        {
            //given metric data

            //when query metric interval data with max offset
            var metrics = managementService.CreateMetricsQuery()
                .Offset(int.MaxValue)
                .Interval();

            //then 0 values are returned
            Assert.AreEqual(0, metrics.Count);
        }

        // INTERVAL //////////////////////////////////////////////////////////////////////

        [Test]
        public virtual void testMeterQueryDefaultInterval()
        {
            //given metric data

            //when query metric interval data with default values
            var metrics = managementService.CreateMetricsQuery()
                .Interval();

            //then default interval is 900 s (15 minutes)
            var lastTimestamp = metrics[0].GetTimestamp()
                .TimeOfDay.Ticks;
            metrics.RemoveAt(0);
            foreach (var metric in metrics)
            {
                var nextTimestamp = metric.GetTimestamp()
                    .TimeOfDay.Ticks;
                if (lastTimestamp != nextTimestamp)
                {
                    Assert.AreEqual(lastTimestamp, nextTimestamp + DEFAULT_INTERVAL_MILLIS);
                    lastTimestamp = nextTimestamp;
                }
            }
        }

        [Test]
        public virtual void testMeterQueryCustomInterval()
        {
            //given metric data

            //when query metric interval data with custom time interval
            var metrics = managementService.CreateMetricsQuery()
                .Interval(300);

            //then custom interval is 300 s (5 minutes)
            var interval = 5 * 60 * 1000;
            var lastTimestamp = metrics[0].GetTimestamp()
                .TimeOfDay.Ticks;
            metrics.RemoveAt(0);
            foreach (var metric in metrics)
            {
                var nextTimestamp = metric.GetTimestamp()
                    .TimeOfDay.Ticks;
                if (lastTimestamp != nextTimestamp)
                {
                    Assert.AreEqual(lastTimestamp, nextTimestamp + interval);
                    lastTimestamp = nextTimestamp;
                }
            }
        }

        // WHERE REPORTER //////////////////////////////////////////////////////////////////////

        [Test]
        public virtual void testMeterQueryDefaultIntervalWhereReporter()
        {
            //given metric data

            //when query metric interval data with reporter in where clause
            var metrics = managementService.CreateMetricsQuery()
                .Reporter(REPORTER_ID)
                .Interval();

            //then result contains only metrics from given reporter, since it is the default it contains all
            Assert.AreEqual(3 * metricsCount, metrics.Count);
            var lastTimestamp = metrics[0].GetTimestamp()
                .TimeOfDay.Ticks;
            var reporter = metrics[0].Reporter;
            metrics.RemoveAt(0);
            foreach (var metric in metrics)
            {
                Assert.AreEqual(reporter, metric.Reporter);
                var nextTimestamp = metric.GetTimestamp()
                    .TimeOfDay.Ticks;
                if (lastTimestamp != nextTimestamp)
                {
                    Assert.AreEqual(lastTimestamp, nextTimestamp + DEFAULT_INTERVAL_MILLIS);
                    lastTimestamp = nextTimestamp;
                }
            }
        }

        [Test]
        public virtual void testMeterQueryDefaultIntervalWhereReporterNotExist()
        {
            //given metric data

            //when query metric interval data with not existing reporter in where clause
            var metrics = managementService.CreateMetricsQuery()
                .Reporter("notExist")
                .Interval();

            //then result contains no metrics from given reporter
            Assert.AreEqual(0, metrics.Count);
        }

        [Test]
        public virtual void testMeterQueryCustomIntervalWhereReporter()
        {
            //given metric data and custom interval
            var interval = 5 * 60;

            //when query metric interval data with custom interval and reporter in where clause
            var metrics = managementService.CreateMetricsQuery()
                .Reporter(REPORTER_ID)
                .Interval(interval);

            //then result contains only metrics from given reporter, since it is the default it contains all
            Assert.AreEqual(9 * metricsCount, metrics.Count);
            interval = interval * 1000;
            var lastTimestamp = metrics[0].GetTimestamp()
                .TimeOfDay.Ticks;
            var reporter = metrics[0].Reporter;
            metrics.RemoveAt(0);
            foreach (var metric in metrics)
            {
                Assert.AreEqual(reporter, metric.Reporter);
                var nextTimestamp = metric.GetTimestamp()
                    .TimeOfDay.Ticks;
                if (lastTimestamp != nextTimestamp)
                {
                    Assert.AreEqual(lastTimestamp, nextTimestamp + interval);
                    lastTimestamp = nextTimestamp;
                }
            }
        }

        [Test]
        public virtual void testMeterQueryCustomIntervalWhereReporterNotExist()
        {
            //given metric data

            //when query metric interval data with custom interval and non existing reporter in where clause
            var metrics = managementService.CreateMetricsQuery()
                .Reporter("notExist")
                .Interval(300);

            //then result contains no metrics from given reporter
            Assert.AreEqual(0, metrics.Count);
        }

        // WHERE NAME //////////////////////////////////////////////////////////////////////

        [Test]
        public virtual void testMeterQueryDefaultIntervalWhereName()
        {
            //given metric data

            //when query metric interval data with name in where clause
            var metrics = managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .Interval();

            //then result contains only metrics with given name
            Assert.AreEqual(3, metrics.Count);
            var lastTimestamp = metrics[0].GetTimestamp()
                .TimeOfDay.Ticks;
            var name = metrics[0].Name;
            metrics.RemoveAt(0);
            foreach (var metric in metrics)
            {
                Assert.AreEqual(name, metric.Name);
                var nextTimestamp = metric.GetTimestamp()
                    .TimeOfDay.Ticks;
                if (lastTimestamp != nextTimestamp)
                {
                    Assert.AreEqual(lastTimestamp, nextTimestamp + DEFAULT_INTERVAL_MILLIS);
                    lastTimestamp = nextTimestamp;
                }
            }
        }

        [Test]
        public virtual void testMeterQueryDefaultIntervalWhereNameNotExist()
        {
            //given metric data

            //when query metric interval data with non existing name in where clause
            var metrics = managementService.CreateMetricsQuery()
                .Name("notExist")
                .Interval();

            //then result contains no metrics with given name
            Assert.AreEqual(0, metrics.Count);
        }

        [Test]
        public virtual void testMeterQueryCustomIntervalWhereName()
        {
            //given metric data and custom interval
            var interval = 5 * 60;

            //when query metric interval data with custom interval and name in where clause
            var metrics = managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .Interval(interval);

            //then result contains only metrics with given name
            Assert.AreEqual(9, metrics.Count);
            interval = interval * 1000;
            var lastTimestamp = metrics[0].GetTimestamp()
                .TimeOfDay.Ticks;
            var name = metrics[0].Name;
            metrics.RemoveAt(0);
            foreach (var metric in metrics)
            {
                Assert.AreEqual(name, metric.Name);
                var nextTimestamp = metric.GetTimestamp()
                    .TimeOfDay.Ticks;
                if (lastTimestamp != nextTimestamp)
                {
                    Assert.AreEqual(lastTimestamp, nextTimestamp + interval);
                    lastTimestamp = nextTimestamp;
                }
            }
        }

        [Test]
        public virtual void testMeterQueryCustomIntervalWhereNameNotExist()
        {
            //given metric data

            //when query metric interval data with custom interval and non existing name in where clause
            var metrics = managementService.CreateMetricsQuery()
                .Name("notExist")
                .Interval(300);

            //then result contains no metrics from given name
            Assert.AreEqual(0, metrics.Count);
        }

        // START DATE //////////////////////////////////////////////////////////////////////

        [Test]
        public virtual void testMeterQueryDefaultIntervalWhereStartDate()
        {
            //given metric data created for 14.9  min intervals

            //when query metric interval data with second last interval as start date in where clause
            //second last interval = start date = Jan 1, 1970 1:15:00 AM
            var startDate = firstInterval.AddMinutes(DEFAULT_INTERVAL);
            var metrics = managementService.CreateMetricsQuery()
                .StartDate(startDate)
                .Interval();

            //then result contains 18 entries since 9 different metrics are created
            //intervals Jan 1, 1970 1:15:00 AM and Jan 1, 1970 1:30:00 AM
            Assert.AreEqual(2 * metricsCount, metrics.Count);
        }

        [Test]
        public virtual void testMeterQueryCustomIntervalWhereStartDate()
        {
            //given metric data created for 15 min intervals

            //when query metric interval data with custom interval and second last interval as start date in where clause
            var startDate = firstInterval.AddMinutes(DEFAULT_INTERVAL);
            var metrics = managementService.CreateMetricsQuery()
                .StartDate(startDate)
                .Interval(300);

            //then result contains 4 intervals * the metrics Count
            //15 20 25 30 35 40
            Assert.AreEqual(6 * metricsCount, metrics.Count);
        }

        // END DATE //////////////////////////////////////////////////////////////////////

        [Test]
        public virtual void testMeterQueryDefaultIntervalWhereEndDate()
        {
            //given metric data created for 15 min intervals

            //when query metric interval data with second interval as end date in where clause
            //second interval = first interval - default interval
            var endDate = firstInterval.AddMinutes(DEFAULT_INTERVAL);
            var metrics = managementService.CreateMetricsQuery()
                .EndDate(endDate)
                .Interval();

            //then result contains one interval with entry for each metric
            //and end time is exclusive
            Assert.AreEqual(metricsCount, metrics.Count);
        }

        [Test]
        public virtual void testMeterQueryCustomIntervalWhereEndDate()
        {
            //given metric data created for 15 min intervals

            //when query metric interval data with custom interval and second interval as end date in where clause
            var endDate = firstInterval.AddMinutes(DEFAULT_INTERVAL);
            var metrics = managementService.CreateMetricsQuery()
                .EndDate(endDate)
                .Interval(300);

            //then result contains 3 * metrics Count 3 interval before end time
            //endTime is exclusive which means the given date is not included in the result
            Assert.AreEqual(3 * metricsCount, metrics.Count);
        }

        // START AND END DATE //////////////////////////////////////////////////////////////////////

        [Test]
        public virtual void testMeterQueryDefaultIntervalWhereStartAndEndDate()
        {
            //given metric data created for 15 min intervals

            //when query metric interval data with start and end date in where clause
            var endDate = firstInterval.AddMinutes(DEFAULT_INTERVAL);
            var startDate = firstInterval;
            var metrics = managementService.CreateMetricsQuery()
                .StartDate(startDate)
                .EndDate(endDate)
                .Interval();

            //then result contains 9 entries since 9 different metrics are created
            //and start date is inclusive and end date exclusive
            Assert.AreEqual(metricsCount, metrics.Count);
        }

        [Test]
        public virtual void testMeterQueryCustomIntervalWhereStartAndEndDate()
        {
            //given metric data created for 15 min intervals

            //when query metric interval data with custom interval, start and end date in where clause
            var endDate = firstInterval.AddMinutes(DEFAULT_INTERVAL);
            var startDate = firstInterval;
            var metrics = managementService.CreateMetricsQuery()
                .StartDate(startDate)
                .EndDate(endDate)
                .Interval(300);

            //then result contains 27 entries since 9 different metrics are created
            //endTime is exclusive which means the given date is not included in the result
            Assert.AreEqual(3 * metricsCount, metrics.Count);
        }

        // VALUE //////////////////////////////////////////////////////////////////////

        [Test]
        public virtual void testMeterQueryDefaultIntervalCalculatedValue()
        {
            //given metric data created for 15 min intervals

            //when query metric interval data with custom interval, start and end date in where clause
            var endDate = firstInterval.AddMinutes(DEFAULT_INTERVAL);
            var startDate = firstInterval;
            var metricQuery = managementService.CreateMetricsQuery()
                .StartDate(startDate)
                .EndDate(endDate)
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart);
            var metrics = metricQuery.Interval();
            var sum = metricQuery.Sum();

            //then result contains 1 entries
            //sum should be equal to the sum which is calculated by the metric query
            Assert.AreEqual(1, metrics.Count);
            Assert.AreEqual(sum, metrics[0].Value);
        }

        [Test]
        public virtual void testMeterQueryCustomIntervalCalculatedValue()
        {
            //given metric data created for 15 min intervals

            //when query metric interval data with custom interval, start and end date in where clause
            var endDate = firstInterval.AddMinutes(DEFAULT_INTERVAL);
            var startDate = firstInterval;
            var metricQuery = managementService.CreateMetricsQuery()
                .StartDate(startDate)
                .EndDate(endDate)
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart);
            var metrics = metricQuery.Interval(300);
            var sum = metricQuery.Sum();

            //then result contains 3 entries
            Assert.AreEqual(3, metrics.Count);
            long summedValue = 0;
            summedValue += metrics[0].Value;
            summedValue += metrics[1].Value;
            summedValue += metrics[2].Value;

            //summed value should be equal to the summed query value
            Assert.AreEqual(sum, summedValue);
        }

        // NOT LOGGED METRICS //////////////////////////////////////////////////////////////////////

        [Test]
        public virtual void testMeterQueryNotLoggedInterval()
        {
            //given metric data
            var metrics = managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .Limit(1)
                .Interval();
            var value = metrics[0].Value;

            //when start process and metrics are not logged
            processEngineConfiguration.MetricsRegistry.MarkOccurrence(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart, 3);

            //then metrics values are either way aggregated to the last interval
            //on query with name
            metrics = managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .Limit(1)
                .Interval();
            var newValue = metrics[0].Value;
            Assert.True(value + 3 == newValue);

            //on query without name also
            metrics = managementService.CreateMetricsQuery()
                .Interval();
            foreach (var intervalValue in metrics)
                if (intervalValue.Name.Equals(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart))
                {
                    newValue = intervalValue.Value;
                    Assert.True(value + 3 == newValue);
                    break;
                }

            //clean up
            clearLocalMetrics();
        }

        // NEW DATA AFTER SOME TIME ////////////////////////////////////////////////
        [Test]
        public virtual void testIntervallQueryWithGeneratedDataAfterSomeTime()
        {
            //given metric data and result of interval query
            var metrics = managementService.CreateMetricsQuery()
                .Interval();

            //when time is running and metrics is reported
            var lastInterval = metrics[0].GetTimestamp();
            var nextTime = lastInterval.Ticks + DEFAULT_INTERVAL_MILLIS;
            ClockUtil.CurrentTime = new DateTime(nextTime);

            reportMetrics();

            //then query returns more results
            var newMetrics = managementService.CreateMetricsQuery()
                .Interval();
            Assert.AreNotEqual(metrics.Count, newMetrics.Count);
            Assert.AreEqual(metrics.Count + metricsCount, newMetrics.Count);
            Assert.AreEqual(newMetrics[0].GetTimestamp()
                .TimeOfDay.Ticks, metrics[0].GetTimestamp()
                                      .TimeOfDay.Ticks + DEFAULT_INTERVAL_MILLIS);
        }
        [Test]
        public virtual void testIntervallQueryWithGeneratedDataAfterSomeTimeForSpecificMetric()
        {
            //given metric data and result of interval query
            var metrics = managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .StartDate(new DateTime())
                .EndDate(new DateTime(DEFAULT_INTERVAL_MILLIS * 200))
                .Interval();

            //when time is running and metrics is reported
            var lastInterval = metrics[0].GetTimestamp();
            var nextTime = lastInterval.Ticks + DEFAULT_INTERVAL_MILLIS;
            ClockUtil.CurrentTime = new DateTime(nextTime);

            reportMetrics();

            //then query returns more results
            var newMetrics = managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .StartDate(new DateTime())
                .EndDate(new DateTime(DEFAULT_INTERVAL_MILLIS * 200))
                .Interval();
            Assert.AreNotEqual(metrics.Count, newMetrics.Count);
            Assert.AreEqual(newMetrics[0].GetTimestamp()
                .TimeOfDay.Ticks, metrics[0].GetTimestamp()
                                      .TimeOfDay.Ticks + DEFAULT_INTERVAL_MILLIS);
            Assert.AreEqual(metrics[0].Value, newMetrics[1].Value);

            //clean up
            clearMetrics();
        }

        [Test]
        public virtual void testMeterQueryDecreaseLimit()
        {
            //given metric data

            //when query metric interval data with limit of 10 values
            var metrics = managementService.CreateMetricsQuery()
                .Limit(10)
                .Interval();

            //then 10 values are returned
            Assert.AreEqual(10, metrics.Count);
        }

        [Test]
        //[ExpectedException(typeof(ProcessEngineException))]
        public virtual void testMeterQueryIncreaseLimit()
        {
            //given metric data

            //when query metric interval data with max results set to 1000
            //exception.Expect(typeof(ProcessEngineException));
            //exception.ExpectMessage("Metrics interval query row limit can't be set larger than 200.");
            managementService.CreateMetricsQuery()
                .Limit(1000)
                .Interval();
        }

        // LIMIT //////////////////////////////////////////////////////////////////////

        [Test]
        public virtual void testMeterQueryLimit()
        {
            //since generating test data of 200 metrics will take a long time we check if the default values are set of the query
            //given metric query
            var query = (MetricsQueryImpl) managementService.CreateMetricsQuery();

            //when no changes are made
            //then max results are 200, lastRow 201, offset 0, firstRow 1
            //Assert.AreEqual(1, query.FirstRow);
            //Assert.AreEqual(0, query.FirstResult);
            //Assert.AreEqual(200, query.MaxResults);
            //Assert.AreEqual(201, query.LastRow);
        }
    }
}