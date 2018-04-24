using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class ResourceImpl : RootElementImpl, Resources
    {

        protected internal static IAttribute/*<string>*/ NameAttribute;
        protected internal static IChildElementCollection/*<IResourceParameter>*/ ResourceParameterCollection;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<Resources>(/*typeof(Resources),*/ BpmnModelConstants.BpmnElementResource)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IRootElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            NameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeName).Required().Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            ResourceParameterCollection = sequenceBuilder.ElementCollection<IResourceParameter>(/*typeof(IResourceParameter)*/).Build/*<IResourceParameter>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<Resources>
        {
            public virtual Resources NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ResourceImpl(instanceContext);
            }
        }

        public ResourceImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public virtual string Name
        {
            get => NameAttribute.GetValue<String>(this);
            set => NameAttribute.SetValue(this, value);
        }

        public virtual ICollection<IResourceParameter> ResourceParameters => ResourceParameterCollection.Get<IResourceParameter>(this);
    }
}