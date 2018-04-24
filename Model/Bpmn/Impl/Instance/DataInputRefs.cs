using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class DataInputRefs : BpmnModelElementInstanceImpl
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<DataInputRefs>(/*typeof(DataInputRefs),*/ BpmnModelConstants.BpmnElementDataInputRefs)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<DataInputRefs>
        {
            public virtual DataInputRefs NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new DataInputRefs(instanceContext);
            }
        }

        public DataInputRefs(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }

}