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
    public class RelationshipImpl : BaseElementImpl, IRelationship
    {

        protected internal static IAttribute/*<string>*/ TypeAttribute;
        protected internal static IAttribute/*<RelationshipDirection>*/ DirectionAttribute;
        protected internal static IChildElementCollection/*<Source>*/ SourceCollection;
        protected internal static IChildElementCollection/*<Target>*/ TargetCollection;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IRelationship>(/*typeof(IRelationship),*/ BpmnModelConstants.BpmnElementRelationship)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            TypeAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeType).Required().Build();

            DirectionAttribute = typeBuilder.EnumAttribute<RelationshipDirection>(BpmnModelConstants.BpmnAttributeDirection/*, typeof(RelationshipDirection)*/).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            SourceCollection = sequenceBuilder.ElementCollection<Source>(/*typeof(Source)*/).MinOccurs(1).Build/*<Source>*/();

            TargetCollection = sequenceBuilder.ElementCollection<Target>(/*typeof(Target)*/).MinOccurs(1).Build/*<Target>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IRelationship>
        {
            public virtual IRelationship NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new RelationshipImpl(instanceContext);
            }
        }

        public RelationshipImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string Type
        {
            get => TypeAttribute.GetValue<String>(this);
            set => TypeAttribute.SetValue(this, value);
        }


        public virtual RelationshipDirection Direction
        {
            get => DirectionAttribute.GetValue<RelationshipDirection>(this);
            set => DirectionAttribute.SetValue(this, value);
        }

        public virtual ICollection<Source> Sources => SourceCollection.Get<Source>(this);

        public virtual ICollection<Target> Targets => TargetCollection.Get<Target>(this);
    }
}