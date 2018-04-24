using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class JobAcquisitionTestHelper
    {
        /// <summary>
        ///     suspends random process instances that are active
        /// </summary>
        public static void SuspendInstances(IProcessEngine processEngine, int numInstances)
        {
            var instancesToSuspend = processEngine
                .RuntimeService
                .CreateProcessInstanceQuery(c => c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .OrderBy(c => c.Id)
                .Skip(0)
                .Take(numInstances)
                .ToList();

            if (instancesToSuspend.Count < numInstances)
                throw new ProcessEngineException("Cannot suspend " + numInstances + " process instances");

            foreach (var activeInstance in instancesToSuspend)
                processEngine.RuntimeService.SuspendProcessInstanceById(activeInstance.Id);
        }

        /// <summary>
        ///     activates random process instances that are active
        /// </summary>
        public static void ActivateInstances(IProcessEngine processEngine, int numInstances)
        {
            var instancesToActivate = processEngine
                .RuntimeService
                .CreateProcessInstanceQuery(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode)
                .OrderBy(c => c.Id)
                .Skip(0)
                .Take(numInstances)
                .ToList();
            
            if (instancesToActivate.Count < numInstances)
                throw new ProcessEngineException("Cannot activate " + numInstances + " process instances");

            foreach (var suspendedInstance in instancesToActivate)
                processEngine.RuntimeService.ActivateProcessInstanceById(suspendedInstance.Id);
        }

        public static void AssertInBetween(long minimum, long maximum, long actualValue)
        {
            Assert.True(actualValue >= minimum && actualValue <= maximum,
                "Expected '" + actualValue + "' to be between '" + minimum + "' and '" + maximum + "'");
        }
    }
}