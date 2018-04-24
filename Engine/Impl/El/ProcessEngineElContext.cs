using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;

namespace ESS.FW.Bpm.Engine.Impl.EL
{
    /// <summary>
    ///     <seealso cref="ELContext" /> used by the process engine.
    ///     
    ///     
    /// </summary>
    public class ProcessEngineElContext : ELContext
    {

        protected internal FunctionMapper functionMapper;

        public ProcessEngineElContext(IList<FunctionMapper> functionMappers, ELResolver elResolver)
            : this(functionMappers)
        {
            this.ELResolver = elResolver;
        }


        public ProcessEngineElContext(IList<FunctionMapper> functionMappers)
        {
            functionMapper = new CompositeFunctionMapper(functionMappers);
        }

        public override ELResolver ELResolver { get; }

        public override FunctionMapper FunctionMapper
        {
            get { return functionMapper; }
        }

        public override VariableMapper VariableMapper
        {
            get { return null; }
        }
    }
}