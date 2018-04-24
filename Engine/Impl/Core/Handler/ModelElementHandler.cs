using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Model.Xml.instance;

namespace ESS.FW.Bpm.Engine.Impl.Core.Handler
{
    /// <summary>
    ///     <para>
    ///         A <seealso cref="IModelElementHandler{T,V,E}" /> handles an instance of a <seealso cref="IModelElementInstance" />
    ///         to create a new <seealso cref="CoreActivity." />
    ///     </para>
    ///     
    /// </summary>
    public interface IModelElementHandler<T, TV, TE> where T : IModelElementInstance where TV : IHandlerContext
    {
        /// <summary>
        ///     <para>This method handles a element to create a new element.</para>
        /// </summary>
        /// <param name="element"> the <seealso cref="IModelElementInstance" /> to be handled. </param>
        /// <param name="context">
        ///     the <seealso cref="IHandlerContext" /> which holds necessary information.
        /// </param>
        /// <returns> a new element. </returns>
        TE HandleElement(T element, TV context);
    }
}