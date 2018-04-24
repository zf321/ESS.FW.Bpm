using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.camunda
{
    public class CamundaValidationImpl : BpmnModelElementInstanceImpl, ICamundaValidation
    {

        protected internal static IChildElementCollection/*<ICamundaConstraint>*/ CamundaConstraintCollection;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICamundaValidation>(/*typeof(ICamundaValidation),*/ BpmnModelConstants.CamundaElementValidation)
                .NamespaceUri(BpmnModelConstants.CamundaNs)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            CamundaConstraintCollection = sequenceBuilder.ElementCollection<ICamundaConstraint>(/*typeof(ICamundaConstraint)*/).Build/*<ICamundaConstraint>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICamundaValidation>
        {
            public virtual ICamundaValidation NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CamundaValidationImpl(instanceContext);
            }
        }

        public CamundaValidationImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual ICollection<ICamundaConstraint> CamundaConstraints => CamundaConstraintCollection.Get<ICamundaConstraint>(this);
    }
}