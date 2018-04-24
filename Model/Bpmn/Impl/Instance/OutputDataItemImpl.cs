using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{

    public class OutputDataItemImpl : DataOutputImpl, IOutputDataItem
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IOutputDataItem>(/*typeof(IOutputDataItem),*/ BpmnModelConstants.BpmnElementOutputDataItem)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IDataOutput))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IOutputDataItem>
        {
            public virtual IOutputDataItem NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new OutputDataItemImpl(instanceContext);
            }
        }

        public OutputDataItemImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}