

using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.camunda.bpm.model.dmn.impl.DmnModelConstants.DMN11_NS;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.camunda.bpm.model.dmn.impl.DmnModelConstants.DMN_ELEMENT_DECISION_OWNED;

    using ModelBuilder = ModelBuilder;
    using ModelTypeInstanceContext = ModelTypeInstanceContext;

    public class DecisionOwnedReferenceImpl : DmnElementReferenceImpl, IDecisionOwnedReference
    {

        public DecisionOwnedReferenceImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IDecisionOwnedReference>(/*typeof(IDecisionOwnedReference),*/ DmnModelConstants.DmnElementDecisionOwned).NamespaceUri(DmnModelConstants.Dmn11Ns).ExtendsType(typeof(IDmnElementReference)).InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IDecisionOwnedReference>
        {
            public ModelTypeInstanceProviderAnonymousInnerClass()
            {
            }

            public virtual IDecisionOwnedReference NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new DecisionOwnedReferenceImpl(instanceContext);
            }
        }
    }
}