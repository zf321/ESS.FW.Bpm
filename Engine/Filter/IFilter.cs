using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Query;

namespace ESS.FW.Bpm.Engine.Filter
{
    /// <summary>
    ///     
    /// </summary>
    public interface IFilter
    {
        /// <returns> the id of the filer </returns>
        string Id { get; }

        /// <returns> the resource type fo the filter </returns>
        string ResourceType { get; }

        /// <returns> the name of the filter </returns>
        string Name { get; }

        /// <returns> the owner of the filter </returns>
        string Owner { get; }

        /// <returns> the properties as map </returns>
        IDictionary<string, object> Properties { get; }

        /// <param name="name"> the name of the filter </param>
        /// <returns> this filter </returns>
        IFilter SetName(string name);

        /// <param name="owner"> the owner of the filter </param>
        /// <returns> this filter </returns>
        IFilter SetOwner(string owner);

        /// <returns> the saved query as query object </returns>
//JAVA TO C# CONVERTER TODO ITask: The following line could not be converted:
        //AbstractQuery<object> getQuery<T>() where T : IQuery<T>;

        /// <param name="query"> the saved query as query object </param>
        /// <returns> this filter </returns>
//JAVA TO C# CONVERTER TODO ITask: The following line could not be converted:
        //IFilter setQuery<T>(T query) where T : IQuery<T>;

        /// <summary>
        ///     Extends the query with the additional query. The query of the filter is therefore modified
        ///     and if the filter is saved the query is updated.
        /// </summary>
        /// <param name="extendingQuery"> the query to extend the filter with </param>
        /// <returns> a copy of this filter with the extended query </returns>
        //JAVA TO C# CONVERTER TODO ITask: The following line could not be converted:
        //IFilter extend<T>(T extendingQuery) where T : IQuery<T>;

        /// <param name="properties"> the properties to set as map </param>
        /// <returns> this filter </returns>
        IFilter SetProperties(IDictionary<string, object> properties);
    }
}