using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Management
{
    /// <summary>
    ///     Structure containing meta data (column names, column types, etc.)
    ///     about a certain database table.
    ///     
    /// </summary>
    public class TableMetaData
    {
        public TableMetaData()
        {
        }

        public TableMetaData(string tableName)
        {
            this.TableName = tableName;
        }

        public virtual string TableName { get; set; }


        public virtual IList<string> ColumnNames { get; set; } = new List<string>();


        public virtual IList<string> ColumnTypes { get; set; } = new List<string>();

        public virtual void AddColumnMetaData(string columnName, string columnType)
        {
            ColumnNames.Add(columnName);
            ColumnTypes.Add(columnType);
        }
    }
}