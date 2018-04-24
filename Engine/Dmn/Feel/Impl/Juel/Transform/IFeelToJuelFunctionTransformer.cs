using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel.Transform
{
    public interface IFeelToJuelFunctionTransformer: IFeelToJuelTransformer
    {
        String GetName();

        MethodInfo GetMethod();
    }
}
