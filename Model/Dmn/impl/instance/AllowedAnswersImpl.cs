

using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.camunda.bpm.model.dmn.impl.DmnModelConstants.DMN11_NS;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.camunda.bpm.model.dmn.impl.DmnModelConstants.DMN_ELEMENT_ALLOWED_ANSWERS;

    using ModelBuilder = ModelBuilder;
    using ModelTypeInstanceContext = ModelTypeInstanceContext;

    public class AllowedAnswersImpl : DmnModelElementInstanceImpl, IAllowedAnswers
    {

        public AllowedAnswersImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public static new void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IAllowedAnswers>(/*typeof(IAllowedAnswers),*/ DmnModelConstants.DmnElementAllowedAnswers).NamespaceUri(DmnModelConstants.Dmn11Ns).InstanceProvider<IAllowedAnswers>(new ModelTypeInstanceProviderAnonymousInnerClass());
            typeBuilder.Build();
        }
        private class ModelTypeInstanceProviderAnonymousInnerClass :IModelTypeInstanceProvider<IAllowedAnswers> 
        {
            public virtual IAllowedAnswers NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new AllowedAnswersImpl(instanceContext);
            }
        }
    }
}