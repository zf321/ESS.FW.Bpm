using System.Collections.Generic;

using ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Juel;
using ESS.FW.Bpm.Engine.Variable.Context;
using ESS.FW.Common.Components;
using System.Linq;
using System;

namespace ESS.FW.Bpm.Engine.Impl.EL
{

    /// <summary>
    ///     <para>
    ///         Central manager for all expressions.
    ///     </para>
    ///     <para>
    ///         Process parsers will use this to build expression objects that are stored in
    ///         the process definitions.
    ///     </para>
    ///     <para>
    ///         Then also this class is used as an entry point for runtime evaluation of the
    ///         expressions.
    ///     </para>
    ///     
    ///     
    /// </summary>
    public class ExpressionManager
    {
        private readonly bool _instanceFieldsInitialized;
        protected internal IDictionary<object, object> Beans;
        protected internal ELResolver ElResolver;
        protected internal ExpressionFactory expressionFactory;


        public IList<FunctionMapper> FunctionMappers = new List<FunctionMapper>();
        // Default implementation (does nothing)
        protected internal ELContext ParsingElContext;

        public ExpressionManager() : this(null)
        {
            if (!_instanceFieldsInitialized)
            {
                InitializeInstanceFields();
                _instanceFieldsInitialized = true;
            }
        }

        public ExpressionManager(IDictionary<object, object> beans)
        {
            if (!_instanceFieldsInitialized)
            {
                InitializeInstanceFields();
                _instanceFieldsInitialized = true;
            }
            // Use the ExpressionFactoryImpl built-in version of juel, with parametrised method expressions enabled
            expressionFactory = new ExpressionFactoryImpl();
            this.Beans = beans;
        }

        public virtual ExpressionFactory ExpressionFactory
        {
            set { expressionFactory = value; }
        }

        protected internal virtual ELResolver CachedElResolver
        {
            get
            {
                if (ElResolver == null)
                    lock (this)
                    {
                        if (ElResolver == null)
                            ElResolver = CreateElResolver();
                    }

                return ElResolver;
            }
        }

        private void InitializeInstanceFields()
        {
            ParsingElContext = new ProcessEngineElContext(FunctionMappers);
        }

        public virtual IExpression CreateExpression(string expression)
        {
            var valueExpression = CreateValueExpression(expression);
            return new JuelExpression(valueExpression, this, expression);
        }

        public virtual ValueExpression CreateValueExpression(string expression)
        {
            return expressionFactory.CreateValueExpression(ParsingElContext, expression, typeof(object));
        }

        public virtual ELContext GetElContext(IVariableScope variableScope)
        {
            ELContext elContext = null;
            if (variableScope is AbstractVariableScope)
            {
                var variableScopeImpl = variableScope as AbstractVariableScope;
                elContext = variableScopeImpl.CachedElContext;
            }

            if (elContext == null)
            {
                elContext = CreateElContext(variableScope);
                if (variableScope is AbstractVariableScope)
                    ((AbstractVariableScope) variableScope).CachedElContext = elContext;
            }

            return elContext;
        }

        public virtual ELContext CreateElContext(IVariableContext variableContext)
        {
            var elResolver = CachedElResolver;
            var elContext = new ProcessEngineElContext(FunctionMappers, elResolver);
            elContext.PutContext(typeof(ExpressionFactory), expressionFactory);
            elContext.PutContext(typeof(IVariableContext), variableContext);
            return elContext;
        }

        protected internal virtual ProcessEngineElContext CreateElContext(IVariableScope variableScope)
        {
            var elResolver = CachedElResolver;
            var elContext = new ProcessEngineElContext(FunctionMappers, elResolver);
            elContext.PutContext(typeof(ExpressionFactory), expressionFactory);
            elContext.PutContext(typeof(IVariableScope), variableScope);

            var registrations = ObjectContainer.Current.AllRegistrations();
            var regDic = new Dictionary<string, Type>();
            foreach (var reg in registrations)
            {
                if (!reg.IsGenericType && !regDic.ContainsKey(reg.Name))
                {
                    regDic.Add(reg.Name,reg);
                }
            }

            elContext.PutContext(typeof(IScope), regDic);

            return elContext;
        }

        protected internal virtual ELResolver CreateElResolver()
        {
            var elResolver = new CompositeELResolver();
            elResolver.Add(new VariableScopeElResolver());
            elResolver.Add(new VariableContextElResolver());

            if (Beans != null)
                elResolver.Add(new ReadOnlyMapElResolver(Beans));


            elResolver.Add(new ArrayELResolver());
            elResolver.Add(new MapELResolver());
            elResolver.Add(new ListELResolver());
            elResolver.Add(new ProcessApplicationObjectElResolverDelegate());

            elResolver.Add(new ProcessApplicationElResolverDelegate());
            return elResolver;
        }

        /// <param name="elFunctionMapper"> </param>
        public virtual void AddFunctionMapper(FunctionMapper elFunctionMapper)
        {
            FunctionMappers.Add(elFunctionMapper);
        }
    }
}