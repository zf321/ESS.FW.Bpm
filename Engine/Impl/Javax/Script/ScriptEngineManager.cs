using ESS.FW.Bpm.Engine.Impl.Javax.Script.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Javax.Script
{
    public class ScriptEngineManager
    {
        public ScriptEngineManager()
        {
            Init();
        }
        private void Init()
        {
            globalScope = new SimpleBindings();
            engineSpis = new HashSet<IScriptEngineFactory>();
            nameAssociations = new Dictionary<String, IScriptEngineFactory>();
            extensionAssociations = new Dictionary<String, IScriptEngineFactory>();
            mimeTypeAssociations = new Dictionary<String, IScriptEngineFactory>();
            //engineSpis.Add(new CSScriptEngineFactory());
            engineSpis.Add(new JuelScriptEngineFactory());
            //engineSpis.Add(new PythonScriptEngineFactory());
        }
        /** Set of script engine factories discovered. */
        private ISet<IScriptEngineFactory> engineSpis;

        /** Map of engine name to script engine factory. */
        private IDictionary<String, IScriptEngineFactory> nameAssociations;

        /** Map of script file extension to script engine factory. */
        private IDictionary<String, IScriptEngineFactory> extensionAssociations;

        /** Map of script script MIME type to script engine factory. */
        private IDictionary<String, IScriptEngineFactory> mimeTypeAssociations;

        public void RegisterEngineName(string name, IScriptEngineFactory factory)
        {
            if (name == null || factory == null)
                throw new NullReferenceException();
            nameAssociations[name] = factory;//.Put(name, factory);
        }

        /** Global bindings associated with script engines created by this manager. */
        private IBindings globalScope;


        public IBindings GetBindings()
        {
            return globalScope;
        }
        public void SetBindings(IBindings bindings)
        {
            globalScope = bindings ?? throw new System.ArgumentNullException("Global scope cannot be null.");
        }
        public void Put(String key, Object value)
        {
            globalScope.Put(key, value);
        }
        public Object Get(String key)
        {
            return globalScope.Get(key);
        }

        public IScriptEngine GetEngineByName(string shortName)
        {
            if (shortName == null) throw new NullReferenceException();// NullPointerException();
            //look for registered name first
            Object obj = nameAssociations.ContainsKey(shortName) ? nameAssociations[shortName] : null;
            if (null != obj )
            {
                IScriptEngineFactory spi = (IScriptEngineFactory)obj;
                try
                {
                    IScriptEngine engine = spi.GetScriptEngine();
                    engine.SetBindings(GetBindings(), ScriptContext_Fields.GLOBAL_SCOPE);
                    return engine;
                }
                catch (System.Exception exp)
                {
                    throw exp;
                    //if (DEBUG) exp.printStackTrace();
                }
            }

            foreach (IScriptEngineFactory spi in engineSpis)
            {
                IList<String> names = null;
                try
                {
                    names = spi.Names;
                }
                catch (System.Exception exp)
                {
                    throw exp;
                    //if (DEBUG) exp.printStackTrace();
                }

                if (names != null)
                {
                    foreach (String name in names)
                    {
                        if (shortName.Equals(name))
                        {
                            try
                            {
                                IScriptEngine engine = spi.GetScriptEngine();
                                engine.SetBindings(GetBindings(), ScriptContext_Fields.GLOBAL_SCOPE);
                                return engine;
                            }
                            catch (System.Exception exp)
                            {
                                throw exp;
                                //if (DEBUG) exp.printStackTrace();
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}
