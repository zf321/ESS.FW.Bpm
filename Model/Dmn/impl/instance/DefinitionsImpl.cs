using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;



namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    
    public class DefinitionsImpl : NamedElementImpl, IDefinitions
    {

        protected internal static IAttribute/*<string>*/ ExpressionLanguageAttribute;
        protected internal static IAttribute/*<string>*/ TypeLanguageAttribute;
        protected internal static IAttribute/*<string>*/ NamespaceAttribute;
        protected internal static IAttribute/*<string>*/ ExporterAttribute;
        protected internal static IAttribute/*<string>*/ ExporterVersionAttribute;

        protected internal static IChildElementCollection/*<IMport>*/ ImportCollection;
        protected internal static IChildElementCollection/*<ITemDefinition>*/ ItemDefinitionCollection;
        protected internal static IChildElementCollection/*<IDrgElement>*/ DrgElementCollection;
        protected internal static IChildElementCollection/*<IArtifact>*/ ArtifactCollection;
        protected internal static IChildElementCollection/*<IElementCollection>*/ ElementCollectionCollection;
        protected internal static IChildElementCollection/*<IBusinessContextElement>*/ BusinessContextElementCollection;

        public DefinitionsImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
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

        public virtual string Namespace
        {
            get => NamespaceAttribute.GetValue<String>(this);
            set => NamespaceAttribute.SetValue(this, value);
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

        public virtual ICollection<ITemDefinition> ItemDefinitions => ItemDefinitionCollection.Get<ITemDefinition>(this);

        public virtual ICollection<IDrgElement> DrgElements => DrgElementCollection.Get<IDrgElement>(this);

        public virtual ICollection<IArtifact> Artifacts => ArtifactCollection.Get<IArtifact>(this);

        public virtual ICollection<IElementCollection> ElementCollections => ElementCollectionCollection.Get<IElementCollection>(this);

        public virtual ICollection<IBusinessContextElement> BusinessContextElements => BusinessContextElementCollection.Get<IBusinessContextElement>(this);

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IDefinitions>(/*typeof(IDefinitions),*/ DmnModelConstants.DmnElementDefinitions)
                .NamespaceUri(DmnModelConstants.Dmn11Ns)
                .ExtendsType(typeof(INamedElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ExpressionLanguageAttribute = typeBuilder.StringAttribute(DmnModelConstants.DmnAttributeExpressionLanguage)
                .DefaultValue("http://www.omg.org/spec/FEEL/20140401")
                .Build();

            TypeLanguageAttribute = typeBuilder.StringAttribute(DmnModelConstants.DmnAttributeTypeLanguage)
                .DefaultValue("http://www.omg.org/spec/FEEL/20140401").Build();

            NamespaceAttribute = typeBuilder.StringAttribute(DmnModelConstants.DmnAttributeNamespace).Required().Build();

            ExporterAttribute = typeBuilder.StringAttribute(DmnModelConstants.DmnAttributeExporter).Build();

            ExporterVersionAttribute = typeBuilder.StringAttribute(DmnModelConstants.DmnAttributeExporterVersion).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            ImportCollection = sequenceBuilder.ElementCollection<IMport>(/*typeof(IMport)*/).Build/*<IMport>*/();

            ItemDefinitionCollection = sequenceBuilder.ElementCollection<ITemDefinition>(/*typeof(ITemDefinition)*/).Build/*<ITemDefinition>*/();

            DrgElementCollection = sequenceBuilder.ElementCollection<IDrgElement>(/*typeof(IDrgElement)*/).Build/*<IDrgElement>*/();

            ArtifactCollection = sequenceBuilder.ElementCollection<IArtifact>(/*typeof(IArtifact)*/).Build/*<IArtifact>*/();

            ElementCollectionCollection = sequenceBuilder.ElementCollection<IElementCollection>(/*typeof(IElementCollection)*/).Build/*<IElementCollection>*/();

            BusinessContextElementCollection = sequenceBuilder.ElementCollection<IBusinessContextElement>(/*typeof(IBusinessContextElement)*/).Build/*<IBusinessContextElement>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IDefinitions>
        {
            public virtual IDefinitions NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new DefinitionsImpl(instanceContext);
            }
        }
    }
}