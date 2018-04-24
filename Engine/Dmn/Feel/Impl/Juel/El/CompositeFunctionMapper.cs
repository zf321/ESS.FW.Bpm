using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel.El
{
    public class CompositeFunctionMapper: FunctionMapper
    {
        public static readonly FeelEngineLogger LOG= FeelLogger.ENGINE_LOGGER;
        protected internal IList<FunctionMapper> functionMappers = new List<FunctionMapper>();

        public CompositeFunctionMapper()
        {
        }

        public override MethodInfo ResolveFunction(string prefix, string localName)
        {
            var i = this.functionMappers.GetEnumerator();

            MethodInfo method;
            do
            {
                if (!i.MoveNext()) //.hasNext())
				{
                    throw LOG.unknownFunction(prefix, localName);
                }

                FunctionMapper functionMapper = i.Current;//.next();
                method = functionMapper.ResolveFunction(prefix, localName);
            } while (method == null);

            return method;
        }

        public virtual void SetFunctionMappers(IList<FunctionMapper> functionMappers)
        {
            this.functionMappers = functionMappers;
        }

        public virtual void Add(FunctionMapper functionMapper)
        {
            this.functionMappers.Add(functionMapper);
        }

        public virtual void Remove(FunctionMapper functionMapper)
        {
            this.functionMappers.Remove(functionMapper);
        }

    }
}
