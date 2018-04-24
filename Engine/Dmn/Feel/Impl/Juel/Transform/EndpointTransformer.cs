using System;
using System.Text.RegularExpressions;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel.Transform
{
    public class EndpointTransformer : IFeelToJuelTransformer
    {
        public static Regex DATE_AND_TIME_PATTERN = new Regex("^date and time\\((.+)\\)$");
        public bool CanTransform(String feelExpression)
        {
            return true;
        }

        public String Transform(IFeelToJuelTransform transform, String feelExpression, String inputName)
        {
            var matcher = DATE_AND_TIME_PATTERN.Match(feelExpression);
            return matcher.Success ? "dateAndTime(" + matcher.Groups[1].Value + ")" : feelExpression;
        }
    }
}