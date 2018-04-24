using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class InputDataItemImpl : DataInputImpl, INputDataItem
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<INputDataItem>(/*typeof(INputDataItem),*/ BpmnModelConstants.BpmnElementInputDataItem)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IDataInput))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<INputDataItem>
        {
            public virtual INputDataItem NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new InputDataItemImpl(instanceContext);
            }
        }

        public InputDataItemImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}