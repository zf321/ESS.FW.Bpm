

using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class DataOutputRefs : BpmnModelElementInstanceImpl
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<DataOutputRefs>(/*typeof(DataOutputRefs),*/ BpmnModelConstants.BpmnElementDataOutputRefs)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<DataOutputRefs>
        {
            public virtual DataOutputRefs NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new DataOutputRefs(instanceContext);
            }
        }

        public DataOutputRefs(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }
}