using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.builder;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.util;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;


namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class ProcessImpl : CallableElementImpl, IProcess
    {
        protected internal static IAttribute/*<ProcessType>*/ ProcessTypeAttribute;
        protected internal static IAttribute/*<bool>*/ IsClosedAttribute;
        protected internal static IAttribute/*<bool>*/ IsExecutableAttribute;
        // TODO: definitionalCollaborationRef
        protected internal static IChildElement/*<IAuditing>*/ AuditingChild;
        protected internal static IChildElement/*<IMonitoring>*/ MonitoringChild;
        protected internal static IChildElementCollection/*<IProperty>*/ PropertyCollection;
        protected internal static IChildElementCollection/*<ILaneSet>*/ LaneSetCollection;
        protected internal static IChildElementCollection/*<IFlowElement>*/ FlowElementCollection;
        protected internal static IChildElementCollection/*<IArtifact>*/ ArtifactCollection;
        protected internal static IChildElementCollection/*<IResourceRole>*/ ResourceRoleCollection;
        protected internal static IChildElementCollection/*<ICorrelationSubscription>*/ CorrelationSubscriptionCollection;
        protected internal static IElementReferenceCollection SupportsCollection;//IElementReferenceCollection<IProcess, Supports>


        protected internal static IAttribute/*<string>*/ CamundaCandidateStarterGroupsAttribute;
        protected internal static IAttribute/*<string>*/ CamundaCandidateStarterUsersAttribute;
        protected internal static IAttribute/*<string>*/ CamundaJobPriorityAttribute;
        protected internal static IAttribute/*<string>*/ CamundaTaskPriorityAttribute;
        protected internal static IAttribute/*<string>*/ CamundaHistoryTimeToLiveAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IProcess>(/*typeof(IProcess),*/ BpmnModelConstants.BpmnElementProcess)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(ICallableElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ProcessTypeAttribute = typeBuilder
                .EnumAttribute<ProcessType>(BpmnModelConstants.BpmnAttributeProcessType/*, typeof(ProcessType)*/)
                .DefaultValue(ProcessType.None)
                .Build();

            IsClosedAttribute = typeBuilder
                .BooleanAttribute(BpmnModelConstants.BpmnAttributeIsClosed)
                .DefaultValue(false)
                .Build();

            IsExecutableAttribute = typeBuilder
                .BooleanAttribute(BpmnModelConstants.BpmnAttributeIsExecutable)
                .Build();

            // TODO: definitionalCollaborationRef

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            AuditingChild = sequenceBuilder
                .Element<IAuditing>(/*typeof(IAuditing)*/)
                .Build/*<IAuditing>*/();

            MonitoringChild = sequenceBuilder
                .Element<IMonitoring>(/*typeof(IMonitoring)*/)
                .Build/*<IMonitoring>*/();

            PropertyCollection = sequenceBuilder
                .ElementCollection<IProperty>(/*typeof(IProperty)*/)
                .Build/*<IProperty>*/();

            LaneSetCollection = sequenceBuilder
                .ElementCollection<ILaneSet>(/*typeof(ILaneSet)*/)
                .Build/*<ILaneSet>*/();

            FlowElementCollection = sequenceBuilder
                .ElementCollection<IFlowElement>(/*typeof(IFlowElement)*/)
                .Build/*<IFlowElement>*/();

            ArtifactCollection = sequenceBuilder
                .ElementCollection<IArtifact>(/*typeof(IArtifact)*/)
                .Build/*<IArtifact>*/();

            ResourceRoleCollection = sequenceBuilder
                .ElementCollection<IResourceRole>(/*typeof(IResourceRole)*/)
                .Build/*<IResourceRole>*/();

            CorrelationSubscriptionCollection = sequenceBuilder
                .ElementCollection<ICorrelationSubscription>(/*typeof(ICorrelationSubscription)*/)
                .Build/*<ICorrelationSubscription>*/();

            SupportsCollection = sequenceBuilder.ElementCollection<Supports>(/*typeof(Supports)*/)
                .QNameElementReferenceCollection<IProcess>(/*typeof(IProcess)*/)
                .Build();

            CamundaCandidateStarterGroupsAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.CamundaAttributeCandidateStarterGroups)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            CamundaCandidateStarterUsersAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.CamundaAttributeCandidateStarterUsers)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            CamundaJobPriorityAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.CamundaAttributeJobPriority)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            CamundaTaskPriorityAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.CamundaAttributeTaskPriority)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            CamundaHistoryTimeToLiveAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.CamundaAttributeHistoryTimeToLive)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IProcess>
        {
            public virtual IProcess NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ProcessImpl(instanceContext);
            }
        }

        public ProcessImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public IProcessBuilder Builder()
        {
            return new ProcessBuilder((IBpmnModelInstance)modelInstance, this);
        }

        public virtual ProcessType ProcessType
        {
            get => ProcessTypeAttribute.GetValue<ProcessType>(this);
            set => ProcessTypeAttribute.SetValue(this, value);
        }


        public virtual bool Closed
        {
            get => IsClosedAttribute.GetValue<Boolean>(this);
            set => IsClosedAttribute.SetValue(this, value);
        }


        public virtual bool Executable
        {
            get => IsExecutableAttribute.GetValue<Boolean>(this);
            set => IsExecutableAttribute.SetValue(this, value);
        }


        public virtual IAuditing Auditing
        {
            get => (IAuditing)AuditingChild.GetChild(this);
            set => AuditingChild.SetChild(this, value);
        }


        public virtual IMonitoring Monitoring
        {
            get => (IMonitoring)MonitoringChild.GetChild(this);
            set => MonitoringChild.SetChild(this, value);
        }


        public virtual ICollection<IProperty> Properties => PropertyCollection.Get<IProperty>(this);

        public virtual ICollection<ILaneSet> LaneSets => LaneSetCollection.Get<ILaneSet>(this);

        public virtual ICollection<IFlowElement> FlowElements => FlowElementCollection.Get<IFlowElement>(this);

        public virtual ICollection<IArtifact> Artifacts => ArtifactCollection.Get<IArtifact>(this);

        public virtual ICollection<ICorrelationSubscription> CorrelationSubscriptions => CorrelationSubscriptionCollection.Get<ICorrelationSubscription>(this);

        public virtual ICollection<IResourceRole> ResourceRoles => ResourceRoleCollection.Get<IResourceRole>(this);

        public virtual ICollection<IProcess> Supports => SupportsCollection.GetReferenceTargetElements<IProcess>(this);

        /// <summary>
        /// camunda extensions </summary>

        public virtual string CamundaCandidateStarterGroups
        {
            get => CamundaCandidateStarterGroupsAttribute.GetValue<String>(this);
            set => CamundaCandidateStarterGroupsAttribute.SetValue(this, value);
        }


        public virtual IList<string> CamundaCandidateStarterGroupsList
        {
            get
            {
                string groupsString = CamundaCandidateStarterGroupsAttribute.GetValue<String>(this);
                return StringUtil.SplitCommaSeparatedList(groupsString);
            }
            set
            {
                string candidateStarterGroups = StringUtil.JoinCommaSeparatedList(value);
                CamundaCandidateStarterGroupsAttribute.SetValue(this, candidateStarterGroups);
            }
        }


        public virtual string CamundaCandidateStarterUsers
        {
            get => CamundaCandidateStarterUsersAttribute.GetValue<String>(this);
            set => CamundaCandidateStarterUsersAttribute.SetValue(this, value);
        }


        public virtual IList<string> CamundaCandidateStarterUsersList
        {
            get
            {
                string candidateStarterUsers = CamundaCandidateStarterUsersAttribute.GetValue<String>(this);
                return StringUtil.SplitCommaSeparatedList(candidateStarterUsers);
            }
            set
            {
                string candidateStarterUsers = StringUtil.JoinCommaSeparatedList(value);
                CamundaCandidateStarterUsersAttribute.SetValue(this, candidateStarterUsers);
            }
        }


        public virtual string CamundaJobPriority
        {
            get => CamundaJobPriorityAttribute.GetValue<String>(this);
            set => CamundaJobPriorityAttribute.SetValue(this, value);
        }


        public virtual string CamundaTaskPriority
        {
            get => CamundaTaskPriorityAttribute.GetValue<String>(this);
            set => CamundaTaskPriorityAttribute.SetValue(this, value);
        }


        public int? CamundaHistoryTimeToLive
        {
            get
            {
                string ttl = CamundaHistoryTimeToLiveString;
                if (ttl != null)
                {
                    return int.Parse(ttl);
                }
                return null;
            }
            set => CamundaHistoryTimeToLiveString = value.ToString();
        }
        

        public string CamundaHistoryTimeToLiveString
        {
            get => CamundaHistoryTimeToLiveAttribute.GetValue<String>(this);
            set => CamundaHistoryTimeToLiveAttribute.SetValue(this, value);
        }

        
    }
}