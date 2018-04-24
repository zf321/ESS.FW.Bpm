using System;
using System.IO;


namespace ESS.FW.Bpm.Model.Xml.impl.util
{



    /// <summary>
    /// uri相关辅助类
    /// </summary>
    public abstract class ReflectUtil
    {
        /// <summary>
        /// 项目生成目录为Binary的才能使用此方法
        /// </summary>
        private static readonly string BasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources");
        /// <summary>
        /// 所有资源放到\Binary\resources\下 根据相对路径获取物理文件流
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static System.IO.Stream GetResourceAsStream(string name)
        {
            using(FileStream file = File.OpenRead(Path.Combine(BasePath, name)))
            {
                byte[] bts = new byte[file.Length];
                file.Read(bts, 0, bts.Length);
                file.Flush();
                return new MemoryStream(bts);
            }
            // Try the current Thread context class loader
            //ClassLoader classLoader = Thread.CurrentThread.ContextClassLoader;
            //System.IO.Stream resourceStream = classLoader.getResourceAsStream(name);
            //if (resourceStream == null)
            //{
            //  // Finally, try the class loader for this class
            //  classLoader = typeof(ReflectUtil).ClassLoader;
            //  resourceStream = classLoader.getResourceAsStream(name);
            //}

            //return resourceStream;
        }

        /// <summary>
        /// 所有资源放到\Binary\resources\下 根据相对路径获取物理路劲
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Uri GetResource(string name)
        {
            return new Uri(Path.Combine(BasePath, name));
            //if (classLoader == null)
            //{
            //  // Try the current Thread context class loader
            //  classLoader = Thread.CurrentThread.ContextClassLoader;
            //}
            //Url url = classLoader.getResource(name);
            //if (url == null)
            //{
            //  // Finally, try the class loader for this class
            //  classLoader = typeof(ReflectUtil).ClassLoader;
            //  url = classLoader.getResource(name);
            //}

            //return url;
        }
        /// <summary>
        /// 获取资源文件信息(传入相对路径)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static FileInfo GetResourceAsFile(string path)
        {
            
            try
            {
                FileInfo fInfo = new FileInfo(Path.Combine(BasePath,path));
                return fInfo;
                //return new File(resource.toURI());
            }
            catch (IOException e)
            {
                throw new ModelException("Exception while loading resource file " + path, e);
            }
        }

        /// <summary>
        /// Create a new instance of the provided type
        /// </summary>
        /// <param name="type"> the class to create a new instance of </param>
        /// <param name="parameters"> the parameters to pass to the constructor </param>
        /// <returns> the created instance </returns>
        public static T CreateInstance<T>(Type type, params object[] parameters)
        {

            // get types for parameters
            Type[] parameterTypes = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                object parameter = parameters[i];
                parameterTypes[i] = parameter.GetType();
            }

            try
            {
                // create instance
                //Constructor<T> constructor = type.GetConstructor(parameterTypes);
                //return constructor.newInstance<T>(parameters);
                return default(T);

            }
            catch (Exception e)
            {
                throw new ModelException("Exception while creating an instance of type " + type, e);
            }
        }

    }

}