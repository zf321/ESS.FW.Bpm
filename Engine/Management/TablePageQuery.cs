namespace ESS.FW.Bpm.Engine.Management
{
    /// <summary>
    ///     Allows programmatic querying of <seealso cref="TablePage" />s.
    ///     
    /// </summary>
    public interface ITablePageQuery
    {
        /// <summary>
        ///     The name of the table of which a page must be fetched.
        /// </summary>
        ITablePageQuery TableName(string tableName);

        /// <summary>
        ///     Orders the resulting table page rows by the given column in ascending order.
        /// </summary>
        ITablePageQuery OrderAsc(string column);

        /// <summary>
        ///     Orders the resulting table page rows by the given column in descending order.
        /// </summary>
        ITablePageQuery OrderDesc(string column);

        /// <summary>
        ///     Executes the query and returns the <seealso cref="TablePage" />.
        /// </summary>
        TablePage ListPage(int firstResult, int maxResults);
    }
}