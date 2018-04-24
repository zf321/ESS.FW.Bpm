using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel.El
{
    public class FeelElContext : ELContext
    {
        protected internal ELResolver elResolver;
        protected internal FunctionMapper functionMapper;
        protected internal VariableMapper variableMapper;

        public FeelElContext(ELResolver elResolver, FunctionMapper functionMapper, VariableMapper variableMapper)
        {
            this.elResolver = elResolver;
            this.functionMapper = functionMapper;
            this.variableMapper = variableMapper;
        }

        public override ELResolver ELResolver
        {
            get
            {
                return this.elResolver;
            }
        }

        public override FunctionMapper FunctionMapper
        {
            get { return this.functionMapper; }
        }

        public override VariableMapper VariableMapper
        {
            get {return this.variableMapper; }
        }

    }
}
