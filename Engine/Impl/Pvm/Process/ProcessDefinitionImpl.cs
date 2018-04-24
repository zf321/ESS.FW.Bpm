using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using ESS.FW.Bpm.Engine.Impl.Util;
using System.ComponentModel.DataAnnotations.Schema;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using Newtonsoft.Json;

namespace ESS.FW.Bpm.Engine.Impl.Pvm.Process
{

    /// <summary>
    ///      
    ///     
    /// </summary>
    [Serializable]
    public class ProcessDefinitionImpl : ScopeImpl, IPvmProcessDefinition
    {

        protected internal string description;
        protected internal ActivityImpl initial;

        protected internal IDictionary<ActivityImpl, IList<ActivityImpl>> InitialActivityStacks =
            new Dictionary<ActivityImpl, IList<ActivityImpl>>();

        private bool _isScope;

        protected internal IList<LaneSet> laneSets;

        //protected internal new string Name;
        protected internal ParticipantProcess participantProcess;

        public ProcessDefinitionImpl(string id) : base(id, null)
        {
            processDefinition = this;
            // the process definition is always "a sub process scope"
            IsSubProcessScope = true;
        }
        [NotMapped]
        [JsonIgnore]
        public virtual IList<ActivityImpl> InitialActivityStack
        {
            get { return GetInitialActivityStack(initial); }
        }
        //public override ICoreActivityBehavior<IBaseDelegateExecution> ActivityBehavior
        //{
        //    get
        //    {
        //        // unsupported in PVM
        //        return null;
        //    }
        //    set { throw new NotImplementedException(); }
        //}
        public override IActivityBehavior ActivityBehavior { get; set; }

        // getters and setters //////////////////////////////////////////////////////
        [NotMapped]
        public virtual IPvmActivity Initial
        {
            get { return initial; }
            set { initial = (ActivityImpl)value; }
        }

        /// <returns> all lane-sets defined on this process-instance. Returns an empty list if none are defined. </returns>
        public virtual IList<LaneSet> LaneSets
        {
            get
            {
                if (laneSets == null)
                    laneSets = new List<LaneSet>();
                return laneSets;
            }
        }

        [NotMapped]
        public virtual ParticipantProcess ParticipantProcess
        {
            set { participantProcess = value; }
            get { return participantProcess; }
        }


        public virtual IPvmProcessInstance CreateProcessInstance()
        {
            EnsureDefaultInitialExists();
            return CreateProcessInstance(null, null, initial);
        }

        public virtual IPvmProcessInstance CreateProcessInstance(string businessKey)
        {
            EnsureDefaultInitialExists();
            return CreateProcessInstance(businessKey, null, initial);
        }

        public virtual IPvmProcessInstance CreateProcessInstance(string businessKey, string caseInstanceId)
        {
            EnsureDefaultInitialExists();
            return CreateProcessInstance(businessKey, caseInstanceId, initial);
        }
        public virtual string DiagramResourceName { get; set; }

        public virtual string DeploymentId { get; set; }

        /// <summary>
        ///     获取Properties的key为documentation的value
        /// </summary>
        public virtual string Description
        {
            get
            {
               return  GetProperty("documentation").ToString();
            }
        }

        public override bool IsScope
        {
            get { return true; }
            set { _isScope = value; }
        }

        public override IPvmScope EventScope { get; set; }

        public override ScopeImpl FlowScope => null;

        public override IPvmScope LevelOfSubprocessScope => null;

        public override bool SubProcessScope => true;

        
        protected internal virtual void EnsureDefaultInitialExists()
        {
            EnsureUtil.EnsureNotNull(
                "Process '" + name +
                "' has no default start activity (e.g. none start event), hence you cannot use 'startProcessInstanceBy...' but have to start it using one of the modeled start events (e.g. message start events)",
                "initial", initial);
        }

        public virtual IPvmProcessInstance CreateProcessInstance(string businessKey, ActivityImpl init)
        {
            return CreateProcessInstance(businessKey, null, init);
        }

        public virtual IPvmProcessInstance CreateProcessInstance(string businessKey, string caseInstanceId,
            ActivityImpl init)
        {
            var processInstance = (PvmExecutionImpl) CreateProcessInstanceForInitial(init);

            processInstance.BusinessKey = businessKey;
            processInstance.CaseInstanceId = caseInstanceId;

            return processInstance;
        }

        /// <summary>
        ///     creates a process instance using the provided activity as initial
        /// </summary>
        public virtual IPvmProcessInstance CreateProcessInstanceForInitial(IPvmActivity init)
        {
            EnsureUtil.EnsureNotNull(
                "Cannot start process instance, initial activity where the process instance should start is null",
                "initial", init);
            //坑 这样申明JAVA中会走子类ExecutionEntity里的方法 processInstance等属性的set方法...
            PvmExecutionImpl processInstance;
            if (this is ProcessDefinitionEntity)
            {
                processInstance = (ExecutionEntity)NewProcessInstance();
            }
            else
            {
                processInstance = NewProcessInstance();
            }
            //PvmExecutionImpl processInstance = NewProcessInstance();
            //ExecutionEntity processInstance = (ExecutionEntity)NewProcessInstance();

            processInstance.ProcessDefinition = this;

            processInstance.ProcessInstance = processInstance;

            // always set the process instance to the initial activity, no matter how deeply it is nested;
            // this is required for firing history events (cf start activity) and persisting the initial activity
            // on async start
            processInstance.Activity=(init);

            return processInstance;
        }

        protected internal virtual PvmExecutionImpl NewProcessInstance()
        {
            return new ExecutionImpl();
        }

        public virtual IList<ActivityImpl> GetInitialActivityStack(ActivityImpl startActivity)
        {
            lock (this)
            {
                var initialActivityStack = InitialActivityStacks[startActivity];
                if (initialActivityStack == null)
                {
                    initialActivityStack = new List<ActivityImpl>();
                    var activity = startActivity;
                    while (activity != null)
                    {
                        initialActivityStack.Insert(0, activity);
                        activity = activity.ParentFlowScopeActivity;
                    }
                    InitialActivityStacks[startActivity] = initialActivityStack;
                }
                return initialActivityStack;
            }
        }

        public virtual void AddLaneSet(LaneSet newLaneSet)
        {
            LaneSets.Add(newLaneSet);
        }

        public virtual Lane GetLaneForId(string id)
        {
            if ((laneSets != null) && (laneSets.Count > 0))
            {
                Lane lane;
                foreach (var set in laneSets)
                {
                    lane = set.GetLaneForId(id);
                    if (lane != null)
                        return lane;
                }
            }
            return null;
        }


        public override string ToString()
        {
            return "ProcessDefinition(" + id + ")";
        }
    }
}