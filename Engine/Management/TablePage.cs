using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Management
{
    /// <summary>
    ///     Data structure used for retrieving database table content.
    ///      
    ///     
    /// </summary>
    public class TablePage
    {
        public virtual string TableName { get; set; }


        /// <returns>
        ///     the start index of this page
        ///     (ie the index of the first element in the page)
        /// </returns>
        public virtual long FirstResult { get; set; }


        public virtual IList<IDictionary<string, object>> Rows { set; get; }


        public virtual long Total { set; get; } = -1;


        /// <returns> the actual number of rows in this page. </returns>
        public virtual long Size
        {
            get { return Rows.Count; }
        }
    }
}