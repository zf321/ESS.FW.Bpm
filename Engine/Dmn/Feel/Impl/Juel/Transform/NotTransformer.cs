using System.Text.RegularExpressions;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel.Transform
{
    public class NotTransformer : IFeelToJuelTransformer
    {
        public static readonly FeelEngineLogger LOG;
        public static readonly /*Pattern*/ Regex NOT_PATTERN;

        public NotTransformer()
        {
        }

        public virtual bool CanTransform(string feelExpression)
        {
            return feelExpression.StartsWith("not(");
        }

        public virtual string Transform(IFeelToJuelTransform transform, string feelExpression, string inputName)
        {
            string simplePositiveUnaryTests = this.ExtractInnerExpression(feelExpression);
            string juelExpression = transform.TransformSimplePositiveUnaryTests(simplePositiveUnaryTests, inputName);
            return "not(" + juelExpression + ")";
        }

        public virtual string ExtractInnerExpression(string feelExpression)
        {
            Match matcher = NOT_PATTERN.Match(feelExpression);
            if (matcher.Success)
            {
                return matcher.Groups[1].Value;
            }
            else
            {
                throw LOG.invalidNotExpression(feelExpression);
            }
        }

        static NotTransformer()
        {
            LOG = FeelLogger.ENGINE_LOGGER;
            NOT_PATTERN = new Regex("^not\\((.+)\\)$");// Pattern.Compile("^not\\((.+)\\)$");
        }

    }
}