using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Management;

namespace Engine.Tests.Api.Runtime
{
    public class BatchModificationHelper : BatchHelper
    {
        protected internal IList<string> CurrentProcessInstances;

        protected internal ProcessEngineTestRule TestRule;

        public BatchModificationHelper(ProcessEngineRule engineRule) : base(engineRule)
        {
            TestRule = new ProcessEngineTestRule(engineRule);
            CurrentProcessInstances = new List<string>();
        }

        public virtual IBatch StartAfterAsync(string key, int numberOfProcessInstances, string activityId,
            string processDefinitionId)
        {
            var runtimeService = EngineRule.RuntimeService;

            var processInstanceIds = StartInstances(key, numberOfProcessInstances);

            return runtimeService.CreateModification(processDefinitionId)
                .StartAfterActivity(activityId)
                .SetProcessInstanceIds(processInstanceIds)
                .ExecuteAsync();
        }

        public virtual IBatch StartBeforeAsync(string key, int numberOfProcessInstances, string activityId,
            string processDefinitionId)
        {
            var runtimeService = EngineRule.RuntimeService;

            var processInstanceIds = StartInstances(key, numberOfProcessInstances);

            return runtimeService.CreateModification(processDefinitionId)
                .StartBeforeActivity(activityId)
                .SetProcessInstanceIds(processInstanceIds)
                .ExecuteAsync();
        }

        public virtual IBatch StartTransitionAsync(string key, int numberOfProcessInstances, string transitionId,
            string processDefinitionId)
        {
            var runtimeService = EngineRule.RuntimeService;

            var processInstanceIds = StartInstances(key, numberOfProcessInstances);

            return runtimeService.CreateModification(processDefinitionId)
                .StartTransition(transitionId)
                .SetProcessInstanceIds(processInstanceIds)
                .ExecuteAsync();
        }

        public virtual IBatch CancelAllAsync(string key, int numberOfProcessInstances, string activityId,
            string processDefinitionId)
        {
            var runtimeService = EngineRule.RuntimeService;

            var processInstanceIds = StartInstances(key, numberOfProcessInstances);

            return runtimeService.CreateModification(processDefinitionId)
                .CancelAllForActivity(activityId)
                .SetProcessInstanceIds(processInstanceIds)
                .ExecuteAsync();
        }

        public virtual IList<string> StartInstances(string key, int numOfInstances)
        {
            IList<string> instances = new List<string>();
            for (var i = 0; i < numOfInstances; i++)
            {
                var processInstance = EngineRule.RuntimeService.StartProcessInstanceByKey(key);
                instances.Add(processInstance.Id);
            }

            CurrentProcessInstances = instances;
            return instances;
        }

        public override IJobDefinition GetExecutionJobDefinition(IBatch batch)
        {
            return EngineRule.ManagementService.CreateJobDefinitionQuery(c=>c.Id == batch.BatchJobDefinitionId&&c.JobType==BatchFields.TypeProcessInstanceModification)
                .First();
        }
    }
}