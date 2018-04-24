

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class TextImpl : BpmnModelElementInstanceImpl, IText
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IText>(/*typeof(IText), */BpmnModelConstants.BpmnElementText)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IText>
        {
            public virtual IText NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new TextImpl(instanceContext);
            }
        }

        public TextImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }

}