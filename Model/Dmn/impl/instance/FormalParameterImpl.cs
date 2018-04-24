

using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.camunda.bpm.model.dmn.impl.DmnModelConstants.DMN11_NS;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.camunda.bpm.model.dmn.impl.DmnModelConstants.DMN_ELEMENT_FORMAL_PARAMETER;

    using ModelBuilder = ModelBuilder;
    using ModelTypeInstanceContext = ModelTypeInstanceContext;

    public class FormalParameterImpl : InformationItemImpl, IFormalParameter
    {

        public FormalParameterImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IFormalParameter>(/*typeof(IFormalParameter), */DmnModelConstants.DmnElementFormalParameter).NamespaceUri(DmnModelConstants.Dmn11Ns).ExtendsType(typeof(INformationItem)).InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IFormalParameter>
        {
            public ModelTypeInstanceProviderAnonymousInnerClass()
            {
            }

            public virtual IFormalParameter NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new FormalParameterImpl(instanceContext);
            }
        }
    }
}