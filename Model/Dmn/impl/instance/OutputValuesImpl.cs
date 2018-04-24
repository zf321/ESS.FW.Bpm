

using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{

    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.camunda.bpm.model.dmn.impl.DmnModelConstants.DMN11_NS;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.camunda.bpm.model.dmn.impl.DmnModelConstants.DMN_ELEMENT_OUTPUT_VALUES;

    using ModelBuilder = ModelBuilder;
    using ModelTypeInstanceContext = ModelTypeInstanceContext;

    public class OutputValuesImpl : UnaryTestsImpl, IOutputValues
    {

        public OutputValuesImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IOutputValues>(/*typeof(IOutputValues),*/ DmnModelConstants.DmnElementOutputValues).NamespaceUri(DmnModelConstants.Dmn11Ns).ExtendsType(typeof(IUnaryTests)).InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IOutputValues>
        {
            public ModelTypeInstanceProviderAnonymousInnerClass()
            {
            }

            public virtual IOutputValues NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new OutputValuesImpl(instanceContext);
            }
        }
    }
}