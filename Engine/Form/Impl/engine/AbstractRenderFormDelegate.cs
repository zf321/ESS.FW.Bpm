using ESS.FW.Bpm.Engine.Form.Impl.Type;

namespace ESS.FW.Bpm.Engine.Form.Impl.Engine
{
    /// <summary>
    ///     
    /// </summary>
    public abstract class AbstractRenderFormDelegate
    {
        /* elements */
        protected internal const string FormElement = "form";
        protected internal const string DivElement = "div";
        protected internal const string SpanElement = "span";
        protected internal const string LabelElement = "label";
        protected internal const string InputElement = "input";
        protected internal const string ButtonElement = "button";
        protected internal const string SelectElement = "select";
        protected internal const string OptionElement = "option";
        protected internal const string Element = "i";
        protected internal const string ScriptElement = "script";

        /* attributes */
        protected internal const string NameAttribute = "name";
        protected internal const string ClassAttribute = "class";
        protected internal const string RoleAttribute = "role";
        protected internal const string ForAttribute = "for";
        protected internal const string ValueAttribute = "value";
        protected internal const string TypeAttribute = "type";
        protected internal const string SelectedAttribute = "selected";

        /* datepicker attributes*/
        protected internal const string IsOpenAttribute = "is-open";
        protected internal const string DatepickerPopupAttribute = "datepicker-popup";

        /* camunda attributes */
        protected internal const string CamVariableTypeAttribute = "cam-variable-type";
        protected internal const string CamVariableNameAttribute = "cam-variable-name";
        protected internal const string CamScriptAttribute = "cam-script";

        /* angular attributes*/
        protected internal const string NgClickAttribute = "ng-click";
        protected internal const string NgIfAttribute = "ng-if";
        protected internal const string NgShowAttribute = "ng-show";

        /* classes */
        protected internal const string FormGroupClass = "form-group";
        protected internal const string FormControlClass = "form-control";
        protected internal const string InputGroupClass = "input-group";
        protected internal const string InputGroupBtnClass = "input-group-btn";
        protected internal const string ButtonDefaultClass = "btn btn-default";
        protected internal const string HasErrorClass = "has-error";
        protected internal const string HelpBlockClass = "help-block";

        /* input[type] */
        protected internal const string TextInputType = "text";
        protected internal const string CheckboxInputType = "checkbox";

        /* button[type] */
        protected internal const string ButtonButtonType = "button";

        /* script[type] */
        protected internal const string TextFormScriptType = "text/form-script";

        /* glyphicons */
        protected internal const string CalendarGlyphicon = "glyphicon glyphicon-calendar";

        /* generated form name */
        protected internal const string GeneratedFormName = "generatedForm";
        protected internal const string FormRole = "form";

        /* error types */
        protected internal const string RequiredErrorType = "required";
        protected internal const string DateErrorType = "date";

        /* JavaScript snippets */
        protected internal const string DateFieldOpenedAttribute = "dateFieldOpened%s";

        protected internal const string OpenDatepickerSnippet =
            "$scope.open%s = function ($event) { $event.preventDefault(); $event.stopPropagation(); $scope.dateFieldOpened%s = true; };";

        protected internal const string OpenDatepickerFunctionSnippet = "open%s($event)";

        /* date format */
        protected internal const string DateFormat = "dd/MM/yyyy";

        /* messages */
        protected internal const string RequiredFieldMessage = "Required field";
        protected internal const string TypeFieldMessage = "Only a %s value is allowed";

        /* form element selector */
        protected internal static readonly string FormElementSelector = "this." + GeneratedFormName + ".%s";

        /* expressions */
        protected internal static readonly string InvalidExpression = FormElementSelector + ".$invalid";
        protected internal static readonly string DirtyExpression = FormElementSelector + ".$dirty";
        protected internal static readonly string ErrorExpression = FormElementSelector + ".$error";
        protected internal static readonly string DateErrorExpression = ErrorExpression + ".date";
        protected internal static readonly string RequiredErrorExpression = ErrorExpression + ".required";
        protected internal static readonly string TypeErrorExpression = ErrorExpression + ".camVariableType";

        protected internal static readonly string InvalidDateFieldMessage =
            "Invalid date format: the date should have the pattern '" + DateFormat + "'";

        protected internal virtual string RenderFormData(IFormData formData)
        {
            if ((formData == null) ||
                (((formData.FormFields == null) || (formData.FormFields.Count == 0)) &&
                 ((formData.FormProperties == null) || (formData.FormProperties.Count == 0))))
                return null;
            var formElement =
                new HtmlElementWriter(FormElement).Attribute(NameAttribute, GeneratedFormName)
                    .Attribute(RoleAttribute, FormRole);

            var documentBuilder = new HtmlDocumentBuilder(formElement);

            // render fields
            foreach (var formField in formData.FormFields)
                RenderFormField(formField, documentBuilder);

            // render deprecated form properties
            foreach (var formProperty in formData.FormProperties)
                RenderFormField(new FormPropertyAdapter(formProperty), documentBuilder);

            // end document element
            documentBuilder.EndElement();

            return documentBuilder.HtmlString;
        }

        protected internal virtual void RenderFormField(IFormField formField, HtmlDocumentBuilder documentBuilder)
        {
            // start group
            var divElement = new HtmlElementWriter(DivElement).Attribute(ClassAttribute, FormGroupClass);

            documentBuilder.StartElement(divElement);

            var formFieldId = formField.Id;
            var formFieldLabel = formField.Label;

            // write label
            if (!ReferenceEquals(formFieldLabel, null) && (formFieldLabel.Length > 0))
            {
                var labelElement =
                    new HtmlElementWriter(LabelElement).Attribute(ForAttribute, formFieldId)
                        .TextContent(formFieldLabel);

                // <label for="...">...</label>
                documentBuilder.StartElement(labelElement).EndElement();
            }

            // render form control
            if (IsEnum(formField))
                RenderSelectBox(formField, documentBuilder);
            else if (IsDate(formField))
                RenderDatePicker(formField, documentBuilder);
            else
                RenderInputField(formField, documentBuilder);

            RenderInvalidMessageElement(formField, documentBuilder);

            // end group
            documentBuilder.EndElement();
        }

        protected internal virtual HtmlElementWriter CreateInputField(IFormField formField)
        {
            var inputField = new HtmlElementWriter(InputElement, true);

            AddCommonFormFieldAttributes(formField, inputField);

            inputField.Attribute(TypeAttribute, TextInputType);

            return inputField;
        }

        protected internal virtual void RenderDatePicker(IFormField formField, HtmlDocumentBuilder documentBuilder)
        {
            //bool isReadOnly = isReadOnly(formField);

            // start input-group
            var inputGroupDivElement = new HtmlElementWriter(DivElement).Attribute(ClassAttribute, InputGroupClass);

            var formFieldId = formField.Id;

            // <div>
            documentBuilder.StartElement(inputGroupDivElement);

            // input field
            var inputField = CreateInputField(formField);

            //if (!isReadOnly)
            {
                inputField.Attribute(DatepickerPopupAttribute, DateFormat)
                    .Attribute(IsOpenAttribute, string.Format(DateFieldOpenedAttribute, formFieldId));
            }

            // <input ... />
            documentBuilder.StartElement(inputField).EndElement();


            // if form field is read only, do not render date picker open button
            //if (!isReadOnly)
            {
                // input addon
                var addonElement = new HtmlElementWriter(DivElement).Attribute(ClassAttribute, InputGroupBtnClass);

                // <div>
                documentBuilder.StartElement(addonElement);

                // button to open date picker
                var buttonElement =
                    new HtmlElementWriter(ButtonElement).Attribute(TypeAttribute, ButtonButtonType)
                        .Attribute(ClassAttribute, ButtonDefaultClass)
                        .Attribute(NgClickAttribute, string.Format(OpenDatepickerFunctionSnippet, formFieldId));

                // <button>
                documentBuilder.StartElement(buttonElement);

                var iconElement = new HtmlElementWriter(Element).Attribute(ClassAttribute, CalendarGlyphicon);

                // <i ...></i>
                documentBuilder.StartElement(iconElement).EndElement();

                // </button>
                documentBuilder.EndElement();

                // </div>
                documentBuilder.EndElement();


                var scriptElement =
                    new HtmlElementWriter(ScriptElement).Attribute(CamScriptAttribute, null)
                        .Attribute(TypeAttribute, TextFormScriptType)
                        .TextContent(string.Format(OpenDatepickerSnippet, formFieldId, formFieldId));

                // <script ...> </script>
                documentBuilder.StartElement(scriptElement).EndElement();
            }

            // </div>
            documentBuilder.EndElement();
        }

        protected internal virtual void RenderInputField(IFormField formField, HtmlDocumentBuilder documentBuilder)
        {
            var inputField = new HtmlElementWriter(InputElement, true);
            AddCommonFormFieldAttributes(formField, inputField);

            var inputType = !IsBoolean(formField) ? TextInputType : CheckboxInputType;

            inputField.Attribute(TypeAttribute, inputType);

            // add default value
            var defaultValue = formField.DefaultValue;
            if (defaultValue != null)
                inputField.Attribute(ValueAttribute, defaultValue.ToString());

            // <input ... />
            documentBuilder.StartElement(inputField).EndElement();
        }

        protected internal virtual void RenderSelectBox(IFormField formField, HtmlDocumentBuilder documentBuilder)
        {
            var selectBox = new HtmlElementWriter(SelectElement, false);

            AddCommonFormFieldAttributes(formField, selectBox);

            // <select ...>
            documentBuilder.StartElement(selectBox);

            // <option ...>
            RenderSelectOptions(formField, documentBuilder);

            // </select>
            documentBuilder.EndElement();
        }

        protected internal virtual void RenderSelectOptions(IFormField formField, HtmlDocumentBuilder documentBuilder)
        {
            //EnumFormType enumFormType = (EnumFormType)formField.Type;
            //IDictionary<string, string> values = enumFormType.Values;

            //foreach (var value in values)
            //{
            //    // <option>
            //    var option =
            //        (new HtmlElementWriter(OPTION_ELEMENT, false)).attribute(VALUE_ATTRIBUTE, value.Key)
            //            .textContent(value.Value);

            //    documentBuilder.startElement(option).endElement();
            //}
        }

        protected internal virtual void RenderInvalidMessageElement(IFormField formField,
            HtmlDocumentBuilder documentBuilder)
        {
            var divElement = new HtmlElementWriter(DivElement);

            var formFieldId = formField.Id;
            var ifExpression = string.Format(InvalidExpression + " && " + DirtyExpression, formFieldId, formFieldId);

            divElement.Attribute(NgIfAttribute, ifExpression).Attribute(ClassAttribute, HasErrorClass);

            // <div ng-if="....$invalid && ....$dirty"...>
            documentBuilder.StartElement(divElement);

            if (!IsDate(formField))
            {
                RenderInvalidValueMessage(formField, documentBuilder);
                RenderInvalidTypeMessage(formField, documentBuilder);
            }
            else
            {
                RenderInvalidDateMessage(formField, documentBuilder);
            }

            documentBuilder.EndElement();
        }

        protected internal virtual void RenderInvalidValueMessage(IFormField formField,
            HtmlDocumentBuilder documentBuilder)
        {
            var divElement = new HtmlElementWriter(DivElement);

            var formFieldId = formField.Id;

            var expression = string.Format(RequiredErrorExpression, formFieldId);

            divElement.Attribute(NgShowAttribute, expression)
                .Attribute(ClassAttribute, HelpBlockClass)
                .TextContent(RequiredFieldMessage);

            documentBuilder.StartElement(divElement).EndElement();
        }

        protected internal virtual void RenderInvalidTypeMessage(IFormField formField,
            HtmlDocumentBuilder documentBuilder)
        {
            var divElement = new HtmlElementWriter(DivElement);

            var formFieldId = formField.Id;

            var expression = string.Format(TypeErrorExpression, formFieldId);

            var typeName = formField.TypeName;

            if (IsEnum(formField))
                typeName = StringFormType.TypeName;

            divElement.Attribute(NgShowAttribute, expression)
                .Attribute(ClassAttribute, HelpBlockClass)
                .TextContent(string.Format(TypeFieldMessage, typeName));

            documentBuilder.StartElement(divElement).EndElement();
        }

        protected internal virtual void RenderInvalidDateMessage(IFormField formField,
            HtmlDocumentBuilder documentBuilder)
        {
            var formFieldId = formField.Id;

            var firstDivElement = new HtmlElementWriter(DivElement);

            var firstExpression = string.Format(RequiredErrorExpression + " && !" + DateErrorExpression,
                formFieldId, formFieldId);

            firstDivElement.Attribute(NgShowAttribute, firstExpression)
                .Attribute(ClassAttribute, HelpBlockClass)
                .TextContent(RequiredFieldMessage);

            documentBuilder.StartElement(firstDivElement).EndElement();

            var secondDivElement = new HtmlElementWriter(DivElement);

            var secondExpression = string.Format(DateErrorExpression, formFieldId);

            secondDivElement.Attribute(NgShowAttribute, secondExpression)
                .Attribute(ClassAttribute, HelpBlockClass)
                .TextContent(InvalidDateFieldMessage);

            documentBuilder.StartElement(secondDivElement).EndElement();
        }

        protected internal virtual void AddCommonFormFieldAttributes(IFormField formField, HtmlElementWriter formControl)
        {
            var typeName = formField.TypeName;

            if (IsEnum(formField) || IsDate(formField))
                typeName = StringFormType.TypeName;

            typeName = typeName.Substring(0, 1).ToUpper() + typeName.Substring(1);

            var formFieldId = formField.Id;

            formControl.Attribute(ClassAttribute, FormControlClass)
                .Attribute(NameAttribute, formFieldId)
                .Attribute(CamVariableTypeAttribute, typeName)
                .Attribute(CamVariableNameAttribute, formFieldId);

            // add validation constraints
            foreach (var constraint in formField.ValidationConstraints)
            {
                var constraintName = constraint.Name;
                var configuration = (string) constraint.Configuration;
                formControl.Attribute(constraintName, configuration);
            }
        }

        // helper /////////////////////////////////////////////////////////////////////////////////////

        protected internal virtual bool IsEnum(IFormField formField)
        {
            return true;
            //return EnumFormType.TYPE_NAME.Equals(formField.TypeName);
        }

        protected internal virtual bool IsDate(IFormField formField)
        {
            return DateFormType.TypeName.Equals(formField.TypeName);
        }

        protected internal virtual bool IsBoolean(IFormField formField)
        {
            return true;
            //return BooleanFormType.TYPE_NAME.Equals(formField.TypeName);
        }

        protected internal virtual bool IsReadOnly(IFormField formField)
        {
            var validationConstraints = formField.ValidationConstraints;
            if (validationConstraints != null)
                foreach (var validationConstraint in validationConstraints)
                    if ("readonly".Equals(validationConstraint.Name))
                        return true;
            return false;
        }
    }
}