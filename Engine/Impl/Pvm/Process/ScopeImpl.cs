using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Process
{
    /// <summary>
    ///     A Bpmn scope. The scope has references to two lists of activities:
    ///     - the flow activities (activities for which the <seealso cref="ActivityImpl#getFlowScope() flow scope" /> is this
    ///     scope
    ///     - event listener activities (activities for which the <seealso cref="ActivityImpl#getEventScope() event scope" />
    ///     is this scope.
    ///      
    ///     
    /// </summary>
    [Serializable]
    public abstract class ScopeImpl : CoreActivity, IPvmScope
    {
        /// <summary>
        ///     The key identifies the activity which is referenced but not read yet.
        ///     The value is the error callback, which is called if the activity is not
        ///     read till the end of parsing.
        /// </summary>
        protected internal readonly IDictionary<string, IBacklogErrorCallback> Backlog =
            new Dictionary<string, IBacklogErrorCallback>();

        /// <summary>
        ///     activities for which this is the event scope
        /// </summary>
        protected internal ISet<ActivityImpl> eventActivities = new HashSet<ActivityImpl>();

        /// <summary>
        ///     The activities for which the flow scope is this scope
        /// </summary>
        protected internal IList<ActivityImpl> FlowActivities = new List<ActivityImpl>();

        protected internal bool IsSubProcessScope;

        protected internal IDictionary<string, ActivityImpl> NamedFlowActivities =
            new Dictionary<string, ActivityImpl>();

        protected internal ProcessDefinitionImpl processDefinition;
        protected bool isTransaction = false;

        public ScopeImpl(string id, ProcessDefinitionImpl processDefinition) : base(id)
        {
            this.processDefinition = processDefinition;
        }

        /// <summary>
        ///     Returns the backlog error callback's.
        /// </summary>
        /// <returns> the callback's </returns>
        public virtual ICollection<IBacklogErrorCallback> BacklogErrorCallbacks
        {
            get { return Backlog.Values; }
        }

        /// <summary>
        ///     Returns true if the backlog is empty.
        /// </summary>
        /// <returns> true if empty, false otherwise </returns>
        public virtual bool BacklogEmpty
        {
            get { return Backlog.Count == 0; }
        }

        [Obsolete]
        public virtual IDictionary<string, IList<IDelegateListener<IBaseDelegateExecution>>> ExecutionListeners
        {
            get
            {
                return Listeners;
            }
        }

        // getters and setters //////////////////////////////////////////////////////

        public override IList<CoreActivity> Activities
        {
            get
            {
                return FlowActivities.Cast<CoreActivity>().ToList();
            }
        }

        public virtual ISet<ActivityImpl> EventActivities
        {
            get { return eventActivities; }
        }


        public virtual IPvmProcessDefinition ProcessDefinition
        {
            get { return processDefinition; }
        }

        public abstract IPvmScope EventScope { get; set; }
        [NotMapped]
        public abstract bool IsScope { get; set; }

        public abstract IPvmScope LevelOfSubprocessScope { get; }
        public abstract ScopeImpl FlowScope { get; }

        public virtual TransitionImpl FindTransition(string transitionId)
        {
            foreach (IPvmActivity childActivity in FlowActivities)
                foreach (var transition in childActivity.OutgoingTransitions)
                    if (transitionId.Equals(transition.Id))
                        return (TransitionImpl)transition;

            foreach (var childActivity in FlowActivities)
            {
                var nestedTransition = childActivity.FindTransition(transitionId);
                if (nestedTransition != null)
                    return nestedTransition;
            }

            return null;
        }


        public virtual bool SubProcessScope
        {
            get { return IsSubProcessScope; }
            set { IsSubProcessScope = value; }
        }

        IList<IPvmActivity> IPvmScope.Activities => FlowActivities.Cast<IPvmActivity>().ToList();


        public override Core.Model.Properties Properties => properties;


        public new IPvmActivity FindActivity(string activityId)
        {
            return (IPvmActivity)base.FindActivity(activityId);
        }

        public virtual IPvmActivity FindActivityAtLevelOfSubprocess(string activityId)
        {
            if (!SubProcessScope)
                throw new ProcessEngineException("This is not a sub process scope.");
            var activity = (ActivityImpl)FindActivity(activityId);
            if ((activity == null) || (activity.LevelOfSubprocessScope != this))
                return null;
            return activity;
        }

        /// <summary>
        ///     重写，找不到key返回null
        ///     searches for the activity locally
        /// </summary>
        public override CoreActivity GetChildActivity(string activityId)
        {
            if (NamedFlowActivities.ContainsKey(activityId))
                return NamedFlowActivities[activityId];
            return null;
        }

        /// <summary>
        ///     Add's the given activity reference and the error callback to the backlog.
        /// </summary>
        /// <param name="activityRef"> the activity reference which is not read until now </param>
        /// <param name="callback"> the error callback which should called if activity will not be read </param>
        public virtual void AddToBacklog(string activityRef, IBacklogErrorCallback callback)
        {
            Backlog[activityRef] = callback;
        }

        public override CoreActivity CreateActivity(string activityId)
        {
            var activity = new ActivityImpl(activityId, processDefinition);
            if (!ReferenceEquals(activityId, null))
            {
                if (processDefinition.FindActivity(activityId) != null)
                    throw new PvmException("duplicate activity id '" + activityId + "'");
                if (Backlog.ContainsKey(activityId))
                    Backlog.Remove(activityId);
                NamedFlowActivities[activityId] = activity;
            }
            activity.flowScope = this;
            FlowActivities.Add(activity);

            return activity;
        }

        public virtual bool IsAncestorFlowScopeOf(ScopeImpl other)
        {
            var otherAncestor = other.FlowScope;
            while (otherAncestor != null)
            {
                if (this == otherAncestor)
                    return true;
                otherAncestor = otherAncestor.FlowScope;
            }

            return false;
        }

        public virtual bool Contains(ActivityImpl activity)
        {
            if (NamedFlowActivities.ContainsKey(activity.Id))
                return true;
            foreach (var nestedActivity in FlowActivities)
                if (nestedActivity.Contains(activity))
                    return true;
            return false;
        }

        // event listeners //////////////////////////////////////////////////////////

        [Obsolete]
        public virtual IList<IDelegateListener<IBaseDelegateExecution>> GetExecutionListeners(string eventName)
        {
            return GetListeners(eventName);
        }

        [Obsolete]
        public virtual void AddExecutionListener(string eventName, IDelegateListener<IBaseDelegateExecution> executionListener)
        {
            AddListener(eventName, executionListener);
        }

        [Obsolete]
        public virtual void AddExecutionListener(string eventName, IDelegateListener<IBaseDelegateExecution> executionListener, int index)
        {
            AddListener(eventName, executionListener, index);
        }


        /// <summary>
        ///     Represents the backlog error callback interface.
        ///     Contains a callback method, which is called if the activity in the backlog
        ///     is not read till the end of parsing.
        /// </summary>
        public interface IBacklogErrorCallback
        {
            /// <summary>
            ///     In error case the callback will called.
            /// </summary>
            void Callback();
        }
        public bool IsTransaction
        {
            get
            {
                return isTransaction;
            }
        }
        public void SetIsTransaction()
        {
            isTransaction = true;
        }
    }
}