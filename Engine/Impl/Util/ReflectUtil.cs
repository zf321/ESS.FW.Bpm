using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;

namespace ESS.FW.Bpm.Engine.Impl.Util
{
    
    /// <summary>
    /// 
    /// </summary>
    public abstract class ReflectUtil
    {
        private static readonly EngineUtilLogger Log = ProcessEngineLogger.UtilLogger;

        private static readonly IDictionary<string, string> CharEncodings = new Dictionary<string, string>();

        static ReflectUtil()
        {
            CharEncodings["ä"] = "%C3%A4";
            CharEncodings["ö"] = "%C3%B6";
            CharEncodings["ü"] = "%C3%BC";
            CharEncodings["Ä"] = "%C3%84";
            CharEncodings["Ö"] = "%C3%96";
            CharEncodings["Ü"] = "%C3%9C";
        }
        /// <summary>
        /// 获取类型(命名空间格式：nameSpace,className) ,类中子类用+号连接
        /// </summary>
        /// <param name="className">类型全名</param>
        /// <param name="nameSpace">命名空间</param>
        /// <returns></returns>
        public static Type LoadClass(string classN,bool isThrowable =true)
        {
            string nameSpace = null;
            string className = classN;
            if (classN.IndexOf(",") > -1)
            {
                nameSpace = className.Split(',')[1];
                className = className.Split(',')[0];
            }
            Type type = Type.GetType(classN);
            if (type == null)
            {
                type = Assembly.GetCallingAssembly().GetType(className);
            }
            if (type == null && nameSpace != null)
            {
                type = Assembly.Load(nameSpace).GetType(className);
            }
            if (type == null && isThrowable)
            {
                throw new System.Exception("未能找到指定类型，请检查xml中类型名称：" + className);
            }
            return type;
        }
        /// <summary>
        /// 是否存在资源文件
        /// </summary>
        /// <param name="filePath">resource\\...</param>
        /// <returns></returns>
        public static bool Existed(string filePath)
        {
            return File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Replace("bin\\Debug", ""), filePath));
        }
        public static Stream GetResourceAsStream(string name)
        {
            //Stream resourceStream = null;
            //ClassLoader classLoader = CustomClassLoader;
            //if (classLoader != null)
            //{
            //  resourceStream = classLoader.getResourceAsStream(name);
            //}

            //if (resourceStream == null)
            //{
            //  // Try the current Thread context classloader
            //  classLoader = Thread.CurrentThread.ContextClassLoader;
            //  resourceStream = classLoader.getResourceAsStream(name);
            //  if (resourceStream == null)
            //  {
            //	// Finally, try the classloader for this class
            //	classLoader = typeof(ReflectUtil).ClassLoader;
            //	resourceStream = classLoader.getResourceAsStream(name);
            //  }
            //}
            //return resourceStream;
            return GetResourceStreamFromPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Replace("bin\\Debug", ""), name));
        }
        
        public static Stream GetResourceStreamFromPath(string filePath)
        {
            var sr = new StreamReader(filePath);
            return sr.BaseStream;
        }

        public static Uri GetResource(string name)
        {
            //Uri url = null;
            //ClassLoader classLoader = CustomClassLoader;
            //if (classLoader != null)
            //{
            //  url = classLoader.getResource(name);
            //}
            //if (url == null)
            //{
            //  // Try the current Thread context classloader
            //  classLoader = Thread.CurrentThread.ContextClassLoader;
            //  url = classLoader.getResource(name);
            //  if (url == null)
            //  {
            //	// Finally, try the classloader for this class
            //	classLoader = typeof(ReflectUtil).ClassLoader;
            //	url = classLoader.getResource(name);
            //  }
            //}
            //改为文件路径
            if (name.Contains("_"))
            {
                name = name.Split('_')[1];
            }
            var uri = new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, name));
            return uri;
        }

        public static string GetResourceUrlAsString(string name)
        {
            var url = GetResource(name).ToString();
            foreach (var mapping in CharEncodings)
                url = url.Replace(mapping.Key, mapping.Value); //.replaceAll(mapping.Key, mapping.Value);
            return url;
        }

        /// <summary>
        ///     Converts an url to an uri. Escapes whitespaces if needed.
        /// </summary>
        /// <param name="url">  the url to convert </param>
        /// <returns> the resulting uri </returns>
        /// <exception cref="ProcessEngineException"> if the url has invalid syntax </exception>
        public static Uri UrlToUri(Uri url)
        {
            return url;
        }


        public static object Instantiate(string className)
        {
            try
            {
                var clazz = LoadClass(className);
                return Activator.CreateInstance(clazz);
            }
            catch (System.Exception e)
            {
                throw Log.ExceptionWhileInstantiatingClass(className, e);
            }
        }

        public static T Instantiate<T>(Type type)
        {
            try
            {
                return (T)Activator.CreateInstance(type);
                ;
            }
            catch (System.Exception e)
            {
                //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
                throw Log.ExceptionWhileInstantiatingClass(type.FullName, e);
            }
        }

        public static object Invoke(object target, string methodName, object[] args)
        {
            try
            {
                var clazz = target.GetType();
                var method = FindMethod(clazz, methodName, args);
                //method.Accessible = true;
                return method.Invoke(target, args);
            }
            catch (System.Exception e)
            {
                throw Log.ExceptionWhileInvokingMethod(methodName, target, e);
            }
        }

        /// <summary>
        ///     Returns the field of the given object or null if it doesnt exist.
        /// </summary>
        public static FieldInfo GetField(string fieldName, object @object)
        {
            return GetField(fieldName, @object.GetType());
        }

        /// <summary>
        ///     Returns the field of the given class or null if it doesnt exist.
        /// </summary>
        public static FieldInfo GetField(string fieldName, Type clazz)
        {
            FieldInfo field = null;
            try
            {
                field = clazz.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                //field = clazz.getDeclaredField(fieldName);
            }
            catch (SecurityException)
            {
                //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
                //throw Log.UnableToAccessField(field, clazz.FullName);
                //throw new NotImplementedException("反射赋值字段错误：")
            }
            //catch (NoSuchFieldException)
            //{
            //  // for some reason getDeclaredFields doesnt search superclasses
            //  // (which getFields() does ... but that gives only public fields)
            //  Type superClass = clazz.BaseType;
            //  if (superClass != null)
            //  {
            //	return getField(fieldName, superClass);
            //  }
            //}
            return field;
        }

        public static void SetField(FieldInfo field, object @object, object value)
        {
            try
            {
                field.SetValue(@object, value);
                //field.Accessible = true;
                //field.Set(@object, value);
            }
            catch (System.Exception e)
            {
                throw new NotImplementedException("反射赋值出错", e);
                //throw LOG.exceptionWhileSettingField(field, @object, value, e);
            }
        }

        /// <summary>
        ///     Returns the setter-method for the given field name or null if no setter exists.
        /// </summary>
        public static MethodInfo GetSetter(string fieldName, Type clazz, Type fieldType)
        {
            var setterName = BuildSetterName(fieldName);
            try
            {
                // Using getMathods(), getMathod(...) expects exact parameter type
                // matching and ignores inheritance-tree.
                var methods = clazz.GetMethods();
                foreach (var method in methods)
                    if (method.Name.ToLower().Equals(setterName))
                    {
                        Type[] paramTypes = method.GetParameters().Select(c=>c.ParameterType).ToArray();
                        if (paramTypes != null && paramTypes.Length == 1 && paramTypes[0].IsAssignableFrom(fieldType))
                        {
                            return method;
                        }
                    }
                return null;
            }
            catch (SecurityException)
            {
                throw Log.UnableToAccessMethod(setterName, clazz.FullName);
            }
        }

        /// <summary>
        ///     Returns a setter method based on the fieldName and the java beans setter naming convention or null if none exists.
        ///     If multiple setters with different parameter types are present, an exception is thrown.
        ///     If they have the same parameter type, one of those methods is returned.
        /// </summary>
        public static MethodInfo GetSingleSetter(string fieldName, Type clazz)
        {
            var setterName = BuildSetterName(fieldName);
            try
            {
                // Using getMathods(), getMathod(...) expects exact parameter type
                // matching and ignores inheritance-tree.
                var methods = clazz.GetMethods();
                IList<MethodInfo> candidates = new List<MethodInfo>();
                ISet<Type> parameterTypes = new HashSet<Type>();
                foreach (var method in methods)
                    if (method.Name.Equals(setterName))
                    {
                        // Type[] paramTypes = method.ParameterTypes;

                        // if (paramTypes != null && paramTypes.Length == 1)
                        // {
                        //candidates.Add(method);
                        //parameterTypes.Add(paramTypes[0]);
                        // }
                    }

                if (parameterTypes.Count > 1)
                    throw Log.AmbiguousSetterMethod(setterName, clazz.FullName);
                if (candidates.Count >= 1)
                    return candidates[0];

                return null;
            }
            catch (SecurityException)
            {
                //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
                throw Log.UnableToAccessMethod(setterName, clazz.FullName);
            }
        }

        private static string BuildSetterName(string fieldName)
        {
            return "set_" + fieldName.ToLower(); /*+ Character.toTitleCase(fieldName[0]) + fieldName.Substring(1, fieldName.Length - 1)*/;
        }

        public static MethodInfo FindMethod(Type clazz, string methodName, object[] args)
        {
            foreach (
                var method in
                clazz.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic |
                                 BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
            {
                if (method.Name.Equals(methodName) && Matches(method.GetParameters().Select(c => c.ParameterType).ToArray(), args))
                {
                    return method;
                }
                if (method.Name.Equals(methodName))
                {
                    return method;
                }
            }
            var superClass = clazz.BaseType;
            if (superClass != null)
            {
                return FindMethod(superClass, methodName, args);
            }
            if (superClass == null)
            {
                foreach (var @interface in clazz.GetInterfaces())
                {
                    var m =FindMethod(@interface, methodName, args);
                    if (m != null) return m;

                }
            }
            return null;
        }

        public static object Instantiate(string className, object[] args)
        {
            var clazz = LoadClass(className);
            //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
            //ORIGINAL LINE: Constructor<object> constructor = findMatchingConstructor(clazz, args);
            //Constructor<object> constructor = findMatchingConstructor(clazz, args);
            //EnsureUtil.EnsureNotNull("couldn't find constructor for " + className + " with args " + (args), "constructor", constructor);
            try
            {
                return null;
            }
            catch (System.Exception e)
            {
                throw Log.ExceptionWhileInstantiatingClass(className, e);
            }
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings({ "unchecked", "rawtypes" }) private static <T> Constructor<T> findMatchingConstructor(Class<T> clazz, Object[] args)
        // private static Constructor<T> findMatchingConstructor<T>(Type<T> clazz, object[] args)
        // {
        //foreach (Constructor constructor in clazz.GetConstructors(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
        //{ // cannot use <object> or <T> due to JDK 5/6 incompatibility
        //  if (matches(constructor.ParameterTypes, args))
        //  {
        //	return constructor;
        //  }
        //}
        //return null;
        // }

        private static bool Matches(Type[] parameterTypes, object[] args)
        {
            if ((parameterTypes == null) || (parameterTypes.Length == 0))
                return (args == null) || (args.Length == 0);
            if ((args == null) || (parameterTypes.Length != args.Length))
                return false;
            for (var i = 0; i < parameterTypes.Length; i++)
                if ((args[i] != null) && (!(args[i] is Type && parameterTypes[i].IsAssignableFrom(args[i] as Type)) &&!parameterTypes[i].IsAssignableFrom(args[i].GetType())))
                    return false;
            return true;
        }

        //  private static ClassLoader CustomClassLoader
        //  {
        //	  get
        //	  {
        //		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
        //		if (processEngineConfiguration != null)
        //		{
        ////JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        ////ORIGINAL LINE: final ClassLoader classLoader = processEngineConfiguration.getClassLoader();
        //		  ClassLoader classLoader = processEngineConfiguration.ClassLoader;
        //		  if (classLoader != null)
        //		  {
        //			return classLoader;
        //		  }
        //		}
        //		return null;
        //	  }
        //  }

        /// <summary>
        ///     Finds a method by name and parameter types.
        /// </summary>
        /// <param name="declaringType"> the name of the class </param>
        /// <param name="methodName"> the name of the method to look for </param>
        /// <param name="parameterTypes"> the types of the parameters </param>
        public static MethodInfo GetMethod(Type declaringType, string methodName, params Type[] parameterTypes)
        {
            return FindMethod(declaringType, methodName, parameterTypes);
        }
    }
}