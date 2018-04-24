using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class ResourceRoleImpl : BaseElementImpl, IResourceRole
    {

        protected internal static IAttribute/*<string>*/ NameAttribute;
        protected internal static IElementReference ResourceRefChild;//IElementReference<Resources, ResourceRef>
        protected internal static IChildElementCollection/*<IResourceParameterBinding>*/ ResourceParameterBindingCollection;
        protected internal static IChildElement/*<IResourceAssignmentExpression>*/ ResourceAssignmentExpressionChild;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IResourceRole>(/*typeof(IResourceRole), */BpmnModelConstants.BpmnElementResourceRole)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            NameAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeName)
                .Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            ResourceRefChild = sequenceBuilder
                .Element<ResourceRef>(/*typeof(ResourceRef)*/)
                .QNameElementReference<Resources>(/*typeof(Resources)*/)
                .Build();

            ResourceParameterBindingCollection = sequenceBuilder
                .ElementCollection<IResourceParameterBinding>(/*typeof(IResourceParameterBinding)*/)
                .Build/*<IResourceParameterBinding>*/();

            ResourceAssignmentExpressionChild = sequenceBuilder
                .Element<IResourceAssignmentExpression>(/*typeof(IResourceAssignmentExpression)*/)
                .Build/*<IResourceAssignmentExpression>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IResourceRole>
        {
            public virtual IResourceRole NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ResourceRoleImpl(instanceContext);
            }
        }

        public ResourceRoleImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string Name
        {
            get => NameAttribute.GetValue<String>(this);
            set => NameAttribute.SetValue(this, value);
        }


        public virtual Resources Resource
        {
            get => (Resources)ResourceRefChild.GetReferenceTargetElement(this);
            set => ResourceRefChild.SetReferenceTargetElement(this, value);
        }


        public virtual ICollection<IResourceParameterBinding> ResourceParameterBinding => ResourceParameterBindingCollection.Get<IResourceParameterBinding>(this);

        public virtual IResourceAssignmentExpression ResourceAssignmentExpression => (IResourceAssignmentExpression)ResourceAssignmentExpressionChild.GetChild(this);
    }
}