using System.IO;

namespace ESS.FW.Bpm.Engine.Impl.Util.IO
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.Util.EnsureUtil.ensureNotNull;


    /// <summary>
    ///      
    /// </summary>
    public class ResourceStreamSource : IStreamSource
    {
        //internal ClassLoader classLoader;

        internal string Resource;

        public ResourceStreamSource(string resource)
        {
            this.Resource = resource;
        }

        //public ResourceStreamSource(string resource)
        //{
        //    this.resource = resource;
        //    this.classLoader = classLoader;
        //}

        public virtual Stream InputStream
        {
            get
            {
                Stream inputStream = null;
                //if (classLoader == null)
                //{
                //    inputStream = ReflectUtil.getResourceAsStream(resource);
                //}
                //else
                //{
                //    //classLoader.getResourceAsStream(resource);
                //}
                EnsureUtil.EnsureNotNull("resource '" + Resource + "' doesn't exist", "inputStream", inputStream);
                return inputStream;
            }
        }

        public override string ToString()
        {
            return "Resource[" + Resource + "]";
        }
    }
}