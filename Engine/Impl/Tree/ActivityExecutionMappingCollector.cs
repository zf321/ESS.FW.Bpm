using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.tree
{
    /// <summary>
    ///     Collect the mappings of scopes and executions. It can be used to collect the mappings over process instances.
    /// </summary>
    /// <seealso cref= "ActivityExecution# createActivityExecutionMapping()"></seealso>
    public class ActivityExecutionMappingCollector : ITreeVisitor<IActivityExecution>
    {
        private readonly IDictionary<ScopeImpl, PvmExecutionImpl> _activityExecutionMapping =
            new Dictionary<ScopeImpl, PvmExecutionImpl>();

        private readonly IActivityExecution _initialExecution;
        private bool _initialized;

        public ActivityExecutionMappingCollector(IActivityExecution execution)
        {
            _initialExecution = execution;
        }

        public virtual void Visit(IActivityExecution execution)
        {
            if (!_initialized)
            {
                // lazy initialization to avoid exceptions on creation
                AppendActivityExecutionMapping(_initialExecution);
                _initialized = true;
            }

            AppendActivityExecutionMapping(execution);
        }

        private void AppendActivityExecutionMapping(IActivityExecution execution)
        {
            if ((execution.Activity != null) &&
                !LegacyBehavior.HasInvalidIntermediaryActivityId((PvmExecutionImpl) execution))
                foreach (var it in execution.CreateActivityExecutionMapping())
                    //_activityExecutionMapping.Add(it.Key, it.Value);
                    _activityExecutionMapping[it.Key]= it.Value;
        }

        /// <returns> the mapped execution for scope or <code>null</code>, if no mapping exists </returns>
        public virtual PvmExecutionImpl GetExecutionForScope(IPvmScope scope)
        {
            return _activityExecutionMapping[(ScopeImpl) scope];
        }
    }
}