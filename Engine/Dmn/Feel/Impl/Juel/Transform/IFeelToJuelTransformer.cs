using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel.Transform
{
    public interface IFeelToJuelTransformer
    {
        bool CanTransform(String var1);

        String Transform(IFeelToJuelTransform var1, String var2, String var3);
    }
}
