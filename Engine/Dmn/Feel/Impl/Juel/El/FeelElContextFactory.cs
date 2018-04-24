using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using ESS.FW.Bpm.Engine.Impl.Juel;
using ESS.FW.Bpm.Engine.Variable.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel.El
{
    public class FeelElContextFactory: IElContextFactory
    {
        public static readonly FeelEngineLogger LOG= FeelLogger.ENGINE_LOGGER;
        protected internal CustomFunctionMapper customFunctionMapper = new CustomFunctionMapper();

        public FeelElContextFactory()
        {
        }

        public virtual ELContext CreateContext(ExpressionFactory expressionFactory, IVariableContext variableContext)
        {
            ELResolver elResolver = this.CreateElResolver();
            FunctionMapper functionMapper = this.CreateFunctionMapper();
            VariableMapper variableMapper = this.CreateVariableMapper(expressionFactory, variableContext);
            return new FeelElContext(elResolver, functionMapper, variableMapper);
        }

        public virtual ELResolver CreateElResolver()
        {
            return new SimpleResolver(true);
        }

        public virtual FunctionMapper CreateFunctionMapper()
        {
            CompositeFunctionMapper functionMapper = new CompositeFunctionMapper();
            functionMapper.Add(new FeelFunctionMapper());
            functionMapper.Add(this.customFunctionMapper);
            return functionMapper;
        }

        public virtual VariableMapper CreateVariableMapper(ExpressionFactory expressionFactory, IVariableContext variableContext)
        {
            return new FeelTypedVariableMapper(expressionFactory, variableContext);
        }

        public virtual void AddCustomFunction(string name, MethodInfo method)
        {
            this.customFunctionMapper.AddMethod(name, method);
        }


    }
}
