using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.camunda
{
    public class CamundaPotentialStarterImpl : BpmnModelElementInstanceImpl, ICamundaPotentialStarter
    {

        protected internal static IChildElement/*<IResourceAssignmentExpression>*/ ResourceAssignmentExpressionChild;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICamundaPotentialStarter>(/*typeof(ICamundaPotentialStarter),*/ BpmnModelConstants.CamundaElementPotentialStarter)
                .NamespaceUri(BpmnModelConstants.CamundaNs)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            ResourceAssignmentExpressionChild = sequenceBuilder.Element<IResourceAssignmentExpression>(/*typeof(IResourceAssignmentExpression)*/).Build/*<IResourceAssignmentExpression>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICamundaPotentialStarter>
        {
            public virtual ICamundaPotentialStarter NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CamundaPotentialStarterImpl(instanceContext);
            }
        }

        public CamundaPotentialStarterImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual IResourceAssignmentExpression ResourceAssignmentExpression
        {
            get => (IResourceAssignmentExpression)ResourceAssignmentExpressionChild.GetChild(this);
            set => ResourceAssignmentExpressionChild.SetChild(this, value);
        }

    }

}