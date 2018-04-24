using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.EL;
using ESS.FW.Bpm.Engine.Impl.Util.xml;

namespace ESS.FW.Bpm.Engine.Form.Impl.Validator
{
    /// <summary>
    ///     <para>Registry for built-in <seealso cref="IFormFieldValidator" /> implementations.</para>
    ///     <para>Factory for <seealso cref="IFormFieldValidator" /> instances.</para>
    ///     
    /// </summary>
    public class FormValidators
    {
        /// <summary>
        ///     the registry of configured validators. Populated through <seealso cref="ProcessEngineConfiguration" />.
        /// </summary>
        protected internal IDictionary<string, System.Type> validators = new Dictionary<string, System.Type>();

        public virtual IDictionary<string, System.Type> Validators
        {
            get { return validators; }
        }

        /// <summary>
        ///     factory method for creating validator instances
        /// </summary>
        public virtual IFormFieldValidator CreateValidator(Element constraint, BpmnParse bpmnParse,
            ExpressionManager expressionManager)
        {
            var name = constraint.GetAttributeValue("name");
            var config = constraint.GetAttributeValue("config");

            if ("validator".Equals(name))
            {
                // custom validators

                if (ReferenceEquals(config, null) || (config.Length == 0))
                    bpmnParse.AddError(
                        "validator configuration needs to provide either a fully " +
                        "qualified classname or an expression resolving to a custom FormFieldValidator implementation.",
                        constraint);
                else
                    return new DelegateFormFieldValidator(config);
            }
            else
            {
                // built-in validators

                var validator = validators[name];
                if (validator != null)
                {
                    var validatorInstance = CreateValidatorInstance(validator);
                    return validatorInstance;
                }
                bpmnParse.AddError("Cannot find validator implementation for name '" + name + "'.", constraint);
            }

            return null;
        }

        protected internal virtual IFormFieldValidator CreateValidatorInstance(System.Type validator)
        {
            try
            {
                return (IFormFieldValidator) Activator.CreateInstance(validator);
            }
            catch (System.Exception e)
            {
                throw new ProcessEngineException("Could not instantiate validator", e);
            }
            //catch (AccessViolationException e)
            //{
            //    throw new ProcessEngineException("Could not instantiate validator", e);
            //}
        }

        public virtual void AddValidator(string name, System.Type validatorType)
        {
            validators[name] = validatorType;
        }
    }
}