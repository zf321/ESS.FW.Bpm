//using ESS.FW.Bpm.Engine.Impl.EL;
//using ESS.FW.Bpm.Engine.Impl.Javax.EL;
//using System;
//using System.IO;
//using System.Text;

//namespace ESS.FW.Bpm.Engine.Impl.Javax.Script.Engine
//{
//    public class CSScriptEngine : AbstractScriptEngine, ICompilable
//    {
//        private IScriptEngineFactory scriptEngineFactory;
//        private ExpressionFactory expressionFactory;
//        public CSScriptEngine(IScriptEngineFactory scriptEngineFactory)
//        {
//            this.scriptEngineFactory = scriptEngineFactory;
//            // Resolve the ExpressionFactory
//            expressionFactory = ExpressionFactoryResolver.ResolveExpressionFactory();
//        }
//        //TODO CS代码编译
//        public CompiledScript Compile(string script)
//        {
//            //try
//            //{
//            //    var x = CSScript.CompileCode(script);
//            //    return new CSCompiledScript(this, x);
//            //}
//            //catch (System.Exception ex)
//            //{
//                return new CSCompiledScript(this, script);
//                //throw new System.Exception(string.Format("无法编译CS脚本,{0} :[{1}]", ex.Message, script));
//            //}
//        }

//        public CompiledScript Compile(Stream reader)
//        {
//            string script = string.Empty;
//            using (StreamReader sr = new StreamReader(reader, Encoding.UTF8, false))
//            {
//                script = sr.ReadToEnd();
//            }
//            return Compile(script);
//        }

//        public override IBindings CreateBindings()
//        {
//            return new SimpleBindings();
//        }

//        public override object Eval(string script, IScriptContext context)
//        {
//            //TODO CS脚本扩展
//            object r = CSScript.LoadMethod(script).CreateObject("*");
//            Type type = r.GetType();
//            foreach (var item in type.GetMethods())
//            {
//                return item.Invoke(r, null);
//            }
//            return null;
//        }

//        public override object Eval(Stream reader, IScriptContext context)
//        {
//            try
//            {
//                string script = string.Empty;
//                using (StreamReader sr = new StreamReader(reader, Encoding.UTF8, false))
//                {
//                    script = sr.ReadToEnd();
//                }
//                return Eval(script, context);
//            }
//            catch (ScriptException ex)
//            {
//                throw ex;
//            }
//            catch (System.Exception ex)
//            {
//                throw new ScriptException(ex);
//            }
//        }

//        public override IScriptEngineFactory GetFactory()
//        {
//            lock (this)
//            {
//                if (scriptEngineFactory == null)
//                {
//                    scriptEngineFactory = new CSScriptEngineFactory();
//                }
//            }
//            return scriptEngineFactory;
//        }
//        #region 内部类
//        private class CSCompiledScript : CompiledScript
//        {
//            private IScriptEngine scriptEngine;
//            private string compiledText;
//            public CSCompiledScript(IScriptEngine scriptEngine, string compiledText)
//            {
//                this.scriptEngine = scriptEngine;
//                this.compiledText = compiledText;
//            }
//            public override object Eval(IScriptContext ctx)
//            {
//                try
//                {
//                    //TODO CS脚本扩展
//                    //context.SetAttribute()
//                    object r = CSScript.LoadMethod(compiledText).CreateObject("*");
//                    Type type = r.GetType();
//                    foreach (var item in type.GetMethods())
//                    {
//                        return item.Invoke(r, null);
//                    }
//                    return null;
//                    //return r.Foo();

//                    //if (ctx.GetAttribute("context", ScriptContext_Fields.ENGINE_SCOPE) == null)
//                    //{
//                    //    // add context to bindings
//                    //    ctx.SetAttribute("context", ctx, ScriptContext_Fields.ENGINE_SCOPE);

//                    //    // direct output to ctx.getWriter
//                    //    // If we're wrapping with a PrintWriter here,
//                    //    // enable autoFlush because otherwise it might not get done!
//                    //    Stream writer = ctx.GetWriter();
//                    //    //    ctx.SetAttribute("out", (writer instanceof PrintWriter) ?
//                    //    //                    writer :
//                    //    //                    new PrintWriter(writer, true),
//                    //    //ScriptContext.ENGINE_SCOPE);
//                    //    ctx.SetAttribute("out", writer,
//                    //    ScriptContext_Fields.ENGINE_SCOPE);
//                    //}
//                    //// Fix for GROOVY-3669: Can't use several times the same JSR-223 ScriptContext for differents groovy script
//                    //if (ctx.GetWriter() != null)
//                    //{
//                    //    ctx.SetAttribute("out", ctx.GetWriter(), ScriptContext_Fields.ENGINE_SCOPE);
//                    //}
//                    //object r = CSScript.LoadMethod(compiledText).CreateObject("*");
//                    //Type type = r.GetType();
//                    //foreach (MethodInfo info in type.GetMethods(BindingFlags.DeclaredOnly))
//                    //{
//                    //    ctx.SetValue(info.Name, info.Invoke(r, null));
//                    //    //MethodInfo me = type.GetMethods()[0];
//                    //    //return me.Invoke(r, null);
//                    //}
//                    //return ctx;
//                }
//                catch (System.Exception ex)
//                {
//                    throw ex;
//                }

//    }

//    public override IScriptEngine GetEngine()
//    {
//        return scriptEngine;
//    }
//}
//        #endregion
//    }
//}
