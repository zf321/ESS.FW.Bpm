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
    public class CorrelationKeyImpl : BaseElementImpl, ICorrelationKey
    {

        protected internal static IAttribute/*<string>*/ NameAttribute;
        protected internal static IElementReferenceCollection CorrelationPropertyRefCollection;//IElementReferenceCollection<ICorrelationProperty, CorrelationPropertyRef>

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICorrelationKey>(/*typeof(ICorrelationKey),*/ BpmnModelConstants.BpmnElementCorrelationKey)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .ExtendsType(typeof(IBaseElement))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            NameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeName).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            CorrelationPropertyRefCollection = sequenceBuilder
                .ElementCollection<CorrelationPropertyRef>(/*typeof(CorrelationPropertyRef)*/)
                .QNameElementReferenceCollection<ICorrelationProperty>(/*typeof(ICorrelationProperty)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICorrelationKey>
        {

            public virtual ICorrelationKey NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CorrelationKeyImpl(instanceContext);
            }
        }


        public CorrelationKeyImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string Name
        {
            get => NameAttribute.GetValue<String>(this);
            set => NameAttribute.SetValue(this, value);
        }

        public virtual ICollection<ICorrelationProperty> CorrelationProperties => CorrelationPropertyRefCollection.GetReferenceTargetElements<ICorrelationProperty>(this);
    }

}