using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel.El
{
    public class CustomFunctionMapper : FunctionMapper
    {
        public static readonly FeelEngineLogger LOG= FeelLogger.ENGINE_LOGGER;
        protected  Dictionary<string, MethodInfo> methods=new Dictionary<string, MethodInfo>();

        public override MethodInfo ResolveFunction(String prefix, String localName)
        {
            return methods.ContainsKey(localName)?this.methods[localName]:null;
        }

        public void AddMethod(String name, MethodInfo method)
        {
            this.methods.Add(name, method);
        }
    }
}
