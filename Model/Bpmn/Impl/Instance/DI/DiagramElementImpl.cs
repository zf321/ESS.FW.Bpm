

using System;
using ESS.FW.Bpm.Model.Bpmn.instance.di;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.di
{

    public abstract class DiagramElementImpl : BpmnModelElementInstanceImpl, IDiagramElement
    {
        //public abstract IExtension Extension { get; set; }

        protected internal static IAttribute/*<string>*/ IdAttribute;
        protected internal static IChildElement/*<IExtension>*/ ExtensionChild;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IDiagramElement>(/*typeof(IDiagramElement),*/ BpmnModelConstants.DiElementDiagramElement)
                .NamespaceUri(BpmnModelConstants.DiNs)
                .AbstractType();

            IdAttribute = typeBuilder.StringAttribute(BpmnModelConstants.DiAttributeId).IdAttribute().Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            ExtensionChild = sequenceBuilder.Element<IExtension>(/*typeof(IExtension)*/).Build/*<IExtension>*/();

            typeBuilder.Build();
        }

        public DiagramElementImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public override string Id
        {
            get => IdAttribute.GetValue<String>(this);
            set => IdAttribute.SetValue(this, value);
        }

        public IExtension Extension
        {
            get => (IExtension)ExtensionChild.GetChild(this);
            set => ExtensionChild.SetChild(this, value);
        }
    }
}