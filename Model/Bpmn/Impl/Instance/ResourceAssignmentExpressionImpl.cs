using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class ResourceAssignmentExpressionImpl : BaseElementImpl, IResourceAssignmentExpression
    {

        protected internal static IChildElement/*<IExpression>*/ ExpressionChild;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IResourceAssignmentExpression>(/*typeof(IResourceAssignmentExpression),*/ BpmnModelConstants.BpmnElementResourceAssignmentExpression)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            ExpressionChild = sequenceBuilder.Element<IExpression>(/*typeof(IExpression)*/).Required().Build/*<IExpression>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IResourceAssignmentExpression>
        {
            public virtual IResourceAssignmentExpression NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ResourceAssignmentExpressionImpl(instanceContext);
            }
        }

        public ResourceAssignmentExpressionImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual IExpression Expression
        {
            get => (IExpression)ExpressionChild.GetChild(this);
            set => ExpressionChild.SetChild(this, value);
        }
    }
}