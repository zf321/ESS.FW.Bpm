using ESS.FW.Bpm.Engine.Impl.Core.Model;

namespace ESS.FW.Bpm.Engine.Impl.Core.Handler
{
    /// <summary>
    ///     <para>
    ///         An implementation of this context should contain necessary
    ///         information to be accessed by a <seealso cref="IModelElementHandler{T,V,E}" />.
    ///     </para>
    ///     
    /// </summary>
    public interface IHandlerContext
    {
        /// <summary>
        ///     <para>
        ///         This method returns an <seealso cref="CoreActivity" />. The
        ///         returned activity represents a parent activity, which can
        ///         contain <seealso cref="CoreActivity activities" />.
        ///     </para>
        ///     <para>
        ///         The returned activity should be used as a parent activity
        ///         for a new <seealso cref="CoreActivity activity" />.
        ///     </para>
        /// </summary>
        /// <returns> a <seealso cref="CoreActivity" /> </returns>
        CoreActivity Parent { get; }
    }
}