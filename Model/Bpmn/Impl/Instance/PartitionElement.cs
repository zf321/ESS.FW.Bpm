

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class PartitionElement : BaseElementImpl
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<PartitionElement>(/*typeof(PartitionElement),*/ BpmnModelConstants.BpmnElementPartitionElement)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<PartitionElement>
        {
            public virtual PartitionElement NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new PartitionElement(instanceContext);
            }
        }

        public PartitionElement(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}