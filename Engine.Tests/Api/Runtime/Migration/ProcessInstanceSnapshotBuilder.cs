using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Runtime;

namespace Engine.Tests.Api.Runtime.Migration
{
    public class ProcessInstanceSnapshotBuilder
    {
        protected internal IProcessEngine ProcessEngine;
        protected internal string ProcessInstanceId;
        protected internal ProcessInstanceSnapshot Snapshot;

        public ProcessInstanceSnapshotBuilder(IProcessInstance processInstance, IProcessEngine processEngine)
        {
            ProcessEngine = processEngine;
            ProcessInstanceId = processInstance.Id;
            Snapshot = new ProcessInstanceSnapshot(processInstance.Id, processInstance.ProcessDefinitionId);
        }

        public virtual ProcessInstanceSnapshotBuilder DeploymentId()
        {
            var deploymentId = ProcessEngine.RepositoryService.GetProcessDefinition(Snapshot.ProcessDefinitionId)
                .DeploymentId;
            Snapshot.DeploymentId = deploymentId;

            return this;
        }

        public virtual ProcessInstanceSnapshotBuilder ActivityTree()
        {
            var activityInstance = ProcessEngine.RuntimeService.GetActivityInstance(ProcessInstanceId);
            Snapshot.ActivityTree = activityInstance;

            return this;
        }

        public virtual ProcessInstanceSnapshotBuilder ExecutionTree()
        {
            var executionTree = Tests.Util.ExecutionTree.ForExecution(ProcessInstanceId, ProcessEngine);
            Snapshot.ExecutionTree = executionTree;

            return this;
        }

        public virtual ProcessInstanceSnapshotBuilder Tasks()
        {
            var tasks = ProcessEngine.TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== ProcessInstanceId)
                
                .ToList();
            Snapshot.Tasks = tasks;

            return this;
        }

        public virtual ProcessInstanceSnapshotBuilder EventSubscriptions()
        {
            var eventSubscriptions = ProcessEngine.RuntimeService.CreateEventSubscriptionQuery(c=>c.ProcessInstanceId== ProcessInstanceId)
                
                .ToList();
            Snapshot.EventSubscriptions = eventSubscriptions;

            return this;
        }

        public virtual ProcessInstanceSnapshotBuilder Jobs()
        {
            var jobs = ProcessEngine.ManagementService.CreateJobQuery(c=>c.ProcessInstanceId== ProcessInstanceId)
                
                .ToList();
            Snapshot.Jobs = jobs;

            var processDefinitionId = ProcessEngine.RuntimeService.CreateProcessInstanceQuery(c=> c.ProcessDefinitionId == ProcessInstanceId)
                .First()
                .ProcessDefinitionId;
            var jobDefinitions = ProcessEngine.ManagementService.CreateJobDefinitionQuery(c=>c.ProcessDefinitionId==processDefinitionId)
                
                .ToList();
            Snapshot.JobDefinitions = jobDefinitions;

            return this;
        }

        public virtual ProcessInstanceSnapshotBuilder Variables()
        {
            var variables = ProcessEngine.RuntimeService.CreateVariableInstanceQuery(c=> c.ProcessInstanceId== ProcessInstanceId)
                
                .ToList();
            //Snapshot.SetVariables(variables);

            return this;
        }

        public virtual ProcessInstanceSnapshot Build()
        {
            return Snapshot;
        }

        public virtual ProcessInstanceSnapshot Full()
        {
            DeploymentId();
            ActivityTree();
            ExecutionTree();
            Tasks();
            EventSubscriptions();
            Jobs();
            Variables();

            return Build();
        }
    }
}