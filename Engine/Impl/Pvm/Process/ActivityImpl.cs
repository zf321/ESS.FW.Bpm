using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Helper;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Core.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Process
{
    /// <summary>
    ///      
    /// </summary>
    [Serializable]
    public class ActivityImpl : ScopeImpl, IPvmActivity, IHasDiBounds
    {


        /// <summary>
        ///     the inner behavior of an activity. For activities which are flow scopes,
        ///     this must be a CompositeActivityBehavior.
        /// </summary>
        protected internal IActivityBehavior activityBehavior;
        /// <summary>
        ///     The start behavior for this activity.
        /// </summary>
        protected internal ActivityStartBehavior activityStartBehavior = ActivityStartBehavior.Default;

        /// <summary>
        ///     The delegate for the async after attribute update.
        /// </summary>
        protected internal IAsyncAfterUpdate delegateAsyncAfterUpdate;

        //============================================================================
        //===============================DELEGATES====================================
        //============================================================================
        /// <summary>
        ///     The delegate for the async before attribute update.
        /// </summary>
        protected internal IAsyncBeforeUpdate delegateAsyncBeforeUpdate;

        protected internal ScopeImpl eventScope;
        protected internal ScopeImpl flowScope;
        protected internal int height = -1;
        protected internal bool IsAsyncAfter;

        protected internal bool IsAsyncBefore;

        protected internal IDictionary<string, TransitionImpl> NamedOutgoingTransitions =
            new Dictionary<string, TransitionImpl>();
        
        protected internal int width = -1;

        // Graphical information ///////////////////////////////////////////

        protected internal int x = -1;
        protected internal int y = -1;

        public ActivityImpl(string id, ProcessDefinitionImpl processDefinition) : base(id, processDefinition)
        {
        }

        // restricted setters ///////////////////////////////////////////////////////

        public virtual IList<IPvmTransition> OutgoingTransitions { get; set; } = new List<IPvmTransition>();

        public virtual IList<IPvmTransition> IncomingTransitions { get; set; } = new List<IPvmTransition>();

        public virtual string ActivityId
        {
            get { return Id; }
        }


        public virtual ActivityImpl ParentFlowScopeActivity
        {
            get
            {
                var flowScope = FlowScope;
                if (flowScope != ProcessDefinition)
                    return (ActivityImpl) flowScope;
                return null;
            }
        }

        /// <summary>
        ///     Indicates whether activity is for compensation.
        /// </summary>
        /// <returns> true if this activity is for compensation. </returns>
        public virtual bool CompensationHandler
        {
            get
            {
                var isForCompensation = (bool?) GetProperty(BpmnParse.PropertynameIsForCompensation);
                return true.Equals(isForCompensation);
            }
        }

        /// <summary>
        ///     Indicates whether activity is a multi instance activity.
        /// </summary>
        /// <returns> true if this activity is a multi instance activity. </returns>
        public virtual bool MultiInstance
        {
            get
            {
                var isMultiInstance = (bool?) GetProperty(BpmnParse.PropertynameIsMultiInstance);
                return true.Equals(isMultiInstance);
            }
        }

        public virtual bool TriggeredByEvent
        {
            get
            {
                bool? isTriggeredByEvent = Properties.Get(BpmnProperties.TriggeredByEvent);
                return true.Equals(isTriggeredByEvent);
            }
        }

        public virtual IAsyncBeforeUpdate DelegateAsyncBeforeUpdate
        {
            get { return delegateAsyncBeforeUpdate; }
            set { delegateAsyncBeforeUpdate = value; }
        }


        public virtual IAsyncAfterUpdate DelegateAsyncAfterUpdate
        {
            get { return delegateAsyncAfterUpdate; }
            set { delegateAsyncAfterUpdate = value; }
        }
        

        public virtual int X
        {
            get { return x; }
            set { x = value; }
        }


        public virtual int Y
        {
            get { return y; }
            set { y = value; }
        }


        public virtual int Width
        {
            get { return width; }
            set { width = value; }
        }


        public virtual int Height
        {
            get { return height; }
            set { height = value; }
        }

        // getters and setters //////////////////////////////////////////////////////

        IActivityBehavior IPvmActivity.ActivityBehavior
        {
            get { return activityBehavior; }
        }

        public virtual ActivityStartBehavior ActivityStartBehavior
        {
            get { return activityStartBehavior; }
            set { activityStartBehavior = value; }
        }


        public override bool IsScope
        {
            get;
            set;
        } = false;


        public virtual bool AsyncBefore
        {
            get { return IsAsyncBefore; }
            set { SetAsyncBefore(value, true); }
        }

        public virtual bool AsyncAfter
        {
            get { return IsAsyncAfter; }
            set { SetAsyncAfter(value, true); }
        }

        public override ScopeImpl FlowScope
        {
            get { return flowScope; }
        }

        public override IPvmScope EventScope
        {
            get { return eventScope; }
            set
            {
                if (eventScope != null)
                    eventScope.eventActivities.Remove(this);

                eventScope = (ScopeImpl) value;

                if (value != null)
                    eventScope.eventActivities.Add(this);
            }
        }


        public override IPvmScope LevelOfSubprocessScope
        {
            get
            {
                var levelOfSubprocessScope = FlowScope;
                while (!levelOfSubprocessScope.IsSubProcessScope)
                    levelOfSubprocessScope = ((IPvmActivity) levelOfSubprocessScope).FlowScope;
                return levelOfSubprocessScope;
            }
        }

        /// <summary>
        /// 异常。类型错误 用Get/SetActivityBehavior
        /// </summary>
        public override IActivityBehavior ActivityBehavior
        {
            get
            {
                return activityBehavior;
            } 
            set { activityBehavior = value; }
        }
        public IActivityBehavior GetActivityBehavior()
        {
            return activityBehavior;
        }
        public void SetActivityBehavior(IActivityBehavior coreActivityBehavior)
        {
            this.activityBehavior = coreActivityBehavior;
        }

        public virtual TransitionImpl CreateOutgoingTransition()
        {
            return CreateOutgoingTransition(null);
        }

        public virtual TransitionImpl CreateOutgoingTransition(string transitionId)
        {
            var transition = new TransitionImpl(transitionId, processDefinition);
            transition.Source = this;
            OutgoingTransitions.Add(transition);

            if (!ReferenceEquals(transitionId, null))
            {
                if (NamedOutgoingTransitions.ContainsKey(transitionId))
                    throw new PvmException("activity '" + id + " has duplicate transition '" + transitionId + "'");
                NamedOutgoingTransitions[transitionId] = transition;
            }

            return transition;
        }

        public virtual IPvmTransition FindOutgoingTransition(string transitionId)
        {
            return NamedOutgoingTransitions[transitionId];
        }

        public override string ToString()
        {
            return "Activity(" + id + ")";
        }


        public virtual void SetAsyncBefore(bool isAsyncBefore, bool exclusive)
        {
            if (delegateAsyncBeforeUpdate != null)
                delegateAsyncBeforeUpdate.UpdateAsyncBefore(isAsyncBefore, exclusive);
            this.IsAsyncBefore = isAsyncBefore;
        }


        public virtual void SetAsyncAfter(bool isAsyncAfter, bool exclusive)
        {
            if (delegateAsyncAfterUpdate != null)
                delegateAsyncAfterUpdate.UpdateAsyncAfter(isAsyncAfter, exclusive);
            this.IsAsyncAfter = isAsyncAfter;
        }

        /// <summary>
        ///     Find the compensation handler of this activity.
        /// </summary>
        /// <returns> the compensation handler or <code>null</code>, if this activity has no compensation handler. </returns>
        public virtual ActivityImpl findCompensationHandler()
        {
            var compensationHandlerId = (string) GetProperty(BpmnParse.PropertynameCompensationHandlerId);
            if (!ReferenceEquals(compensationHandlerId, null))
                return (ActivityImpl) ProcessDefinition.FindActivity(compensationHandlerId);
            return null;
        }

        public IList<ActivityImpl> GetActivities()
        {
            return FlowActivities;
        }


        /// <summary>
        ///     Delegate interface for the asyncBefore property update.
        /// </summary>
        public interface IAsyncBeforeUpdate
        {
            /// <summary>
            ///     Method which is called if the asyncBefore property should be updated.
            /// </summary>
            /// <param name="asyncBefore"> the new value for the asyncBefore flag </param>
            /// <param name="exclusive"> the exclusive flag </param>
            void UpdateAsyncBefore(bool asyncBefore, bool exclusive);
        }

        /// <summary>
        ///     Delegate interface for the asyncAfter property update
        /// </summary>
        public interface IAsyncAfterUpdate
        {
            /// <summary>
            ///     Method which is called if the asyncAfter property should be updated.
            /// </summary>
            /// <param name="asyncAfter"> the new value for the asyncBefore flag </param>
            /// <param name="exclusive"> the exclusive flag </param>
            void UpdateAsyncAfter(bool asyncAfter, bool exclusive);
        }
        public bool IsInTransaction
        {
            get
            {
                return FlowScope.IsTransaction;
            }
        }
    }
}