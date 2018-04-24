using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel.Transform
{
    public class FeelToJuelTransformImpl : IFeelToJuelTransform
    {
        public static FeelEngineLogger LOG= FeelLogger.ENGINE_LOGGER;
        public static IFeelToJuelTransformer NOT_TRANSFORMER= new NotTransformer();
        public static IFeelToJuelTransformer HYPHEN_TRANSFORMER= new HyphenTransformer();
        public static IFeelToJuelTransformer LIST_TRANSFORMER = new ListTransformer();
        public static IFeelToJuelTransformer INTERVAL_TRANSFORMER = new IntervalTransformer();
        public static IFeelToJuelTransformer COMPARISON_TRANSFORMER= new ComparisonTransformer();
        public static IFeelToJuelTransformer EQUAL_TRANSFORMER= new EqualTransformer();
        public static IFeelToJuelTransformer ENDPOINT_TRANSFORMER= new EndpointTransformer();
        public static List<IFeelToJuelTransformer> CUSTOM_FUNCTION_TRANSFORMERS = new List<IFeelToJuelTransformer>();
        public void AddCustomFunctionTransformer(IFeelToJuelTransformer functionTransformer)
        {
            CUSTOM_FUNCTION_TRANSFORMERS.Add(functionTransformer);
        }

        public string TransformEndpoint(String endpoint, String inputName)
        {
            endpoint = endpoint.Trim();
            return ENDPOINT_TRANSFORMER.Transform(this, endpoint, inputName);
        }

        public string TransformSimplePositiveUnaryTest(String simplePositiveUnaryTest, String inputName)
        {
            simplePositiveUnaryTest = simplePositiveUnaryTest.Trim();
            var i = CUSTOM_FUNCTION_TRANSFORMERS.GetEnumerator();//.iterator();

            IFeelToJuelTransformer functionTransformer;
            do
            {
                if (!i.MoveNext()) {
                    if (INTERVAL_TRANSFORMER.CanTransform(simplePositiveUnaryTest))
                    {
                        return INTERVAL_TRANSFORMER.Transform(this, simplePositiveUnaryTest, inputName);
                    }

                    if (COMPARISON_TRANSFORMER.CanTransform(simplePositiveUnaryTest))
                    {
                        return COMPARISON_TRANSFORMER.Transform(this, simplePositiveUnaryTest, inputName);
                    }

                    return EQUAL_TRANSFORMER.Transform(this, simplePositiveUnaryTest, inputName);
                }

                functionTransformer = (IFeelToJuelTransformer)i.Current;
            } while (!functionTransformer.CanTransform(simplePositiveUnaryTest));

            return functionTransformer.Transform(this, simplePositiveUnaryTest, inputName);
        }

        public string TransformSimplePositiveUnaryTests(String simplePositiveUnaryTests, String inputName)
        {
            simplePositiveUnaryTests = simplePositiveUnaryTests.Trim();
            return LIST_TRANSFORMER.CanTransform(simplePositiveUnaryTests) ? LIST_TRANSFORMER.Transform(this, simplePositiveUnaryTests, inputName) : this.TransformSimplePositiveUnaryTest(simplePositiveUnaryTests, inputName);
        }

        public string TransformSimpleUnaryTests(String simpleUnaryTests, String inputName)
        {
            simpleUnaryTests = simpleUnaryTests.Trim();
            String juelExpression;
            if (HYPHEN_TRANSFORMER.CanTransform(simpleUnaryTests))
            {
                juelExpression = HYPHEN_TRANSFORMER.Transform(this, simpleUnaryTests, inputName);
            }
            else if (NOT_TRANSFORMER.CanTransform(simpleUnaryTests))
            {
                juelExpression = NOT_TRANSFORMER.Transform(this, simpleUnaryTests, inputName);
            }
            else
            {
                juelExpression = this.TransformSimplePositiveUnaryTests(simpleUnaryTests, inputName);
            }

            return "${" + juelExpression + "}";
        }
    }
}
