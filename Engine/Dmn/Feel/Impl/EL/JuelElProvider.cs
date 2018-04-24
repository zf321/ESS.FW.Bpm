using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.el;
using ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel.El;
using ESS.FW.Bpm.Engine.Impl.EL;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using ESS.FW.Bpm.Engine.Impl.Juel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.EL
{
    public class JuelElProvider : IELProvider
    {
        protected internal readonly ExpressionFactoryImpl factory;
        protected internal readonly JuelElContextFactory elContextFactory;
        protected internal readonly ELContext parsingElContext;

        public JuelElProvider() : this(new ExpressionFactoryImpl(), new JuelElContextFactory(CreateDefaultResolver()))
        {
        }

        public JuelElProvider(ExpressionFactoryImpl expressionFactory, JuelElContextFactory elContextFactory)
        {
            this.factory = expressionFactory;
            this.elContextFactory = elContextFactory;
            this.parsingElContext = CreateDefaultParsingElContext();
        }

        protected internal virtual SimpleContext CreateDefaultParsingElContext()
        {
            return new SimpleContext();
        }

        public virtual IELExpression CreateExpression(string expression)
        {
            /*Tree*/
            ValueExpression juelExpr = factory.CreateValueExpression(parsingElContext, expression, typeof(object));
            return new JuelExpression(juelExpr, elContextFactory);
        }

        public virtual ExpressionFactoryImpl GetFactory()
        {
            return factory;
        }

        public virtual JuelElContextFactory GetElContextFactory()
        {
            return elContextFactory;
        }

        public virtual ELContext GetParsingElContext()
        {
            return parsingElContext;
        }

        protected internal static ELResolver CreateDefaultResolver()
        {
            CompositeELResolver resolver = new CompositeELResolver();
            resolver.Add(new VariableContextElResolver());
            resolver.Add(new ArrayELResolver(true));
            resolver.Add(new ListELResolver(true));
            resolver.Add(new MapELResolver(true));
            resolver.Add(new ResourceBundleElResolver());
            resolver.Add(new ObjectELResolver());
            return resolver;
        }

    }
}
