using NUnit.Framework;

namespace Engine.Tests.Api.Mgmt.Metrics
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public abstract class AbstractMetricsTest : PluggableProcessEngineTestCase
    {
        [SetUp]
        protected internal virtual void setUp()
        {
            clearMetrics();
        }

        [TearDown]
        protected internal virtual void tearDown()
        {
            clearMetrics();
        }

        protected internal virtual void clearMetrics()
        {
            var meters = processEngineConfiguration.MetricsRegistry.Meters.Values;
            foreach (var meter in meters)
            {
                //meter.AndClear;
            }
            managementService.DeleteMetrics(null);
        }
    }
}