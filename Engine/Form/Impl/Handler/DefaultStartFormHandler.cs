
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Util.xml;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Variable;

namespace ESS.FW.Bpm.Engine.Form.Impl.Handler
{
    /// <summary>
    ///      
    /// </summary>
    public class DefaultStartFormHandler : DefaultFormHandler, IStartFormHandler
    {
        protected internal IExpression formKey;

        // getters //////////////////////////////////////////////

        public virtual IExpression FormKey
        {
            get { return formKey; }
        }

        public override void ParseConfiguration(Element activityElement, DeploymentEntity deployment,
            ESS.FW.Bpm.Engine.Persistence.Entity.ProcessDefinitionEntity processDefinition, BpmnParse bpmnParse)
        {
            base.ParseConfiguration(activityElement, deployment, processDefinition, bpmnParse);

            var expressionManager = context.Impl.Context.ProcessEngineConfiguration.ExpressionManager;

            var formKeyAttribute = activityElement.GetAttributeNS(BpmnParse.CamundaBpmnExtensionsNs,
                "formKey");

            if (!ReferenceEquals(formKeyAttribute, null))
                formKey = expressionManager.CreateExpression(formKeyAttribute);

            if (formKey != null)
                processDefinition.StartFormKey = true;
        }

        public virtual IStartFormData CreateStartFormData(Persistence.Entity.ProcessDefinitionEntity processDefinition)
        {
            var startFormData = new StartFormDataImpl();

            if (formKey != null)
                startFormData.FormKey = formKey.ExpressionText;
            startFormData.DeploymentId = DeploymentId;
            startFormData.ProcessDefinition = (IProcessDefinition) processDefinition;
            InitializeFormProperties(startFormData, null);
            InitializeFormFields(startFormData, null);
            return startFormData;
        }

        public virtual ExecutionEntity SubmitStartFormData(ExecutionEntity processInstance, IVariableMap properties)
        {
            SubmitFormVariables(properties, (IVariableScope) processInstance);
            return processInstance;
        }
    }
}