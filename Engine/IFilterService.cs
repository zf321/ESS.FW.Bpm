using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Filter;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Xml.instance;

namespace ESS.FW.Bpm.Engine
{
    /// <summary>
    ///     
    /// </summary>
    public interface IFilterService
    {
        /// <summary>
        ///     Creates a new ITask filter.
        /// </summary>
        /// <returns> a new ITask filter </returns>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#CREATE" /> permissions on
        ///     <seealso cref="Resources#FILTER" />.
        /// </exception>
        IFilter NewTaskFilter();

        /// <summary>
        ///     Creates a new ITask filter with a given name.
        /// </summary>
        /// <returns> a new ITask filter with a name </returns>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#CREATE" /> permissions on
        ///     <seealso cref="Resources#FILTER" />.
        /// </exception>
        IFilter NewTaskFilter(string filterName);

        /// <summary>
        ///     Creates a new filter query
        /// </summary>
        /// <returns> a new query for filters </returns>
        IQueryable<IFilter> CreateFilterQuery(Expression<Func<FilterEntity, bool>> expression = null);


        /// <summary>
        ///     Creates a new ITask filter query.
        /// </summary>
        /// <returns> a new query for ITask filters </returns>
        IQueryable<IFilter> CreateTaskFilterQuery();

        /// <summary>
        ///     Saves the filter in the database.
        /// </summary>
        /// <param name="filter"> the filter to save </param>
        /// <returns> return the saved filter </returns>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#CREATE" /> permissions on <seealso cref="Resources#FILTER" /> (save
        ///     new filter)
        ///     or if user has no <seealso cref="Permissions#UPDATE" /> permissions on <seealso cref="Resources#FILTER" /> (update
        ///     existing filter).
        /// </exception>
        /// <exception cref="BadUserRequestException">
        ///     <ul>
        ///         <li>
        ///             When the filter query uses expressions and expression evaluation is deactivated for stored queries.
        ///             Expression evaluation can be activated by setting the process engine configuration properties
        ///             <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
        ///             <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>.
        /// </exception>
        IFilter SaveFilter(IFilter filter);

        /// <summary>
        ///     Returns the filter for the given filter id.
        /// </summary>
        /// <param name="filterId"> the id of the filter </param>
        /// <returns> the filter </returns>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#READ" /> permissions on
        ///     <seealso cref="Resources#FILTER" />.
        /// </exception>
        IFilter GetFilter(string filterId);

        /// <summary>
        ///     Deletes a filter by its id.
        /// </summary>
        /// <param name="filterId"> the id of the filter </param>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#DELETE" /> permissions on
        ///     <seealso cref="Resources#FILTER" />.
        /// </exception>
        void DeleteFilter(string filterId);

        /// <summary>
        ///     Executes the query of the filter and returns the result as list.
        /// </summary>
        /// <param name="filterId"> the the id of the filter </param>
        /// <returns> the query result as list </returns>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#READ" /> permissions on
        ///     <seealso cref="Resources#FILTER" />.
        /// </exception>
        /// <exception cref="BadUserRequestException">
        ///     <ul>
        ///         <li>
        ///             When the filter query uses expressions and expression evaluation is deactivated for stored queries.
        ///             Expression evaluation can be activated by setting the process engine configuration properties
        ///             <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
        ///             <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>.
        /// </exception>
//JAVA TO C# CONVERTER TODO ITask: The following line could not be converted:
        List<T> List<T>(string filterId);

        /// <summary>
        ///     Executes the extended query of a filter and returns the result as list.
        /// </summary>
        /// <param name="filterId"> the id of the filter </param>
        /// <param name="extendingQuery"> additional query to extend the filter query </param>
        /// <returns> the query result as list </returns>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#READ" /> permissions on
        ///     <seealso cref="Resources#FILTER" />.
        /// </exception>
        /// <exception cref="BadUserRequestException">
        ///     <ul>
        ///         <li>
        ///             When the filter query uses expressions and expression evaluation is deactivated for stored queries.
        ///             <li>
        ///                 When the extending query uses expressions and expression evaluation is deactivated for adhoc queries.
        ///                 Expression evaluation can be activated by setting the process engine configuration properties
        ///                 <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
        ///                 <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>.
        /// </exception>
        /// <summary>
        ///     Executes the query of the filter and returns the result in the given boundaries as list.
        /// </summary>
        /// <param name="filterId"> the the id of the filter </param>
        /// <param name="firstResult"> first result to select </param>
        /// <param name="maxResults"> maximal number of results </param>
        /// <returns> the query result as list </returns>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#READ" /> permissions on
        ///     <seealso cref="Resources#FILTER" />.
        /// </exception>
        /// <exception cref="BadUserRequestException">
        ///     <ul>
        ///         <li>
        ///             When the filter query uses expressions and expression evaluation is deactivated for stored queries.
        ///             Expression evaluation can be activated by setting the process engine configuration properties
        ///             <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
        ///             <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>.
        /// </exception>
        /// <summary>
        ///     Executes the extended query of a filter and returns the result in the given boundaries as list.
        /// </summary>
        /// <param name="extendingQuery"> additional query to extend the filter query </param>
        /// <param name="filterId"> the id of the filter </param>
        /// <param name="firstResult"> first result to select </param>
        /// <param name="maxResults"> maximal number of results </param>
        /// <returns> the query result as list </returns>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#READ" /> permissions on
        ///     <seealso cref="Resources#FILTER" />.
        /// </exception>
        /// <exception cref="BadUserRequestException">
        ///     <ul>
        ///         <li>
        ///             When the filter query uses expressions and expression evaluation is deactivated for stored queries.
        ///             <li>
        ///                 When the extending query uses expressions and expression evaluation is deactivated for adhoc queries.
        ///                 Expression evaluation can be activated by setting the process engine configuration properties
        ///                 <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
        ///                 <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>.
        /// </exception>
        /// <summary>
        ///     Executes the query of the filter and returns the a single result.
        /// </summary>
        /// <param name="filterId"> the the id of the filter </param>
        /// <returns> the single query result </returns>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#READ" /> permissions on
        ///     <seealso cref="Resources#FILTER" />.
        /// </exception>
        /// <exception cref="BadUserRequestException">
        ///     <ul>
        ///         <li>
        ///             When the filter query uses expressions and expression evaluation is deactivated for stored queries.
        ///             Expression evaluation can be activated by setting the process engine configuration properties
        ///             <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
        ///             <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>.
        /// </exception>
//JAVA TO C# CONVERTER TODO ITask: The following line could not be converted:
        T SingleResult<T>(string filterId);

        /// <summary>
        ///     Executes the extended query of the filter and returns the a single result.
        /// </summary>
        /// <param name="filterId"> the the id of the filter </param>
        /// <param name="extendingQuery"> additional query to extend the filter query </param>
        /// <returns> the single query result </returns>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#READ" /> permissions on
        ///     <seealso cref="Resources#FILTER" />.
        /// </exception>
        /// <exception cref="BadUserRequestException">
        ///     <ul>
        ///         <li>
        ///             When the filter query uses expressions and expression evaluation is deactivated for stored queries.
        ///             <li>
        ///                 When the extending query uses expressions and expression evaluation is deactivated for adhoc queries.
        ///                 Expression evaluation can be activated by setting the process engine configuration properties
        ///                 <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
        ///                 <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>.
        /// </exception>
        /// <summary>
        ///     Executes the query of the filter and returns the result count.
        /// </summary>
        /// <param name="filterId"> the the id of the filter </param>
        /// <returns> the result count </returns>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#READ" /> permissions on
        ///     <seealso cref="Resources#FILTER" />.
        /// </exception>
        /// <exception cref="BadUserRequestException">
        ///     <ul>
        ///         <li>
        ///             When the filter query uses expressions and expression evaluation is deactivated for stored queries.
        ///             Expression evaluation can be activated by setting the process engine configuration properties
        ///             <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
        ///             <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>.
        /// </exception>
        long? Count(string filterId);

        /// <summary>
        ///     Executes the extended query of the filter and returns the result count.
        /// </summary>
        /// <param name="filterId"> the the id of the filter </param>
        /// <param name="extendingQuery"> additional query to extend the filter query </param>
        /// <returns> the result count </returns>
        /// <exception cref="AuthorizationException">
        ///     if the user has no <seealso cref="Permissions#READ" /> permissions on
        ///     <seealso cref="Resources#FILTER" />.
        /// </exception>
        /// <exception cref="BadUserRequestException">
        ///     <ul>
        ///         <li>
        ///             When the filter query uses expressions and expression evaluation is deactivated for stored queries.
        ///             <li>
        ///                 When the extending query uses expressions and expression evaluation is deactivated for adhoc queries.
        ///                 Expression evaluation can be activated by setting the process engine configuration properties
        ///                 <code>enableExpressionsInAdhocQueries</code> (default <code>false</code>) and
        ///                 <code>enableExpressionsInStoredQueries</code> (default <code>true</code>) to <code>true</code>.
        /// </exception>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: Nullable<long> count(String filterId, org.camunda.bpm.engine.query.Query<?, object> extendingQuery);
        long? Count<T1>(string filterId, IQuery<T1> extendingQuery) where T1 : IModelElementInstance;
    }
}