using System;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Util;
using NUnit.Framework;

namespace Engine.Tests.Api.Mgmt.Metrics
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MetricsTest
    {
        [SetUp]
        public static void initMetrics()
        {
            runtimeService = ENGINE_RULE.RuntimeService;
            processEngineConfiguration = ENGINE_RULE.ProcessEngineConfiguration;
            managementService = ENGINE_RULE.ManagementService;

            //clean up before start
            clearMetrics();
            TEST_RULE.Deploy(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("testProcess")
                .StartEvent()
                .ManualTask()
                .EndEvent()
                .Done());
        }

        [TearDown]
        public virtual void cleanUp()
        {
            clearMetrics();
        }

        protected internal static readonly ProcessEngineRule ENGINE_RULE = new ProvidedProcessEngineRule();
        protected internal static readonly ProcessEngineTestRule TEST_RULE = new ProcessEngineTestRule(ENGINE_RULE);

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.junit.Rules.RuleChain RULE_CHAIN = org.junit.Rules.RuleChain.outerRule(ENGINE_RULE).around(TEST_RULE);
        //public static RuleChain RULE_CHAIN = RuleChain.outerRule(ENGINE_RULE).around(TEST_RULE);

        protected internal static IRuntimeService runtimeService;
        protected internal static ProcessEngineConfigurationImpl processEngineConfiguration;
        protected internal static IManagementService managementService;

        protected internal static void clearMetrics()
        {
            var meters = processEngineConfiguration.MetricsRegistry.Meters.Values;
            foreach (var meter in meters)
            {
                //meter.AndClear;
            }
            managementService.DeleteMetrics(DateTime.MaxValue);
            processEngineConfiguration.SetDbMetricsReporterActivate(false);
        }

        [Test]
        public virtual void testDeleteMetrics()
        {
            // given
            runtimeService.StartProcessInstanceByKey("testProcess");
            processEngineConfiguration.DbMetricsReporter.ReportNow();

            // a Count of six (start and end)
            Assert.AreEqual(6l, managementService.CreateMetricsQuery()
                .Sum());

            // if
            // we Delete with timestamp "null"
            managementService.DeleteMetrics(DateTime.MaxValue);

            // then
            // all entries are deleted
            Assert.AreEqual(0l, managementService.CreateMetricsQuery()
                .Sum());
        }

        [Test]
        public virtual void testDeleteMetricsWithTimestamp()
        {
            // given
            runtimeService.StartProcessInstanceByKey("testProcess");
            processEngineConfiguration.DbMetricsReporter.ReportNow();

            // a Count of six (start and end)
            Assert.AreEqual(6l, managementService.CreateMetricsQuery()
                .Sum());

            // if
            // we Delete with timestamp older or equal to the timestamp of the log entry
            managementService.DeleteMetrics(ClockUtil.CurrentTime);

            // then
            // all entries are deleted
            Assert.AreEqual(0l, managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .Sum());
        }

        [Test]
        public virtual void testDeleteMetricsWithTimestampBefore()
        {
            // given
            runtimeService.StartProcessInstanceByKey("testProcess");
            processEngineConfiguration.DbMetricsReporter.ReportNow();

            // a Count of six (start and end)
            Assert.AreEqual(6l, managementService.CreateMetricsQuery()
                .Sum());

            // if
            // we Delete with timestamp before the timestamp of the log entry
            managementService.DeleteMetrics(new DateTime(ClockUtil.CurrentTime.TimeOfDay.Ticks - 10000));

            // then
            // the entires are NOT deleted
            Assert.AreEqual(6l, managementService.CreateMetricsQuery()
                .Sum());
        }

        [Test]
        public virtual void testDeleteMetricsWithReporterId()
        {
            // indicate that db metrics reporter is active (although it is not)
            processEngineConfiguration.SetDbMetricsReporterActivate(true);

            // given
            processEngineConfiguration.DbMetricsReporter.ReporterId = "reporter1";
            runtimeService.StartProcessInstanceByKey("testProcess");
            managementService.ReportDbMetricsNow();

            processEngineConfiguration.DbMetricsReporter.ReporterId = "reporter2";
            runtimeService.StartProcessInstanceByKey("testProcess");
            managementService.ReportDbMetricsNow();

            Assert.AreEqual(3l, managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .Reporter("reporter1")
                .Sum());

            // when the metrics for reporter1 are deleted
            managementService.DeleteMetrics(DateTime.MaxValue, "reporter1");

            // then
            Assert.AreEqual(0l, managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .Reporter("reporter1")
                .Sum());
            Assert.AreEqual(3l, managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .Reporter("reporter2")
                .Sum());

            // cleanup
            processEngineConfiguration.SetDbMetricsReporterActivate(false);
            processEngineConfiguration.DbMetricsReporter.ReporterId = null;
        }

        [Test]
        public virtual void testReportNow()
        {
            // indicate that db metrics reporter is active (although it is not)
            processEngineConfiguration.SetDbMetricsReporterActivate(true);

            // given
            runtimeService.StartProcessInstanceByKey("testProcess");

            // when
            managementService.ReportDbMetricsNow();

            // then the metrics have been reported
            Assert.AreEqual(3l, managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .Sum());

            // cleanup
            processEngineConfiguration.SetDbMetricsReporterActivate(false);
        }

        [Test]
        public virtual void testReportNowIfMetricsIsDisabled()
        {
            var defaultIsMetricsEnabled = processEngineConfiguration.MetricsEnabled;

            // given
            processEngineConfiguration.SetMetricsEnabled(false);

            try
            {
                // when
                managementService.ReportDbMetricsNow();
                Assert.Fail("Exception expected");
            }
            catch (ProcessEngineException e)
            {
                // then an exception is thrown
                Assert.True(e.Message.Contains("Metrics reporting is disabled"));
            }
            finally
            {
                // reset metrics setting
                processEngineConfiguration.SetMetricsEnabled(defaultIsMetricsEnabled);
            }
        }
        [Test]
        public virtual void testReportNowIfReporterIsNotActive()
        {
            var defaultIsMetricsEnabled = processEngineConfiguration.MetricsEnabled;
            var defaultIsMetricsReporterActivate = processEngineConfiguration.DbMetricsReporterActivate;

            // given
            processEngineConfiguration.SetMetricsEnabled(true);
            processEngineConfiguration.SetDbMetricsReporterActivate(false);

            try
            {
                // when
                managementService.ReportDbMetricsNow();
                Assert.Fail("Exception expected");
            }
            catch (ProcessEngineException e)
            {
                // then an exception is thrown
                Assert.True(e.Message.Contains("Metrics reporting to database is disabled"));
            }
            finally
            {
                processEngineConfiguration.SetMetricsEnabled(defaultIsMetricsEnabled);
                processEngineConfiguration.SetDbMetricsReporterActivate(defaultIsMetricsReporterActivate);
            }
        }

        [Test]
        public virtual void testQuery()
        {
            // given
            runtimeService.StartProcessInstanceByKey("testProcess");
            processEngineConfiguration.DbMetricsReporter.ReportNow();

            // then (query Assertions)
            Assert.AreEqual(0l, managementService.CreateMetricsQuery()
                .Name("UNKNOWN")
                .Sum());
            Assert.AreEqual(3l, managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .Sum());

            Assert.AreEqual(6l, managementService.CreateMetricsQuery()
                .Sum());
            Assert.AreEqual(6l, managementService.CreateMetricsQuery()
                .StartDate(new DateTime(1000))
                .Sum());
            Assert.AreEqual(6l, managementService.CreateMetricsQuery()
                .StartDate(new DateTime(1000))
                .EndDate(new DateTime(ClockUtil.CurrentTime.TimeOfDay.Ticks + 2000l))
                .Sum()); // + 2000 for milliseconds imprecision on some databases (MySQL)
            Assert.AreEqual(0l, managementService.CreateMetricsQuery()
                .StartDate(new DateTime(ClockUtil.CurrentTime.TimeOfDay.Ticks + 1000l))
                .Sum());
            Assert.AreEqual(0l, managementService.CreateMetricsQuery()
                .StartDate(new DateTime(ClockUtil.CurrentTime.TimeOfDay.Ticks + 1000l))
                .EndDate(ClockUtil.CurrentTime)
                .Sum());

            // given
            runtimeService.StartProcessInstanceByKey("testProcess");
            processEngineConfiguration.DbMetricsReporter.ReportNow();

            // then (query Assertions)
            Assert.AreEqual(12l, managementService.CreateMetricsQuery()
                .Sum());
            Assert.AreEqual(12l, managementService.CreateMetricsQuery()
                .StartDate(new DateTime(1000))
                .Sum());
            Assert.AreEqual(12l, managementService.CreateMetricsQuery()
                .StartDate(new DateTime(1000))
                .EndDate(new DateTime(ClockUtil.CurrentTime.TimeOfDay.Ticks + 2000l))
                .Sum()); // + 2000 for milliseconds imprecision on some databases (MySQL)
            Assert.AreEqual(0l, managementService.CreateMetricsQuery()
                .StartDate(new DateTime(ClockUtil.CurrentTime.TimeOfDay.Ticks + 1000l))
                .Sum());
            Assert.AreEqual(0l, managementService.CreateMetricsQuery()
                .StartDate(new DateTime(ClockUtil.CurrentTime.TimeOfDay.Ticks + 1000l))
                .EndDate(ClockUtil.CurrentTime)
                .Sum());
        }

        [Test]
        public virtual void testQueryEndDateExclusive()
        {
            // given
            // note: dates should be exact seconds due to missing milliseconds precision on
            // older mysql versions
            // cannot insert 1970-01-01 00:00:00 into MySQL
            ClockUtil.CurrentTime = new DateTime(5000L);
            runtimeService.StartProcessInstanceByKey("testProcess");
            processEngineConfiguration.DbMetricsReporter.ReportNow();

            ClockUtil.CurrentTime = new DateTime(6000L);
            runtimeService.StartProcessInstanceByKey("testProcess");
            processEngineConfiguration.DbMetricsReporter.ReportNow();

            ClockUtil.CurrentTime = new DateTime(7000L);
            runtimeService.StartProcessInstanceByKey("testProcess");
            processEngineConfiguration.DbMetricsReporter.ReportNow();

            // then Query#startDate is inclusive and Query#endDate is exclusive
            Assert.AreEqual(18l, managementService.CreateMetricsQuery()
                .Sum());
            Assert.AreEqual(18l, managementService.CreateMetricsQuery()
                .StartDate(new DateTime())
                .Sum());
            Assert.AreEqual(12l, managementService.CreateMetricsQuery()
                .StartDate(new DateTime())
                .EndDate(new DateTime(7000L))
                .Sum());
            Assert.AreEqual(18l, managementService.CreateMetricsQuery()
                .StartDate(new DateTime())
                .EndDate(new DateTime(8000L))
                .Sum());
        }

        [Test]
        public virtual void testReportWithReporterId()
        {
            // indicate that db metrics reporter is active (although it is not)
            processEngineConfiguration.SetDbMetricsReporterActivate(true);

            // given

            // when
            processEngineConfiguration.DbMetricsReporter.ReporterId = "reporter1";
            runtimeService.StartProcessInstanceByKey("testProcess");
            managementService.ReportDbMetricsNow();

            // and
            processEngineConfiguration.DbMetricsReporter.ReporterId = "reporter2";
            runtimeService.StartProcessInstanceByKey("testProcess");
            managementService.ReportDbMetricsNow();

            // then the metrics have been reported
            Assert.AreEqual(6l, managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .Sum());

            // and are grouped by reporter
            Assert.AreEqual(3l, managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .Reporter("reporter1")
                .Sum());
            Assert.AreEqual(3l, managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .Reporter("reporter2")
                .Sum());
            Assert.AreEqual(0l, managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .Reporter("aNonExistingReporter")
                .Sum());

            // cleanup
            processEngineConfiguration.SetDbMetricsReporterActivate(false);
            processEngineConfiguration.DbMetricsReporter.ReporterId = null;
        }

        [Test]
        public virtual void testEndMetricWithWaitState()
        {
            //given
            TEST_RULE.Deploy(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("userProcess")
                .StartEvent()
                .UserTask("Task")
                .EndEvent()
                .Done());

            //when
            runtimeService.StartProcessInstanceByKey("userProcess");
            processEngineConfiguration.DbMetricsReporter.ReportNow();

            //then end is not equal to start since a wait state exist at Task
            var start = managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .Sum();
            var end = managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceEnd)
                .Sum();
            Assert.AreNotEqual(end, start);
            Assert.AreEqual(2, start);
            Assert.AreEqual(1, end);

            //when completing the task
            var id = ENGINE_RULE.TaskService.CreateTaskQuery(c=>c.ProcessDefinitionId=="userProcess")
                .First()
                .Id;
            ENGINE_RULE.TaskService.Complete(id);

            //then start and end is equal
            start = managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .Sum();
            end = managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceEnd)
                .Sum();
            Assert.AreEqual(end, start);
        }

        [Test]
        public virtual void testStartAndEndMetricsAreEqual()
        {
            // given

            //when
            runtimeService.StartProcessInstanceByKey("testProcess");
            processEngineConfiguration.DbMetricsReporter.ReportNow();

            //then end and start metrics are equal
            var start = managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceStart)
                .Sum();
            var end = managementService.CreateMetricsQuery()
                .Name(ESS.FW.Bpm.Engine.Management.Metrics.ActivtyInstanceEnd)
                .Sum();
            Assert.AreEqual(end, start);
        }
    }
}