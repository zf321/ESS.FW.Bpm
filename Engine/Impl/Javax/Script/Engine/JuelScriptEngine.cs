using ESS.FW.Bpm.Engine.Impl.EL;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using ESS.FW.Bpm.Engine.Impl.Juel;
using ESS.FW.Bpm.Engine.Impl.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Javax.Script.Engine
{
    /// <summary>
	/// ScriptEngine that used JUEL for script evaluation and compilation (JSR-223).
	/// 
	/// Uses EL 1.1 if available, to resolve expressions. Otherwise it reverts to EL
	/// 1.0, using <seealso cref="ExpressionFactoryResolver"/>.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public class JuelScriptEngine : AbstractScriptEngine
    {

        private IScriptEngineFactory scriptEngineFactory;
        private ExpressionFactory expressionFactory;

        public JuelScriptEngine(IScriptEngineFactory scriptEngineFactory)
        {
            this.scriptEngineFactory = scriptEngineFactory;
            // Resolve the ExpressionFactory
            expressionFactory = ExpressionFactoryResolver.ResolveExpressionFactory();
        }

        public JuelScriptEngine() : this(null)
        {
        }

        public override object Eval(string script, IScriptContext scriptContext)
        {
            try
            {
                ValueExpression expr = Parse(script, scriptContext);
                return EvaluateExpression(expr, scriptContext);
            }
            catch(ScriptException e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                throw new ScriptException(e);
            }
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public Object eval(java.io.Reader reader, javax.script.ScriptContext scriptContext) throws javax.script.ScriptException
        public override object Eval(Stream reader, IScriptContext scriptContext)
        {
            return Eval(ReadFully(reader), scriptContext);
        }

        public override IScriptEngineFactory GetFactory()
        {
            lock (this)
            {
                if (scriptEngineFactory == null)
                {
                    scriptEngineFactory = new JuelScriptEngineFactory();
                }
            }
            return scriptEngineFactory;
        }

        public override IBindings CreateBindings()
        {
            return new SimpleBindings();
        }

        private object EvaluateExpression(ValueExpression expr, IScriptContext ctx)
        {
            try
            {
                return expr.GetValue(CreateElContext(ctx));
            }
            catch (ELException elexp)
            {
                throw new ScriptException(elexp);
            }
        }

        private ELResolver CreateElResolver()
        {
            CompositeELResolver compositeResolver = new CompositeELResolver();
            compositeResolver.Add(new ArrayELResolver());
            compositeResolver.Add(new ListELResolver());
            compositeResolver.Add(new MapELResolver());
            compositeResolver.Add(new ResourceBundleElResolver());
            compositeResolver.Add(new ObjectELResolver());
            return new SimpleResolver(compositeResolver);
        }

        private string ReadFully(Stream reader)
        {
            try
            {
                using(StreamReader sr=new StreamReader(reader))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (IOException exp)
            {
                throw new ScriptException(exp);
            }
        }
        private ValueExpression Parse(string script, IScriptContext scriptContext)
        {
            try
            {
                return expressionFactory.CreateValueExpression(CreateElContext(scriptContext), script, typeof(object));
            }
            catch (ELException ele)
            {
                throw new ScriptException(ele);
            }
        }

        private ELContext CreateElContext(IScriptContext scriptCtx)
        {
            // Check if the ELContext is already stored on the ScriptContext
            object existingELCtx = scriptCtx.GetAttribute("elcontext");
            if (existingELCtx is ELContext)
            {
                return (ELContext)existingELCtx;
            }

            scriptCtx.SetAttribute("context", scriptCtx, ScriptContext_Fields.ENGINE_SCOPE);

            // Built-in function are added to ScriptCtx
            scriptCtx.SetAttribute("out:print", GetPrintMethod(), ScriptContext_Fields.ENGINE_SCOPE);

            //SecurityManager securityManager = System.getSecurityManager();
            //if (securityManager == null)
            //{
                scriptCtx.SetAttribute("lang:import", getImportMethod(), ScriptContext_Fields.ENGINE_SCOPE);
            //}

            ELContext elContext = new ELContextAnonymousInnerClassHelper(this, scriptCtx);
            // Store the elcontext in the scriptContext to be able to reuse
            scriptCtx.SetAttribute("elcontext", elContext, ScriptContext_Fields.ENGINE_SCOPE);
            return elContext;
        }

        private class ELContextAnonymousInnerClassHelper : ELContext
        {
            private readonly JuelScriptEngine outerInstance;

            private IScriptContext scriptCtx;

            public ELContextAnonymousInnerClassHelper(JuelScriptEngine outerInstance, IScriptContext scriptCtx)
            {
                this.outerInstance = outerInstance;
                this.scriptCtx = scriptCtx;
                resolver = outerInstance.CreateElResolver();
                varMapper = new ScriptContextVariableMapper(outerInstance, scriptCtx);
                funcMapper = new ScriptContextFunctionMapper(outerInstance, scriptCtx);
            }


            internal ELResolver resolver;
            internal VariableMapper varMapper;
            internal FunctionMapper funcMapper;

            public override ELResolver ELResolver
            {
                get { return resolver; }
               
            }

            public override VariableMapper VariableMapper
            {
                get {return varMapper; }
                
            }

            public override FunctionMapper FunctionMapper
            {
                get {return funcMapper; }
                
            }
        }

        private static MethodInfo GetPrintMethod()
        {
            try
            {
                return typeof(JuelScriptEngine).GetMethod("print", new Type[] { typeof(object) });
            }
            catch (System.Exception e)
            {
                // Will never occur
                //return null;
                //TODO debug抛异常
                throw e;
            }
        }

        public static void print(object @object)
        {
            Console.Write(@object);
        }

        private static MethodInfo getImportMethod()
        {
            try
            {
                return typeof(JuelScriptEngine).GetMethod("ImportFunctions", new Type[] { typeof(IScriptContext), typeof(string), typeof(object) });
            }
            catch (System.Exception e)
            {
                // Will never occur
                //return null;
                //TODO debug抛异常
                throw e;
            }
        }

        public static void ImportFunctions(IScriptContext ctx, string @namespace, object obj)
        {
            Type clazz = null;
            if (obj is Type)
            {
                clazz = (Type)obj;
            }
            else if (obj is string)
            {
                try
                {
                    clazz = ReflectUtil.LoadClass((string)obj);
                }
                catch (ProcessEngineException ae)
                {
                    throw new ELException(ae);
                }
            }
            else
            {
                throw new ELException("Class or class name is missing");
            }
            MethodInfo[] methods = clazz.GetMethods();
            foreach (MethodInfo m in methods)
            {
                //int mod = m..GetModifiers();
                //if (Modifier.isStatic(mod) && Modifier.isPublic(mod))
                if(m.IsStatic&&m.IsPublic)
                {
                    string name = @namespace + ":" + m.Name;
                    ctx.SetAttribute(name, m, ScriptContext_Fields.ENGINE_SCOPE);
                }
            }
        }

        /// <summary>
        /// Class representing a compiled script using JUEL.
        /// 
        /// @author Frederik Heremans
        /// </summary>
        private class JuelCompiledScript : CompiledScript
        {
            private readonly JuelScriptEngine outerInstance;


            internal ValueExpression valueExpression;

             JuelCompiledScript(JuelScriptEngine outerInstance, ValueExpression valueExpression)
            {
                this.outerInstance = outerInstance;
                this.valueExpression = valueExpression;
            }

            public override IScriptEngine GetEngine()
            {
                // Return outer class instance
                return outerInstance;
            }

            public override object Eval(IScriptContext ctx)
            {
                try
                {
                    return outerInstance.EvaluateExpression(valueExpression, ctx);
                }
                catch(ScriptException ex)
                {
                    throw ex;
                }
                catch(System.Exception ex)
                {
                    throw new ScriptException(ex);
                }
            }
        }

        /// <summary>
        /// ValueMapper that uses the ScriptContext to get variable values or value
        /// expressions.
        /// 
        /// @author Frederik Heremans
        /// </summary>
        private class ScriptContextVariableMapper : VariableMapper
        {
            private readonly JuelScriptEngine outerInstance;


            internal IScriptContext scriptContext;

            internal ScriptContextVariableMapper(JuelScriptEngine outerInstance, IScriptContext scriptCtx)
            {
                this.outerInstance = outerInstance;
                this.scriptContext = scriptCtx;
            }

            public override ValueExpression ResolveVariable(string variableName)
            {
                int scope = scriptContext.GetAttributesScope(variableName);
                if (scope != -1)
                {
                    object value = scriptContext.GetAttribute(variableName, scope);
                    if (value is ValueExpression)
                    {
                        // Just return the existing ValueExpression
                        return (ValueExpression)value;
                    }
                    else
                    {
                        // Create a new ValueExpression based on the variable value
                        return outerInstance.expressionFactory.CreateValueExpression(value, typeof(object));
                    }
                }
                return null;
            }

            public override ValueExpression SetVariable(string name, ValueExpression value)
            {
                ValueExpression previousValue = ResolveVariable(name);
                scriptContext.SetAttribute(name, value, ScriptContext_Fields.ENGINE_SCOPE);
                return previousValue;
            }
        }

        /// <summary>
        /// FunctionMapper that uses the ScriptContext to resolve functions in EL.
        /// 
        /// @author Frederik Heremans
        /// </summary>
        private class ScriptContextFunctionMapper : FunctionMapper
        {
            private readonly JuelScriptEngine outerInstance;


            internal IScriptContext scriptContext;

            internal ScriptContextFunctionMapper(JuelScriptEngine outerInstance, IScriptContext ctx)
            {
                this.outerInstance = outerInstance;
                this.scriptContext = ctx;
            }

            internal virtual string getFullFunctionName(string prefix, string localName)
            {
                return prefix + ":" + localName;
            }

            public override MethodInfo ResolveFunction(string prefix, string localName)
            {
                string functionName = getFullFunctionName(prefix, localName);
                int scope = scriptContext.GetAttributesScope(functionName);
                if (scope != -1)
                {
                    // Methods are added as variables in the ScriptScope
                    object attributeValue = scriptContext.GetAttribute(functionName);
                    return (attributeValue is MethodInfo) ? (MethodInfo)attributeValue : null;
                }
                else
                {
                    return null;
                }
            }
        }

    }

}
