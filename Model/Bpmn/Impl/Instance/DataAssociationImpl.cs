using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;
using ESS.FW.Bpm.Model.Bpmn.instance.di;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class DataAssociationImpl : BaseElementImpl, IDataAssociation
    {

        protected internal static IElementReferenceCollection SourceRefCollection;//IElementReferenceCollection<IItemAwareElement, SourceRef>
        protected internal static IElementReference TargetRefChild;//IElementReference<IItemAwareElement, TargetRef>
        protected internal static IChildElement/*<Transformation>*/ TransformationChild;
        protected internal static IChildElementCollection/*<IAssignment>*/ AssignmentCollection;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IDataAssociation>(/*typeof(IDataAssociation),*/ BpmnModelConstants.BpmnElementDataAssociation)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            SourceRefCollection = sequenceBuilder
                .ElementCollection<SourceRef>(/*typeof(SourceRef)*/)
                .IdElementReferenceCollection<IItemAwareElement>(/*typeof(IItemAwareElement)*/)
                .Build();

            TargetRefChild = sequenceBuilder
                .Element<TargetRef>(/*typeof(TargetRef)*/)
                .Required()
                .IdElementReference<IItemAwareElement>(/*typeof(IItemAwareElement)*/)
                .Build();

            TransformationChild = sequenceBuilder.Element<Transformation>(/*typeof(Transformation)*/).Build/*<Transformation>*/();

            AssignmentCollection = sequenceBuilder.ElementCollection<IAssignment>(/*typeof(IAssignment)*/).Build/*<IAssignment>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IDataAssociation>
        {

            public virtual IDataAssociation NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new DataAssociationImpl(instanceContext);
            }
        }

        public DataAssociationImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual ICollection<IItemAwareElement> Sources => SourceRefCollection.GetReferenceTargetElements<IItemAwareElement>(this);

        public virtual IItemAwareElement Target
        {
            get => (IItemAwareElement)TargetRefChild.GetReferenceTargetElement(this);
            set => TargetRefChild.SetReferenceTargetElement(this, value);
        }


        public virtual IFormalExpression GetTransformation()
        {
            return (IFormalExpression)TransformationChild.GetChild(this);
        }

        public virtual void SetTransformation(Transformation transformation)
        {
            TransformationChild.SetChild(this, transformation);
        }

        public virtual ICollection<IAssignment> Assignments => AssignmentCollection.Get<IAssignment>(this);

        public override IDiagramElement DiagramElement => (IBpmnEdge)base.DiagramElement;
    }
}