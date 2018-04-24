using ESS.FW.Bpm.Model.Bpmn.instance.di;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.di
{
    public abstract class LabeledShapeImpl : ShapeImpl, ILabeledShape
    {
        //public override abstract IExtension Extension { set; }

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ILabeledShape>(/*typeof(ILabeledShape),*/ BpmnModelConstants.DiElementLabeledShape)
                .NamespaceUri(BpmnModelConstants.DiNs)
                .ExtendsType(typeof(IShape))
                .AbstractType();

            typeBuilder.Build();
        }

        public LabeledShapeImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}