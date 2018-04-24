using ESS.FW.Bpm.Model.Bpmn.instance.dc;
using ESS.FW.Bpm.Model.Bpmn.instance.di;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.di
{
    public abstract class LabelImpl : NodeImpl, ILabel
    {
        //public override abstract IExtension Extension { set; }

        protected internal static IChildElement/*<IBounds>*/ BoundsChild;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ILabel>(/*typeof(ILabel),*/ BpmnModelConstants.DiElementLabel)
                .NamespaceUri(BpmnModelConstants.DiNs)
                .ExtendsType(typeof(INode))
                .AbstractType();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            BoundsChild = sequenceBuilder.Element<IBounds>(/*typeof(IBounds)*/).Build/*<IBounds>*/();

            typeBuilder.Build();
        }

        public LabelImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual IBounds Bounds
        {
            get => (IBounds)BoundsChild.GetChild(this);
            set => BoundsChild.SetChild(this, value);
        }
    }
}