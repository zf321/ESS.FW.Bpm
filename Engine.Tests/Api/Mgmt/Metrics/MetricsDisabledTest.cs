using ESS.FW.Bpm.Engine;
using NUnit.Framework;

namespace Engine.Tests.Api.Mgmt.Metrics
{
    /// <summary>
    ///     Asserts engine functionality is metrics are disabled
    /// </summary>
    [TestFixture]
    public class MetricsDisabledTest : ResourceProcessEngineTestCase
    {
        public MetricsDisabledTest() : base("resources/api/mgmt/metrics/metricsDisabledTest.cfg.xml")
        {
        }

        // FAILING, see https://app.Camunda.com/jira/browse/CAM-4053
        // (to run, remove "FAILING" from methodname)
        public virtual void FAILING_testQueryMetricsIfMetricsIsDisabled()
        {
            // given
            // that the metrics are disabled (see xml configuration referenced in constructor)
            Assert.IsFalse(processEngineConfiguration.MetricsEnabled);
            Assert.IsFalse(processEngineConfiguration.DbMetricsReporterActivate);

            // then
            // it is possible to execute a query
            managementService.CreateMetricsQuery()
                .Sum();
        }

        [Test]
        public virtual void testReportNowIfMetricsDisabled()
        {
            // given
            // that the metrics reporter is disabled
            Assert.IsFalse(processEngineConfiguration.DbMetricsReporterActivate);

            try
            {
                // then
                // I cannot invoke
                managementService.ReportDbMetricsNow();
                Assert.Fail("Exception expected");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Metrics reporting is disabled", e.Message);
            }
        }
    }
}