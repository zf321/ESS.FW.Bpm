using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    /// </summary>
    public class ActivityCancellationCmd : AbstractProcessInstanceModificationCommand
    {
        protected internal string ActivityId;

        public ActivityCancellationCmd(string activityId): this(null, activityId)
        {
           
        }

        public ActivityCancellationCmd(string processInstanceId, string activityId) : base(processInstanceId)
        {
            this.ActivityId = activityId;
        }
        
        public override object Execute(CommandContext commandContext)
        {
            IActivityInstance activityInstanceTree = commandContext.RunWithoutAuthorization(()=> (new GetActivityInstanceCmd(processInstanceId)).Execute(commandContext));

            ExecutionEntity processInstance = commandContext.ExecutionManager.FindExecutionById(processInstanceId);
            ProcessDefinitionImpl processDefinition = processInstance.ProcessDefinition;
            ISet<string> parentScopeIds = CollectParentScopeIdsForActivity(processDefinition, ActivityId);

            IList<IActivityInstance> childrenForActivity = GetActivityInstancesForActivity(activityInstanceTree, parentScopeIds);
            foreach (IActivityInstance instance in childrenForActivity)
            {
                ActivityInstanceCancellationCmd cmd = new ActivityInstanceCancellationCmd(processInstanceId, instance.Id);
                cmd.SkipCustomListeners = skipCustomListeners;
                cmd.SkipIoMappings = skipIoMappings;
                cmd.Execute(commandContext);
            }

            IList<ITransitionInstance> transitionInstancesForActivity = GetTransitionInstancesForActivity(activityInstanceTree, parentScopeIds);
            foreach (ITransitionInstance instance in transitionInstancesForActivity)
            {
                TransitionInstanceCancellationCmd cmd = new TransitionInstanceCancellationCmd(processInstanceId, instance.Id);
                cmd.SkipCustomListeners = skipCustomListeners;
                cmd.SkipIoMappings = skipIoMappings;
                cmd.Execute(commandContext);
            }
            return null;
        }

        protected internal virtual ISet<string> CollectParentScopeIdsForActivity(
            ProcessDefinitionImpl processDefinition, string activityId)
        {
            ISet<string> parentScopeIds = new HashSet<string>();
            var scope = (ScopeImpl) processDefinition.FindActivity(activityId);

            while (scope != null)
            {
                parentScopeIds.Add(scope.Id);
                scope = scope.FlowScope;
            }

            return parentScopeIds;
        }

        protected internal virtual IList<ITransitionInstance> GetTransitionInstancesForActivity(IActivityInstance tree,
            ISet<string> parentScopeIds)
        {
            // prune all search paths that are not in the scope hierarchy of the activity in question
            if (!parentScopeIds.Contains(tree.ActivityId))
                return null;

            IList<ITransitionInstance> instances = new List<ITransitionInstance>();
            var transitionInstances = tree.ChildTransitionInstances;

            foreach (var transitionInstance in transitionInstances)
                if (ActivityId.Equals(transitionInstance.ActivityId))
                    instances.Add(transitionInstance);

            foreach (var child in tree.ChildActivityInstances)
                ((List<ITransitionInstance>) instances).AddRange(GetTransitionInstancesForActivity(child, parentScopeIds));

            return instances;
        }

        protected internal virtual IList<IActivityInstance> GetActivityInstancesForActivity(IActivityInstance tree,
            ISet<string> parentScopeIds)
        {
            // prune all search paths that are not in the scope hierarchy of the activity in question
            if (!parentScopeIds.Contains(tree.ActivityId))
                return null;

            IList<IActivityInstance> instances = new List<IActivityInstance>();

            if (ActivityId.Equals(tree.ActivityId))
                instances.Add(tree);

            foreach (var child in tree.ChildActivityInstances)
                ((List<IActivityInstance>) instances).AddRange(GetActivityInstancesForActivity(child, parentScopeIds));

            return instances;
        }

        protected internal override string Describe()
        {
            return "Cancel all instances of activity '" + ActivityId + "'";
        }
        
    }
}