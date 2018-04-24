using System;
using System.Text.RegularExpressions;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel.Transform
{
    public class IntervalTransformer : IFeelToJuelTransformer
    {
        public  FeelEngineLogger LOG= FeelLogger.ENGINE_LOGGER;
        public static Regex INTERVAL_PATTERN =new Regex(@"^(\(|\[|\])(.*[^\.])\.\.(.+)(\)|\]|\[)$");// Pattern.compile("^(\\(|\\[|\\])(.*[^\\.])\\.\\.(.+)(\\)|\\]|\\[)$");
        
        public IntervalTransformer()
        {
        }

        public bool CanTransform(String feelExpression)
        {
            return feelExpression.StartsWith("(") || feelExpression.StartsWith("[") || feelExpression.StartsWith("]");
        }

        public String Transform(IFeelToJuelTransform transform, String feelExpression, String inputName)
        {
            //Matcher matcher = INTERVAL_PATTERN.matcher(feelExpression);
            var matcher = INTERVAL_PATTERN.Match(feelExpression);
            if (matcher.Success)
            {
                return this.TransformInterval(transform, matcher.Groups[1].Value, matcher.Groups[2].Value, matcher.Groups[3].Value, matcher.Groups[4].Value, inputName);
            }
            else
            {
                throw LOG.invalidIntervalExpression(feelExpression);
            }
        }

        public String TransformInterval(IFeelToJuelTransform transform, String startIntervalSymbol, String lowerEndpoint, String upperEndpoint, String stopIntervalSymbol, String inputName)
        {
            String juelLowerEndpoint = transform.TransformEndpoint(lowerEndpoint, inputName);
            String juelUpperEndpoint = transform.TransformEndpoint(upperEndpoint, inputName);
            String lowerEndpointComparator = this.transformLowerEndpointComparator(startIntervalSymbol);
            String upperEndpointComparator = this.transformUpperEndpointComparator(stopIntervalSymbol);
            return String.Format("{0} {1} {2} && {3} {4} {5}",inputName, lowerEndpointComparator, juelLowerEndpoint, inputName, upperEndpointComparator, juelUpperEndpoint);
        }

        protected String transformLowerEndpointComparator(String startIntervalSymbol)
        {
            return startIntervalSymbol.Equals("[") ? ">=" : ">";
        }

        protected String transformUpperEndpointComparator(String stopIntervalSymbol)
        {
            return stopIntervalSymbol.Equals("]") ? "<=" : "<";
        }
    }
}