using System.Collections.Generic;
using System.IO;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.validation;


namespace ESS.FW.Bpm.Model.Xml.impl.validation
{
    public class ModelValidationResultsImpl : IValidationResults
    {

        protected internal IDictionary<IModelElementInstance, IList<IValidationResult>> CollectedResults;

        protected internal int errorCount;
        protected internal int WarningCount;

        public ModelValidationResultsImpl(IDictionary<IModelElementInstance, IList<IValidationResult>> collectedResults, int errorCount, int warningCount)
        {
            this.CollectedResults = collectedResults;
            this.errorCount = errorCount;
            this.WarningCount = warningCount;
        }

        public virtual bool HasErrors()
        {
            return errorCount > 0;
        }

        public virtual int ErrorCount => errorCount;

        public virtual int WarinigCount => WarningCount;

        public virtual void Write(StringWriter writer, IValidationResultFormatter formatter)
        {
            foreach (KeyValuePair<IModelElementInstance, IList<IValidationResult>> entry in CollectedResults)
            {

                IModelElementInstance element = entry.Key;
                IList<IValidationResult> results = entry.Value;

                formatter.FormatElement(writer, element);

                foreach (IValidationResult result in results)
                {
                    formatter.FormatResult(writer, result);
                }
            }
        }

        public virtual IDictionary<IModelElementInstance, IList<IValidationResult>> Results => CollectedResults;
    }
}