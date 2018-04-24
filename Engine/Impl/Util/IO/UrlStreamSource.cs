using System;
using System.IO;

namespace ESS.FW.Bpm.Engine.Impl.Util.IO
{
    /// <summary>
    ///      
    /// </summary>
    public class UrlStreamSource : IStreamSource
    {
        internal Uri Url;

        public UrlStreamSource(Uri url)
        {
            this.Url = url;
        }

        public virtual Stream InputStream
        {
            get
            {
                try
                {
//                    return url.openStream();
                }
                catch (IOException e)
                {
                    throw new ProcessEngineException("couldn't open url '" + Url + "'", e);
                }
                return null;
            }
        }
    }
}