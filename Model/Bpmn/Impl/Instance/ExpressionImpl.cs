

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class ExpressionImpl : BaseElementImpl, IExpression
    {

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IExpression>(/*typeof(IExpression),*/ BpmnModelConstants.BpmnElementExpression)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IExpression>
        {
            public virtual IExpression NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ExpressionImpl(instanceContext);
            }
        }

        public ExpressionImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }
    }

}