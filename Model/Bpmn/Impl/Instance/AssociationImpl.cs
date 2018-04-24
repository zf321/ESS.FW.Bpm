

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;
using ESS.FW.Bpm.Model.Bpmn.instance.di;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    /// <summary>
    /// 
    /// </summary>
    public class AssociationImpl : ArtifactImpl, IAssociation
    {

        protected internal static IAttributeReference SourceRefAttribute;//IAttributeReference<IBaseElement>
        protected internal static IAttributeReference TargetRefAttribute; //IAttributeReference<IBaseElement>
        protected internal static IAttribute/*<AssociationDirection>*/ AssociationDirectionAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IAssociation>(/*typeof(IAssociation),*/ BpmnModelConstants.BpmnElementAssociation)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IArtifact))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            //SourceRefAttribute = (IAttributeReference<IBaseElement>)typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeSourceRef)
            //    .Required()
            //    .QNameAttributeReference<IBaseElement>(typeof(IBaseElement))
            //    .Build();

            SourceRefAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeSourceRef)
                .Required()
                .QNameAttributeReference<IBaseElement>(/*typeof(IBaseElement)*/)
                .Build();

            //TargetRefAttribute = (IAttributeReference<IBaseElement>)typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeTargetRef)
            //    .Required()
            //    .QNameAttributeReference<IBaseElement>(typeof(IBaseElement))
            //    .Build();

            TargetRefAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeTargetRef)
                .Required()
                .QNameAttributeReference<IBaseElement>(/*typeof(IBaseElement)*/)
                .Build();

            AssociationDirectionAttribute = typeBuilder
                .EnumAttribute<AssociationDirection>(BpmnModelConstants.BpmnAttributeAssociationDirection/*, typeof(AssociationDirection)*/)
                .DefaultValue(AssociationDirection.None)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IAssociation>
        {
            public virtual IAssociation NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new AssociationImpl(instanceContext);
            }
        }

        public AssociationImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual IBaseElement Source
        {
            get => SourceRefAttribute.GetReferenceTargetElement<IBaseElement>(this);
            set => SourceRefAttribute.SetReferenceTargetElement(this, value);
        }


        public virtual IBaseElement Target
        {
            get => TargetRefAttribute.GetReferenceTargetElement<IBaseElement>(this);
            set => TargetRefAttribute.SetReferenceTargetElement(this, value);
        }


        public virtual AssociationDirection AssociationDirection
        {
            get => AssociationDirectionAttribute.GetValue<AssociationDirection>(this);
            set => AssociationDirectionAttribute.SetValue(this, value);
        }

        //IBpmnEdge IAssociation.DiagramElement { get; }

        public override IDiagramElement DiagramElement => (IBpmnEdge)base.DiagramElement;
    }

}