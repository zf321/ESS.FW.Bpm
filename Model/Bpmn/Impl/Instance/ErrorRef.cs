using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class ErrorRef : BpmnModelElementInstanceImpl
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ErrorRef>(/*typeof(ErrorRef),*/ BpmnModelConstants.BpmnElementErrorRef)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ErrorRef>
        {
            public virtual ErrorRef NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ErrorRef(instanceContext);
            }
        }

        public ErrorRef(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }

}