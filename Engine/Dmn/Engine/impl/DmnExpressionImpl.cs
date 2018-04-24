using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.el;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.type;
using ESS.FW.Bpm.Engine.Dmn.Engine.impl;
using ESS.FW.Bpm.Engine.Impl.Javax.Script;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl
{
    public class DmnExpressionImpl : CachedExpressionSupport , ICachedCompiledScriptSupport
    {
        protected internal CompiledScript cachedCompiledScript;
        protected internal IELExpression cachedExpression;
        protected internal string expression;
        protected internal string expressionLanguage;

        protected internal string id;
        protected internal string name;

        protected internal IDmnTypeDefinition typeDefinition;

        public virtual string Id
        {
            get { return id; }
            set { id = value; }
        }


        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }


        public virtual IDmnTypeDefinition TypeDefinition
        {
            get { return typeDefinition; }
            set { typeDefinition = value; }
        }


        public virtual string ExpressionLanguage
        {
            get { return expressionLanguage; }
            set { expressionLanguage = value; }
        }


        public virtual string Expression
        {
            get { return expression; }
            set { expression = value; }
        }

        public virtual void CacheCompiledScript(CompiledScript compiledScript)
        {
            cachedCompiledScript = compiledScript;
        }

        public virtual CompiledScript GetCachedCompiledScript()
        {
             return cachedCompiledScript; 
        }

        public virtual IELExpression CachedExpression
        {
            get { return cachedExpression; }
            set { cachedExpression = value; }
        }

        public override string ToString()
        {
            return "DmnExpressionImpl{" + "id='" + id + '\'' + ", name='" + name + '\'' + ", typeDefinition=" +
                   typeDefinition + ", expressionLanguage='" + expressionLanguage + '\'' + ", expression='" + expression +
                   '\'' + '}';
        }
    }
}