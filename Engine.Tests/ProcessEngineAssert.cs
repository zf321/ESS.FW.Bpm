using System.Linq;
using ESS.FW.Bpm.Engine;

namespace Engine.Tests
{
    public class ProcessEngineAssert
    {
        public static void AssertProcessEnded(IProcessEngine processEngine, string processInstanceId)
        {
            var processInstance =
                processEngine.RuntimeService.CreateProcessInstanceQuery(c=>c.ProcessInstanceId ==processInstanceId)
                    .First();

            if (processInstance != null)
                throw new System.Exception("expected finished process instance '" + processInstanceId +
                                    "' but it was still in the db");
        }
    }
}