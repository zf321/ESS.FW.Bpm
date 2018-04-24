using System.Collections.Generic;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.validation;


namespace ESS.FW.Bpm.Model.Xml.impl.validation
{
    public class ValidationResultsCollectorImpl : IValidationResultCollector
    {

        protected internal IModelElementInstance currentElement;

        protected internal IDictionary<IModelElementInstance, IList<IValidationResult>> CollectedResults = new Dictionary<IModelElementInstance, IList<IValidationResult>>();

        protected internal int ErrorCount = 0;
        protected internal int WarningCount = 0;

        public virtual void AddError(int code, string message)
        {
            ResultsForCurrentElement().Add(new ModelValidationResultImpl(currentElement, ValidationResultType.Error, code, message));

            ++ErrorCount;
        }

        public virtual void AddWarning(int code, string message)
        {
            ResultsForCurrentElement().Add(new ModelValidationResultImpl(currentElement, ValidationResultType.Warning, code, message));

            ++WarningCount;
        }

        public virtual IModelElementInstance CurrentElement
        {
            set => this.currentElement = value;
        }

        public virtual IValidationResults Results => new ModelValidationResultsImpl(CollectedResults, ErrorCount, WarningCount);

        protected internal virtual IList<IValidationResult> ResultsForCurrentElement()
        {
            IList<IValidationResult> resultsByElement = CollectedResults[currentElement];

            if (resultsByElement == null)
            {
                resultsByElement = new List<IValidationResult>();
                CollectedResults[currentElement] = resultsByElement;
            }
            return resultsByElement;
        }

    }

}