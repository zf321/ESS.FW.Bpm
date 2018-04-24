using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel.Transform
{
    public interface IFeelToJuelTransform
    {
        String TransformSimpleUnaryTests(String var1, String var2);

        String TransformSimplePositiveUnaryTests(String var1, String var2);

        String TransformSimplePositiveUnaryTest(String var1, String var2);

        String TransformEndpoint(String var1, String var2);

        void AddCustomFunctionTransformer(IFeelToJuelTransformer var1);
    }
}
