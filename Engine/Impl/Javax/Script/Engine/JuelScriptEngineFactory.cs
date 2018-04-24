using ESS.FW.Bpm.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Javax.Script.Engine
{
    public class JuelScriptEngineFactory : IScriptEngineFactory
    {
        public static IList<string> names;
        private static IList<string> extensions;
        private static IList<string> mimeTypes;

        static JuelScriptEngineFactory()
        {
            //names = Collections.UnmodifiableList(Arrays.asList("juel"));
            names = new List<string>() { "juel" };
            extensions = names;
            //mimeTypes = Collections.unmodifiableList(new List<string>(0));
            mimeTypes = new List<string>();
        }

        public virtual string EngineName
        {
            get { return "juel"; }
        }

        public virtual string EngineVersion
        {
            get { return "1.0"; }
        }

        public virtual IList<string> Extensions
        {
            get { return extensions; }
        }

        public virtual string LanguageName
        {
            get { return "JSP 2.1 EL"; }
        }

        public virtual string LanguageVersion
        {
            get { return "2.1"; }
        }

        public virtual string GetMethodCallSyntax(string obj, string method, params string[] arguments)
        {
            throw new System.NotSupportedException("Method getMethodCallSyntax is not supported");
        }

        public virtual IList<string> MimeTypes
        {
            get { return mimeTypes; }
        }

        public virtual IList<string> Names
        {
            get { return names; }
        }

        public virtual string GetOutputStatement(string toDisplay)
        {
            // We will use out:print function to output statements
            StringBuilder stringBuffer = new StringBuilder();
            stringBuffer.Append("out:print(\"");

            int length = toDisplay.Length;
            for (int i = 0; i < length; i++)
            {
                char c = toDisplay[i];
                switch (c)
                {
                    case '"':
                        stringBuffer.Append("\\\"");
                        break;
                    case '\\':
                        stringBuffer.Append("\\\\");
                        break;
                    default:
                        stringBuffer.Append(c);
                        break;
                }
            }
            stringBuffer.Append("\")");
            return stringBuffer.ToString();
        }

        public virtual object GetParameter(string key)
        {
            if (key.Equals(ScriptEngine_Fields.NAME))
            {
                return LanguageName;
            }
            else if (key.Equals(ScriptEngine_Fields.ENGINE))
            {
                return EngineName;
            }
            else if (key.Equals(ScriptEngine_Fields.ENGINE_VERSION))
            {
                return EngineVersion;
            }
            else if (key.Equals(ScriptEngine_Fields.LANGUAGE))
            {
                return LanguageName;
            }
            else if (key.Equals(ScriptEngine_Fields.LANGUAGE_VERSION))
            {
                return LanguageVersion;
            }
            else if (key.Equals("THREADING"))
            {
                return "MULTITHREADED";
            }
            else
            {
                return null;
            }
        }

        public virtual string GetProgram(params string[] statements)
        {
            // Each statement is wrapped in '${}' to comply with EL
            StringBuilder buf = new StringBuilder();
            if (statements.Length != 0)
            {
                for (int i = 0; i < statements.Length; i++)
                {
                    buf.Append("${");
                    buf.Append(statements[i]);
                    buf.Append("} ");
                }
            }
            return buf.ToString();
        }

        public virtual IScriptEngine GetScriptEngine()
        {
            return new JuelScriptEngine(this);
        }


    }
}
