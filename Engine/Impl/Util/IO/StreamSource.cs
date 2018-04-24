using System.IO;

namespace ESS.FW.Bpm.Engine.Impl.Util.IO
{
    /// <summary>
    ///      
    /// </summary>
    public interface IStreamSource
    {
        Stream InputStream { get; }
    }
}