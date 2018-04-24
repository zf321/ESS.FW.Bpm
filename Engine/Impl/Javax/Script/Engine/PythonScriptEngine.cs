//using ESS.FW.Bpm.Engine.Impl.EL;
//using ESS.FW.Bpm.Engine.Impl.Javax.EL;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ESS.FW.Bpm.Engine.Impl.Javax.Script.Engine
//{
//    public class PythonScriptEngine : AbstractScriptEngine, ICompilable
//    {
//        private IScriptEngineFactory scriptEngineFactory;
//        private ExpressionFactory expressionFactory;
//        public PythonScriptEngine(IScriptEngineFactory scriptEngineFactory)
//        {
//            this.scriptEngineFactory = scriptEngineFactory;
//            // Resolve the ExpressionFactory
//            expressionFactory = ExpressionFactoryResolver.ResolveExpressionFactory();
//        }
//        public CompiledScript Compile(string script)
//        {
//            return new PythonCompiledScript(this, script);
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
//            try
//            {
//                script = script.TrimStart();//Python对格式要求严格
//                var engine = IronPython.Hosting.Python.CreateEngine();
//                var scope = engine.CreateScope();
//                var source = engine.CreateScriptSourceFromString(script);
//                source.Execute(scope);
//                //TODO 暂时写死方法名add,后期按指定规则反射扩展scope.Varialbes
//                dynamic func = scope.GetVariable<Func<object>>("add");
//                return func();
//            }
//            catch (System.Exception ex)
//            {
//                throw new ScriptException("Python脚本执行错误:" + ex.Message);
//            }
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
//        private class PythonCompiledScript : CompiledScript
//        {
//            private IScriptEngine scriptEngine;
//            private string compiledText;
//            public PythonCompiledScript(IScriptEngine scriptEngine, string compiledText)
//            {
//                this.scriptEngine = scriptEngine;
//                this.compiledText = compiledText;
//            }
//            public override object Eval(IScriptContext ctx)
//            {
//                try
//                {
//                    var engine = IronPython.Hosting.Python.CreateEngine();
//                    var scope = engine.CreateScope();
//                    var source = engine.CreateScriptSourceFromString(compiledText);
//                    source.Execute(scope);
//                    //TODO 暂时写死方法名add,后期反射扩展scope.Varialbes
//                    dynamic func = scope.GetVariable<Func<object>>("add");
//                    return func();
//                }
//                catch (System.Exception ex)
//                {
//                    throw new ScriptException("Python脚本执行错误:" + ex.Message);
//                }
//            }

//            public override IScriptEngine GetEngine()
//            {
//                return scriptEngine;
//            }
//            #endregion
//        }
//    }
//}
