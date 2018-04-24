using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel.Transform
{
    public class ListTransformer : IFeelToJuelTransformer
    {
        public static FeelEngineLogger LOG = FeelLogger.ENGINE_LOGGER;
        public static string COMMA_SEPARATOR_REGEX = ",(?=([^\"]*\"[^\"]*\")*[^\"]*$)";

        public ListTransformer()
        {
        }
        public bool CanTransform(string feelExpression)
        {
            return this.SplitExpression(feelExpression).Count > 1;
        }

        public string Transform(IFeelToJuelTransform transform, string feelExpression, string inputName)
        {
            List<string> juelExpressions = this.TransformExpressions(transform, feelExpression, inputName);
            return this.JoinExpressions(juelExpressions);
        }

        protected List<String> CollectExpressions(String feelExpression)
        {
            return this.SplitExpression(feelExpression);
        }

        private List<String> SplitExpression(String feelExpression)
        {
            //return Arrays.asList(feelExpression.Split(COMMA_SEPARATOR_REGEX, -1));
            return Regex.Split(feelExpression, COMMA_SEPARATOR_REGEX).ToList();
        }

        protected List<String> TransformExpressions(IFeelToJuelTransform transform, String feelExpression, String inputName)
        {
            List<String> expressions = this.CollectExpressions(feelExpression);
            List<String> juelExpressions = new List<string>();
            //Iterator i$ = expressions.iterator();
            var i = expressions.GetEnumerator();

            while (i.MoveNext()) // hasNext()) {
            {
                String expression = (String)i.Current;//.next();
                if (string.IsNullOrEmpty(expression.Trim()))//.IsEmpty())
                {
                    throw LOG.invalidListExpression(feelExpression);
                }

                String juelExpression = transform.TransformSimplePositiveUnaryTest(expression, inputName);
                juelExpressions.Add(juelExpression);
            }

            return juelExpressions;
        }

        protected String JoinExpressions(List<String> juelExpressions)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("(").Append((String)juelExpressions[0]).Append(")");

            for (int i = 1; i < juelExpressions.Count; ++i)
            {
                builder.Append(" || (").Append((String)juelExpressions[i]).Append(")");
            }

            return builder.ToString();
        }
    }
}