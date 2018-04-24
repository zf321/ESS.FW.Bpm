using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.validation;


namespace ESS.FW.Bpm.Model.Xml.impl.validation
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    public class ModelInstanceValidator
    {

        protected internal ModelInstanceImpl ModelInstanceImpl;
        //Todo： IModelElementValidator<T>泛型接口是否需要重构
        protected internal ICollection<IModelElementValidator<IModelElementInstance>> Validators;

        public ModelInstanceValidator(ModelInstanceImpl modelInstanceImpl, ICollection<IModelElementValidator<IModelElementInstance>> validators)
        {
            this.ModelInstanceImpl = modelInstanceImpl;
            this.Validators = validators;
        }
        
        public virtual IValidationResults Validate()
        {

            ValidationResultsCollectorImpl resultsCollector = new ValidationResultsCollectorImpl();

            foreach (var validator in Validators)
            {

                IModelElementType elementType = validator.ElementType;
                IEnumerable<IModelElementInstance> modelElementsByType = ModelInstanceImpl.GetModelElementsByType(elementType);

                foreach (IModelElementInstance element in modelElementsByType)
                {

                    resultsCollector.CurrentElement = element;

                    try
                    {
                        validator.Validate(element, resultsCollector);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Validator " + validator + " threw an exception while validating " + element, e);
                    }
                }
            }
            return resultsCollector.Results;
        }
    }
}