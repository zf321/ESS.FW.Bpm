//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ESS.FW.Bpm.Engine.Impl.Javax.Script.Engine
//{
//    public class CSScriptEngineFactory : IScriptEngineFactory
//    {
//        public static IList<string> names = new List<string>() { "cs" };
//        private static IList<string> extensions;
//        private static IList<string> mimeTypes;
//        public string EngineName
//        {
//            get
//            {
//                return "CSScript";
//            }
//        }

//        public string EngineVersion
//        {
//            get { return "1.0"; }
//        }

//        public IList<string> Extensions
//        {
//            get { return extensions; }
//        }

//        public string LanguageName
//        {
//            get { return "cs"; }
//        }

//        public string LanguageVersion
//        {
//            get { return "4.5"; }
//        }

//        public string GetMethodCallSyntax(string obj, string m, params string[] args)
//        {
//            throw new NotImplementedException();
//        }

//        public IList<string> MimeTypes
//        {
//            get { return mimeTypes; }
//        }

//        public IList<string> Names
//        {
//            get { return names; }
//        }

//        public string GetOutputStatement(string toDisplay)
//        {
//            // We will use out:print function to output statements
//            StringBuilder stringBuffer = new StringBuilder();
//            stringBuffer.Append("out:print(\"");

//            int length = toDisplay.Length;
//            for (int i = 0; i < length; i++)
//            {
//                char c = toDisplay[i];
//                switch (c)
//                {
//                    case '"':
//                        stringBuffer.Append("\\\"");
//                        break;
//                    case '\\':
//                        stringBuffer.Append("\\\\");
//                        break;
//                    default:
//                        stringBuffer.Append(c);
//                        break;
//                }
//            }
//            stringBuffer.Append("\")");
//            return stringBuffer.ToString();
//        }

//        public object GetParameter(string key)
//        {
//            if (key.Equals(ScriptEngine_Fields.NAME))
//            {
//                return LanguageName;
//            }
//            else if (key.Equals(ScriptEngine_Fields.ENGINE))
//            {
//                return EngineName;
//            }
//            else if (key.Equals(ScriptEngine_Fields.ENGINE_VERSION))
//            {
//                return EngineVersion;
//            }
//            else if (key.Equals(ScriptEngine_Fields.LANGUAGE))
//            {
//                return LanguageName;
//            }
//            else if (key.Equals(ScriptEngine_Fields.LANGUAGE_VERSION))
//            {
//                return LanguageVersion;
//            }
//            else if (key.Equals("THREADING"))
//            {
//                return "MULTITHREADED";
//            }
//            else
//            {
//                return null;
//            }
//        }

//        public string GetProgram(params string[] statements)
//        {
//            // Each statement is wrapped in '${}' to comply with EL
//            StringBuilder buf = new StringBuilder();
//            if (statements.Length != 0)
//            {
//                for (int i = 0; i < statements.Length; i++)
//                {
//                    buf.Append("${");
//                    buf.Append(statements[i]);
//                    buf.Append("} ");
//                }
//            }
//            return buf.ToString();
//        }

//        public IScriptEngine GetScriptEngine()
//        {
//            return new CSScriptEngine(this);
//        }
//    }
//}
