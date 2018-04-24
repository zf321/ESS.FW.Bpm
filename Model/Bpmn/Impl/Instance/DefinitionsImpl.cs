using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;


namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class DefinitionsImpl : BpmnModelElementInstanceImpl, IDefinitions
    {

        protected internal static IAttribute/*<string>*/ IdAttribute;
        protected internal static IAttribute/*<string>*/ NameAttribute;
        protected internal static IAttribute/*<string>*/ TargetNamespaceAttribute;
        protected internal static IAttribute/*<string>*/ ExpressionLanguageAttribute;
        protected internal static IAttribute/*<string>*/ TypeLanguageAttribute;
        protected internal static IAttribute/*<string>*/ ExporterAttribute;
        protected internal static IAttribute/*<string>*/ ExporterVersionAttribute;

        protected internal static IChildElementCollection/*<IMport>*/ ImportCollection;
        protected internal static IChildElementCollection/*<IExtension>*/ ExtensionCollection;
        protected internal static IChildElementCollection/*<IRootElement>*/ RootElementCollection;
        protected internal static IChildElementCollection/*<IBpmnDiagram>*/ BpmnDiagramCollection;
        protected internal static IChildElementCollection/*<IRelationship>*/ RelationshipCollection;

        public new static void RegisterType(ModelBuilder bpmnModelBuilder)
        {

            IModelElementTypeBuilder typeBuilder = bpmnModelBuilder.DefineType<IDefinitions>(/*typeof(IDefinitions),*/ BpmnModelConstants.BpmnElementDefinitions)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            IdAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeId).IdAttribute().Build();

            NameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeName).Build();

            TargetNamespaceAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeTargetNamespace).Required().Build();

            ExpressionLanguageAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeExpressionLanguage).DefaultValue(BpmnModelConstants.XpathNs).Build();

            TypeLanguageAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeTypeLanguage).DefaultValue(BpmnModelConstants.XmlSchemaNs).Build();

            ExporterAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeExporter).Build();

            ExporterVersionAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeExporterVersion).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            ImportCollection = sequenceBuilder.ElementCollection<IMport>(/*typeof(IMport)*/).Build/*<IMport>*/();

            ExtensionCollection = sequenceBuilder.ElementCollection<IExtension>(/*typeof(IExtension)*/).Build/*<IExtension>*/();

            RootElementCollection = sequenceBuilder.ElementCollection<IRootElement>(/*typeof(IRootElement)*/).Build/*<IRootElement>*/();

            BpmnDiagramCollection = sequenceBuilder.ElementCollection<IBpmnDiagram>(/*typeof(IBpmnDiagram)*/).Build/*<IBpmnDiagram>*/();

            RelationshipCollection = sequenceBuilder.ElementCollection<IRelationship>(/*typeof(IRelationship)*/).Build/*<IRelationship>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IDefinitions>
        {
            public virtual IDefinitions NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new DefinitionsImpl(instanceContext);
            }
        }

        public DefinitionsImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public override string Id
        {
            get => IdAttribute.GetValue<String>(this);
            set => IdAttribute.SetValue(this, value);
        }


        public virtual string Name
        {
            get => NameAttribute.GetValue<String>(this);
            set => NameAttribute.SetValue(this, value);
        }


        public virtual string TargetNamespace
        {
            get => TargetNamespaceAttribute.GetValue<String>(this);
            set => TargetNamespaceAttribute.SetValue(this, value);
        }


        public virtual string ExpressionLanguage
        {
            get => ExpressionLanguageAttribute.GetValue<String>(this);
            set => ExpressionLanguageAttribute.SetValue(this, value);
        }


        public virtual string TypeLanguage
        {
            get => TypeLanguageAttribute.GetValue<String>(this);
            set => TypeLanguageAttribute.SetValue(this, value);
        }


        public virtual string Exporter
        {
            get => ExporterAttribute.GetValue<String>(this);
            set => ExporterAttribute.SetValue(this, value);
        }


        public virtual string ExporterVersion
        {
            get => ExporterVersionAttribute.GetValue<String>(this);
            set => ExporterVersionAttribute.SetValue(this, value);
        }


        public virtual ICollection<IMport> Imports => ImportCollection.Get<IMport>(this);

        public virtual ICollection<IExtension> Extensions => ExtensionCollection.Get<IExtension>(this);

        public virtual ICollection<IRootElement> RootElements => RootElementCollection.Get<IRootElement>(this);

        public virtual ICollection<IBpmnDiagram> BpmDiagrams => BpmnDiagramCollection.Get<IBpmnDiagram>(this);

        public virtual ICollection<IRelationship> Relationships => RelationshipCollection.Get<IRelationship>(this);
    }

}