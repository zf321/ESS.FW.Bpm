using System;
using System.Text.RegularExpressions;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel.Transform
{
    public class ComparisonTransformer : IFeelToJuelTransformer
    {
        public  FeelEngineLogger LOG= FeelLogger.ENGINE_LOGGER;
        public Regex COMPARISON_PATTERN = new Regex("^(<=|>=|<|>)([^=].*)$");
        public bool CanTransform(String feelExpression)
        {
            return feelExpression.StartsWith("<=") || feelExpression.StartsWith("<") || feelExpression.StartsWith(">=") || feelExpression.StartsWith(">");
        }

        public String Transform(IFeelToJuelTransform transform, String feelExpression, String inputName)
        {
            //Matcher matcher = COMPARISON_PATTERN.matcher(feelExpression);
            var matcher = COMPARISON_PATTERN.Match(feelExpression);
            if (matcher.Success) 
            {
                return this.TransformComparison(transform, matcher.Groups[1].Value, matcher.Groups[2].Value, inputName);
            }
            else
            {
                throw LOG.invalidComparisonExpression(feelExpression);
            }
        }

        protected String TransformComparison(IFeelToJuelTransform transform, String op, String endpoint, String inputName)
        {
            String juelEndpoint = transform.TransformEndpoint(endpoint, inputName);
            return String.Format("{0} {1} {2}",inputName, op, juelEndpoint );
        }
    }
}