using System;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Metrics;
using ESS.FW.Bpm.Engine.Impl.Util;
using NUnit.Framework;

namespace Engine.Tests.Api.Mgmt.Metrics
{
    /// <summary>
    ///     Represents the abstract metrics interval test class, which contains methods
    ///     for generating metrics and clean up afterwards.
    /// </summary>
    [TestFixture]
    public abstract class AbstractMetricsIntervalTest
    {
        [SetUp]
        public virtual void initMetrics()
        {
            runtimeService = ENGINE_RULE.RuntimeService;
            processEngineConfiguration = ENGINE_RULE.ProcessEngineConfiguration;
            managementService = ENGINE_RULE.ManagementService;

            //clean up before start
            clearMetrics();

            //init metrics
            processEngineConfiguration.SetDbMetricsReporterActivate(true);
            lastReporterId = processEngineConfiguration.DbMetricsReporter.MetricsCollectionTask.Reporter;
            processEngineConfiguration.DbMetricsReporter.ReporterId = REPORTER_ID;
            metricsRegistry = processEngineConfiguration.MetricsRegistry;
            rand = new Random((int) DateTime.Now.Ticks);
            generateMeterData(3, DEFAULT_INTERVAL_MILLIS);
        }

        [TearDown]
        public virtual void cleanUp()
        {
            ClockUtil.Reset();
            processEngineConfiguration.SetDbMetricsReporterActivate(false);
            processEngineConfiguration.DbMetricsReporter.ReporterId = lastReporterId;
            clearMetrics();
        }

        private readonly bool InstanceFieldsInitialized;

        public AbstractMetricsIntervalTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            TEST_RULE = new ProcessEngineTestRule(ENGINE_RULE);
            //RULE_CHAIN = RuleChain.outerRule(ENGINE_RULE).around(TEST_RULE);
        }


        protected internal readonly ProcessEngineRule ENGINE_RULE = new ProvidedProcessEngineRule();
        protected internal ProcessEngineTestRule TEST_RULE;
        protected internal readonly string REPORTER_ID = "REPORTER_ID";
        protected internal const int DEFAULT_INTERVAL = 15;
        protected internal const int DEFAULT_INTERVAL_MILLIS = 15 * 60 * 1000;
        protected internal const int MIN_OCCURENCE = 1;
        protected internal const int MAX_OCCURENCE = 250;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain RULE_CHAIN = org.junit.Rules.RuleChain.outerRule(ENGINE_RULE).around(TEST_RULE);
        //public RuleChain RULE_CHAIN;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public final org.junit.Rules.ExpectedException exception = org.junit.Rules.ExpectedException.None();
        //public readonly ExpectedException exception = ExpectedException.None();

        protected internal IRuntimeService runtimeService;
        protected internal ProcessEngineConfigurationImpl processEngineConfiguration;
        protected internal IManagementService managementService;
        protected internal string lastReporterId;
        protected internal DateTime firstInterval;
        protected internal int metricsCount;
        protected internal MetricsRegistry metricsRegistry;
        protected internal Random rand;

        private void generateMeterData(long dataCount, long interval)
        {
            //set up for randomnes
            var metricNames = metricsRegistry.Meters.Keys;
            metricsCount = metricNames.Count;

            //start date is the default interval since mariadb can't set 0 as timestamp
            long startDate = DEFAULT_INTERVAL_MILLIS;
            firstInterval = new DateTime(startDate);
            //we will have 5 metric reports in an interval
            var dataPerInterval = 5;

            //generate data
            for (var i = 0; i < dataCount; i++)
            {
                //calulate diff so timer can be set correctly
                var diff = interval / dataPerInterval;
                for (var j = 0; j < dataPerInterval; j++)
                {
                    ClockUtil.CurrentTime = new DateTime(startDate);
                    //generate random Count of data per interv
                    //for each metric
                    reportMetrics();
                    startDate += diff;
                }
            }
        }

        protected internal virtual void reportMetrics()
        {
            foreach (var metricName in metricsRegistry.Meters.Keys)
            {
                //mark random occurence
                long occurence = rand.Next(MAX_OCCURENCE - MIN_OCCURENCE + 1) + MIN_OCCURENCE;
                metricsRegistry.MarkOccurrence(metricName, occurence);
            }
            //report logged metrics
            processEngineConfiguration.DbMetricsReporter.ReportNow();
        }

        protected internal virtual void clearMetrics()
        {
            clearLocalMetrics();
            managementService.DeleteMetrics(DateTime.MaxValue);
        }

        protected internal virtual void clearLocalMetrics()
        {
            var meters = processEngineConfiguration.MetricsRegistry.Meters.Values;
            foreach (var meter in meters)
            {
                //meter.AndClear;
            }
        }
    }
}