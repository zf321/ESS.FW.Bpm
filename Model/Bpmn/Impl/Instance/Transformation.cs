

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class Transformation : FormalExpressionImpl
    {
        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<Transformation>(/*typeof(Transformation),*/ BpmnModelConstants.BpmnElementTransformation)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .ExtendsType(typeof(IFormalExpression))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<Transformation>
        {
            public virtual Transformation NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new Transformation(instanceContext);
            }
        }

        public Transformation(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }

}