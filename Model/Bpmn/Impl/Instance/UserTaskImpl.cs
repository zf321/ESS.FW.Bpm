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



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class UserTaskImpl : TaskImpl, IUserTask
    {

        protected internal static IAttribute/*<string>*/ ImplementationAttribute;
        protected internal static IChildElementCollection/*<IRendering>*/ RenderingCollection;
        
        protected internal static IAttribute/*<string>*/ CamundaAssigneeAttribute;
        protected internal static IAttribute/*<string>*/ CamundaCandidateGroupsAttribute;
        protected internal static IAttribute/*<string>*/ CamundaCandidateUsersAttribute;
        protected internal static IAttribute/*<string>*/ CamundaDueDateAttribute;
        protected internal static IAttribute/*<string>*/ CamundaFollowUpDateAttribute;
        protected internal static IAttribute/*<string>*/ CamundaFormHandlerClassAttribute;
        protected internal static IAttribute/*<string>*/ CamundaFormKeyAttribute;
        protected internal static IAttribute/*<string>*/ CamundaPriorityAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IUserTask>(/*typeof(IUserTask),*/ BpmnModelConstants.BpmnElementUserTask)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(ITask))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ImplementationAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeImplementation).DefaultValue("##unspecified").Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            RenderingCollection = sequenceBuilder.ElementCollection<IRendering>(/*typeof(IRendering)*/).Build/*<IRendering>*/();
            
            CamundaAssigneeAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeAssignee).Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaCandidateGroupsAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.CamundaAttributeCandidateGroups)
                .Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaCandidateUsersAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.CamundaAttributeCandidateUsers)
                .Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaDueDateAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeDueDate)
                .Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaFollowUpDateAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeFollowUpDate)
                .Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaFormHandlerClassAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.CamundaAttributeFormHandlerClass)
                .Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaFormKeyAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeFormKey)
                .Namespace(BpmnModelConstants.CamundaNs).Build();

            CamundaPriorityAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributePriority)
                .Namespace(BpmnModelConstants.CamundaNs).Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IUserTask>
        {
            public virtual IUserTask NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new UserTaskImpl(instanceContext);
            }
        }

        public UserTaskImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public new UserTaskBuilder Builder()
        {
            return new UserTaskBuilder((IBpmnModelInstance)modelInstance, this);
        }

        public virtual string Implementation
        {
            get => ImplementationAttribute.GetValue<String>(this);
            set => ImplementationAttribute.SetValue(this, value);
        }

        public virtual ICollection<IRendering> Renderings => RenderingCollection.Get<IRendering>(this);

        /// <summary>
        /// camunda extensions </summary>

        public virtual string CamundaAssignee
        {
            get => CamundaAssigneeAttribute.GetValue<String>(this);
            set => CamundaAssigneeAttribute.SetValue(this, value);
        }


        public virtual string CamundaCandidateGroups
        {
            get => CamundaCandidateGroupsAttribute.GetValue<String>(this);
            set => CamundaCandidateGroupsAttribute.SetValue(this, value);
        }


        public virtual IList<string> CamundaCandidateGroupsList
        {
            get
            {
                string candidateGroups = CamundaCandidateGroupsAttribute.GetValue<String>(this);
                return StringUtil.SplitCommaSeparatedList(candidateGroups);
            }
            set
            {
                string candidateGroups = StringUtil.JoinCommaSeparatedList(value);
                CamundaCandidateGroupsAttribute.SetValue(this, candidateGroups);
            }
        }


        public virtual string CamundaCandidateUsers
        {
            get => CamundaCandidateUsersAttribute.GetValue<String>(this);
            set => CamundaCandidateUsersAttribute.SetValue(this, value);
        }


        public virtual IList<string> CamundaCandidateUsersList
        {
            get
            {
                string candidateUsers = CamundaCandidateUsersAttribute.GetValue<String>(this);
                return StringUtil.SplitCommaSeparatedList(candidateUsers);
            }
            set
            {
                string candidateUsers = StringUtil.JoinCommaSeparatedList(value);
                CamundaCandidateUsersAttribute.SetValue(this, candidateUsers);
            }
        }


        public virtual string CamundaDueDate
        {
            get => CamundaDueDateAttribute.GetValue<String>(this);
            set => CamundaDueDateAttribute.SetValue(this, value);
        }


        public virtual string CamundaFollowUpDate
        {
            get => CamundaFollowUpDateAttribute.GetValue<String>(this);
            set => CamundaFollowUpDateAttribute.SetValue(this, value);
        }


        public virtual string CamundaFormHandlerClass
        {
            get => CamundaFormHandlerClassAttribute.GetValue<String>(this);
            set => CamundaFormHandlerClassAttribute.SetValue(this, value);
        }


        public virtual string CamundaFormKey
        {
            get => CamundaFormKeyAttribute.GetValue<String>(this);
            set => CamundaFormKeyAttribute.SetValue(this, value);
        }


        public virtual string CamundaPriority
        {
            get => CamundaPriorityAttribute.GetValue<String>(this);
            set => CamundaPriorityAttribute.SetValue(this, value);
        }
    }
}