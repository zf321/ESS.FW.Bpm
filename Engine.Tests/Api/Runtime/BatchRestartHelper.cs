using System.Linq;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    public class BatchRestartHelper : BatchHelper
    {
        public BatchRestartHelper(ProcessEngineRule engineRule) : base(engineRule)
        {
        }

        public BatchRestartHelper(PluggableProcessEngineTestCase testCase) : base(testCase)
        {
        }

        public override IJobDefinition GetExecutionJobDefinition(IBatch batch)
        {
            return ManagementService.CreateJobDefinitionQuery(c=>c.Id == batch.BatchJobDefinitionId&&c.JobType==BatchFields.TypeProcessInstanceRestart)
                .First();
        }


        public override void ExecuteJob(IJob job)
        {
            Assert.NotNull(job, "IJob to execute does not exist");
            ManagementService.ExecuteJob(job.Id);
        }
    }
}