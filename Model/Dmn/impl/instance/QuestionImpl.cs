

using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{

    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.camunda.bpm.model.dmn.impl.DmnModelConstants.DMN11_NS;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.camunda.bpm.model.dmn.impl.DmnModelConstants.DMN_ELEMENT_QUESTION;

    using ModelBuilder = ModelBuilder;
    using ModelTypeInstanceContext = ModelTypeInstanceContext;

    public class QuestionImpl : DmnModelElementInstanceImpl, IQuestion
    {

        public QuestionImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IQuestion>(/*typeof(IQuestion),*/ DmnModelConstants.DmnElementQuestion).NamespaceUri(DmnModelConstants.Dmn11Ns).InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IQuestion>
        {
            public ModelTypeInstanceProviderAnonymousInnerClass()
            {
            }

            public virtual IQuestion NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new QuestionImpl(instanceContext);
            }
        }
    }
}