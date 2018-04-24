using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public abstract class LoopCharacteristicsImpl : BaseElementImpl, ILoopCharacteristics
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ILoopCharacteristics>(/*typeof(ILoopCharacteristics),*/ BpmnModelConstants.BpmnElementLoopCharacteristics)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .AbstractType();

            typeBuilder.Build();
        }

        public LoopCharacteristicsImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}