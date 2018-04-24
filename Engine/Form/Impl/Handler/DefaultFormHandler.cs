using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Form.Impl.Type;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.History.Impl.Producer;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.EL;
using ESS.FW.Bpm.Engine.Impl.Util.xml;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Value;
using IExpression = ESS.FW.Bpm.Engine.Impl.EL.IExpression;


namespace ESS.FW.Bpm.Engine.Form.Impl.Handler
{

    /// <summary>
    ///      
    ///     
    /// </summary>
    public class DefaultFormHandler : IFormHandler
    {
        public const string FormFieldElement = "formField";
        public const string FormPropertyElement = "formProperty";
        private const string BusinessKeyAttribute = "businessKey";

        protected internal IList<FormFieldHandler> FormFieldHandlers = new List<FormFieldHandler>();


        protected internal virtual FormTypes FormTypes
        {
            get
            {
                var formTypes = Context.ProcessEngineConfiguration.FormTypes;
                return formTypes;
            }
        }


        // getters and setters //////////////////////////////////////////////////////

        public virtual string DeploymentId { get; set; }


        public virtual IList<FormPropertyHandler> FormPropertyHandlers { get; set; } = new List<FormPropertyHandler>();


        public virtual string BusinessKeyFieldId { get; set; }

        public virtual void SubmitFormVariables(IVariableMap properties, IVariableScope variableScope)
        {
            VariableMapImpl propertiesCopy = new VariableMapImpl(properties);

            // support legacy form properties
            foreach (var formPropertyHandler in FormPropertyHandlers)
                formPropertyHandler.SubmitFormProperty(variableScope, propertiesCopy);

            // support form data:
            foreach (var formFieldHandler in FormFieldHandlers)
                formFieldHandler.HandleSubmit(variableScope, propertiesCopy, properties);

            // any variables passed in which are not handled by form-fields or form
            // properties are added to the process as variables
            foreach (var propertyId in propertiesCopy.KeySet())
                variableScope.SetVariable(propertyId, propertiesCopy.GetValueTyped<ITypedValue>(propertyId));

            FireFormPropertyHistoryEvents(properties, variableScope);
        }

        public virtual void ParseConfiguration(Element activityElement, DeploymentEntity deployment,
            ProcessDefinitionEntity processDefinition, BpmnParse bpmnParse)
        {
            DeploymentId = deployment.Id;

            var expressionManager = Context.ProcessEngineConfiguration.ExpressionManager;

            var extensionElement = activityElement.element("extensionElements");
            if (extensionElement != null)
            {
                // provide support for deprecated form properties
                ParseFormProperties(bpmnParse, expressionManager, extensionElement);

                // provide support for new form field metadata
                ParseFormData(bpmnParse, expressionManager, extensionElement);
            }
        }

        protected internal virtual void ParseFormData(BpmnParse bpmnParse, ExpressionManager expressionManager,
            Element extensionElement)
        {
            var formData = extensionElement.ElementNs(BpmnParse.CamundaBpmnExtensionsNs, "formData");
            if (formData != null)
            {
                BusinessKeyFieldId = formData.GetAttributeValue(BusinessKeyAttribute);
                ParseFormFields(formData, bpmnParse, expressionManager);
            }
        }

        protected internal virtual void ParseFormFields(Element formData, BpmnParse bpmnParse,
            ExpressionManager expressionManager)
        {
            // parse fields:
            var formFields = formData.ElementsNS(BpmnParse.CamundaBpmnExtensionsNs, FormFieldElement);
            foreach (var formField in formFields)
                ParseFormField(formField, bpmnParse, expressionManager);
        }

        protected internal virtual void ParseFormField(Element formField, BpmnParse bpmnParse,
            ExpressionManager expressionManager)
        {
            var formFieldHandler = new FormFieldHandler();

            // parse Id
            var id = formField.GetAttributeValue("id");
            if (ReferenceEquals(id, null) || (id.Length == 0))
                bpmnParse.AddError("attribute id must be set for FormFieldGroup and must have a non-empty value",
                    formField);
            else
                formFieldHandler.Id = id;

            if (id.Equals(BusinessKeyFieldId))
                formFieldHandler.BusinessKey = true;

            // parse name
            var name = formField.GetAttributeValue("label");
            if (!ReferenceEquals(name, null))
            {
                IExpression nameExpression = (Bpm.Engine.Impl.EL.IExpression)expressionManager.CreateExpression(name);
                formFieldHandler.Label = nameExpression;
            }

            // parse properties
            ParseProperties(formField, formFieldHandler, bpmnParse, expressionManager);

            // parse validation
            ParseValidation(formField, formFieldHandler, bpmnParse, expressionManager);

            // parse type
            var formTypes = FormTypes;
            var formType = formTypes.ParseFormPropertyType(formField, bpmnParse);
            formFieldHandler.SetType(formType);

            // parse default value
            var defaultValue = formField.GetAttributeValue("defaultValue");
            if (!ReferenceEquals(defaultValue, null))
            {
                IExpression defaultValueExpression = (Bpm.Engine.Impl.EL.IExpression)expressionManager.CreateExpression(defaultValue);
                formFieldHandler.DefaultValueExpression = defaultValueExpression;
            }

            FormFieldHandlers.Add(formFieldHandler);
        }

        protected internal virtual void ParseProperties(Element formField, FormFieldHandler formFieldHandler,
            BpmnParse bpmnParse, ExpressionManager expressionManager)
        {
            var propertiesElement = formField.ElementNs(BpmnParse.CamundaBpmnExtensionsNs, "properties");

            if (propertiesElement != null)
            {
                var propertyElements = propertiesElement.ElementsNS(BpmnParse.CamundaBpmnExtensionsNs,
                    "property");

                // use linked hash map to preserve item ordering as provided in XML
                IDictionary<string, string> propertyMap = new Dictionary<string, string>();
                foreach (var property in propertyElements)
                {
                    var id = property.GetAttributeValue("id");
                    var value = property.GetAttributeValue("value");
                    propertyMap[id] = value;
                }

                formFieldHandler.Properties = propertyMap;
            }
        }

        protected internal virtual void ParseValidation(Element formField, FormFieldHandler formFieldHandler,
            BpmnParse bpmnParse, ExpressionManager expressionManager)
        {
            var validationElement = formField.ElementNs(BpmnParse.CamundaBpmnExtensionsNs, "validation");

            if (validationElement != null)
            {
                var constraintElements = validationElement.ElementsNS(
                    BpmnParse.CamundaBpmnExtensionsNs.NamespaceUri, "constraint");

                foreach (var property in constraintElements)
                {
                    var validator = Context.ProcessEngineConfiguration.FormValidators.CreateValidator(property,
                        bpmnParse, expressionManager);

                    var validatorName = property.GetAttributeValue("name");
                    var validatorConfig = property.GetAttributeValue("config");

                    if (validator != null)
                    {
                        var handler = new FormFieldValidationConstraintHandler();
                        handler.Name = validatorName;
                        handler.Config = validatorConfig;
                        handler.Validator = validator;
                        formFieldHandler.ValidationHandlers.Add(handler);
                    }
                }
            }
        }

        protected internal virtual void ParseFormProperties(BpmnParse bpmnParse, ExpressionManager expressionManager,
            Element extensionElement)
        {
            var formTypes = FormTypes;

            var formPropertyElements = extensionElement.ElementsNS(BpmnParse.CamundaBpmnExtensionsNs,
                FormPropertyElement);
            foreach (var formPropertyElement in formPropertyElements)
            {
                var formPropertyHandler = new FormPropertyHandler();

                var id = formPropertyElement.GetAttributeValue("id");
                if (ReferenceEquals(id, null))
                    bpmnParse.AddError("attribute 'id' is required", formPropertyElement);
                formPropertyHandler.Id = id;

                var name = formPropertyElement.GetAttributeValue("name");
                formPropertyHandler.Name = name;

                var type = formTypes.ParseFormPropertyType(formPropertyElement, bpmnParse);
                formPropertyHandler.SetType(type);

                var requiredText = formPropertyElement.GetAttributeValue("required", "false");
                var required = bpmnParse.ParseBooleanAttribute(requiredText);
                if (required != null)
                    formPropertyHandler.Required = required.Value;
                else
                    bpmnParse.AddError(
                        "attribute 'required' must be one of {on|yes|true|enabled|active|off|no|false|disabled|inactive}",
                        formPropertyElement);

                var readableText = formPropertyElement.GetAttributeValue("readable", "true");
                var readable = bpmnParse.ParseBooleanAttribute(readableText);
                if (readable != null)
                    formPropertyHandler.Readable = readable.Value;
                else
                    bpmnParse.AddError(
                        "attribute 'readable' must be one of {on|yes|true|enabled|active|off|no|false|disabled|inactive}",
                        formPropertyElement);

                var writableText = formPropertyElement.GetAttributeValue("writable", "true");
                var writable = bpmnParse.ParseBooleanAttribute(writableText);
                if (writable != null)
                    formPropertyHandler.Writable = writable.Value;
                else
                    bpmnParse.AddError(
                        "attribute 'writable' must be one of {on|yes|true|enabled|active|off|no|false|disabled|inactive}",
                        formPropertyElement);

                var variableName = formPropertyElement.GetAttributeValue("variable");
                formPropertyHandler.VariableName = variableName;

                var expressionText = formPropertyElement.GetAttributeValue("expression");
                if (!ReferenceEquals(expressionText, null))
                {
                    IExpression expression = (Bpm.Engine.Impl.EL.IExpression)expressionManager.CreateExpression(expressionText);
                    formPropertyHandler.VariableExpression = expression;
                }

                var defaultExpressionText = formPropertyElement.GetAttributeValue("default");
                if (!ReferenceEquals(defaultExpressionText, null))
                {
                    IExpression defaultExpression = (Bpm.Engine.Impl.EL.IExpression)expressionManager.CreateExpression(defaultExpressionText);
                    formPropertyHandler.DefaultExpression = defaultExpression;
                }

                FormPropertyHandlers.Add(formPropertyHandler);
            }
        }

        protected internal virtual void InitializeFormProperties(FormDataImpl formData, ExecutionEntity execution)
        {
            IList<IFormProperty> formProperties = new List<IFormProperty>();
            foreach (var formPropertyHandler in FormPropertyHandlers)
                if (formPropertyHandler.Readable)
                {
                    var formProperty = formPropertyHandler.CreateFormProperty(execution);
                    formProperties.Add(formProperty);
                }
            formData.FormProperties = formProperties;
        }

        protected internal virtual void InitializeFormFields(FormDataImpl taskFormData, ExecutionEntity execution)
        {
            // add form fields
            var formFields = taskFormData.FormFields;
            foreach (var formFieldHandler in FormFieldHandlers)
                formFields.Add(formFieldHandler.CreateFormField(execution));
        }

        protected internal virtual void FireFormPropertyHistoryEvents(IVariableMap properties,
            IVariableScope variableScope)
        {
            var processEngineConfiguration = Context.ProcessEngineConfiguration;
            var historyLevel = processEngineConfiguration.HistoryLevel;

            if (historyLevel.IsHistoryEventProduced(HistoryEventTypes.FormPropertyUpdate, variableScope))
            {
                // fire history events
                ExecutionEntity executionEntity;
                string taskId;
                if (variableScope is ExecutionEntity)
                {
                    executionEntity = (ExecutionEntity)variableScope;
                    taskId = null;
                }
                else if (variableScope is TaskEntity)
                {
                    var task = (TaskEntity)variableScope;
                    executionEntity = task.GetExecution();
                    taskId = task.Id;
                }
                else
                {
                    executionEntity = null;
                    taskId = null;
                }

                if (executionEntity != null)
                {
                    foreach (var variableName in properties.Keys)
                    {
                        string stringValue = (String)variableName;

                        HistoryEventProcessor.ProcessHistoryEvents(new HistoryEventCreatorAnonymousInnerClass(this, executionEntity, taskId, stringValue));
                    }
                }
            }
        }

        private class HistoryEventCreatorAnonymousInnerClass : HistoryEventProcessor.HistoryEventCreator
        {
            private readonly DefaultFormHandler _outerInstance;

            private ExecutionEntity _executionEntity;
            private string _stringValue;
            private string _taskId;

            public HistoryEventCreatorAnonymousInnerClass(DefaultFormHandler outerInstance,
                ExecutionEntity executionEntity, string taskId, string stringValue)
            {
                this._outerInstance = outerInstance;
                this._executionEntity = executionEntity;
                this._taskId = taskId;
                this._stringValue = stringValue;
            }

            public override HistoryEvent CreateHistoryEvent(IHistoryEventProducer producer)
            {
                //return producer.CreateFormPropertyUpdateEvt(executionEntity, variableName, stringValue, taskId);
                return null;
            }
        }
    }
}